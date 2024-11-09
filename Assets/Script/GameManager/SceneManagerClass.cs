using System;
using System.Collections;
using System.Collections.Generic;
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
        SceneManager.LoadScene("Start scene");
        GameManager.Instance.CloseAllPanel();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameManager.Instance.CloseAllPanel();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.CloseAllPanel();
    }

    public void LoadThisScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        GameManager.Instance.CloseAllPanel();
    }
    #endregion
}