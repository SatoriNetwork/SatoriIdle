using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {
	public static SettingsManager instance { get; private set; }
	public float masterVolume { get; private set; } = 1f;
	public float sfxVolume { get; private set; } = 1f;
	public float musicVolume { get; private set; } = 1f;
	public const string PLAYER_PREF_MASTER_VOLUME = "MASTERVOLUME";
	public const string PLAYER_PREF_SFX_VOLUME = "SFXVOLUME";
	public const string PLAYER_PREF_MUSIC_VOLUME = "MUSICVOLUME";
	private const string PLAYER_PREF_QUALITY = "QUALITY";

	[SerializeField] Slider MasterSlider;
	[SerializeField] Slider SFXSlider;
	[SerializeField] Slider MusicSlider;

	private void Awake() {
		instance = this;
		sfxVolume = PlayerPrefs.GetFloat(PLAYER_PREF_SFX_VOLUME, 1f);
		masterVolume = PlayerPrefs.GetFloat(PLAYER_PREF_MASTER_VOLUME, 1f);
		musicVolume = PlayerPrefs.GetFloat(PLAYER_PREF_MUSIC_VOLUME, 1f);
	}

	private void Start() {
		SoundManager.instance.ChangeVolume(masterVolume * sfxVolume);
		MusicManager.instance.ChangeVolume(masterVolume * musicVolume);
		SetQuality(PlayerPrefs.GetInt(PLAYER_PREF_QUALITY, 2));
		MasterSlider.value = masterVolume;
		SFXSlider.value = sfxVolume;
		MusicSlider.value = musicVolume;
	}

	public void SetMasterVolume(float Volume) {
		masterVolume = Volume;
		PlayerPrefs.SetFloat(PLAYER_PREF_MASTER_VOLUME, masterVolume);
		PlayerPrefs.Save();
		MusicManager.instance.ChangeVolume(masterVolume * musicVolume);
		SoundManager.instance.ChangeVolume(masterVolume * sfxVolume);
	}

	public void SetMusicVolume(float Volume) {
		musicVolume = Volume;
		PlayerPrefs.SetFloat(PLAYER_PREF_MUSIC_VOLUME, musicVolume);
		PlayerPrefs.Save();
		MusicManager.instance.ChangeVolume(masterVolume * musicVolume);
	}

	public void SetSFXVolume(float Volume) {
		sfxVolume = Volume;
		PlayerPrefs.SetFloat(PLAYER_PREF_SFX_VOLUME, sfxVolume);
		PlayerPrefs.Save();
		SoundManager.instance.ChangeVolume(masterVolume * sfxVolume);
	}

    public void SetQuality (int quality) {
        QualitySettings.SetQualityLevel(quality);
		PlayerPrefs.SetInt(PLAYER_PREF_QUALITY, quality);
		PlayerPrefs.Save();
    }

}
