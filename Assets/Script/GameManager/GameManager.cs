using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [Header("Score Settings")]
    public int scoreValue = 0;
    public int scoreMax;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Win, lose Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }
    
    #region Health Management
    public void DecreaseHealth(int amount)
    {
        currentHealth -= amount;
        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }
    
    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            hearts[i].enabled = i < maxHealth;
        }
    }
    #endregion
    
    #region Score Management
    public void IncreaseScore(int amount)
    {
        scoreValue += amount;
        UpdateScoreText();
        if (scoreValue >= scoreMax)
        {
            WinGame();
        }
    }

    public int GetScore()
    {
        return scoreValue;
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = scoreValue + $"/{scoreMax}";
    }
    #endregion
    
    private void GameOver()
    {
        scoreValue = 0;
        currentHealth = maxHealth;
        UpdateHeartsUI();
        UpdateScoreText();
        gameOverPanel.SetActive(true);
    }

    private void WinGame()
    {
        scoreValue = 0;
        currentHealth = maxHealth;
        UpdateHeartsUI();
        UpdateScoreText();
        winPanel.SetActive(true);
    }
}
