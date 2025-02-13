using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager instance { get; private set; }
	[SerializeField] private AudioSource music;
	[SerializeField] private float baseVolume;
	private float volume = 1f;


	private void Awake() {
		instance = this;
	}

	public void ChangeVolume(float Volume) {
		volume = Volume;
		//music.volume = volume * baseVolume;
	}

	public float GetVolume() {
		return volume;
	}
}
