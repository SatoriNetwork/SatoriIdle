using UnityEngine;

public class BackFunction : MonoBehaviour
{

	public void Back() {
		shelf.Instance.gameObject.SetActive(true);
	}
}
