using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour {
	public static GameManager instance { get; private set; }

	[SerializeField] int startingMoneys = 5;
	[SerializeField] public BGN SatoriPoints;
	[SerializeField] public BGN SatoriPointsTotal;
	[SerializeField] TextMeshProUGUI SPText;
	[SerializeField] TextMeshProUGUI RebirthText;
	[SerializeField] GameObject RebirthButton;
	[SerializeField] ShelfGenerator shelfGenerator;
    [SerializeField] private int RebirthMultiplier = 1;
    [SerializeField] public int RebirthCost = 10000;
	[SerializeField] public int SatoriConnectionMultiplier;
	[SerializeField] public bool SatoriConnected;
	[SerializeField] string UserAddress;
    private void Start() {
		instance = this;
        SatoriPoints = new BGN(startingMoneys);
        SatoriPointsTotal = new BGN(startingMoneys);
		CheckSatoriConnection();
    }
    public int getRebirthMultiplier()
	{ 
		return RebirthMultiplier;
	}
	public void CheckSatoriConnection()
	{
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
			}
			else
			{
				SatoriConnectionMultiplier = 1;
				SatoriConnected = false;	
			}
        }
		Debug.Log(request.result);


    }
	public void addPoints(BGN sp) {
		sp *= SatoriConnectionMultiplier;
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
}
