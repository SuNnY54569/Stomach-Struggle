using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class itemClickWater : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite[] waterSprites;
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private Vector3 targetScale = Vector3.zero;
    
    public Collider2D col;
    public spawnWatertwo spawnWatertwo;
    
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        RandomizeFoodSprites();
    }

    private void RandomizeFoodSprites()
    {
        if(waterSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, waterSprites.Length);
            spriteRenderer.sprite = waterSprites[randomIndex];
        }
    }

    private void OnMouseOver()
    {
        if (GameManager.Instance.isGamePaused) return;
        
        sprite.color = Color.gray;
    }

    private void OnMouseExit()
    {
        if (GameManager.Instance.isGamePaused) return;
        sprite.color = Color.white;
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;

        if (gameObject.CompareTag("GoodWater"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DecreaseHealth(1);
            }
        }
        else if (gameObject.CompareTag("BadWater"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IncreaseScore(1);
                if (GameManager.Instance.GetScore() == GameManager.Instance.scoreMax)
                {
                    spawnWatertwo.DisableAllCollider();
                }
            }
        }
        LeanTween.scale(gameObject, targetScale, scaleDuration)
            .setEase(LeanTweenType.easeInOutQuad) // Optional: Customize easing
            .setOnComplete(() =>
            {
                // Optionally destroy the object or trigger other actions
                gameObject.SetActive(false);
            });
        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
    }
}
