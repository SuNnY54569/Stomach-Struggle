using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    public void StartGame()
    {
        SceneManagerClass.Instance.LoadNextScene();
    }

    public void Setting()
    {
        GameManager.Instance.BlurBackGround();
    }

    public void Exit()
    {
        SceneManagerClass.Instance.ExitGame();
    }
}
