using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treatment : MonoBehaviour
{
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private Vector3 targetScale = Vector3.zero;
    private SpriteRenderer sprite;
    private Color originalColor;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;

        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
        if (gameObject.CompareTag("GoodTreat"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IncreaseScore(1);
            }
        }
        else if (gameObject.CompareTag("BadTreat"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DecreaseHealth(1);
            }
        }
        LeanTween.scale(gameObject, targetScale, scaleDuration)
            .setEase(LeanTweenType.easeInOutQuad) // Optional: Customize easing
            .setOnComplete(() =>
            {
                // Optionally destroy the object or trigger other actions
                gameObject.SetActive(false);
            });
    }

    private void OnMouseOver()
    {
        if (GameManager.Instance.isGamePaused) return;
        
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.gray;
    }

    private void OnMouseExit()
    {
        if (GameManager.Instance.isGamePaused) return;
        
        sprite.color = originalColor;
    }
}
