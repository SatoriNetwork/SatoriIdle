using UnityEngine;
using UnityEngine.UI;

public class Neuron : MonoBehaviour
{
	[SerializeField] Button neuron;
	[SerializeField] Slider progress;
	[SerializeField] float progressTimer = 5;
	[SerializeField] float progressTimerMax = 5;
	bool working = false;
	[SerializeField] float worth = 1;

	private void Start() {
		neuron.onClick.AddListener(() => {
			working = true;
			neuron.interactable = false;
		});
	}

	void Update() {
		if (working) {
			progressTimer -= Time.deltaTime;
			progress.value = 1 - (progressTimer / progressTimerMax);
			if (progressTimer <= 0) {
				working = false;
				// add worth
				neuron.interactable = true;
				progressTimer = progressTimerMax;
			}
		}
	}
}
