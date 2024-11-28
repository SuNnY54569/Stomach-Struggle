using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreVisual : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient scoreColorGradient;

    private void Start()
    {
        slider.fillRect.gameObject.SetActive(false);
        GameManager.Instance.SetScoreTextActive(false);
        slider.maxValue = GameManager.Instance.scoreMax;
        slider.minValue = 0;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        slider.value = GameManager.Instance.GetScore();
        if (slider.value > 0)
        {
            slider.fillRect.gameObject.SetActive(true);
            Color fillColor = scoreColorGradient.Evaluate(slider.value / slider.maxValue);
            slider.fillRect.GetComponent<Image>().color = fillColor;
        }
    }
}
