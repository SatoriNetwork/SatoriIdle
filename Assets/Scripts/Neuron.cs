using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Neuron : MonoBehaviour
{
	[SerializeField] Button neuron;
	[SerializeField] Slider progress;
	[SerializeField] public float progressTimer = 5;
	[SerializeField] public float progressTimerMax = 5;
	public bool working = false;
	public bool stake = false;
	public float critChance = 0;
	[SerializeField] public BGN worth = new BGN(1);
	[SerializeField] public BGN GPUMultiplier = new BGN(1);
	public bool enableVisuals = true;

	//visuals
	[SerializeField] Image NeuronImage;
	[SerializeField] Sprite DefaultNeuronSprite;
	[SerializeField] Sprite ClickedNeuronSprite;
	[SerializeField] Sprite StakedNeuronSprite;

	[SerializeField] GameObject floatingTextPrefab;
	[SerializeField] GameObject CritfloatingTextPrefab;
	//events

	public static event EventHandler OnNeuronPressed;
	public static event EventHandler OnNeuronComplete;
	private void Awake() {
		neuron.onClick.AddListener(() => {
			OnNeuronPressed?.Invoke(this, EventArgs.Empty);
			NeuronImage.sprite = ClickedNeuronSprite;
		});
	}

	private void Start() {
		neuron.onClick.AddListener(() => {
			working = true;
			neuron.interactable = false;
		});
	}

	void Update() {
		if (!enableVisuals) return;
		if (working || stake) {
			progressTimer -= Time.deltaTime;
			progress.value = 1 - (progressTimer / progressTimerMax);
			if (progressTimer <= 0) {
				working = false;
				OnNeuronComplete?.Invoke(this, EventArgs.Empty);
				if (UnityEngine.Random.Range(0, 100) <= critChance)
				{
                    GameManager.instance.addPoints((new BGN(2))*GPUMultiplier * GameManager.instance.SatoriConnectionMultiplier);
					if (enableVisuals) {
						GameObject textGO = Instantiate(CritfloatingTextPrefab, gameObject.transform);
						textGO.GetComponentInChildren<TextMeshProUGUI>().text = ( (new BGN(2)) * GPUMultiplier * GameManager.instance.SatoriConnectionMultiplier).ToString();
						Destroy(textGO, 1);
					}
				}
				else
				{
					GameManager.instance.addPoints(GPUMultiplier * GameManager.instance.SatoriConnectionMultiplier);
					if (enableVisuals) {
						GameObject textGO = Instantiate(floatingTextPrefab, gameObject.transform);
						textGO.GetComponentInChildren<TextMeshProUGUI>().text = ( GPUMultiplier * GameManager.instance.SatoriConnectionMultiplier).ToString();
						Destroy(textGO, 1);
					}
				}
				
				progressTimer = progressTimerMax;
				if (stake) {
					NeuronImage.sprite = StakedNeuronSprite;
				} else {
					neuron.interactable = true;
					NeuronImage.sprite = DefaultNeuronSprite;
				}
			}
		}
	}

	public void CreateStake() {
		stake = true;
		NeuronImage.sprite = StakedNeuronSprite;
		neuron.interactable = false;
	}
}
