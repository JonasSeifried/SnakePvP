using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class LoadingPlayerReady : NetworkBehaviour
{

    public static LoadingPlayerReady Singleton;
    private Dictionary<ulong, bool> playerReadyDict;

    private void Awake() {
        Assert.IsNull(Singleton, $"Multiple instances of {nameof(LoadingPlayerReady)} detected. This should not happen.");
        Singleton = this;
        playerReadyDict = new Dictionary<ulong, bool>();
    }

    public override void OnNetworkSpawn()
    {
        SetLocalPlayerReadyServerRpc();
        base.OnNetworkSpawn();
    }

    private void Update() {
        //Debug
        if(IsServer && Input.GetKeyDown(KeyCode.Escape)) {
            Loader.LoadOnNetwork(Loader.Scene.SnakeScene);
        }
    }
    

    [ServerRpc(RequireOwnership = false)]
    private void SetLocalPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        playerReadyDict[serverRpcParams.Receive.SenderClientId] = true;

        if(playerReadyDict.Count == 2) {
            Loader.LoadOnNetwork(Loader.Scene.SnakeScene);
        }
    }
}
