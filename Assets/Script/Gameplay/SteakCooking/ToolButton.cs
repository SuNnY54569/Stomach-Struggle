using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    
    [Header("Tool Button Settings")]
    [SerializeField] private Tools.ToolType toolType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject otherButton;
    [SerializeField] private Collider2D buttonCollider;

    private Vector3 initialScale;
    private Vector3 otherButtonInitialScale;
    
    private static readonly Color HoverColor = Color.gray;
    private static readonly Color NormalColor = Color.white;
    private const float AnimationDuration = 0.3f;
    
    private void Awake()
    {
        initialScale = transform.localScale;
        if (otherButton != null)
        {
            otherButtonInitialScale = otherButton.transform.localScale;
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused) return;
        
        if (Time.time - Tools.Instance.lastToolChangeTime < Tools.Instance.toolChangeCooldown) return;
        
        Tools.Instance.SetCurrentTool(toolType);
        
        AnimateButton(gameObject, false, initialScale);
        if (otherButton != null)
        {
            AnimateButton(otherButton, true, otherButtonInitialScale);
        }
        
        StartCoroutine(DisableButtonsTemporarily());
        
        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused || !gameObject.activeSelf) return;
        spriteRenderer.color = HoverColor;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused || !gameObject.activeSelf) return;
        spriteRenderer.color = NormalColor;
    }
    
    private void AnimateButton(GameObject button, bool isPoppingUp, Vector3 originalScale)
    {
        if (button == null) return;
        
        Vector3 targetScale = isPoppingUp ? originalScale : Vector3.zero;
        
        if (isPoppingUp && !button.activeSelf)
        {
            button.SetActive(true);
        }

        LeanTween.scale(button, targetScale, AnimationDuration);
    }
    
    private IEnumerator DisableButtonsTemporarily()
    {
        SetButtonsInteractable(false);
        yield return new WaitForSeconds(Tools.Instance.toolChangeCooldown);
        SetButtonsInteractable(true);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (buttonCollider != null)
        {
            buttonCollider.enabled = interactable;
        }
    }
}
