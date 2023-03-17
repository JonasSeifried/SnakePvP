using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

public class Scores : MonoBehaviour {
    
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_Text enemyText;
    private int playerScore;
    private int enemyScore;

    private void LateUpdate()
    {
        if (playerScore != GameManager.Singleton.playerScore || enemyScore != GameManager.Singleton.enemyScore) {
            playerScore = GameManager.Singleton.playerScore;
            enemyScore = GameManager.Singleton.enemyScore;
        }
            playerText.text = playerScore.ToString();
            enemyText.text = enemyScore.ToString();
    }

}