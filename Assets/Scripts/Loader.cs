using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene {
        MainMenuScene,
        LobbyScene,
        PreGameLobbyScene,
        SnakeScene,
    }


    public static void Load(Scene targetScene) {

        SceneManager.LoadScene(targetScene.ToString());
    }

    public static void LoadOnNetwork(Scene targetScene) {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

}
