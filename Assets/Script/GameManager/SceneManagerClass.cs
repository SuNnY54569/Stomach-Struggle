using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerClass : MonoBehaviour
{
    public void LoadMenuScene() 
    {
        SceneManager.LoadScene("MenuScene");
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
}