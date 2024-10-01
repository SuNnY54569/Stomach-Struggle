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

    void Start()
    {
        slider.maxValue = scoreMeatShop.scoreMax;
        slider.minValue = scoreMeatShop.scoreMin;
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = scoreMeatShop.GetScore();
    }
}
