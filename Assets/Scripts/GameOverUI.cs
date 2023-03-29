using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] Button playAgainButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] TMP_Text headlineText;
    [SerializeField] TMP_Text mainText;

    private const string WIN_TEXT = "You won!";
    private const string LOSE_TEXT = "You lost";


    void Start()
    {
        GameManager.Singleton.OnStateChanged += OnGameStateChanged;

        playAgainButton.onClick.AddListener(() => {
            SnakePvPMultiplayer.Singleton.Restart();
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            SnakePvPMultiplayer.Singleton.Shutdown(true);
             
        });
        if(SnakePvPMultiplayer.playSingleplayer) playAgainButton.gameObject.SetActive(false);
        Hide();

    }



    private void OnGameStateChanged(object sender, EventArgs e)
    {
        if(GameManager.Singleton.IsGameOver())
        {
            headlineText.text = GameManager.Singleton.IsWinner() ? WIN_TEXT : LOSE_TEXT;
            Show();
        }
        
    }

    private void Hide() { gameObject.SetActive(false); }

    private void Show() { gameObject.SetActive(true); }
}
