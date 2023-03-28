using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class SnakePvPLobby : MonoBehaviour
{
    public static SnakePvPLobby Singleton { get; private set; }

    private Lobby joinedLobby;
    private const float heartbeatTimerMax = 15;
    private float heartbeatTimer = heartbeatTimerMax;

    private void Awake()
    {
        Singleton = this;

        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {

            InitializationOptions options = new InitializationOptions();
            options.SetProfile(Random.Range(0, 1000).ToString());
        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, SnakePvPMultiplayer.MAX_PLAYER, new CreateLobbyOptions { IsPrivate = isPrivate, });
            SnakePvPMultiplayer.Singleton.StartHost();
            Loader.LoadOnNetwork(Loader.Scene.PreGameLobbyScene);
        }
        catch (LobbyServiceException e)
        {

            Debug.LogError(e.Message);
        }
    }

    public async void JoinLobby(string lobbyCode)
    {
        try
        {
            Debug.Log(lobbyCode);
            await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            SnakePvPMultiplayer.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {

            Debug.LogError(e.Message);
        }

    }


    private void Update()
    {
        HandleLobbyHeartbeat();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (joinedLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0)
            {
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }
}
