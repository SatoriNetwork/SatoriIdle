using System;
using UnityEngine;

public class BackFunction : MonoBehaviour
{
	public static EventHandler HardwareBack;
	public void Back() {
		HardwareBack.Invoke(this, EventArgs.Empty);
		shelfCanvas.Instance.gameObject.SetActive(true);
	}
}
