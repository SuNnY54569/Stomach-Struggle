using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool isGamePaused;
    
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [Header("Score Settings")]
    private int scoreValue = 0;
    public int scoreMax;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Win, lose Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    public int totalHeart;
    public int totalHeartLeft;

    public int totalHeart1;
    public int totalHeartLeft1;
    
    public int totalHeart2;
    public int totalHeartLeft2;
    
    public int totalHeart3;
    public int totalHeartLeft3;
    
    [Header("Scenes to Deactivate GameManager")]
    [SerializeField] private List<string> scenesToDeactivate;
    [SerializeField] private List<GameObject> objectsToDeactivate;
    
    [Header("Post Processing")]
    [SerializeField] private PostProcessVolume volume;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float intensity = 0f;
    [SerializeField] private float initialIntensity;
    [SerializeField] private float shakeIntensity = 0.1f;
    private Vignette _vignette;

    [Header("Player Info")] 
    public string playerName = "Player";
    public int preTestScore;
    public int postTestScore;
    
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

        volume.profile.TryGetSettings<Vignette>(out _vignette);

        if (!_vignette)
        {
            Debug.LogError("Error, vignette empty");
        }
        else
        {
            _vignette.enabled.Override(false);
        }
    }
    
    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scenesToDeactivate.Contains(scene.name))
        {
            foreach (var obj in objectsToDeactivate)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            foreach (var obj in objectsToDeactivate)
            {
                obj.SetActive(true);
            }
        }
    }
    
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
    #endregion
    
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

    public void ResetScore()
    {
        scoreValue = 0;
        UpdateScoreText();
    }

    public void PauseGame()
    {
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1;
        }
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
    
    
}
