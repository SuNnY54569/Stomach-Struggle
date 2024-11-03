using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime;
    public bool isGameOver = false;

    private void Start()
    {
        remainingTime = 30f;
    }

    private void Update()
    {
        if (GameManager.Instance.currentHealth <= 0)
        {
            isGameOver = true;
            timerText.text = "00:00";
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
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

    }
}
