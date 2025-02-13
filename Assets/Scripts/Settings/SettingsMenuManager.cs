using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    [SerializeField] private Button Open;
    [SerializeField] private Button Close;
    [SerializeField] private GameObject Menu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Open.onClick.AddListener(() => {
            Menu.SetActive(true);
        });
		Close.onClick.AddListener(() => {
			Menu.SetActive(false);
		});
        Menu.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
