using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;
    private bool isGameOver = false;

    public GameOver gameOver;
    private Health playerHealth;

    private void Start()
    {
        remainingTime = 60f;
        playerHealth = FindObjectOfType<Health>();
    }
    void Update()
    {
        if (!isGameOver && playerHealth.HealthValue > 0)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
            }
            else
            {
                remainingTime = 0;
                gameOver.setUp();
                isGameOver = true;
            }

            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else if (playerHealth.HealthValue <= 0)
        {
            isGameOver = true;
            timerText.text = "00:00";
        }

    }
}
