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
    [SerializeField] public TutorialManager TutorialManager;

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
	[SerializeField] Slider RebirthNeededSlider;
	[SerializeField] GameObject RebirthGO;
	[SerializeField] ShelfGenerator shelfGenerator;

	[SerializeField] GameObject OfflineEarningGO;
	[SerializeField] TextMeshProUGUI OfflineEarningText;
	//rebirth
    [SerializeField] private BGN RebirthMultiplier = new BGN(1);
    [SerializeField] public int RebirthCost = 10000;
    private BGN rebirths = new BGN(0);
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
	private bool popup = true;
	public BGN offlineEarning = new BGN(0);
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
	private float progressbarTimer = 10;
    
	private void Awake() {
        instance = this; 
		int firstTime = PlayerPrefs.GetInt("FIRSTTIME", 0);    

		if (firstTime == 0) {
			SatoriPoints = new BGN(startingMoneysFloat,startingMoneysStructure);
            //SatoriPoints = new BGN((int)startingMoneysFloat);
            StartCoroutine(TutorialManager.startAfterTime(0.1f));
            SatoriPointsTotal = new BGN(startingMoneysFloat,startingMoneysStructure);
			//SatoriPointsTotal = new BGN((int)startingMoneysFloat);
			RebirthMultiplier.Save(REBIRTH_MULTIPLIER_PP);
			SatoriPoints.Save(SATORI_POINTS_PP);
			SatoriPointsTotal.Save(SATORI_POINTS_TOTAL_PP);
			popup = false;
        } else {
            TutorialManager.EndTutorial();
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
		if (Time.frameCount > 5 && popup) {
			popup = false;
			OfflineEarningGO.SetActive(true);
			OfflineEarningText.text = "You've Earned " + offlineEarning.ToString() + " While Away!";

		}
        SPText.text = SatoriPoints.ToString();
		if (progressbarTimer < 0 )
		{
            BGN rebirthNeeded = new BGN(RebirthCost);
            rebirthNeeded *= RebirthMultiplier;
            RebirthNeededSlider.value = Mathf.Clamp01(BGN.DivideF(SatoriPointsTotal, rebirthNeeded));
			progressbarTimer = 10;
        }
		progressbarTimer -= Time.deltaTime;

    }
	public void Rebirth()
	{
        BGN rebirthNeeded = new BGN(RebirthCost);
        rebirthNeeded = rebirthNeeded * RebirthMultiplier;
		if (SatoriPointsTotal >= rebirthNeeded)
		{
			totalSPPerSec = new BGN(0);
			cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            RebirthButton.SetActive(false);
            RebirthMultiplier += rebirths;
			RebirthText.text = "Rebirth Progress";
            shelfGenerator.Rebirth();
            SatoriPoints = new BGN(5);
            SatoriPointsTotal = new BGN(5);
            RebirthMultiplier.Save(REBIRTH_MULTIPLIER_PP);
            _ = RunCalculateRebirthLoop();
        }
	}
	public void resetSave()
	{
		float master = PlayerPrefs.GetFloat(SettingsManager.PLAYER_PREF_MASTER_VOLUME);
		float sfx = PlayerPrefs.GetFloat(SettingsManager.PLAYER_PREF_SFX_VOLUME);
		float music = PlayerPrefs.GetFloat(SettingsManager.PLAYER_PREF_MUSIC_VOLUME);
		PlayerPrefs.DeleteAll();
		PlayerPrefs.SetFloat(SettingsManager.PLAYER_PREF_MASTER_VOLUME, master);
		PlayerPrefs.SetFloat(SettingsManager.PLAYER_PREF_SFX_VOLUME, sfx);
		PlayerPrefs.SetFloat(SettingsManager.PLAYER_PREF_MUSIC_VOLUME, music);
		PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
			if (Application.platform == RuntimePlatform.Android) {

				if (Input.GetKeyUp(KeyCode.Escape)) {
					Application.Quit();
					return;
				}
			}
	}


}
