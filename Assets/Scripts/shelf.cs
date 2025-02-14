using UnityEngine;

public class shelf : MonoBehaviour {
	public static shelf Instance { get; private set; }

	private void Awake() {
		Instance = this;
	}
}
