using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class nextScene : MonoBehaviour
{
    public bool isSkip;
    public GameObject skipButton;
    public PlayableDirector timeline;

    public void Awake()
    {
        UITransitionUtility.Instance.Initialize(skipButton, new Vector2(0, 0));
        isSkip = false;
    }

    public void Start()
    {
        if (timeline != null)
        {
            timeline.Play();
        }
        StartCoroutine(EnableSkipButton());
    }

    public void LoadNextScene()
    {
        
        if (timeline != null && timeline.state == PlayState.Playing)
        {
            timeline.Stop();
        }

        UITransitionUtility.Instance.MoveOut(GameManager.Instance.gameplayPanel);
        SoundManager.PlaySound(SoundType.UIClick, VolumeType.SFX);
        UITransitionUtility.Instance.PopDown(skipButton);

        SceneManagerClass.Instance.LoadNextScene();
        SoundManager.StopAllSounds();
        isSkip = true;
    }

    public IEnumerator EnableSkipButton()
    {
        yield return new WaitForSeconds(1f);
        UITransitionUtility.Instance.MoveIn(GameManager.Instance.gameplayPanel);
        UITransitionUtility.Instance.PopUp(skipButton);
    }
}
