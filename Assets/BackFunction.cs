using UnityEngine;

public class BackFunction : MonoBehaviour
{

	public void Back() {
		shelfCanvas.Instance.gameObject.SetActive(true);
	}
}
