using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour {
	public static GameManager instance { get; private set; }

	[SerializeField] public BGN SatoriPoints = new BGN(100000000);
	[SerializeField] public BGN SatoriPointsTotal = new BGN(5);
	[SerializeField] TextMeshProUGUI SPText;
	[SerializeField] TextMeshProUGUI RebirthText;
	[SerializeField] GameObject RebirthButton;
	[SerializeField] ShelfGenerator shelfGenerator;
    [SerializeField] private int RebirthMultiplier = 1;
    [SerializeField] public int RebirthCost = 10000;
    private void Start() {
		instance = this;
	}
	public int getRebirthMultiplier()
	{ 
		return RebirthMultiplier;
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
			RebirthText.text = "Rebirth: " + calc;
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
