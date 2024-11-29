using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountTime : MonoBehaviour
{
    [SerializeField] private float totalTime;
    private float timeRemaining;

    public TextMeshProUGUI timerText;

    private bool timerIsRunning = false;

    public GameOver gameOver;

    void Start()
    {
        timeRemaining = totalTime;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            timeRemaining -= Time.deltaTime;

            DisplayTime(timeRemaining);

            if (timeRemaining <= 0)
            {
                timerIsRunning = false;
                gameOver.setUp();

            }
        }
    }

    public void StartCountdown()
    {
        timerIsRunning = true;
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timerText != null)
        {
            timerText.text = Mathf.FloorToInt(timeToDisplay).ToString();
        }
    }
}
