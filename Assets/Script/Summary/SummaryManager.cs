using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SummaryManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Health Settings")]
    [SerializeField, Tooltip("The total amount of hearts the player can have.")]
    private int totalHeartLeft;
    [SerializeField, Tooltip("The total number of hearts the player started with.")]
    private int totalHeart;

    [Header("UI Elements")]
    [SerializeField, Tooltip("The text element that displays the score.")]
    private TMP_Text scoreText;
    [SerializeField, Tooltip("The text element that displays the health condition.")]
    private TMP_Text healthConditionText;

    [Header("Health Condition Texts")]
    [SerializeField, Tooltip("The text displayed when the player has good health.")]
    private string goodHealthText;
    [SerializeField, Tooltip("The text displayed when the player has medium health.")]
    private string mediumHealthText;
    [SerializeField, Tooltip("The text displayed when the player has bad health.")]
    private string badHealthText;

    [Header("Scene Management")]
    [SerializeField, Tooltip("The name of the scene used to retry the current day.")]
    private string retrySceneName;
    private const string MAIN_MENU_SCENE = "Start scene";
    #endregion

    #region Unity Lifecycle
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
    #endregion
    
    #region UI Update
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
    #endregion

    #region Level Navigation
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

        GameManager.Instance.ResetTotalHeart();
        SceneManagerClass.Instance.LoadNextScene();
        GameManager.Instance.gameObject.SetActive(true);
    }

    public void RetryDay()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetTotalHeart();
            SceneManagerClass.Instance.LoadThisScene(retrySceneName);
        }
    }

    public void ToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetAllTotalHeart();
            SceneManagerClass.Instance.LoadMenuScene();
        }
    }
    #endregion
}
