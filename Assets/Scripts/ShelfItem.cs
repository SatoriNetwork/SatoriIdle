using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShelfItem : MonoBehaviour
{
    [SerializeField] Button HardwareButton;
    [SerializeField] Button PurchaseButton;
    [SerializeField] TextMeshProUGUI PurchaseCostText;
    [SerializeField] GameObject Hardware;
    [SerializeField] CanvasGroup HardwareCG;

    [SerializeField] bool Purchased;
    [SerializeField] float cost;
    [SerializeField] BGN.Structures costStructure;
    BGN Cost;

	private void Awake() {
		Cost = new BGN(cost, costStructure);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        HardwareButton.onClick.AddListener(() => {
            shelf.Instance.gameObject.SetActive(false);
            HardwareCG.alpha = 1;
        });

        PurchaseButton.onClick.AddListener(() => {
            if (GameManager.instance.SatoriPoints >= Cost) {
                PurchaseHW();
                GameManager.instance.SatoriPoints -= Cost;
            }
        });
        PurchaseCostText.text = Cost.ToString();

        if (!Purchased) {
            HardwareButton.interactable = false;
            PurchaseButton.gameObject.SetActive(true);
        } else {
            PurchaseHW();
        }
    }

    private void PurchaseHW() {
        Purchased = true;
		HardwareButton.interactable = true;
		PurchaseButton.gameObject.SetActive(false);
        Hardware = Instantiate(Hardware);
        HardwareCG = Hardware.GetComponent<CanvasGroup>();
        HardwareCG.alpha = 0;
	}


    // Update is called once per frame
    void Update()
    {
        
    }
}
