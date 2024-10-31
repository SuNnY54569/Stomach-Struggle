using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingUI : MonoBehaviour
{
    [SerializeField] private Slider backgroundSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider dialogSlider;
    
    private void Start()
    {
        
        InitializeSliders();
        
        backgroundSlider.onValueChanged.AddListener(value => UpdateBackgroundVolume(value));
        sfxSlider.onValueChanged.AddListener(value => UpdateSFXVolume(value));
        dialogSlider.onValueChanged.AddListener(value => UpdateDialogVolume(value));
    }
    
    private void InitializeSliders()
    {
        backgroundSlider.value = SoundManager.GetVolume(VolumeType.Background);
        sfxSlider.value = SoundManager.GetVolume(VolumeType.SFX);
        dialogSlider.value = SoundManager.GetVolume(VolumeType.Dialog);
    }
    
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
}
