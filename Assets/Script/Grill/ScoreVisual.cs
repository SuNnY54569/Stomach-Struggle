using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreVisual : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Start()
    {
        GameManager.Instance.SetScoreTextActive(false);
        slider.maxValue = GameManager.Instance.scoreMax;
        slider.minValue = 0;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        slider.value = GameManager.Instance.GetScore();
    }
}
