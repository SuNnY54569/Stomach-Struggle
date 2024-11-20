using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCutScene : MonoBehaviour
{
    public void PlayBgInBuild()
    {
        SoundManager.PlaySound(SoundType.BgInBuild, VolumeType.SFX);
    }
    public void PlayBgOutHome()
    {
        SoundManager.PlaySound(SoundType.BgOutHome, VolumeType.SFX);
    }
    public void PlayBgInHome()
    {
        SoundManager.PlaySound(SoundType.BgInHome, VolumeType.SFX);
    }
    public void BgDay()
    {
        SoundManager.PlaySound(SoundType.BgDay, VolumeType.SFX);
    }
    public void PlayBgNight()
    {
        SoundManager.PlaySound(SoundType.BgNight, VolumeType.SFX);
    }
    public void PlayPressCard()
    {
        SoundManager.PlaySound(SoundType.PressCard, VolumeType.SFX);
    }
    public void PlayOpenHomeDoor()
    {
        SoundManager.PlaySound(SoundType.OpenHomeDoor, VolumeType.SFX);
    }
    public void PlayCloseHomeDoor()
    {
        SoundManager.PlaySound(SoundType.CloseHomeDoor, VolumeType.SFX);
    }
    public void PlayBusOpenDoor()
    {
        SoundManager.PlaySound(SoundType.BusOpenDoor, VolumeType.SFX);
    }
    public void PlayBuildOpenDoor()
    {
        SoundManager.PlaySound(SoundType.BuildOpenDoor, VolumeType.SFX);
    }
    public void Playwalk()
    {
        SoundManager.PlaySound(SoundType.walk, VolumeType.SFX);
    }
    public void PlayBusCome()
    {
        SoundManager.PlaySound(SoundType.BusCome, VolumeType.SFX);
    }
    public void PlayBusRun()
    {
        SoundManager.PlaySound(SoundType.BusRun, VolumeType.SFX);
    }
    public void PlayBgMarket()
    {
        SoundManager.PlaySound(SoundType.BgMarket, VolumeType.SFX);
    }
    public void PlayBusRunTenS()
    {
        SoundManager.PlaySound(SoundType.BusRunTenS, VolumeType.SFX);
    }
    public void PlayPhoneRing()
    {
        SoundManager.PlaySound(SoundType.PhoneRing, VolumeType.SFX);
    }
    public void PlayMarketBGM()
    {
        SoundManager.instance.CrossfadeBGM(SoundType.BgMarket, 1f);
    }
    public void PlayOutHomeBGM()
    {
        SoundManager.instance.CrossfadeBGM(SoundType.BgOutHome, 1f);
    }
    public void PlayBgNightBGM()
    {
        SoundManager.instance.CrossfadeBGM(SoundType.BgNight, 1f);
    }
    public void PlayBgInHomeBGM()
    {
        SoundManager.instance.CrossfadeBGM(SoundType.BgInHome, 1f);
    }
    public void StopAllSoundsInCutScene()
    {
        SoundManager.StopAllSounds();
    }
    
}
