using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour {
	public static GameManager instance { get; private set; }

	[SerializeField] double SatoriPoints;
	[SerializeField] int ENMultiplier = 1; // SatoriPoints * 10^ENMultiplier // this is to handle larger numbers then the integer limit

	private void Start() {
		instance = this;
	}

	public void addPoints(double sp, int spEN) {
		SatoriPoints += sp;
	}

}
