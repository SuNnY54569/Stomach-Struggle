using System;
using System.Collections;
using System.Collections.Generic;
using MaskTransitions;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerClass : MonoBehaviour
{
    #region Singleton
    public static SceneManagerClass Instance { get; private set; }
    #endregion
    
    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensures only one instance exists
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keeps the singleton instance across scenes
    }
    #endregion
    
    #region Scene Management
    public void LoadMenuScene()
    {
        TransitionManager.Instance.LoadLevel("StartScene", 0.5f);
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            string nextSceneName = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
            TransitionManager.Instance.LoadLevel(nextSceneName, 0.5f);
        }
        else
        {
            Debug.LogError("LoadNextScene: Next scene index out of range.");
        }
    }

    public void ReloadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        if (SceneExists(currentSceneName))
        {
            TransitionManager.Instance.LoadLevel(currentSceneName, 0.5f);
        }
        else
        {
            Debug.LogError("ReloadScene: Current scene not found in Build Settings.");
        }
    }

    public void LoadThisScene(string sceneName)
    {
        if (SceneExists(sceneName))
        {
            TransitionManager.Instance.LoadLevel(sceneName, 0.5f);
        }
        else
        {
            Debug.LogError($"LoadThisScene: Scene '{sceneName}' not found in Build Settings.");
        }
    }
    
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
    #endregion
    
    #region Helper Method
    private bool SceneExists(string sceneName)
    {
        // Check if the scene exists in the build settings
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string scene = System.IO.Path.GetFileNameWithoutExtension(path);
            if (scene.Equals(sceneName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
    #endregion
}