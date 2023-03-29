using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PreGameLobbyUI : MonoBehaviour
{
    [SerializeField] TMP_Text lobbyCodeText;
    [SerializeField] TMP_Text statusText;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button startGameButton;

    private const string ALL_PLAYERS_READY_MSG = "All Players are Ready.";
    private const string HOST_LEFT_MSG = "The Host left the Lobby";
    private const string WAITING_PLAYER_MSG = "waiting for other player..";
    private void Awake()
    {
        if(SnakePvPLobby.Singleton.GetLobby() != null)
        {
            lobbyCodeText.text = "Lobby Code: " + SnakePvPLobby.Singleton.GetLobby().LobbyCode;
            statusText.text = WAITING_PLAYER_MSG;
        }        

        if (NetworkManager.Singleton.IsServer)
        {
            startGameButton.onClick.AddListener(() =>
            {
                LoadingPlayerReady.Singleton.StartGame();
            });
        }

        mainMenuButton.onClick.AddListener(() =>
        {
            SnakePvPLobby.Singleton.LeaveLobby();
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        startGameButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        LoadingPlayerReady.Singleton.OnAllPlayersReady += Singleton_OnAllPlayersReady;
        //SnakePvPLobby.Singleton.OnLobbyDeleted += SnakePvPLobby_OnLobbyDeleted;
        //SnakePvPLobby.Singleton.OnPlayerLeavedLobby += SnakePvPLobby_OnPlayerLeavedLobby;
    }

    private void SnakePvPLobby_OnPlayerLeavedLobby(object sender, System.EventArgs e)
    {
        statusText.text = WAITING_PLAYER_MSG;
    }

    private void SnakePvPLobby_OnLobbyDeleted(object sender, System.EventArgs e)
    {
        statusText.text = HOST_LEFT_MSG;
        lobbyCodeText.text = "";
    }

    private void Singleton_OnAllPlayersReady(object sender, System.EventArgs e)
    {
        if(NetworkManager.Singleton.IsServer)
        {
        startGameButton.gameObject.SetActive(true);
        }
        statusText.text = ALL_PLAYERS_READY_MSG;
    }
}
