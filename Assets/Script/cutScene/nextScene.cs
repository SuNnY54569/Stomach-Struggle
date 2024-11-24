using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextScene : MonoBehaviour
{
    public bool isSkip;

    public void Awake()
    {
        isSkip = false;
    }

    public void LoadNextScene()
    {
        GameManager.Instance.pauseButton.SetActive(false);
        SoundManager.PlaySound(SoundType.UIClick, VolumeType.SFX);
        SceneManagerClass.Instance.LoadNextScene();
        SoundManager.StopAllSounds();
        isSkip = true;
    }
}
