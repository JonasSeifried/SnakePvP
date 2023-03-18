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
    [SerializeField] TMP_Text headline;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Singleton.OnStateChanged += OnGameStateChanged;

        playAgainButton.onClick.AddListener(() => {
            Loader.LoadOnNetwork(Loader.Scene.LoadingScene);
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
             
        });
        Hide();

    }



    private void OnGameStateChanged(object sender, EventArgs e)
    {
        if(GameManager.Singleton.IsGameOver())
        {
            Show();
        }
        
    }

    private void Hide() { gameObject.SetActive(false); }

    private void Show() { gameObject.SetActive(true); }
}
