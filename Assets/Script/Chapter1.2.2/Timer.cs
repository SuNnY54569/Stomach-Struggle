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
        clockGameObject.SetActive(true); // Ensure the panel is active
        clockGameObject.transform.localScale = Vector3.zero; // Start from zero scale
        LeanTween.scale(clockGameObject, initialClockScale, 0.5f)
            .setEase(LeanTweenType.easeOutBack);
    }

    private void Update()
    {
        clockGameObject.SetActive(!GameManager.Instance.isGamePaused);

        if (GameManager.Instance.currentHealth <= 0)
        {
            isGameOver = true;
            timerText.text = "00";
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

            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{00}", seconds);
        }

        switch (isGameOver)
        {
            case true when isPopDown:
                return;
            case true:
                LeanTween.scale(clockGameObject, Vector3.zero, 0.5f)
                    .setEase(LeanTweenType.easeInOutQuad).setIgnoreTimeScale(true)
                    .setOnComplete(() => { clockGameObject.SetActive(false); });
                isPopDown = true;
                break;
            case false:
                clockGameObject.SetActive(!GameManager.Instance.isGamePaused);
                break;
        }
    }
}
