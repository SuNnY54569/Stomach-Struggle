using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransitionUtility : MonoBehaviour
{
    public static UITransitionUtility Instance { get; private set; }
    
    private Dictionary<GameObject, Vector2> initialPositions = new Dictionary<GameObject, Vector2>();
    private Dictionary<GameObject, Vector2> targetPositions = new Dictionary<GameObject, Vector2>();
    
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
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning($"[UITransitionUtility] GameObject {target.name} is missing a RectTransform!");
            return;
        }

        if (!initialPositions.ContainsKey(target))
        {
            initialPositions[target] = rectTransform.anchoredPosition; // Store initial position
        }

        targetPositions[target] = targetPosition; // Store target position
    }
    
    public void MoveIn(GameObject target, LeanTweenType easeType = LeanTweenType.easeInOutQuad, float duration = 1f, System.Action onComplete = null)
    {
        if (!targetPositions.ContainsKey(target))
        {
            Debug.LogWarning($"[UITransitionUtility] Target position not initialized for {target.name}!");
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
}
