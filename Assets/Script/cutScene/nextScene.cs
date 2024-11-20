using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextScene : MonoBehaviour
{
    public void LoadNextScene()
    {
        SoundManager.PlaySound(SoundType.UIClick, VolumeType.SFX);
        SceneManagerClass.Instance.LoadNextScene();
        SoundManager.StopAllSounds();
    }
}
