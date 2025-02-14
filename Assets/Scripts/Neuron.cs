using System;
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
	[SerializeField] BGN worth = new BGN(1);


	//visuals
	[SerializeField] Image NeuronImage;
	[SerializeField] Sprite DefaultNeuronSprite;
	[SerializeField] Sprite ClickedNeuronSprite;
	[SerializeField] Sprite StakedNeuronSprite;
	//events

	public static event EventHandler OnNeuronPressed;
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
		if (working || stake) {
			progressTimer -= Time.deltaTime;
			progress.value = 1 - (progressTimer / progressTimerMax);
			if (progressTimer <= 0) {
				working = false;
				GameManager.instance.addPoints(worth);
				neuron.interactable = true;
				progressTimer = progressTimerMax;
				if (stake) {
					NeuronImage.sprite = StakedNeuronSprite;
				} else {
					NeuronImage.sprite = DefaultNeuronSprite;
				}
			}
		}
	}

	public void CreateStake() {
		stake = true;
		NeuronImage.sprite = StakedNeuronSprite;
	}
}
