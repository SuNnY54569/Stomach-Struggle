using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ShopType
{
    GoodShop,
    MidShop,
    BadShop
}

public class ShopButton : MonoBehaviour
{

    [SerializeField] private ShopType shopType;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject[] objectsToClose;
    [SerializeField] private GameObject[] objectsToOpen;
    [Range(0,1)] [SerializeField] private float clawChance;
    [SerializeField] private ClawController clawController;
    [SerializeField] private bool isReturnButton;
    [SerializeField] private Collider2D collider;

    private void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        collider.enabled = !GameManager.Instance.isGamePaused;
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
        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
        foreach (var objectToClose in objectsToClose)
        {
            objectToClose.SetActive(false);
        }
        foreach (var objectToOpen in objectsToOpen)
        {
            objectToOpen.SetActive(true);
        }

        if (!isReturnButton)
        {
            clawController.SetChance0to1(clawChance);
        }
        else
        {
            clawController.RePosition();
            clawController.SetDefaultSprite();
        }
        ChangeAmbientSound();
        
        sprite.color = Color.white;
    }

    private void ChangeAmbientSound()
    {
        switch (shopType)
        {
            case ShopType.GoodShop:
                SoundManager.PlaySound(SoundType.CleanShopSFX,VolumeType.SFX);
                break;
            case ShopType.MidShop:
                SoundManager.instance.CrossfadeBGM(SoundType.ClawBG,0.5f);
                break;
            case ShopType.BadShop:
                SoundManager.instance.CrossfadeBGM(SoundType.BadShopBG,0.5f);
                break;
        }
    }
}
