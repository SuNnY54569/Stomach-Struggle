using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCutScene : MonoBehaviour
{
    private nextScene _nextScene;
    [SerializeField] private GameObject dialogCanvas;

    public void Awake()
    {
        _nextScene = GetComponent<nextScene>();
    }

    public void LateUpdate()
    {
        dialogCanvas.SetActive(!GameManager.Instance.isGamePaused);
    }

    private void PlaySound(SoundType soundType, VolumeType volumeType = VolumeType.Dialog)
    {
        if (_nextScene.isSkip) return;
        SoundManager.PlaySound(soundType, volumeType);
    }

    public void PlayBackgroundMusic(SoundType bgMusic)
    {
        if (_nextScene.isSkip) return;
        SoundManager.instance.CrossfadeBGM(bgMusic, 1f);
    }

    public void PlayBgInBuild() => PlayBackgroundMusic(SoundType.BgInBuild);
    public void PlayBgOutHome() => PlayBackgroundMusic(SoundType.BgOutHome);
    public void PlayBgInHome() => PlayBackgroundMusic(SoundType.BgInHome);
    public void BgDay() => PlayBackgroundMusic(SoundType.BgDay);
    public void PlayBgNight() => PlayBackgroundMusic(SoundType.BgNight);
    public void PlayPressCard() => PlaySound(SoundType.PressCard);
    public void PlayOpenHomeDoor() => PlaySound(SoundType.OpenHomeDoor);
    public void PlayCloseHomeDoor() => PlaySound(SoundType.CloseHomeDoor);

    public void PlayBusOpenDoor() => PlaySound(SoundType.BusOpenDoor);
    public void PlayBuildOpenDoor() => PlaySound(SoundType.BuildOpenDoor);
    public void Playwalk() => PlaySound(SoundType.walk);
    public void PlayBusCome() => PlaySound(SoundType.BusCome);
    public void PlayBusRun() => PlaySound(SoundType.BusRun);
    public void PlayBgMarket() => PlaySound(SoundType.BgMarket);
    public void PlayBusRunTenS() => PlaySound(SoundType.BusRunTenS);
    public void PlayPhoneRing() => PlaySound(SoundType.PhoneRing);
    public void Dialouge1Day1() => PlaySound(SoundType.Dialouge1Day1);
    public void Dialouge2Day1() => PlaySound(SoundType.Dialouge2Day1);
    public void Dialouge3Day1() => PlaySound(SoundType.Dialouge3Day1);
    public void Dialouge4Day1() => PlaySound(SoundType.Dialouge4Day1);
    public void Dialouge5Day1() => PlaySound(SoundType.Dialouge5Day1);
    public void Dialouge6Day1() => PlaySound(SoundType.Dialouge6Day1);
    public void Dialouge7Day1() => PlaySound(SoundType.Dialouge7Day1);
    public void Dialouge8Day1() => PlaySound(SoundType.Dialouge8Day1);
    public void Dialouge9Day1() => PlaySound(SoundType.Dialouge9Day1);
    public void Dialouge10Day1() => PlaySound(SoundType.Dialouge10Day1);
    public void Dialouge11Day1() => PlaySound(SoundType.Dialouge11Day1);
    public void Dialouge12Day1() => PlaySound(SoundType.Dialouge12Day1);
    public void Dialouge13Day1() => PlaySound(SoundType.Dialouge13Day1);
    public void Dialouge14Day1() => PlaySound(SoundType.Dialouge14Day1);
    public void Dialouge3Day2() => PlaySound(SoundType.Dialouge3Day2);
    public void Dialouge4Day2() => PlaySound(SoundType.Dialouge4Day2);
    public void Dialouge5Day2() => PlaySound(SoundType.Dialouge5Day2);
    public void Dialouge6Day2() => PlaySound(SoundType.Dialouge6Day2);
    public void Dialouge7Day2() => PlaySound(SoundType.Dialouge7Day2);
    public void Dialouge1Day3() => PlaySound(SoundType.Dialouge1Day3);
    public void Dialouge2Day3() => PlaySound(SoundType.Dialouge2Day3);
    public void Dialouge3Day3() => PlaySound(SoundType.Dialouge3Day3);
    public void Dialouge4Day3() => PlaySound(SoundType.Dialouge4Day3);
    public void Dialouge5Day3() => PlaySound(SoundType.Dialouge5Day3);
    public void Dialouge6Day3() => PlaySound(SoundType.Dialouge6Day3);
    public void Dialouge7Day3() => PlaySound(SoundType.Dialouge7Day3);
    public void Dialouge8Day3() => PlaySound(SoundType.Dialouge8Day3);
    public void PlayMarketBGM() => PlayBackgroundMusic(SoundType.BgMarket);
    public void PlayOutHomeBGM() => PlayBackgroundMusic(SoundType.BgOutHome);
    public void PlayBgNightBGM() => PlayBackgroundMusic(SoundType.BgNight);
    public void PlayBgInHomeBGM() => PlayBackgroundMusic(SoundType.BgInHome);
    public void StopAllSoundsInCutScene() => SoundManager.StopAllSounds();
    
}
