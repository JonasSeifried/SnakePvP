using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

public class OverlayUI : MonoBehaviour {
    
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_Text enemyText;
    [SerializeField] private TMP_Text gameTimerText;
    [SerializeField] private TMP_Text infoText;
    private int playerScore;
    private int enemyScore;
    private int currTime = GameManager.gameTimerMax;
    private int closeMessageAfter = 0;
    private Animator infoTextanimator;

    private void Awake()
    {
        infoTextanimator = infoText.GetComponent<Animator>();
        HideMessage();
    }


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
        if(currTime != Mathf.CeilToInt(GameManager.Singleton.GetGameTimer()))
        {
            currTime = Mathf.CeilToInt(GameManager.Singleton.GetGameTimer());
            gameTimerText.text = currTime.ToString();

            if(currTime % 30  == 0)
            {
                ShowMessage("Snake speed x 1.25", 3);
            }

            if (currTime == closeMessageAfter) HideMessage();
        }
        

    }

    private void Hide() => gameObject.SetActive(false);


    public void ShowMessage(string message, int duration)
    {
        infoTextanimator.SetTrigger("Start");
        infoText.gameObject.SetActive(true);

        infoText.text = message;
        closeMessageAfter = Math.Max(currTime - duration, 0);

    }

    private void HideMessage()
    {
        infoTextanimator.StopPlayback();
        infoText.gameObject?.SetActive(false);
    }
}