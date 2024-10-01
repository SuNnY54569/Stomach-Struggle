using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreVisual : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private ScoreMeatShop scoreMeatShop;

    private void Awake()
    {
        scoreMeatShop = FindObjectOfType<ScoreMeatShop>();
    }

    private void Start()
    {
        slider.maxValue = scoreMeatShop.scoreMax;
        slider.minValue = scoreMeatShop.scoreMin;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        slider.value = scoreMeatShop.GetScore();
    }
}
