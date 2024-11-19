using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treatment : MonoBehaviour
{
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
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("BadTreat"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DecreaseHealth(1);
            }
            Destroy(gameObject);
        }
    }

    private void OnMouseOver()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.gray;
    }

    private void OnMouseExit()
    {
        sprite.color = originalColor;
    }
}
