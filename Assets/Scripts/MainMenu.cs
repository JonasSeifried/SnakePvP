using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button multiplayerBtn;
    [SerializeField] Button singleplayerBtn;
    [SerializeField] Button settingsBtn;
    [SerializeField] Button exitBtn;
    [SerializeField] Settings settings;

    // Start is called before the first frame update
    void Start()
    {
        multiplayerBtn.onClick.AddListener(() => {
            SnakePvPMultiplayer.playSingleplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        singleplayerBtn.onClick.AddListener(() =>
        {
            SnakePvPMultiplayer.playSingleplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        settingsBtn.onClick.AddListener(() => {
            this.gameObject.SetActive(false);
            settings.gameObject.SetActive(true);
        });
        
        exitBtn.onClick.AddListener(() => {
            SceneManager.LoadScene("SnakeScene");
        });
    }
}
