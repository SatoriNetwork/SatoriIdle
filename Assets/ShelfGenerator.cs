using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelfGenerator : MonoBehaviour
{
	[SerializeField] GameManager gameManager;
    [SerializeField] GameObject shelfItemPrefab;
	[SerializeField] List<Sprite> GPUSprites = new List<Sprite>();
	[SerializeField] List<GameObject> ShelfGO = new List<GameObject>();

	[SerializeField] float costIncrease = 3;
	[SerializeField] int costInital = 10;
	[SerializeField] float multiIncrease = 3;
	[SerializeField] int multiInital = 10;

    private void Start()
    {
		Generate();
    }

    private void Generate() {
		BGN cost = new BGN(0);
		BGN multiplier = new BGN(1);
        for (int i = 0; i < GPUSprites.Count; i++) {
			GameObject go = Instantiate(shelfItemPrefab, gameObject.transform);
			Image[] imgs = go.GetComponentsInChildren<Image>();
			imgs[2].sprite = GPUSprites[i];
			ShelfGO.Add(go);
			ShelfItem  si = go.GetComponent<ShelfItem>();
			si.RebirthMultiplier = gameManager.getRebirthMultiplier();
            si.setPrice(cost);
			BGN costtemp = new BGN(costInital);
			cost = costtemp * BGN.Pow(costIncrease, i);
			si.Multiplier = multiplier; 
			BGN multitemp = new BGN(multiInital);
			multiplier = multitemp * BGN.Pow(multiIncrease, i);
			
			//add GPU
		}
	}

	public void Rebirth()
	{
		clearShelf();
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
}
