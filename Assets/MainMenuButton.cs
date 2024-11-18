using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    public void StartGame()
    {
        PlayUIClick();
        SceneManagerClass.Instance.LoadNextScene();
    }

    public void Setting()
    {
        PlayUIClick();
        GameManager.Instance.BlurBackGround();
    }

    public void Exit()
    {
        PlayUIClick();
        SceneManagerClass.Instance.ExitGame();
    }

    public void PlayUIClick()
    {
        SoundManager.instance.PlayUIClick();
    }
}
