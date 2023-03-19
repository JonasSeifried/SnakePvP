using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class SnakePvPMultiplayer : NetworkBehaviour
{
    private const int MAX_PLAYER = 2;

    public static SnakePvPMultiplayer Singleton;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    private NetworkList<PlayerData> playerDataNetworkList;
    private void Awake() {

        Assert.IsNull(Singleton, $"Multiple instances of {nameof(SnakePvPMultiplayer)} detected. This should not happen.");

        Singleton = this;

        DontDestroyOnLoad(this);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApproval;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }



    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
        });
    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {

    }

    public void StartClient() {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    private void ConnectionApproval(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if(SceneManager.GetActiveScene().name != Loader.Scene.LoadingScene.ToString()) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game already running!";
        }

        if(NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
        }

        connectionApprovalResponse.Approved = true;
    }

    public Nullable<PlayerData> GetPlayerData(ulong clientId) 
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId) return playerData;
        }

        return null;
    }

    public void Shutdown(bool MainMenu)
    {
        NetworkManager.Singleton.Shutdown();
        if (MainMenu)
        {
        Loader.Load(Loader.Scene.MainMenuScene);
        }
    }

    public void Restart() => RestartServerRpc();

    [ServerRpc(RequireOwnership = false)]
    private void RestartServerRpc() => Loader.LoadOnNetwork(Loader.Scene.LoadingScene);
}
