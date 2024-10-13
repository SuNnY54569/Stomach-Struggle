using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SummaryManager : MonoBehaviour
{
    [SerializeField] private int totalMaxHeart;
    [SerializeField] private int totalHeart;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text healthConditionText;
    [SerializeField] private string goodHealthText;
    [SerializeField] private string mediumHealthText;
    [SerializeField] private string badHealthText;

    private void Start()
    {
        totalMaxHeart = GameManager.Instance.totalHeart;
        totalHeart = GameManager.Instance.totalHeartLeft;
        GameManager.Instance.gameObject.SetActive(false);
        UpdateHealthUI();
    }
    
    private void UpdateHealthUI()
    {
        // Update the score display
        scoreText.text = $"{totalHeart}/{totalMaxHeart}";

        // Determine the health condition text based on the health ratio
        float healthRatio = (float)totalHeart / totalMaxHeart;

        if (healthRatio >= 0.75f)
        {
            // Good health condition (>= 3/4 of totalMaxHeart)
            healthConditionText.text = goodHealthText;
        }
        else if (healthRatio >= 0.50f)
        {
            // Medium health condition (>= 1/4 of totalMaxHeart)
            healthConditionText.text = mediumHealthText;
        }
        else
        {
            // Bad health condition (< 1/4 of totalMaxHeart)
            healthConditionText.text = badHealthText;
        }
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameManager.Instance.gameObject.SetActive(true);
    }
}
