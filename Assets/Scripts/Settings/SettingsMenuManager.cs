using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour {
    [SerializeField] private Button Open;
    [SerializeField] private GameObject Menu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static EventHandler onInteract;
    void Start() {
        Open.onClick.AddListener(() => {
            Menu.SetActive(flip());
            Interact();
        });
        Menu.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

    }

    bool flip() {
        return !Menu.activeSelf;
    }

    public void Interact() {
        onInteract.Invoke(this, EventArgs.Empty);
    }
}
