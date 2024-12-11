using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    [SerializeField] private GameObject picturePanel;
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject creditPanel;
    
    private void Start()
    {
        Initialize();
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = GameManager.Instance.noPostCamera;
        canvas.planeDistance = 1;
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
        UITransitionUtility.Instance.MoveOut(creditPanel, LeanTweenType.easeInQuad, 0.5f);
        UITransitionUtility.Instance.MoveIn(picturePanel, LeanTweenType.easeOutBounce);
        UITransitionUtility.Instance.MoveIn(buttonPanel, LeanTweenType.easeOutBounce);
        GameManager.Instance.BlurBackGround();
    }

    public void Credit()
    {
        PlayUIClick();
        UITransitionUtility.Instance.MoveIn(creditPanel, LeanTweenType.easeInQuad, 0.5f);
        UITransitionUtility.Instance.MoveOut(picturePanel, LeanTweenType.easeOutBounce);
        UITransitionUtility.Instance.MoveOut(buttonPanel, LeanTweenType.easeOutBounce);
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

    private void Initialize()
    {
        UITransitionUtility.Instance.Initialize(picturePanel, Vector2.zero);
        UITransitionUtility.Instance.Initialize(buttonPanel, Vector2.zero);
        UITransitionUtility.Instance.Initialize(settingPanel, Vector2.zero);
        UITransitionUtility.Instance.Initialize(creditPanel, Vector2.zero);
        UITransitionUtility.Instance.MoveIn(picturePanel, LeanTweenType.easeOutBounce, 2f);
        UITransitionUtility.Instance.MoveIn(buttonPanel,LeanTweenType.easeOutBounce, 2f);
    }
}
