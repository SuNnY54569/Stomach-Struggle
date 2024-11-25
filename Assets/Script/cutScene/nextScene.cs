using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextScene : MonoBehaviour
{
    public bool isSkip;
    public GameObject skipButton;

    public void Awake()
    {
        UITransitionUtility.Instance.Initialize(skipButton, new Vector2(0,0));
        isSkip = false;
    }

    public void Start()
    {
        StartCoroutine(StartCutscene());
    }

    public void LoadNextScene()
    {
        UITransitionUtility.Instance.MoveOut(GameManager.Instance.gameplayPanel);
        SoundManager.PlaySound(SoundType.UIClick, VolumeType.SFX);
        UITransitionUtility.Instance.PopDown(skipButton);
        SceneManagerClass.Instance.LoadNextScene();
        SoundManager.StopAllSounds();
        isSkip = true;
    }

    public IEnumerator StartCutscene()
    {
        yield return new WaitForSeconds(1f);
        UITransitionUtility.Instance.MoveIn(GameManager.Instance.gameplayPanel);
        UITransitionUtility.Instance.PopUp(skipButton);
    }
}
