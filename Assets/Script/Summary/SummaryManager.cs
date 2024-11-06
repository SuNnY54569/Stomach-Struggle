using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SummaryManager : MonoBehaviour
{
    [SerializeField] private int totalHeartLeft;
    [SerializeField] private int totalHeart;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text healthConditionText;
    [SerializeField] private string goodHealthText;
    [SerializeField] private string mediumHealthText;
    [SerializeField] private string badHealthText;
    [SerializeField] private string retrySceneName;
    
    private const string MAIN_MENU_SCENE = "Start scene";

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            totalHeart = GameManager.Instance.totalHeart;
            totalHeartLeft = GameManager.Instance.totalHeartLeft;
            UpdateHealthUI();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }
    
    private void UpdateHealthUI()
    {
        scoreText.text = $"{totalHeartLeft}/{totalHeart}";
        
        float healthRatio = (float)totalHeartLeft / totalHeart;

        if (healthRatio >= 0.75f)
        {
            healthConditionText.text = goodHealthText;
        }
        else if (healthRatio >= 0.50f)
        {
            healthConditionText.text = mediumHealthText;
        }
        else
        {
            healthConditionText.text = badHealthText;
        }
    }

    public void NextLevel()
    {
        if (GameManager.Instance == null) return;
        
        string currentScene = SceneManager.GetActiveScene().name;
        switch (currentScene)
        {
            case "Summary1":
                GameManager.Instance.totalHeart1 = totalHeart;
                GameManager.Instance.totalHeartLeft1 = totalHeartLeft;
                break;
            case "Summary2":
                GameManager.Instance.totalHeart2 = totalHeart;
                GameManager.Instance.totalHeartLeft2 = totalHeartLeft;
                break;
            case "Summary3":
                GameManager.Instance.totalHeart3 = totalHeart;
                GameManager.Instance.totalHeartLeft3 = totalHeartLeft;
                break;
        }

        GameManager.Instance.totalHeart = 0;
        GameManager.Instance.totalHeartLeft = 0;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameManager.Instance.gameObject.SetActive(true);
    }

    public void RetryDay()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.totalHeart = 0;
            GameManager.Instance.totalHeartLeft = 0;
            SceneManager.LoadScene(retrySceneName);
        }
    }

    public void ToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.totalHeart = 0;
            GameManager.Instance.totalHeartLeft = 0;
            SceneManager.LoadScene(MAIN_MENU_SCENE);
        }
    }
}
