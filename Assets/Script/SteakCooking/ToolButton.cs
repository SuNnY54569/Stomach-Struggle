using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolButton : MonoBehaviour
{
    
    [Header("Tool Button Settings")]
    [SerializeField] private Tools.ToolType toolType;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject otherButton;

    private Vector3 initialScale;
    private Vector3 otherButtonInitialScale;
    private void Awake()
    {
        initialScale = gameObject.transform.localScale;
        if (otherButton != null)
        {
            otherButtonInitialScale = otherButton.transform.localScale;
        }
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;
        
        Tools.Instance.SetCurrentTool(toolType);
        
        AnimateButton(gameObject, false, initialScale);
        if (otherButton != null)
        {
            AnimateButton(otherButton, true, otherButtonInitialScale);
        }
        
        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
    }

    private void OnMouseOver()
    {
        if (GameManager.Instance.isGamePaused || !gameObject.activeSelf) return;
        sprite.color = Color.gray;
    }
    
    private void OnMouseExit()
    {
        if (GameManager.Instance.isGamePaused || !gameObject.activeSelf) return;
        sprite.color = Color.white;
    }
    
    private void AnimateButton(GameObject button, bool isPoppingUp, Vector3 originalScale)
    {
        Vector3 targetScale = isPoppingUp ? originalScale : Vector3.zero;
        float duration = 0.3f;
        
        if (isPoppingUp) button.SetActive(true);
        
        LeanTween.scale(button, targetScale, duration);
    }
}
