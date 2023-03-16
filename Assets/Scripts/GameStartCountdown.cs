using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameStartCountdown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countdownText;

    private Animator animator;
    private const String NUMBER_WIGGLE = "NumberWiggle";
    private int prevCountdownNumber;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        GameManager.Singleton.OnStateChanged += OnStateChanged;
        Hide(); 
    }


    private void OnStateChanged(object sender, EventArgs e)
    {
        if(GameManager.Singleton.IsCountingDown())
        {
            Show();
            return;
        }
        Hide();
    }

    private void Update() {
        if(!GameManager.Singleton.IsCountingDown()) return;
        int countdown = Mathf.CeilToInt(GameManager.Singleton.GetCountdownTimer());
        countdownText.text = countdown.ToString();
        if (countdown != prevCountdownNumber) {
            prevCountdownNumber = countdown;
            animator.SetTrigger(NUMBER_WIGGLE);
        }
    }

    private void Show() { this.gameObject.SetActive(true); }
    private void Hide() { this.gameObject.SetActive(false); }
}
