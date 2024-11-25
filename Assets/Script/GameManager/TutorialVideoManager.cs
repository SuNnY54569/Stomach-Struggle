using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialVideoManager : MonoBehaviour
{
    #region Fields
    [Header("Video Player Components")]
    [Tooltip("The VideoPlayer component to play tutorial videos.")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Tutorial Scene Data")]
    [Tooltip("List of tutorial videos associated with specific scenes.")]
    public TutorialSceneData[] tutorialScenes;

    [Header("UI Components")]
    [Tooltip("Button to replay the tutorial video.")]
    [SerializeField] private Button replayButton;
    [Tooltip("Button to skip the tutorial")] 
    [SerializeField] private Button skipButton;
    [Tooltip("Tutorial Text")] 
    [SerializeField] private TMP_Text tutorialText;

    #endregion
    #region Unity Lifecycle

    private void Awake()
    {
        if (replayButton != null)
        {
            replayButton.gameObject.SetActive(false);
        }
        
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false);
        }
        
        if (SoundManager.instance != null && SoundManager.instance.audioSources.ContainsKey(VolumeType.Tutorial))
        {
            videoPlayer.SetTargetAudioSource(0, SoundManager.instance.audioSources[VolumeType.Tutorial]);
        }
        
        UITransitionUtility.Instance.Initialize(skipButton.gameObject, skipButton.gameObject.transform.position);
        UITransitionUtility.Instance.Initialize(replayButton.gameObject, replayButton.gameObject.transform.position);
    }

    private void Start()
    {
        if (SoundManager.instance != null && SoundManager.instance.audioSources.ContainsKey(VolumeType.Tutorial))
        {
            videoPlayer.SetTargetAudioSource(0, SoundManager.instance.audioSources[VolumeType.Tutorial]);
        }
    }

    #endregion
    
    #region Public Methods

    public void SetupVideoForScene(string sceneName)
    {
        VideoClip videoClip = GetVideoForScene(sceneName);
        if (videoClip != null)
        {
            videoPlayer.clip = videoClip;
            StartCoroutine(ShowSkipButtonWithDelay(1f));
        }
    }
    
    public void StartVideo()
    {
        if (videoPlayer.clip != null)
        {
            videoPlayer.Play();
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }
    
    public void ReplayVideo()
    {
        videoPlayer.Stop();
        videoPlayer.Play();
        replayButton.gameObject.SetActive(false);
        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
    }
    
    public void SkipVideo()
    {
        videoPlayer.Stop();
        UITransitionUtility.Instance.PopDown(replayButton.gameObject, LeanTweenType.easeInBack, 0.25f);
        UITransitionUtility.Instance.PopDown(skipButton.gameObject, LeanTweenType.easeInBack,0.25f);
        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
        UITransitionUtility.Instance.MoveOut(GameManager.Instance.tutorialPanel);
        UITransitionUtility.Instance.MoveIn(GameManager.Instance.gameplayPanel);
        GameManager.Instance.PauseGame();
    }
    
    public VideoClip GetVideoForScene(string sceneName)
    {
        foreach (var tutorialScene in tutorialScenes)
        {
            if (tutorialScene.sceneName != sceneName) continue;
            tutorialText.text = tutorialScene.tutorialText;
            return tutorialScene.videoClip;
        }
        return null;
    }
    
    #endregion
    
    #region Private Methods
    
    private void OnVideoEnd(VideoPlayer vp)
    {
        UITransitionUtility.Instance.PopUp(replayButton.gameObject);
    }
    
    private IEnumerator ShowSkipButtonWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        UITransitionUtility.Instance.PopUp(skipButton.gameObject);
    }
    
    #endregion
}

[System.Serializable]
public class TutorialSceneData
{
    [Tooltip("The name of the scene.")]
    public string sceneName;

    [Tooltip("The video clip associated with this scene.")]
    public VideoClip videoClip;
    
    [Tooltip("tutorial text")] 
    public string tutorialText;
}
