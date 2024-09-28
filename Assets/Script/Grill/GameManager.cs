using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int health = 100;
    public int score = 0;
    public int targetScore = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void DecreaseHealth(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Debug.Log("Game Over! You ran out of health.");
            // End the game
        }
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        if (score >= targetScore)
        {
            Debug.Log("You win! You've served enough dishes.");
            // Win the game
        }
    }
}
