using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

public class Scores : MonoBehaviour {
    
    [SerializeField] private TMP_Text sP1;
    [SerializeField] private TMP_Text sP2;

    public void UpdateOwnText(int score) {
        sP1.text = score + "/20";
    }
    public void UpdateEnemyText(int score) {
        sP2.text = "Enemy: " + score + "/20";
    }
    

}