using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	public static SoundManager instance { get; private set; }
	[SerializeField] private SoundsSO sounds;
	private float volume = 1f;

	private void Awake() {
		instance = this;
		Neuron.OnNeuronPressed += OnNeuronPress;
		Neuron.OnNeuronComplete += Neuron_OnNeuronComplete;
		BackFunction.HardwareBack += ShelfItem_openHardware;
		Hardware.OnButtonPressed += Hardware_OnButtonPressed;
		SettingsMenuManager.onInteract += Hardware_OnButtonPressed;
		ShelfItem.purchase += Hardware_OnButtonPressed;
		ShelfItem.openHardware += ShelfItem_openHardware;
	}

	private void Start() {
		//MainMenuUI.OnButtonPress += OnButtonPress;
	}

	private void Neuron_OnNeuronComplete(object sender, System.EventArgs e) {
		PlaySound(sounds.coinPickUp, Camera.allCameras[0].transform.position, sounds.coinPickupVolume - Random.Range(-0.25f, 0.25f));
	}

	private void ShelfItem_openHardware(object sender, System.EventArgs e) {
		PlaySound(sounds.HardwareSwoosh, Camera.allCameras[0].transform.position, sounds.HardwareSwooshVolume);
	}



	private void Hardware_OnButtonPressed(object sender, System.EventArgs e) {
		PlaySound(sounds.buttonPress, Camera.allCameras[0].transform.position, sounds.buttonPressVolume);
	}

	//example of what MainMenuUI.OnButtonPress
	// public static event EventHandler OnButtonPress;
	//private void Awake() {
	//PlayButton.onClick.AddListener(() => {
	//OnButtonPress?.Invoke(this, EventArgs.Empty);
	//});
	//}

	//play sounds
	private void OnNeuronPress(object sender, System.EventArgs e) {
		PlaySound(sounds.neuronPress, Camera.allCameras[0].transform.position, sounds.neuronPressVolume);
	}


	//sound manager functions
	private void PlaySound(AudioClip audio, Vector3 position, float VolumeMultiplier = 1f) {
		AudioSource.PlayClipAtPoint(audio, position, VolumeMultiplier * volume);
	}

	public void ChangeVolume(float newVolume) {
		volume = newVolume;
	}

	public float GetVolume() {
		return volume;
	}

	private void OnDestroy() {
		//MainMenuUI.OnButtonPress -= OnButtonPress;
		Neuron.OnNeuronPressed -= OnNeuronPress;
		Neuron.OnNeuronComplete -= Neuron_OnNeuronComplete;
		BackFunction.HardwareBack -= ShelfItem_openHardware;
		Hardware.OnButtonPressed -= Hardware_OnButtonPressed;
		ShelfItem.purchase -= Hardware_OnButtonPressed;
		ShelfItem.openHardware -= ShelfItem_openHardware;
		SettingsMenuManager.onInteract -= Hardware_OnButtonPressed;
	}
}
