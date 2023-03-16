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
    private void Awake() {
        Assert.IsNull(Singleton, $"Multiple instances of {nameof(SnakePvPMultiplayer)} detected. This should not happen.");
        Singleton = this;

        DontDestroyOnLoad(this);
    }

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApproval;
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient() {
        NetworkManager.Singleton.StartClient();
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
}
