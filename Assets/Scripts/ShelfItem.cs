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
    [SerializeField] GameObject diskImages;
    [SerializeField] GameObject RAMImages;
    Hardware hwScript;
    public int position;
	BGN Cost;
    public BGN Multiplier;
    public BGN UpgradeCostMultiplier;
    public int RebirthMultiplier;
    public Sprite bgTexture;
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
            hwScript.enableNeurons();
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
            //PurchaseHW();
        }
    }

    public void PurchaseHW() {
        Purchased = true;
		HardwareButton.interactable = true;
		PurchaseButton.gameObject.SetActive(false);
        hardware = Instantiate(hardware);
        hwScript = hardware.GetComponentInChildren<Hardware>();
        HardwareCG = hardware.GetComponent<CanvasGroup>();
        HardwareC = hardware.GetComponent<Canvas>();
        HardwareCG.alpha = 0;
        hardware.GetComponentInChildren<Image>().sprite = bgTexture;
        hwScript.upgradeCostMultiplier = UpgradeCostMultiplier;
        hwScript.SetGPUMultiplier(Multiplier, position);
        hwScript.RebirthMultiplier = RebirthMultiplier;
        hwScript.diskImages = diskImages.GetComponentsInChildren<Image>();
        hwScript.RAMImages = RAMImages.GetComponentsInChildren<Image>();
        Image[] imgs = this.GetComponentsInChildren<Image>();
        imgs[2].color = new Color(1, 1, 1, 1);
		PlayerPrefs.SetInt("PurchasedHW" + position, 1);
        PlayerPrefs.Save();
	}

    public void setPrice(BGN price) {
        Cost = price;
    }

	public void Load(double seconds) {
		hwScript.Load(position, seconds);
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
