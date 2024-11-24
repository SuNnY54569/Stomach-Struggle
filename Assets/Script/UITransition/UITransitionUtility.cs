using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        if (rectTransform == null) return;

        if (!initialPositions.ContainsKey(target))
        {
            initialPositions[target] = rectTransform.anchoredPosition; // Store initial position
        }

        targetPositions[target] = targetPosition; // Store target position
    }
    
    public void MoveOut(GameObject target, float duration = 1f, System.Action onComplete = null)
    {
        if (!targetPositions.ContainsKey(target)) return;

        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null) return;

        Vector2 targetPosition = targetPositions[target];
        rectTransform.DOAnchorPos(targetPosition, duration).OnComplete(() =>
        {
            target.SetActive(false); // Optional: Deactivate UI
            onComplete?.Invoke();
        });
    }
    
    public void MoveIn(GameObject target, float duration = 1f, System.Action onComplete = null)
    {
        if (!initialPositions.ContainsKey(target)) return;

        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null) return;

        Vector2 initialPosition = initialPositions[target];
        target.SetActive(true); // Reactivate the UI element
        rectTransform.DOAnchorPos(initialPosition, duration).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}
