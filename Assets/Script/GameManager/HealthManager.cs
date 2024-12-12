using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    
    public int maxHealth = 3;
    public int currentHealth;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Image heartFill;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float shakeIntensity = 0.3f;
    [SerializeField] private float intensity = 0.4f;
    
    private float initialIntensity;
    
    private void Start()
    {
        initialIntensity = intensity;
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }
    
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
    
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }
    
    private IEnumerator TakeDamageEffect()
    {
        if (GameManager.Instance._vignette != null)
        {
            GameManager.Instance._vignette.enabled.Override(true);
        }
        else
        {
            Debug.LogWarning("Vignette is not assigned!");
        }
        
        float elapsedTime = 0f;
        intensity = initialIntensity;
        Transform cameraTransform = Camera.main.transform;
        Vector3 originalPosition = cameraTransform.position;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            intensity = Mathf.Lerp(initialIntensity, 0, elapsedTime / fadeDuration);
            GameManager.Instance._vignette.intensity.Override(intensity);

            float shakeMagnitude = Mathf.Lerp(shakeIntensity, 0, elapsedTime / fadeDuration);
            cameraTransform.position = originalPosition + Random.insideUnitSphere * shakeMagnitude;

            yield return null;
        }

        if (GameManager.Instance._vignette != null)
        {
            GameManager.Instance._vignette.enabled.Override(false);
        }
        
        cameraTransform.position = originalPosition;
    }
    #endregion
    
    public void GameOver()
    {
        GameManager.Instance.scoreManager.ResetScore();
        SoundManager.PlaySound(SoundType.Lose,VolumeType.SFX);
        UITransitionUtility.Instance.MoveOut(GameManager.Instance.gameplayPanel);
        UITransitionUtility.Instance.PopUp(GameManager.Instance.gameOverPanel);
        GameManager.Instance.PauseGame();
    }

    public void WinGame()
    {
        GameManager.Instance.totalHeart += maxHealth;
        GameManager.Instance.totalHeartLeft += currentHealth;
        UpdateHeartFill();
        
        SoundManager.PlaySound(SoundType.Win,VolumeType.SFX);
        UITransitionUtility.Instance.MoveOut(GameManager.Instance.gameplayPanel);
        UITransitionUtility.Instance.PopUp(GameManager.Instance.winPanel);
        GameManager.Instance.PauseGame();
    }
    
    private void UpdateHeartFill()
    {
        float rawFill = (float)currentHealth / maxHealth;
        heartFill.fillAmount = Mathf.Round(rawFill * 10) / 10f;
    }
}
