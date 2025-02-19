using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour {
	public static GameManager instance { get; private set; }

	[SerializeField] public BGN SatoriPoints = new BGN(4);
	[SerializeField] TextMeshProUGUI SPText;


	private void Start() {
		instance = this;
	}

	public void addPoints(BGN sp) {
		SatoriPoints += sp;
	}

	private void FixedUpdate() {
		SPText.text = SatoriPoints.ToString();
	}

}
