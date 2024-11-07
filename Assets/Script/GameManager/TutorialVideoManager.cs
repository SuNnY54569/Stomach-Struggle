using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public class SceneVideoMapping
{
    [Tooltip("Name of the scene this video is associated with.")]
    public string sceneName;

    [Tooltip("Video clip for this specific scene.")]
    public VideoClip videoClip;
}

public class TutorialVideoManager : MonoBehaviour
{
    #region Fields
    [Header("Video Player Components")]
    [Tooltip("The VideoPlayer component to play tutorial videos.")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Tooltip("AudioSource for the tutorial video's audio.")]
    [SerializeField] private AudioSource tutorialAudioSource;

    [Header("Tutorial Scene Data")]
    [Tooltip("List of tutorial videos associated with specific scenes.")]
    public TutorialSceneData[] tutorialScenes;

    [Header("UI Components")]
    [Tooltip("Button to replay the tutorial video.")]
    [SerializeField] private Button replayButton;

    #endregion
    #region Unity Lifecycle

    private void Awake()
    {
        if (replayButton != null)
        {
            replayButton.gameObject.SetActive(false);
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
        }
    }
    
    public void SetTutorialVolume(float volume)
    {
        tutorialAudioSource.volume = volume;
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
    }
    
    public void SkipVideo()
    {
        videoPlayer.Stop();
        replayButton.gameObject.SetActive(false);
        GameManager.Instance.tutorialPanel.SetActive(false);
        GameManager.Instance.PauseGame();
    }
    
    public VideoClip GetVideoForScene(string sceneName)
    {
        foreach (var tutorialScene in tutorialScenes)
        {
            if (tutorialScene.sceneName == sceneName)
                return tutorialScene.videoClip;
        }
        return null;
    }
    
    #endregion
    
    #region Private Methods
    
    private void OnVideoEnd(VideoPlayer vp)
    {
        replayButton.gameObject.SetActive(true);
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
}
