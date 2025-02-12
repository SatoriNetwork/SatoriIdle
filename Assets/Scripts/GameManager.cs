using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour {
	public static GameManager instance { get; private set; }

	BGN SatoriPoints = new BGN();

	private void Start() {
		instance = this;
	}

	public void addPoints(BGN sp) {
		SatoriPoints += sp;
	}

}
