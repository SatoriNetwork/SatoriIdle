using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShelfGenerator : MonoBehaviour
{
    [SerializeField] GameObject shelfItemPrefab;
	[SerializeField] List<Sprite> GPUSprites = new List<Sprite>();

	[SerializeField] float costIncrease = 3;
	[SerializeField] int costInital = 10;
	[SerializeField] float multiIncrease = 3;
	[SerializeField] int multiInital = 10;

	private void Start() {
		BGN cost = new BGN(0);
		BGN multiplier = new BGN(1);
        for (int i = 0; i < GPUSprites.Count; i++) {
			GameObject go = Instantiate(shelfItemPrefab, gameObject.transform);
			ShelfItem  si = go.GetComponent<ShelfItem>();
			si.setPrice(cost);
			BGN costtemp = new BGN(costInital);
			cost = costtemp * BGN.Pow(costIncrease, i);
			si.Multiplier = multiplier; 
			BGN multitemp = new BGN(multiInital);
			multiplier = multitemp * BGN.Pow(multiIncrease, i);

			//add GPU
		}
	}
}
