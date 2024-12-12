using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CookingClock : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text cookingTimeText;
    [SerializeField] private GameObject clock;

    [Header("Shake Setting")] 
    [SerializeField] private float duration = 0.5f;
    [SerializeField]private float strength = 0.2f;
    [SerializeField] private int vibrato = 20;
    [SerializeField] private float randomness = 90;
    
    private Steak currentlyCookingSteak;
    private Tween shakeTween;
    private Vector3 clockOriginalPos;
    private bool isClockSoundPlaying;
    
    private const float ShakeStartThreshold = 5f;
    private const float ShakeStopThreshold = 10f;

    private void Start()
    {
        clockOriginalPos = clock.transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        currentlyCookingSteak = Tools.Instance.currentlyCookingSteak;

        if (currentlyCookingSteak != null)
        {
            UpdateCookingTime();
            ManageClockEffects(currentlyCookingSteak.CookingTimeElapsed());
        }
        else
        {
            ResetClockUI();
        }
    }
    
    private void UpdateCookingTime()
    {
        if (currentlyCookingSteak != null)
        {
            float timeElapsed = currentlyCookingSteak.CookingTimeElapsed();
            int minutes = Mathf.FloorToInt(timeElapsed); // Treat elapsed seconds as minutes
            int seconds = 0;
            cookingTimeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
    
    private void ManageClockEffects(float elapsed)
    {
        if (elapsed >= ShakeStartThreshold && elapsed < ShakeStopThreshold)
        {
            if (!isClockSoundPlaying)
            {
                StartCoroutine(PlayClockSound());
            }

            if (shakeTween == null || !shakeTween.IsActive())
            {
                StartShake();
            }
        }
        else if (elapsed >= ShakeStopThreshold || elapsed < ShakeStartThreshold)
        {
            StopShake();
        }
    }
    
    private void StartShake()
    {
        StopShake();
        
        shakeTween = clock.transform.DOShakePosition(duration, strength, vibrato, randomness).SetLoops(-1);
    }
    
    private void StopShake()
    {
        if (shakeTween != null && shakeTween.IsActive())
        {
            shakeTween.Kill();
            clock.transform.localPosition = clockOriginalPos;
        }
    }
    
    IEnumerator PlayClockSound()
    {
        isClockSoundPlaying = true;
        SoundManager.PlaySound(SoundType.Clock, VolumeType.SFX);

        yield return new WaitForSeconds(0.75f);
        isClockSoundPlaying = false;
    }
    
    private void ResetClockUI()
    {
        cookingTimeText.text = "00:00";
        StopShake();
    }
}
