using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null) Destroy(NetworkManager.Singleton.gameObject);
        if (SnakePvPMultiplayer.Singleton != null) Destroy(SnakePvPMultiplayer.Singleton.gameObject);
        if(SnakePvPLobby.Singleton != null) Destroy(SnakePvPLobby.Singleton.gameObject);
    }
}
