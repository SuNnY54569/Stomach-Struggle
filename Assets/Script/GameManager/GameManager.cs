using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    public static event Action OnGamePaused;
    public static event Action OnGameUnpaused;
    
    public bool isGamePaused;
    public bool isBlurEnabled;
    private Coroutine blurCoroutine;
    
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
    
    [Header("Win Heart")]
    [SerializeField] private Image heartFill;
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

    [SerializeField, Tooltip("setting panel")]
    private GameObject settingPanel;

    public GameObject pauseButton;

    [SerializeField] private GameObject[] normalPause;
    [SerializeField] private GameObject[] cutScenePause;
    
    [SerializeField] private List<GameObject> uiPanels;
    #endregion

    #region Tutorial Settings
    [Header("Tutorial")] 
    [SerializeField, Tooltip("Panel to display Tutorial when scene start")] 
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
    public int totalHeart1, totalHeartLeft1;
    public int totalHeart2, totalHeartLeft2;
    public int totalHeart3, totalHeartLeft3;
    #endregion
    
    #region Scene Management
    public enum SceneCategory { Gameplay, Cutscene, Test, Summary, StartScene}

    [Header("Scenes to Deactivate object")]
    [SerializeField] private List<string> gameplayScenes;
    [SerializeField] private List<string> cutScenes;
    [SerializeField] private List<string> testScenes;
    [SerializeField] private List<string> summaryScenes;
    
    [SerializeField, Tooltip("Objects to deactivate in certain scenes.")]
    private List<GameObject> objectsToDeactivate;
    
    private SceneCategory currentSceneCategory;
    #endregion
    
    #region Post Processing Effects
    [Header("Post Processing")]
    [SerializeField] private PostProcessVolume volume;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float intensity;
    
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
            return;
        }
        
        SetupPostProcessing();
    }
    
    private void Start()
    {
        InitializeAllPanel();
        initialIntensity = intensity;
        currentHealth = maxHealth;
        UpdateHeartsUI();
        SoundManager.instance.UpdateLevelBGM();
    }

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        SoundManager.instance.UpdateLevelBGM();
        SetMaxScoreForLevel(scene.name);
        UpdateScoreText();
        UITransitionUtility.Instance.MoveIn(gameplayPanel);
        
        currentSceneCategory = DetermineSceneCategory(scene.name);
        
        bool isGameplayScene = currentSceneCategory == SceneCategory.Gameplay;
        foreach (var obj in objectsToDeactivate)
            obj.SetActive(isGameplayScene);

        foreach (var button in normalPause)
        {
            button.SetActive(isGameplayScene);
        }
        
        foreach (var button in cutScenePause)
        {
            button.SetActive(!isGameplayScene);
        }
        
        bool isTestScene = currentSceneCategory == SceneCategory.Test;
        
        foreach (var button in normalPause)
        {
            button.SetActive(!isTestScene);
        }
        
        foreach (var button in cutScenePause)
        {
            button.SetActive(isTestScene);
        }

        pauseButton.SetActive(currentSceneCategory is SceneCategory.Gameplay or SceneCategory.Cutscene or SceneCategory.Test);
        
        SetupTutorial(scene.name);
    }
    
    public SceneCategory DetermineSceneCategory(string sceneName)
    {
        if (gameplayScenes.Contains(sceneName)) return SceneCategory.Gameplay;
        if (cutScenes.Contains(sceneName)) return SceneCategory.Cutscene;
        if (testScenes.Contains(sceneName)) return SceneCategory.Test;
        if (summaryScenes.Contains(sceneName)) return SceneCategory.Summary;

        return SceneCategory.StartScene;
    }
    
    private void SetupTutorial(string sceneName)
    {
        if (tutorialVideoManager == null) return;

        VideoClip videoClip = tutorialVideoManager.GetVideoForScene(sceneName);
        if (videoClip != null)
        {
            tutorialVideoManager.SetupVideoForScene(sceneName);
            UITransitionUtility.Instance.MoveIn(tutorialPanel);
            UITransitionUtility.Instance.MoveOut(gameplayPanel);
            tutorialVideoManager.StartVideo();
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
    #endregion
    
    #region Health Management
    public void DecreaseHealth(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
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
            elapsedTime += Time.unscaledDeltaTime;
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
        if (scoreValue >= scoreMax) WinGame();
    }

    public int GetScore()
    {
        return scoreValue;
    }

    public void UpdateScoreText()
    {
        scoreText.text = $"{scoreValue}/{scoreMax}";
    }

    private void ResetScore()
    {
        scoreValue = 0;
        UpdateScoreText();
    }
    
    private void SetMaxScoreForLevel(string levelName)
    {
        var level = levelSettings.FirstOrDefault(l => l.levelName == levelName);
        scoreMax = level?.maxScore ?? 3;
    }
    #endregion
    
    #region Game End States
    public void GameOver()
    {
        ResetScore();
        SoundManager.PlaySound(SoundType.Lose,VolumeType.SFX);
        UITransitionUtility.Instance.MoveOut(gameplayPanel);
        UITransitionUtility.Instance.PopUp(gameOverPanel);
        PauseGame();
    }

    public void WinGame()
    {
        totalHeart += maxHealth;
        totalHeartLeft += currentHealth;
        UpdateHeartFill();
        
        SoundManager.PlaySound(SoundType.Win,VolumeType.SFX);
        UITransitionUtility.Instance.MoveOut(gameplayPanel);
        UITransitionUtility.Instance.PopUp(winPanel);
        PauseGame();
    }

    public void ExitToMenu(Button button)
    {
        ResetHealth();
        ResetScore();
        ResetAllTotalHeart();
        MoveAllPanelOut();
        SceneManagerClass.Instance.LoadMenuScene();
        StartCoroutine(waitForSecond(button));
    }

    public void NextScene(Button button)
    {
        ResetHealth();
        ResetScore();
        MoveAllPanelOut();
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

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }
    #endregion

    #region Utility Functions
    public void PauseGame()
    {
        if (isGamePaused)
        {
            // If the game is currently paused, start the unblur coroutine to unpause.
            StartCoroutine(UnblurBeforeResume());
            OnGameUnpaused?.Invoke();
        }
        else
        {
            // If the game is not paused, pause the game immediately and apply blur.
            isGamePaused = true;
            SoundManager.PauseAllSounds();
            Time.timeScale = 0f;
            BlurBackGround();
            OnGamePaused?.Invoke();
        }
    }

    public void SetScoreTextActive(bool isActive)
    {
        scoreGameObject.gameObject.SetActive(isActive);
    }

    public void CloseAllPanel()
    {
        UITransitionUtility.Instance.MoveOut(winPanel);
        UITransitionUtility.Instance.MoveOut(gameOverPanel);
        UITransitionUtility.Instance.MoveOut(pausePanel);
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
        ResetTotalHeart();
        ResetScore();
        MoveAllPanelOut();
        SceneManagerClass.Instance.ReloadScene();
        StartCoroutine(waitForSecond(button));
    }

    public void BlurBackGround()
    {
        if (blurCoroutine != null)
        {
            StopCoroutine(blurCoroutine);
            blurCoroutine = null; // Clear the reference
        }

        blurCoroutine = StartCoroutine(isBlurEnabled ?
            // Disable blur smoothly
            SmoothBlurTransition(false) :
            // Enable blur smoothly
            SmoothBlurTransition(true));

        isBlurEnabled = !isBlurEnabled;
    }
    
    private IEnumerator SmoothBlurTransition(bool enableBlur)
    {
        float startBlurIntensity = _depthOfField.active ? _depthOfField.focalLength.value : 0f;
        float targetBlurIntensity = enableBlur ? 10f : 1f;

        float elapsedTime = 0f;

        _depthOfField.enabled.Override(true);
        _colorGrading.enabled.Override(true);

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / 0.5f;

            float currentBlurIntensity = Mathf.Lerp(startBlurIntensity, targetBlurIntensity, t);

            _depthOfField.focalLength.Override(currentBlurIntensity);

            yield return null;
        }

        _depthOfField.focalLength.Override(targetBlurIntensity);

        if (!enableBlur)
        {
            _depthOfField.enabled.Override(false);
        }

        // Clear the reference once the coroutine finishes
        blurCoroutine = null;
    }
    
    private IEnumerator UnblurBeforeResume()
    {
        // Unblur the background.
        BlurBackGround();

        // Wait for the unblur transition to complete.
        yield return new WaitForSecondsRealtime(0.5f);

        // Resume the game.
        isGamePaused = false;
        SoundManager.ResumeAllSounds();
        Time.timeScale = 1f;
    }
    
    private void SetupPostProcessing()
    {
        if (volume.profile.TryGetSettings(out _vignette))
            _vignette.enabled.Override(false);

        if (volume.profile.TryGetSettings(out _depthOfField))
            _depthOfField.enabled.Override(false);

        if (volume.profile.TryGetSettings(out _colorGrading))
            _colorGrading.enabled.Override(false);
    }

    private void UpdateHeartFill()
    {
        float rawFill = (float)currentHealth / maxHealth;
        heartFill.fillAmount = Mathf.Round(rawFill * 10) / 10f;
    }

    private void InitializeAllPanel()
    {
        foreach (GameObject panel in uiPanels)
        {
            Vector2 targetPosition = new Vector2(0, 0);
            UITransitionUtility.Instance.Initialize(panel, targetPosition);
        }
    }

    public void MovePanelIn(GameObject panel)
    {
        SetActivePauseButton(false);
        
        UITransitionUtility.Instance.MoveIn
            (panel, LeanTweenType.easeInOutQuad , 1f, () => { SetActivePauseButton(true);});
    }

    public void MovePanelOut(GameObject panel)
    {
        SetActivePauseButton(false);
        
        UITransitionUtility.Instance.MoveOut
            (panel, LeanTweenType.easeInOutQuad , 1f, () => { SetActivePauseButton(true);});
    }
    
    public void MoveAllPanelOut()
    {
        UITransitionUtility.Instance.PopDown(winPanel);
        UITransitionUtility.Instance.PopDown(gameOverPanel);
        UITransitionUtility.Instance.MoveOut(pausePanel);
        UITransitionUtility.Instance.MoveOut(tutorialPanel);
        UITransitionUtility.Instance.MoveOut(gameplayPanel);
        UITransitionUtility.Instance.MoveOut(settingPanel);
    }

    private void SetActivePauseButton(bool isActive)
    {
        foreach (var button in normalPause)
        {
            button.GetComponent<Button>().interactable = isActive;
        }

        foreach (var button in cutScenePause)
        {
            button.GetComponent<Button>().interactable = isActive;
        }
    }

    #endregion
}
