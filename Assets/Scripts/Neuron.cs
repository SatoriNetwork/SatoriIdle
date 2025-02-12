using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Neuron : MonoBehaviour
{
	[SerializeField] Button neuron;
	[SerializeField] Slider progress;
	[SerializeField] float progressTimer = 5;
	[SerializeField] float progressTimerMax = 5;
	bool working = false;
	bool stake = false;
	[SerializeField] BGN worth = new BGN(1);

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
			}
		}
	}

	public void CreateStake() {
		stake = true;
	}
}
