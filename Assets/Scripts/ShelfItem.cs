using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShelfItem : MonoBehaviour
{
    [SerializeField] Button HardwareButton;
    [SerializeField] Button PurchaseButton;
    [SerializeField] TextMeshProUGUI PurchaseCostText;
    [SerializeField] GameObject hardware;
    [SerializeField] CanvasGroup HardwareCG;
    [SerializeField] Canvas HardwareC;
    
    [SerializeField] public bool Purchased = false;
    [SerializeField] float cost;
    [SerializeField] BGN.Structures costStructure;
    BGN Cost;
    public BGN Multiplier;
    public BGN UpgradeCostMultiplier;
    public int RebirthMultiplier;
	private void Awake() {
        Cost = new BGN(cost, costStructure);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        HardwareButton.onClick.AddListener(() => {
            shelfCanvas.Instance.gameObject.SetActive(false);
            HardwareCG.alpha = 1;
            HardwareC.sortingOrder = 2;
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
        hardware = Instantiate(hardware);
        Hardware hwScript = hardware.GetComponentInChildren<Hardware>();
        HardwareCG = hardware.GetComponent<CanvasGroup>();
        HardwareC = hardware.GetComponent<Canvas>();
        HardwareCG.alpha = 0;
        hwScript.upgradeCostMultiplier = UpgradeCostMultiplier;
        hwScript.SetGPUMultiplier(Multiplier);
        hwScript.RebirthMultiplier = RebirthMultiplier;
        Image[] imgs = this.GetComponentsInChildren<Image>();
        imgs[2].color = new Color(1, 1, 1, 1);
    }

    public void setPrice(BGN price) {
        Cost = price;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if (HardwareC) Destroy(HardwareC.gameObject);
    }
}
