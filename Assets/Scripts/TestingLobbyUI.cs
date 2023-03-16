using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] Button createGameButton;
    [SerializeField] Button joinGameButton;

        private void Awake() {
            createGameButton.onClick.AddListener(() => {
                SnakePvPMultiplayer.Singleton.StartHost();
                Loader.LoadOnNetwork(Loader.Scene.LoadingScene);
            });
            joinGameButton.onClick.AddListener(() => {
                SnakePvPMultiplayer.Singleton.StartClient();
            });
        }
}
