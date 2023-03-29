using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingUI : MonoBehaviour
{
    [SerializeField] TMP_Text messageText;
    [SerializeField] Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {   
            SnakePvPMultiplayer.Singleton.Shutdown(false);
            Hide();
            
        });
        HideCloseButton();
    }
    private void Start()
    {
        SnakePvPLobby.Singleton.OnTryingToJoin += Lobby_OnTryingToJoinGame;
        SnakePvPLobby.Singleton.OnFailedToJoin += Lobby_OnFailedToJoinGame;
        SnakePvPLobby.Singleton.OnLobbyCreation += Lobby_OnLobbyCreation;
        SnakePvPLobby.Singleton.OnLobbyCreationFailed += Lobby_OnLobbyCreationFailed;
        Hide();
    }

    private void Lobby_OnLobbyCreationFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create Lobby!");
    }

    private void Lobby_OnLobbyCreation(object sender, System.EventArgs e)
    {
        ShowMessage("Creating Lobby...");
    }


    private void Lobby_OnTryingToJoinGame(object sender, System.EventArgs e)
    {
        ShowCloseButton();
        ShowMessage("Connecting...");
        closeButton.onClick.AddListener(() =>
        {
            SnakePvPMultiplayer.Singleton.Shutdown(false);
            Hide();
        });
        HideCloseButton();
    }
    private void Lobby_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to connect");
        ShowCloseButton();
    }

    private void OnDestroy()
    {
        SnakePvPMultiplayer.Singleton.OnTryingToJoinGame -= Lobby_OnTryingToJoinGame;
        SnakePvPMultiplayer.Singleton.OnFailedToJoinGame -= Lobby_OnFailedToJoinGame;
        SnakePvPLobby.Singleton.OnLobbyCreation -= Lobby_OnLobbyCreation;
        SnakePvPLobby.Singleton.OnLobbyCreationFailed -= Lobby_OnLobbyCreationFailed;
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }



    void Show() => gameObject.SetActive(true);
    void Hide() => gameObject.SetActive(false);

    void ShowCloseButton() => closeButton.gameObject.SetActive(true);

    void HideCloseButton() => closeButton.gameObject.SetActive(false);
}
