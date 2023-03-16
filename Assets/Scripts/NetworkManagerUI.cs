using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake() {
        serverButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            HideAll();
        });
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            HideAll();
        });
        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            HideAll();
        });
    }


    private void HideAll() {
        serverButton.gameObject.SetActive(false);
        hostButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
    }
}
