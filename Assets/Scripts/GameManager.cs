using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public static GameManager instance { get; private set; }

	//base economy setup
	[SerializeField] int startingMoneys = 5;
	[SerializeField] public BGN SatoriPoints = new BGN(5);
	[SerializeField] public BGN SatoriPointsTotal = new BGN(5);

	//ui
	[SerializeField] TextMeshProUGUI SPText;
	[SerializeField] TextMeshProUGUI RebirthText;
	[SerializeField] GameObject RebirthButton;
	[SerializeField] ShelfGenerator shelfGenerator;

	//rebirth
    [SerializeField] private int RebirthMultiplier = 1;
    [SerializeField] public int RebirthCost = 10000;

	//neuron connection
	[SerializeField] public string UserAddress = "";
	[SerializeField] public int SatoriConnectionMultiplier = 1;
	[SerializeField] public bool SatoriConnected = false;
	[SerializeField] public Image SettingButtonImg;
	[SerializeField] public Sprite[] ConnectedImages;
	[SerializeField] public TMP_InputField playerAddressText;

	//save
	private const string SATORI_POINTS_PP = "SatoriPointsPP";
	private const string SATORI_POINTS_TOTAL_PP = "SatoriPointsTotalPP";
	private const string REBIRTH_MULTIPLIER_PP = "RebirthMultiplierPP";

	private void Awake() {
		PlayerPrefs.DeleteAll();
		instance = this; 
		int firstTime = PlayerPrefs.GetInt("FIRSTTIME", 0);
		if (firstTime == 0) {

			SatoriPoints = new BGN(startingMoneys);
			SatoriPointsTotal = new BGN(startingMoneys);
			PlayerPrefs.SetInt("FIRSTTIME", 1);
			SatoriPoints.Save(SATORI_POINTS_PP);
			SatoriPointsTotal.Save(SATORI_POINTS_TOTAL_PP);

		} else {
			SatoriPoints.Load(SATORI_POINTS_PP);
			SatoriPointsTotal.Load(SATORI_POINTS_TOTAL_PP);
		}
		RebirthMultiplier = PlayerPrefs.GetInt(REBIRTH_MULTIPLIER_PP, RebirthMultiplier);
	}

	private void Start() {

		UserAddress = PlayerPrefs.GetString("StringUserAddress");
		playerAddressText.text = UserAddress;
        CheckSatoriConnection();
	}
	public int getRebirthMultiplier()
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
			Debug.Log(response);
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
		Debug.Log(request);


    }
	public void addPoints(BGN sp) {
		SatoriPoints += sp;
		SatoriPointsTotal += sp;
        Debug.Log(SatoriPointsTotal);
    }

	private void FixedUpdate() {
        SPText.text = SatoriPoints.ToString();
		int calc = calculateRebirth();
		if (calc > 0)
		{
			RebirthButton.SetActive(true);
			RebirthText.text = "Rebirth: " + calc + "X";
		}
		else
		{
			RebirthButton.SetActive(false);
		}
		

	}
	public void Rebirth()
	{
        BGN rebirthNeeded = new BGN(RebirthCost);
        rebirthNeeded = rebirthNeeded * RebirthMultiplier;
		if (SatoriPointsTotal >= rebirthNeeded)
		{
			RebirthMultiplier += calculateRebirth();
			SatoriPoints = new BGN(5);
			SatoriPointsTotal = new BGN(5);
			shelfGenerator.Rebirth();

			PlayerPrefs.SetInt(REBIRTH_MULTIPLIER_PP, RebirthMultiplier);
			PlayerPrefs.Save();
		}
	}
	private int calculateRebirth()
	{
        BGN rebirthNeeded = new BGN(RebirthCost);
        rebirthNeeded *= RebirthMultiplier;  
        if (SatoriPointsTotal >= rebirthNeeded)
        {
            int count = 0;
            BGN satoriTemp = SatoriPointsTotal;

            while (satoriTemp >= rebirthNeeded)
            {
                satoriTemp -= rebirthNeeded;
                count++;
                rebirthNeeded = new BGN(RebirthCost) * (RebirthMultiplier + count);
            }
            return count;
        }
        return 0;
    }

	private void Update() {
		PlayerPrefs.SetInt(REBIRTH_MULTIPLIER_PP , RebirthMultiplier);
		SatoriPoints.Save(SATORI_POINTS_PP);
		SatoriPointsTotal.Save(SATORI_POINTS_TOTAL_PP);

		
    }


}
