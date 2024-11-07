using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

[Serializable]
public class LevelSettings
{
    [Tooltip("The name of the level.")]
    public string levelName;
    [Tooltip("The maximum score required to win this level.")]
    public int maxScore;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool isGamePaused;
    
    #region Health Settings
    [Header("Health Settings")]
    [Tooltip("The maximum health the player can have.")]
    public int maxHealth = 3;
    [Tooltip("The player's current health.")]
    public int currentHealth;
    [SerializeField, Tooltip("Array of heart UI images to display health.")]
    private Image[] hearts;
    [SerializeField, Tooltip("Sprite to represent a full heart.")]
    private Sprite fullHeart;
    [SerializeField, Tooltip("Sprite to represent an empty heart.")]
    private Sprite emptyHeart;
    #endregion

    #region Score Settings
    [Header("Score Settings")]
    [SerializeField,Tooltip("The current score of the player.")]
    private int scoreValue = 0;
    [Tooltip("The maximum score required to win.")]
    public int scoreMax;
    [SerializeField, Tooltip("UI Text element to display score.")]
    private TextMeshProUGUI scoreText;
    [SerializeField, Tooltip("List of level settings to configure max score for each level.")]
    private List<LevelSettings> levelSettings;
    #endregion


    #region Panel Settings
    [Header("Win/Lose Panel")]
    [SerializeField, Tooltip("Panel to display when the game is over.")]
    private GameObject gameOverPanel;
    [SerializeField, Tooltip("Panel to display when the player wins.")]
    private GameObject winPanel;
    #endregion

    #region Tutorial Settings
    [Header("Tutorial")] [SerializeField, Tooltip("Panel to display Tutorial when scene start")]
    public GameObject tutorialPanel;
    [SerializeField, Tooltip("Tutorial Video Manager Script")]
    private TutorialVideoManager tutorialVideoManager;
    #endregion

    #region Total Health Tracking
    [Header("Health Tracking")]
    [Tooltip("Total hearts the player has accumulated.")]
    public int totalHeart;
    [Tooltip("Total remaining hearts.")]
    public int totalHeartLeft;
    
    [Tooltip("Health stats for specific levels or checkpoints.")]
    public int totalHeart1;
    public int totalHeartLeft1;
    
    public int totalHeart2;
    public int totalHeartLeft2;
    
    public int totalHeart3;
    public int totalHeartLeft3;
    #endregion
    
    #region Scene Management
    [Header("Scenes to Deactivate GameManager")]
    [SerializeField, Tooltip("List of scenes where GameManager should be deactivated.")]
    private List<string> scenesToDeactivate;
    [SerializeField, Tooltip("Objects to deactivate in certain scenes.")]
    private List<GameObject> objectsToDeactivate;
    #endregion
    
    #region Post Processing Effects
    [Header("Post Processing")]
    [SerializeField, Tooltip("The PostProcessVolume used for visual effects.")]
    private PostProcessVolume volume;
    [SerializeField, Tooltip("Duration of screen fade when taking damage.")]
    private float fadeDuration = 0.3f;
    [SerializeField, Tooltip("The intensity of the camera shake on damage.")]
    private float shakeIntensity = 0.1f;
    [SerializeField, Tooltip("Current intensity for visual effects.")]
    private float intensity;
    
    private float initialIntensity;
    private Vignette _vignette;
    #endregion

    #region Player Information
    [Header("Player Info")]
    [Tooltip("The name of the player.")]
    public string playerName = "Player";
    [Tooltip("Score before the test.")]
    public int preTestScore;
    [Tooltip("Score after the test.")]
    public int postTestScore;
    #endregion
    
    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        if (volume.profile.TryGetSettings<Vignette>(out _vignette))
        {
            _vignette.enabled.Override(false);
        }
        else
        {
            Debug.LogError("Vignette effect not found");
        }
    }
    
    private void Start()
    {
        initialIntensity = intensity;
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetMaxScoreForLevel(scene.name);
        
        foreach (var obj in objectsToDeactivate)
        {
            obj.SetActive(!scenesToDeactivate.Contains(scene.name));
        }
        
        if (tutorialVideoManager != null)
        {
            VideoClip videoClip = tutorialVideoManager.GetVideoForScene(scene.name);
            if (videoClip != null)
            {
                // Set up the video and show tutorial panel
                tutorialVideoManager.SetupVideoForScene(scene.name);
                tutorialVideoManager.StartVideo();
                tutorialPanel.SetActive(true);
                PauseGame();
            }
            else
            {
                // Hide the tutorial panel and ensure the game is not paused
                tutorialPanel.SetActive(false);
                if (isGamePaused)
                {
                    PauseGame(); // Unpause the game if it was paused
                }
            }
        }
    }
    
    #endregion
    
    #region Health Management
    public void DecreaseHealth(int amount)
    {
        StartCoroutine(TakeDamageEffect());
        currentHealth -= amount;
        UpdateHeartsUI();
        SoundManager.PlaySound(SoundType.Hurt, VolumeType.SFX);

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }
    
    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < currentHealth ? fullHeart : emptyHeart;
            hearts[i].enabled = i < maxHealth;
        }
    }

    private IEnumerator TakeDamageEffect()
    {
        _vignette.enabled.Override(true);
        float elapsedTime = 0f;
        intensity = initialIntensity;
        Transform cameraTransform = Camera.main.transform;
        Vector3 originalPosition = cameraTransform.position;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            intensity = Mathf.Lerp(initialIntensity, 0, elapsedTime / fadeDuration);
            _vignette.intensity.Override(intensity);

            float shakeMagnitude = Mathf.Lerp(shakeIntensity, 0, elapsedTime / fadeDuration);
            cameraTransform.position = originalPosition + Random.insideUnitSphere * shakeMagnitude;

            yield return null;
        }

        _vignette.enabled.Override(false);
        cameraTransform.position = originalPosition;
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

    public void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = scoreValue + $"/{scoreMax}";
    }
    
    public void ResetScore()
    {
        scoreValue = 0;
        UpdateScoreText();
    }
    
    private void SetMaxScoreForLevel(string levelName)
    {
        foreach (LevelSettings settings in levelSettings)
        {
            if (settings.levelName == levelName)
            {
                scoreMax = settings.maxScore;
                return;
            }
        }
        
        Debug.LogWarning($"Max score for {levelName} not set. Using default.");
        scoreMax = 3;
    }
    #endregion
    
    #region Game End States
    public void GameOver()
    {
        ResetScore();
        gameOverPanel.SetActive(true);
    }

    public void WinGame()
    {
        totalHeart += maxHealth;
        totalHeartLeft += currentHealth;
        winPanel.SetActive(true);
    }

    public void ExitToMenu()
    {
        ResetHealth();
        ResetScore();
        totalHeart = 0;
        totalHeartLeft = 0;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }
    #endregion

    #region Utility Functions
    public void PauseGame()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f;
    }

    public void SetScoreTextActive(bool isActive)
    {
        scoreText.gameObject.SetActive(isActive);
    }

    public void CloseAllPanel()
    {
        winPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void SetMaxScore(int maxScore)
    {
        scoreMax = maxScore;
    }

    public int GetSumTotalHeartLeft()
    {
        return totalHeartLeft1 + totalHeartLeft2 + totalHeartLeft3;
    }

    public int GetSumTotalHeart()
    {
        return totalHeart1 + totalHeart2 + totalHeart3;
    }

    public void ResetTotalHeart()
    {
        totalHeart = 0;
        totalHeartLeft = 0;
    }

    public void ResetAllTotalHeart()
    {
        ResetTotalHeart();
        totalHeart1 = 0;
        totalHeart2 = 0;
        totalHeart3 = 0;
        totalHeartLeft1 = 0;
        totalHeartLeft2 = 0;
        totalHeartLeft3 = 0;
    }

    public void ResetPrePostTest()
    {
        preTestScore = 0;
        postTestScore = 0;
    }
    #endregion
}
