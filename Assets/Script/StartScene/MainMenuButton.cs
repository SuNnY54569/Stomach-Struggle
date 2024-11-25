using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    [SerializeField] private GameObject picturePanel;
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private GameObject settingPanel;

    private void Awake()
    {
    }

    private void Start()
    {
        UITransitionUtility.Instance.Initialize(picturePanel, new Vector2(0,0));
        UITransitionUtility.Instance.Initialize(buttonPanel, new Vector2(0,0));
        UITransitionUtility.Instance.Initialize(settingPanel, new Vector2(0,0));
        UITransitionUtility.Instance.MoveIn(picturePanel, LeanTweenType.easeOutBounce, 2f);
        UITransitionUtility.Instance.MoveIn(buttonPanel,LeanTweenType.easeOutBounce, 2f);
    }

    public void StartGame()
    {
        PlayUIClick();
        UITransitionUtility.Instance.MoveOut(picturePanel, LeanTweenType.easeOutBounce);
        UITransitionUtility.Instance.MoveOut(buttonPanel, LeanTweenType.easeOutBounce);
        SceneManagerClass.Instance.LoadNextScene();
    }

    public void Setting()
    {
        PlayUIClick();
        UITransitionUtility.Instance.MoveIn(settingPanel, LeanTweenType.easeInQuad, 0.5f);
        UITransitionUtility.Instance.MoveOut(picturePanel, LeanTweenType.easeOutBounce);
        UITransitionUtility.Instance.MoveOut(buttonPanel, LeanTweenType.easeOutBounce);
        GameManager.Instance.BlurBackGround();
    }

    public void Resume()
    {
        PlayUIClick();
        UITransitionUtility.Instance.MoveOut(settingPanel, LeanTweenType.easeInQuad, 0.5f);
        UITransitionUtility.Instance.MoveIn(picturePanel, LeanTweenType.easeOutBounce);
        UITransitionUtility.Instance.MoveIn(buttonPanel, LeanTweenType.easeOutBounce);
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
