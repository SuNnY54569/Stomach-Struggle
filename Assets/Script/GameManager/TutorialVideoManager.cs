using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public class SceneVideoMapping
{
    public string sceneName;       // Scene name
    public VideoClip videoClip;    // Video clip for this scene
}

public class TutorialVideoManager : MonoBehaviour
{
    [Tooltip("Add a dictionary or list of videos with scene names")]
    [SerializeField] private VideoPlayer videoPlayer;
    
    [Tooltip("Assign tutorial videos for each scene")]
    public TutorialSceneData[] tutorialScenes;

    [Tooltip("Assign the replay button in the UI")]
    [SerializeField] private Button replayButton;

    private void Awake()
    {
        if (replayButton != null)
            replayButton.gameObject.SetActive(false);
    }

    public void SetupVideoForScene(string sceneName)
    {
        VideoClip videoClip = GetVideoForScene(sceneName);
        if (videoClip != null)
        {
            videoPlayer.clip = videoClip;
        }
    }
    
    public void StartVideo()
    {
        if (videoPlayer.clip != null)
        {
            videoPlayer.Play();
            videoPlayer.loopPointReached += (VideoPlayer vp) =>
            {
                // When video finishes, show the replay button
                replayButton.gameObject.SetActive(true);
            };
        }
    }
    
    public void ReplayVideo()
    {
        // Restart the video from the beginning
        videoPlayer.Stop();
        videoPlayer.Play();
        
        // Hide the replay button while video is playing
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
}

[System.Serializable]
public class TutorialSceneData
{
    public string sceneName;
    public VideoClip videoClip;
}
