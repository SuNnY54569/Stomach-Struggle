using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransitionUtility : MonoBehaviour
{
    public static UITransitionUtility Instance { get; private set; }
    
    private class UIElementState
    {
        public Vector2 InitialPosition;
        public Vector2 TargetPosition;
        public Vector3 InitialScale;
        public RectTransform RectTransform;
    }

    private Dictionary<GameObject, UIElementState> uiElements = new Dictionary<GameObject, UIElementState>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public void Initialize(GameObject target, Vector2 targetPosition)
    {
        if (target == null)
        {
            Debug.LogWarning($"[UITransitionUtility] Target is null!");
            return;
        }

        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning($"[UITransitionUtility] {target.name} is missing a RectTransform!");
            return;
        }

        if (!uiElements.ContainsKey(target))
        {
            uiElements[target] = new UIElementState
            {
                InitialPosition = rectTransform.anchoredPosition,
                TargetPosition = targetPosition,
                InitialScale = rectTransform.localScale,
                RectTransform = rectTransform
            };
        }
    }
    
    public void MoveIn(GameObject target, LeanTweenType easeType = LeanTweenType.easeInOutQuad, float duration = 1f, Action onComplete = null)
    {
        if (!uiElements.TryGetValue(target, out var state))
        {
            Debug.LogWarning($"[UITransitionUtility] Target not initialized for {target.name}!");
            return;
        }

        if (LeanTween.isTweening(target))
        {
            LeanTween.cancel(target);
        }

        target.SetActive(true);
        LeanTween.move(state.RectTransform, state.TargetPosition, duration)
            .setIgnoreTimeScale(true)
            .setEase(easeType)
            .setOnComplete(() => onComplete?.Invoke());
    }
    
    public void MoveOut(GameObject target, LeanTweenType easeType = LeanTweenType.easeInOutQuad, float duration = 1f, Action onComplete = null)
    {
        if (!uiElements.TryGetValue(target, out var state))
        {
            Debug.LogWarning($"[UITransitionUtility] Initial position not initialized for {target.name}!");
            return;
        }

        if (LeanTween.isTweening(target))
        {
            LeanTween.cancel(target);
        }

        LeanTween.move(state.RectTransform, state.InitialPosition, duration)
            .setIgnoreTimeScale(true)
            .setEase(easeType)
            .setOnComplete(() =>
            {
                target.SetActive(false);
                onComplete?.Invoke();
            });
    }
    
    public void PopUp(GameObject target, LeanTweenType easeType = LeanTweenType.easeOutBack, float duration = 0.5f, Action onComplete = null)
    {
        if (!uiElements.TryGetValue(target, out var state))
        {
            Debug.LogWarning($"[UITransitionUtility] Target scale not initialized for {target.name}!");
            return;
        }

        if (LeanTween.isTweening(target))
        {
            LeanTween.cancel(target);
        }

        target.SetActive(true);
        state.RectTransform.localScale = Vector3.zero;

        LeanTween.scale(state.RectTransform, state.InitialScale, duration)
            .setEase(easeType)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => onComplete?.Invoke());
    }
    
    public void PopDown(GameObject target, LeanTweenType easeType = LeanTweenType.easeInBack, float duration = 0.5f, Action onComplete = null)
    {
        if (!uiElements.TryGetValue(target, out var state))
        {
            Debug.LogWarning($"[UITransitionUtility] Target not initialized for {target.name}!");
            return;
        }

        if (LeanTween.isTweening(target))
        {
            LeanTween.cancel(target);
        }

        LeanTween.scale(state.RectTransform, Vector3.zero, duration)
            .setEase(easeType)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                target.SetActive(false);
                onComplete?.Invoke();
            });
    }
}
