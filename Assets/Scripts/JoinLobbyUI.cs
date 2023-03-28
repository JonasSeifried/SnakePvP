using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class JoinLobbyUI : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button joinLobbyButton;
    [SerializeField] Button cancleButton;



    private void Awake()
    {
        joinLobbyButton.onClick.AddListener(() =>
        {
            SnakePvPLobby.Singleton.JoinLobby(inputField.text);
        });

        cancleButton.onClick.AddListener(() =>
        {
            Hide();
        });
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
