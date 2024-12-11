using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreVisual : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient scoreColorGradient;
    [SerializeField] private float lerpSpeed = 5f; 
    
    private Coroutine lerpCoroutine;
    private float targetValue;
    private void Start()
    {
        if (slider == null)
        {
            Debug.LogError("Slider is not assigned in ScoreVisual.");
            return;
        }

        slider.fillRect.gameObject.SetActive(false);
        slider.maxValue = GameManager.Instance.scoreManager.scoreMax;
        slider.minValue = 0;
        
        targetValue = 0;
        slider.value = 0;
        UpdateSliderColor();
    }
    
    public void SetScore(int newScore)
    {
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        lerpCoroutine = StartCoroutine(LerpSlider(newScore));
    }
    
    private IEnumerator LerpSlider(int newScore)
    {
        targetValue = newScore;
        float duration = Mathf.Abs(slider.value - targetValue) / lerpSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            slider.value = Mathf.Lerp(slider.value, targetValue, elapsedTime / duration);
            UpdateSliderColor();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        slider.value = targetValue;
        UpdateSliderColor();
    }
    
    private void UpdateSliderColor()
    {
        if (slider.value > 0)
        {
            slider.fillRect.gameObject.SetActive(true);
            Color fillColor = scoreColorGradient.Evaluate(slider.value / slider.maxValue);
            slider.fillRect.GetComponent<Image>().color = fillColor;
        }
        else
        {
            slider.fillRect.gameObject.SetActive(false);
        }
    }
}
