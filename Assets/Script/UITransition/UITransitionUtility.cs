using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransitionUtility : MonoBehaviour
{
    public static UITransitionUtility Instance { get; private set; }
    
    private Dictionary<GameObject, Vector2> initialPositions = new Dictionary<GameObject, Vector2>();
    private Dictionary<GameObject, Vector2> targetPositions = new Dictionary<GameObject, Vector2>();
    private Dictionary<GameObject, Vector3> initialScales = new Dictionary<GameObject, Vector3>();
    
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

    private void Start()
    {
        Initialize(GameManager.Instance.gameplayPanel,Vector2.zero);
        Initialize(GameManager.Instance.tutorialPanel,Vector2.zero);
    }

    public void Initialize(GameObject target, Vector2 targetPosition)
    {
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning($"[UITransitionUtility] GameObject {target.name} is missing a RectTransform!");
            return;
        }

        if (!initialPositions.ContainsKey(target))
        {
            initialPositions[target] = rectTransform.anchoredPosition;
        }
        
        if (!initialScales.ContainsKey(target))
        {
            initialScales[target] = rectTransform.localScale;
        }

        targetPositions[target] = targetPosition;
    }
    
    public void MoveIn(GameObject target, LeanTweenType easeType = LeanTweenType.easeInOutQuad, float duration = 1f, System.Action onComplete = null)
    {
        if (!targetPositions.ContainsKey(target))
        {
            Debug.LogWarning($"[UITransitionUtility] Target position not initialized for {target.name}!");
            Initialize(target, Vector2.zero);
            MoveIn(target);
            return;
        }

        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning($"[UITransitionUtility] GameObject {target.name} is missing a RectTransform!");
            return;
        }

        Vector2 targetPosition = targetPositions[target];
        target.SetActive(true); // Ensure the UI element is active

        LeanTween.move(rectTransform, targetPosition, duration)
            .setIgnoreTimeScale(true) // Use unscaled time
            .setEase(easeType)        // Set the easing type
            .setOnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }
    
    public void MoveOut(GameObject target, LeanTweenType easeType = LeanTweenType.easeInOutQuad, float duration = 1f, System.Action onComplete = null)
    {
        if (!initialPositions.ContainsKey(target))
        {
            Debug.LogWarning($"[UITransitionUtility] Initial position not initialized for {target.name}!");
            return;
        }

        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning($"[UITransitionUtility] GameObject {target.name} is missing a RectTransform!");
            return;
        }

        Vector2 initialPosition = initialPositions[target];
        LeanTween.move(rectTransform, initialPosition, duration)
            .setIgnoreTimeScale(true) // Use unscaled time
            .setEase(easeType)        // Set the easing type
            .setOnComplete(() =>
            {
                target.SetActive(false); // Deactivate UI element
                onComplete?.Invoke();
            });
    }
    
    public void PopUp(GameObject target, LeanTweenType easeType = LeanTweenType.easeOutBack, float duration = 0.5f, System.Action onComplete = null)
    {
        if (!initialScales.ContainsKey(target))
        {
            Debug.LogWarning($"[UITransitionUtility] Target scale not initialized for {target.name}!");
            return;
        }
        
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning($"[UITransitionUtility] GameObject {target.name} is missing a RectTransform!");
            return;
        }

        target.SetActive(true); // Ensure the panel is active
        rectTransform.localScale = Vector3.zero; // Start from zero scale

        Vector3 initialScale = initialScales[target];
        LeanTween.scale(rectTransform,initialScale, duration)
            .setEase(easeType) // Set easing type
            .setIgnoreTimeScale(true) // Use unscaled time
            .setOnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }
    
    public void PopDown(GameObject target, LeanTweenType easeType = LeanTweenType.easeInBack, float duration = 0.5f, System.Action onComplete = null)
    {
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning($"[UITransitionUtility] GameObject {target.name} is missing a RectTransform!");
            return;
        }

        LeanTween.scale(rectTransform, Vector3.zero, duration)
            .setEase(easeType) // Set easing type
            .setIgnoreTimeScale(true) // Use unscaled time
            .setOnComplete(() =>
            {
                target.SetActive(false); // Deactivate the panel after scaling down
                onComplete?.Invoke();
            });
    }
}
