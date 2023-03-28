using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreGameLobbyUI : MonoBehaviour
{
    [SerializeField] TMP_Text lobbyCodeText;
    private void Awake()
    {
        lobbyCodeText.text = "Lobby Code: " + SnakePvPLobby.Singleton.GetLobby().LobbyCode;
    }
}
