using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button playBtn;
    [SerializeField] Button settingsBtn;
    [SerializeField] Button exitBtn;
    [SerializeField] Settings settings;

    // Start is called before the first frame update
    void Start()
    {
        playBtn.onClick.AddListener(() => {
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
