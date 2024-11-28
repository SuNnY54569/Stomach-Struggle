using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CookingClock : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text cookingTimeText;
    [SerializeField] private GameObject clock;

    [Header("Shake Setting")] 
    [SerializeField] private float duration = 0.5f;
    [SerializeField]private float strength = 0.2f;
    [SerializeField]private int vibrato = 20;
    [SerializeField]private float randomness = 90;
    
    private Steak currentlyCookingSteak;
    private Tween shakeTween;
    private Vector3 clockOriginalPos;
    private bool isClockSoundPlaying;

    private void Start()
    {
        clockOriginalPos = clock.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        currentlyCookingSteak = Tools.Instance.currentlyCookingSteak;

        if (currentlyCookingSteak != null)
        {
            UpdateCookingTime();
            
            float elapsed = currentlyCookingSteak.CookingTimeElapsed();

            if (elapsed >= 5f && elapsed < 10f)
            {
                if (!isClockSoundPlaying)
                {
                    StartCoroutine(PlayClockSound());
                }
                if (shakeTween == null || !shakeTween.IsActive())
                {
                    StartShake(); // Start shaking if not already shaking
                }
            }
            else if (elapsed >= 10f && shakeTween != null)
            {
                StopShake(); // Stop shaking once elapsed time is 10 seconds or more
            }
            else if (elapsed < 5f && shakeTween != null)
            {
                StopShake();
            }
        }
        else
        {
            cookingTimeText.text = "00:00";
            StopShake(); // Ensure shake stops if no steak is cooking
        }
    }
    
    private void UpdateCookingTime()
    {
        if (currentlyCookingSteak != null)
        {
            float timeElapsed = currentlyCookingSteak.CookingTimeElapsed();
            
            int minutes = Mathf.FloorToInt(timeElapsed); // Treat elapsed seconds as minutes
            int seconds = 0;
            cookingTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            cookingTimeText.text = "00:00";
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
}
