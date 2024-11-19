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
    public bool isBlurEnabled;
    
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

    [SerializeField, Tooltip("Score GameObject")]
    private GameObject scoreGameObject;
    
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

    [SerializeField, Tooltip("Panel to display pause menu")]
    private GameObject pausePanel;
    
    #endregion

    #region Tutorial Settings
    
    [Header("Tutorial")] [SerializeField, Tooltip("Panel to display Tutorial when scene start")]
    public GameObject tutorialPanel;

    [SerializeField] public GameObject gameplayPanel;
    
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
    private DepthOfField _depthOfField;
    private ColorGrading _colorGrading;
    
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

        if (volume.profile.TryGetSettings<DepthOfField>(out _depthOfField))
        {
            _depthOfField.enabled.Override(false);
        }
        
        if (volume.profile.TryGetSettings<ColorGrading>(out _colorGrading))
        {
            _colorGrading.enabled.Override(false);
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
                tutorialVideoManager.SetupVideoForScene(scene.name);
                tutorialVideoManager.StartVideo();
                tutorialPanel.SetActive(true);
                gameplayPanel.gameObject.SetActive(false);
                PauseGame();
            }
            else
            {
                Debug.Log("No Video");
                tutorialPanel.SetActive(false);
                if (isGamePaused)
                {
                    PauseGame();
                }
            }
        }
    }
    
    #endregion
    
    #region Health Management
    public void DecreaseHealth(int amount)
    {
        currentHealth -= amount;
        UpdateHeartsUI();
        SoundManager.PlaySound(SoundType.Hurt, VolumeType.SFX);

        if (currentHealth <= 0)
        {
            GameOver();
            return;
        }
        StartCoroutine(TakeDamageEffect());
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
        
        //Debug.Log($"Max score for {levelName} not set. Using default.");
        scoreMax = 3;
    }
    #endregion
    
    #region Game End States
    public void GameOver()
    {
        ResetScore();
        SoundManager.PlaySound(SoundType.Lose,VolumeType.SFX);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        PauseGame();
    }

    public void WinGame()
    {
        totalHeart += maxHealth;
        totalHeartLeft += currentHealth;
        
        SoundManager.PlaySound(SoundType.Win,VolumeType.SFX);
        gameplayPanel.SetActive(false);
        winPanel.SetActive(true);
        PauseGame();
    }

    public void ExitToMenu(Button button)
    {
        ResetHealth();
        ResetScore();
        ResetAllTotalHeart();
        SceneManagerClass.Instance.LoadMenuScene();
        StartCoroutine(waitForSecond(button));
    }

    public void NextScene(Button button)
    {
        ResetHealth();
        ResetScore();
        SceneManagerClass.Instance.LoadNextScene();
        StartCoroutine(waitForSecond(button));
    }

    private IEnumerator waitForSecond(Button button, float second = 1f)
    {
        PauseGame();
        BlurBackGround();
        button.interactable = false;
        yield return new WaitForSeconds(second);
        BlurBackGround();
        button.interactable = true;
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
        BlurBackGround();
        Time.timeScale = isGamePaused ? 0f : 1f;
    }

    public void SetScoreTextActive(bool isActive)
    {
        scoreGameObject.gameObject.SetActive(isActive);
    }

    public void CloseAllPanel()
    {
        winPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
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

    public void RestartScene(Button button)
    {
        ResetHealth();
        ResetScore();
        SceneManagerClass.Instance.ReloadScene();
        StartCoroutine(waitForSecond(button));
    }

    public void BlurBackGround()
    {
        isBlurEnabled = !isBlurEnabled;
        _depthOfField.enabled.Override(isBlurEnabled);
        _colorGrading.enabled.Override(isBlurEnabled);
    }
    
    #endregion
}
