using UnityEngine;

public class shelfCanvas : MonoBehaviour {
	public static shelfCanvas Instance { get; private set; }

	private void Awake() {
		Instance = this;
	}
}
