using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SoundsSO : ScriptableObject {
    public AudioClip neuronPress;
    public float neuronPressVolume = .6f;
	public AudioClip buttonPress;
	public float buttonPressVolume = .6f;
	public AudioClip HardwareSwoosh;
	public float HardwareSwooshVolume = .6f;
	public AudioClip coinPickUp;
	public float coinPickupVolume = .6f;

}
