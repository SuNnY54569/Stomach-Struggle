using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    #region Score Settings
    [Header("Score Settings")]
    [SerializeField,Tooltip("The current score of the player.")]
    private int scoreValue = 0;
    
    [Tooltip("The maximum score required to win.")]
    public int scoreMax;

    [SerializeField, Tooltip("Score GameObject")]
    public GameObject scoreGameObject;
    
    [SerializeField, Tooltip("UI Text element to display score.")]
    private TextMeshProUGUI scoreText;
    
    [SerializeField, Tooltip("List of level settings to configure max score for each level.")]
    private List<LevelSettings> levelSettings;
    
    [SerializeField, Tooltip("List of scene names where the scoreGameObject should be visible.")]
    private List<string> scenesToShowScoreGameObject;
    #endregion
    
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateScoreGameObjectVisibility(SceneManager.GetActiveScene().name);
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateScoreGameObjectVisibility(scene.name);
    }
    
    private void UpdateScoreGameObjectVisibility(string sceneName)
    {
        if (scoreGameObject != null)
        {
            scoreGameObject.SetActive(scenesToShowScoreGameObject.Contains(sceneName));
        }
    }
    
    #region Score Management
    public void IncreaseScore(int amount)
    {
        scoreValue += amount;
        UpdateScoreText();
        if (scoreValue >= scoreMax) GameManager.Instance.healthManager.WinGame();
    }

    public int GetScore()
    {
        return scoreValue;
    }

    public void UpdateScoreText()
    {
        scoreText.text = $"{scoreValue}/{scoreMax}";
    }

    public void ResetScore()
    {
        scoreValue = 0;
        UpdateScoreText();
    }
    
    public void SetMaxScoreForLevel(string levelName)
    {
        var level = levelSettings.FirstOrDefault(l => l.levelName == levelName);
        scoreMax = level?.maxScore ?? 3;
    }
    #endregion
}
