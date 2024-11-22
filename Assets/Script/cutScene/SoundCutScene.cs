using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCutScene : MonoBehaviour
{
    public void PlayBgInBuild()
    {
        SoundManager.PlaySound(SoundType.BgInBuild, VolumeType.Dialog);
    }
    public void PlayBgOutHome()
    {
        SoundManager.PlaySound(SoundType.BgOutHome, VolumeType.Dialog);
    }
    public void PlayBgInHome()
    {
        SoundManager.PlaySound(SoundType.BgInHome, VolumeType.Dialog);
    }
    public void BgDay()
    {
        SoundManager.PlaySound(SoundType.BgDay, VolumeType.Dialog);
    }
    public void PlayBgNight()
    {
        SoundManager.PlaySound(SoundType.BgNight, VolumeType.Dialog);
    }
    public void PlayPressCard()
    {
        SoundManager.PlaySound(SoundType.PressCard, VolumeType.Dialog);
    }
    public void PlayOpenHomeDoor()
    {
        SoundManager.PlaySound(SoundType.OpenHomeDoor, VolumeType.Dialog);
    }
    public void PlayCloseHomeDoor()
    {
        SoundManager.PlaySound(SoundType.CloseHomeDoor, VolumeType.Dialog);
    }
    public void PlayBusOpenDoor()
    {
        SoundManager.PlaySound(SoundType.BusOpenDoor, VolumeType.Dialog);
    }
    public void PlayBuildOpenDoor()
    {
        SoundManager.PlaySound(SoundType.BuildOpenDoor, VolumeType.Dialog);
    }
    public void Playwalk()
    {
        SoundManager.PlaySound(SoundType.walk, VolumeType.Dialog);
    }
    public void PlayBusCome()
    {
        SoundManager.PlaySound(SoundType.BusCome, VolumeType.Dialog);
    }
    public void PlayBusRun()
    {
        SoundManager.PlaySound(SoundType.BusRun, VolumeType.Dialog);
    }
    public void PlayBgMarket()
    {
        SoundManager.PlaySound(SoundType.BgMarket, VolumeType.Dialog);
    }
    public void PlayBusRunTenS()
    {
        SoundManager.PlaySound(SoundType.BusRunTenS, VolumeType.Dialog);
    }
    public void PlayPhoneRing()
    {
        SoundManager.PlaySound(SoundType.PhoneRing, VolumeType.Dialog);
    }

    public void Dialouge1Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge1Day1, VolumeType.Dialog);
    }
    public void Dialouge2Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge2Day1, VolumeType.Dialog);
    }
    public void Dialouge3Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge3Day1, VolumeType.Dialog);
    }
    public void Dialouge4Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge4Day1, VolumeType.Dialog);
    }
    public void Dialouge5Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge5Day1, VolumeType.Dialog);
    }
    public void Dialouge6Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge6Day1, VolumeType.Dialog);
    }
    public void Dialouge7Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge7Day1, VolumeType.Dialog);
    }
    public void Dialouge8Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge8Day1, VolumeType.Dialog);
    }
    public void Dialouge9Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge9Day1, VolumeType.Dialog);
    }
    public void Dialouge10Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge10Day1, VolumeType.Dialog);
    }
    public void Dialouge11Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge11Day1, VolumeType.Dialog);
    }
    public void Dialouge12Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge12Day1, VolumeType.SFX);
    }
    public void Dialouge13Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge13Day1, VolumeType.Dialog);
    }

    public void Dialouge14Day1()
    {
        SoundManager.PlaySound(SoundType.Dialouge14Day1, VolumeType.Dialog);
    }

    public void Dialouge3Day2()
    {
        SoundManager.PlaySound(SoundType.Dialouge3Day2, VolumeType.Dialog);
    }
    public void Dialouge4Day2()
    {
        SoundManager.PlaySound(SoundType.Dialouge4Day2, VolumeType.Dialog);
    }
    public void Dialouge5Day2()
    {
        SoundManager.PlaySound(SoundType.Dialouge5Day2, VolumeType.Dialog);
    }
    public void Dialouge6Day2()
    {
        SoundManager.PlaySound(SoundType.Dialouge6Day2, VolumeType.Dialog);
    }
    public void Dialouge7Day2()
    {
        SoundManager.PlaySound(SoundType.Dialouge7Day2, VolumeType.Dialog);
    }

    public void Dialouge1Day3()
    {
        SoundManager.PlaySound(SoundType.Dialouge1Day3, VolumeType.Dialog);
    }
    public void Dialouge2Day3()
    {
        SoundManager.PlaySound(SoundType.Dialouge2Day3, VolumeType.Dialog);
    }
    public void Dialouge3Day3()
    {
        SoundManager.PlaySound(SoundType.Dialouge3Day3, VolumeType.Dialog);
    }
    public void Dialouge4Day3()
    {
        SoundManager.PlaySound(SoundType.Dialouge4Day3, VolumeType.Dialog);
    }
    public void Dialouge5Day3()
    {
        SoundManager.PlaySound(SoundType.Dialouge5Day3, VolumeType.Dialog);
    }
    public void Dialouge6Day3()
    {
        SoundManager.PlaySound(SoundType.Dialouge6Day3, VolumeType.Dialog);
    }
    public void Dialouge7Day3()
    {
        SoundManager.PlaySound(SoundType.Dialouge7Day3, VolumeType.Dialog);
    }
    public void Dialouge8Day3()
    {
        SoundManager.PlaySound(SoundType.Dialouge8Day3, VolumeType.Dialog);
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
