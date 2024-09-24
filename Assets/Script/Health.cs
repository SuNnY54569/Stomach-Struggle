using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int health;
    public int HealthValue => health;
    [SerializeField] private int currentHealth;

    [SerializeField] private Image[] hearts;

    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    public GameOver gameOver;


    private void Start()
    {
        health = currentHealth;
        UpdateHeartsUI();
    }

    public void DecreaseHealth(int amount)
    {
        health -= amount;
        UpdateHeartsUI();

        if (health <= 0)
        {
            Score.scoreValue = 0;
            gameOver.setUp();
        }
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < currentHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}

