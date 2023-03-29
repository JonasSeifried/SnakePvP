using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button createGameButton;
    [SerializeField] Button joinGameButton;
    [SerializeField] TMP_InputField joinGameInputField;
    private Regex inputRegex = new("[A-Za-z0-9]+");

        private void Awake() {
            mainMenuButton.onClick.AddListener(() =>
            {
                Loader.Load(Loader.Scene.MainMenuScene);
            });
            createGameButton.onClick.AddListener(() => {
                SnakePvPLobby.Singleton.CreateLobby("1", false);
            });
            joinGameButton.onClick.AddListener(() => {
                if(joinGameInputField.text.Length == 0)
                {
                    return;
                }
                SnakePvPLobby.Singleton.JoinLobby(joinGameInputField.text);
            });

            joinGameInputField.onValidateInput += delegate (string s, int i, char c) 
            {
                if (!inputRegex.IsMatch(c.ToString())) return '\0';
                return char.ToUpper(c); };
    }
}
