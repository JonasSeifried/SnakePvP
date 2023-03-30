using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

public class Scores : MonoBehaviour {
    
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_Text enemyText;
    [SerializeField] private TMP_Text gameTimerText;
    private int playerScore;
    private int enemyScore;
    private int oldTime = 0;


    private void LateUpdate()
    {
        if (gameObject.activeSelf && GameManager.Singleton.IsGameOver()) Hide();

        if (!GameManager.Singleton.IsInGame()) return;

        if (playerScore != GameManager.Singleton.playerScore || enemyScore != GameManager.Singleton.enemyScore)
        {
            playerScore = GameManager.Singleton.playerScore;
            enemyScore = GameManager.Singleton.enemyScore;
            playerText.text = playerScore.ToString();
            if(!SnakePvPMultiplayer.playSingleplayer) enemyText.text = enemyScore.ToString();
        }
        if(oldTime != Mathf.CeilToInt(GameManager.Singleton.GetGameTimer()))
        {
            oldTime = Mathf.CeilToInt(GameManager.Singleton.GetGameTimer());
            gameTimerText.text = oldTime.ToString();
        }
        

    }

    private void Hide() => gameObject.SetActive(false);
}