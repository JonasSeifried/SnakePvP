using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button createGameButton;
    [SerializeField] Button joinGameButton;
    [SerializeField] JoinLobbyUI createLobbyUI;

        private void Awake() {
            createGameButton.onClick.AddListener(() => {
                SnakePvPLobby.Singleton.CreateLobby("1", false);
                //createLobbyUI.Show();
                //SnakePvPMultiplayer.Singleton.StartHost();
                //Loader.LoadOnNetwork(Loader.Scene.LoadingScene);
            });
            joinGameButton.onClick.AddListener(() => {
                createLobbyUI.Show();
            });
        }
}
