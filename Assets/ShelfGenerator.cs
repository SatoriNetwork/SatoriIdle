using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ShelfGenerator : MonoBehaviour
{
	[SerializeField] GameManager gameManager;
    [SerializeField] GameObject shelfItemPrefab;
	[SerializeField] List<Sprite> GPUSprites = new List<Sprite>();
    [SerializeField] List<Sprite> BGTextures = new List<Sprite>();
    [SerializeField] List<GameObject> ShelfGO = new List<GameObject>();


	[SerializeField] float costIncrease = 3;
	[SerializeField] int costInital = 10;
	[SerializeField] float upgradeCostIncrease = 1.15f;
	[SerializeField] int upgradeCostInital = 10;
	[SerializeField] float multiIncrease = 3;
	[SerializeField] int multiInital = 10;

	private const string PLAYER_PREFS_DATE = "date";
    private void Start()
    {
		Generate();
      
    }

    private void Generate() {
		BGN cost = new BGN(0);
		BGN multiplier = new BGN(1);
		BGN upgradeCost = new BGN(1);

		DateTime oldtime = DateTime.Parse(PlayerPrefs.GetString(PLAYER_PREFS_DATE, DateTime.Now.ToString()));
		TimeSpan time = DateTime.Now - oldtime;
		double seconds = time.TotalSeconds;

		for (int i = 0; i < GPUSprites.Count; i++) {
			GameObject go = Instantiate(shelfItemPrefab, gameObject.transform);
			Image[] imgs = go.GetComponentsInChildren<Image>();
			imgs[2].sprite = GPUSprites[i];
			ShelfGO.Add(go);
			ShelfItem  si = go.GetComponent<ShelfItem>();
			si.position = i;
			si.bgTexture = BGTextures[i];
			si.RebirthMultiplier = gameManager.getRebirthMultiplier();
            si.setPrice(cost);
			BGN costtemp = new BGN(costInital);
			cost = costtemp * BGN.Pow(costIncrease, i);
			si.Multiplier = multiplier; 
			BGN multitemp = new BGN(multiInital);
			multiplier = multitemp * BGN.Pow(multiIncrease, i);
			si.UpgradeCostMultiplier = upgradeCost;
			BGN upgradeTemp = new BGN(upgradeCostInital);
			upgradeCost = upgradeTemp * BGN.Pow(upgradeCostIncrease, i);


            int firstTime = PlayerPrefs.GetInt("FIRSTTIME", 0);
            if (firstTime == 0 && i == 0)
            {
                GameManager.instance.TutorialManager.tutorialSteps[0].targetElement = ShelfGO[0].GetComponentInChildren<RectTransform>();
                GameManager.instance.TutorialManager.tutorialSteps[0].targetButton = ShelfGO[0].GetComponentInChildren<UnityEngine.UI.Button>();
                GameManager.instance.TutorialManager.tutorialSteps[0].message = "This is the first GPU you can buy. Click on it to buy it.";
            }


            int purchased = PlayerPrefs.GetInt("PurchasedHW" + i, 0);
			if (purchased == 1 || i == 0) {
				si.PurchaseHW();
				if (i == 0 && purchased == 0) {
					si.ResetHW();
				}
				si.Load(seconds);
				if (firstTime == 0)
                {
                    si.hwScript.firstTime();
                }
            }





                //add GPU
            }
	}

	public void Rebirth()
	{
		clearShelf();
		PlayerPrefs.SetInt("neuronCount" + 0, 1);
		for (int i = 0; i < GPUSprites.Count; i++) {

			PlayerPrefs.SetInt("PurchasedHW" + i, 0);
		}
		PlayerPrefs.Save();
		Generate();
	}
	private void clearShelf()
	{
		foreach (GameObject go in ShelfGO)
		{
			Destroy(go);
		}
		ShelfGO = new List<GameObject>();	
	}

	private void Update() {
		PlayerPrefs.SetString(PLAYER_PREFS_DATE, DateTime.Now.ToString());
		PlayerPrefs.Save();
	}
}
