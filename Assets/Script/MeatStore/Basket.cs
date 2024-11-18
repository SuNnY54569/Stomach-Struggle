using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField] private ClawController clawController;
    [SerializeField] private SpriteRenderer objectSprite;
    [SerializeField] private Sprite[] BasketSprite;

    private void Awake()
    {
        clawController = FindObjectOfType<ClawController>().GetComponent<ClawController>();
    }

    private void Update()
    {
        UpdateVisual();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BadMeat"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance.DecreaseHealth(1);
        }
        else if (collision.CompareTag("GoodMeat"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance.IncreaseScore(1);
        }
        
        SoundManager.PlaySound(SoundType.PickUpMeat,VolumeType.SFX);
        clawController.SetDefaultSprite();
    }

    private void UpdateVisual()
    {
        switch (GameManager.Instance.GetScore())
        {
            case 0:
                objectSprite.sprite = BasketSprite[0];
                return;
            case 1:
                objectSprite.sprite = BasketSprite[0];
                return;
            case 2:
                objectSprite.sprite = BasketSprite[1];
                return;
            case 3:
                objectSprite.sprite = BasketSprite[2];
                return;
            case > 3:
                objectSprite.sprite = BasketSprite[3];
                return;
        }
    }
}
