using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime;
    [SerializeField] private GameObject clockGameObject;
    public bool isGameOver = false;
    private bool isPopDown;

    private Vector3 initialClockScale;

    private void Awake()
    {
        isPopDown = false;
        initialClockScale = clockGameObject.transform.localScale;
    }

    private void Start()
    {
        remainingTime = 30f;
    }
    
    private void OnEnable()
    {
        GameManager.OnGamePaused += HandleGamePause;
        GameManager.OnGameUnpaused += HandleGameUnpause;
    }

    private void OnDisable()
    {
        GameManager.OnGamePaused -= HandleGamePause;
        GameManager.OnGameUnpaused -= HandleGameUnpause;
    }

    private void Update()
    {
        if (GameManager.Instance.currentHealth <= 0)
        {
            if (!isGameOver)
            {
                isGameOver = true;
                timerText.text = "00";
                HandleGameOver();
            }
            return;
        }
        
        if (!isGameOver && GameManager.Instance.currentHealth > 0)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
            }
            else
            {
                remainingTime = 0;
                GameManager.Instance.WinGame();
                isGameOver = true;
                timerText.gameObject.SetActive(false);
            }

            UpdateTimerText();
        }
    }

    private void HandleGamePause()
    {
        if (isGameOver) return;
        LeanTween.scale(clockGameObject, Vector3.zero, 0.2f)
            .setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => { clockGameObject.SetActive(false); });
    }

    private void HandleGameUnpause()
    {
        if (isGameOver) return;
        clockGameObject.SetActive(true);
        clockGameObject.transform.localScale = Vector3.zero;
        LeanTween.scale(clockGameObject, initialClockScale, 0.3f)
            .setEase(LeanTweenType.easeOutBack)
            .setDelay(0.5f)
            .setIgnoreTimeScale(true);
    }
    
    private void HandleGameOver()
    {
        if (isPopDown) return;
        LeanTween.scale(clockGameObject, Vector3.zero, 0.5f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => { clockGameObject.SetActive(false); });
        isPopDown = true;
    }
    
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{00}", seconds);
    }
}
