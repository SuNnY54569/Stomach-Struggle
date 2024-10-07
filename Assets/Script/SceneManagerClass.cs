using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerClass : MonoBehaviour
{
    public void LoadMenuScene() 
    {
        SceneManager.LoadScene("MenuScene");
    }
    
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void ReloadScene() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadThisScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}