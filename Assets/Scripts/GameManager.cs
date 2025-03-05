using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BGN;

public class GameManager : MonoBehaviour {
	public static GameManager instance { get; private set; }

	//base economy setup
	[SerializeField] float startingMoneysFloat = 5;
	[SerializeField] Structures startingMoneysStructure = Structures.NONE;
    [SerializeField] public BGN SatoriPoints = new BGN(5);
	[SerializeField] public BGN SatoriPointsTotal = new BGN(5);

	//ui
	[SerializeField] TextMeshProUGUI SPText;
	[SerializeField] TextMeshProUGUI SPPerSecText;
	[SerializeField] TextMeshProUGUI RebirthText;
	[SerializeField] GameObject RebirthButton;
	[SerializeField] ShelfGenerator shelfGenerator;

	//rebirth
    [SerializeField] private BGN RebirthMultiplier = new BGN(1);
    [SerializeField] public int RebirthCost = 10000;
    private BGN rebirths = new BGN(0);
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();


    //neuron connection
    [SerializeField] public string UserAddress = "";
	[SerializeField] public int SatoriConnectionMultiplier = 1;
	[SerializeField] public bool SatoriConnected = false;
	[SerializeField] public Image SettingButtonImg;
	[SerializeField] public Sprite[] ConnectedImages;
	[SerializeField] public TMP_InputField playerAddressText;
	public bool OnShelf;
	public BGN totalSPPerSec = new BGN(0);
	//save
	private const string SATORI_POINTS_PP = "SatoriPointsPP";
	private const string SATORI_POINTS_TOTAL_PP = "SatoriPointsTotalPP";
	private const string REBIRTH_MULTIPLIER_PP = "RebirthMultiplierPP";
	private void Awake() {
        instance = this; 
		int firstTime = PlayerPrefs.GetInt("FIRSTTIME", 0);
		if (firstTime == 0) {
			SatoriPoints = new BGN(startingMoneysFloat,startingMoneysStructure);
			//SatoriPoints = new BGN((int)startingMoneysFloat);

            SatoriPointsTotal = new BGN(startingMoneysFloat,startingMoneysStructure);
            //SatoriPointsTotal = new BGN((int)startingMoneysFloat);
			PlayerPrefs.SetInt("FIRSTTIME", 1);
			SatoriPoints.Save(SATORI_POINTS_PP);
			SatoriPointsTotal.Save(SATORI_POINTS_TOTAL_PP);

		} else {
			SatoriPoints.Load(SATORI_POINTS_PP);
			SatoriPointsTotal.Load(SATORI_POINTS_TOTAL_PP);
		}
		RebirthMultiplier.Load(REBIRTH_MULTIPLIER_PP);
	}

	private void Start() {

		UserAddress = PlayerPrefs.GetString("StringUserAddress");
		playerAddressText.text = UserAddress;
        RebirthButton.SetActive(false);
        CheckSatoriConnection();
        _ = RunCalculateRebirthLoop();
    }
    public BGN getRebirthMultiplier()
	{ 
		return RebirthMultiplier;
	}

	public void changeSatoriConnection(){
        UserAddress = playerAddressText.text;
        if (playerAddressText.text.Length == 0)
        {
            SettingButtonImg.sprite = ConnectedImages[0];
        }
		PlayerPrefs.SetString("StringUserAddress", UserAddress);
		PlayerPrefs.Save();
	}
	public void CheckSatoriConnection()
	{
		changeSatoriConnection();
		string url = "https://stage.satorinet.io/api/v0/neuron/activity/" + UserAddress;
		StartCoroutine(getRequest(url));
    }
	IEnumerator getRequest(string url)
	{
        UnityWebRequest request = UnityWebRequest.Get(url);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			string response = request.downloadHandler.text;
			//change to whatever true or false is.
			if (response == "true")
			{
				SatoriConnectionMultiplier = 5;
				SatoriConnected = true;
                SettingButtonImg.sprite = ConnectedImages[1];
            }
			else
			{
				SatoriConnectionMultiplier = 1;
				SatoriConnected = false;
                SettingButtonImg.sprite = ConnectedImages[2];
            }
		}
		else
		{
			SettingButtonImg.sprite = ConnectedImages[0];
		}

    }
	public void addPoints(BGN sp) {
		SatoriPoints += sp;
		SatoriPointsTotal += sp;

    }

	private void FixedUpdate() {
        SPText.text = SatoriPoints.ToString();

    }
	public void Rebirth()
	{
        BGN rebirthNeeded = new BGN(RebirthCost);
        rebirthNeeded = rebirthNeeded * RebirthMultiplier;
		if (SatoriPointsTotal >= rebirthNeeded)
		{
			cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            RebirthButton.SetActive(false);
            RebirthMultiplier += rebirths;
            Debug.Log(RebirthMultiplier);
            SatoriPoints = new BGN(5);
            SatoriPointsTotal = new BGN(5);
            shelfGenerator.Rebirth();
            RebirthMultiplier.Save(REBIRTH_MULTIPLIER_PP);
            _ = RunCalculateRebirthLoop();
        }
	}
	public void resetSave()
	{
		PlayerPrefs.DeleteAll();
		SceneManager.LoadScene("ShelfTesting");
	}
    private async Task RunCalculateRebirthLoop()
    {
        while (true)
        {
            await calculateRebirth(cancellationTokenSource.Token);
            await Task.Delay(1000);
        }
    }

    private async Task<BGN> calculateRebirth(CancellationToken cancellationToken)
    {
        BGN rebirthNeeded = new BGN(RebirthCost);
        rebirthNeeded *= RebirthMultiplier;
		Task<BGN> rebirthsTask = null;
        if (SatoriPointsTotal >= rebirthNeeded)
        {
            BGN count = new BGN(0);
            BGN satoriTemp = SatoriPointsTotal;
            count = satoriTemp * (1 / (double)RebirthCost);
            rebirthsTask = Task.Run(() => BGN.Sqrt(new BGN(2) * count), cancellationToken); ;
			rebirths = await rebirthsTask;

        }
        if (rebirths > new BGN(0) && rebirthsTask != null && rebirthsTask.IsCompleted)
        {
            rebirths = rebirths - RebirthMultiplier + new BGN(1);
            RebirthButton.SetActive(true);
            RebirthText.text = "Rebirth: " + rebirths + " X";
        }
        else
        {
            RebirthButton.SetActive(false);
        }

        return rebirths;
    }

	public void updateSPPerSec( BGN bgn) {
		SPPerSecText.text = (bgn.ToString()) + " SP/S";

	}


    private void Update() {
        SatoriPoints.Save(SATORI_POINTS_PP);
		SatoriPointsTotal.Save(SATORI_POINTS_TOTAL_PP);
		if (!OnShelf) {
			updateSPPerSec(totalSPPerSec);
		}
		
    }


}
