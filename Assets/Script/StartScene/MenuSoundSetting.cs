using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSoundSetting : MonoBehaviour
{
    #region Fields

    [Header("Volume Sliders")]
    [Tooltip("Slider to adjust background music volume.")]
    [SerializeField] private Slider backgroundSlider;

    [Tooltip("Slider to adjust sound effects volume.")]
    [SerializeField] private Slider sfxSlider;

    [Tooltip("Slider to adjust dialog volume.")]
    [SerializeField] private Slider dialogSlider;

    [Tooltip("Slider to adjust tutorial volume.")]
    [SerializeField] private Slider tutorialSlider;
    

    #endregion
    
    #region Initialization
    
    private void Start()
    {
        
        InitializeSliders();
        AddSliderListeners();
        
    }

    public void InitializeSliders()
    {
        backgroundSlider.value = PlayerPrefs.GetFloat(VolumeType.Background.ToString(), 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(VolumeType.SFX.ToString(), 1f);
        dialogSlider.value = PlayerPrefs.GetFloat(VolumeType.Dialog.ToString(), 1f);
        tutorialSlider.value = PlayerPrefs.GetFloat(VolumeType.Tutorial.ToString(), 1f);
    }
    
    private void AddSliderListeners()
    {
        backgroundSlider.onValueChanged.AddListener(value => UpdateBackgroundVolume(value));
        sfxSlider.onValueChanged.AddListener(value => UpdateSFXVolume(value));
        dialogSlider.onValueChanged.AddListener(value => UpdateDialogVolume(value));
        tutorialSlider.onValueChanged.AddListener(value => UpdateTutorialVolume(value));
    }
    
    #endregion
    
    #region Volume Update Methods
    
    private void UpdateBackgroundVolume(float value)
    {
        SoundManager.SetVolume(VolumeType.Background, value);
    }

    private void UpdateSFXVolume(float value)
    {
        SoundManager.SetVolume(VolumeType.SFX, value);
    }

    private void UpdateDialogVolume(float value)
    {
        SoundManager.SetVolume(VolumeType.Dialog, value);
    }

    private void UpdateTutorialVolume(float value)
    {
        SoundManager.SetVolume(VolumeType.Tutorial, value);
    }

    #endregion
}
