using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class LoadingPlayerReady : NetworkBehaviour
{

    public static LoadingPlayerReady Singleton;
    private Dictionary<ulong, bool> playerReadyDict;

    public event EventHandler OnAllPlayersReady;
    public bool allPlayersReady { get; private set; }  = false;

    private void Awake() {
        Assert.IsNull(Singleton, $"Multiple instances of {nameof(LoadingPlayerReady)} detected. This should not happen.");
        Singleton = this;
        playerReadyDict = new Dictionary<ulong, bool>();
    }

    public override void OnNetworkSpawn()
    {
        SetLocalPlayerReadyServerRpc();
        if(!IsServer) allPlayersReady = true;
        base.OnNetworkSpawn();
    }    

    [ServerRpc(RequireOwnership = false)]
    private void SetLocalPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playerReadyDict[serverRpcParams.Receive.SenderClientId] = true;

        if(playerReadyDict.Count == 2) {
            allPlayersReady = true;
            OnAllPlayersReady.Invoke(this, EventArgs.Empty);
        }
    }

    public void StartGame()
    {
        SnakePvPLobby.Singleton.DeleteLobby();
        Loader.LoadOnNetwork(Loader.Scene.SnakeScene);
    }
}
