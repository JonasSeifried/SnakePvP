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
    private const string FAILED_TO_CONNECT = "Failed to connect";

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {   
            SnakePvPMultiplayer.Singleton.Shutdown(false);
            Hide();
        });
    }
    private void Start()
    {
        SnakePvPMultiplayer.Singleton.OnTryingToJoinGame += SnakePvPMultiplayer_OnTryingToJoinGame;
        SnakePvPMultiplayer.Singleton.OnFailedToJoinGame += SnakePvPMultiplayer_OnFailedToJoinGame;
        Hide();
    }

    private void SnakePvPMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if (messageText.text == "") messageText.text = FAILED_TO_CONNECT;
    }

    private void SnakePvPMultiplayer_OnTryingToJoinGame(object sender, System.EventArgs e)
    {
        Show();
    }

    private void OnDestroy()
    {
        SnakePvPMultiplayer.Singleton.OnTryingToJoinGame -= SnakePvPMultiplayer_OnTryingToJoinGame;
        SnakePvPMultiplayer.Singleton.OnFailedToJoinGame -= SnakePvPMultiplayer_OnFailedToJoinGame;
    }



    void Show() => gameObject.SetActive(true);
    void Hide() => gameObject.SetActive(false);
}
