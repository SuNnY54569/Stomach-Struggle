﻿using System;
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
    
    private const float AnimationDuration = 0.5f;
    private const LeanTweenType CloseEaseType = LeanTweenType.easeInBack;
    private const LeanTweenType OpenEaseType = LeanTweenType.easeOutBack;
    
    private Dictionary<GameObject, Vector3> objectScales = new Dictionary<GameObject, Vector3>();

    private void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        
        foreach (var obj in objectsToClose)
        {
            objectScales[obj] = obj.transform.localScale;
        }

        foreach (var obj in objectsToOpen)
        {
            if (!objectScales.ContainsKey(obj))
            {
                objectScales[obj] = obj.transform.localScale;
            }
        }
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
        HandleObjectTransitions();
        HandleClawBehavior();
        ChangeAmbientSound();
        
        sprite.color = Color.white;
    }
    
    private void HandleObjectTransitions()
    {
        foreach (var objectToClose in objectsToClose)
        {
            LeanTween.scale(objectToClose, Vector3.zero, AnimationDuration)
                .setEase(CloseEaseType)
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    objectToClose.SetActive(false);
                    foreach (var objectToOpen in objectsToOpen)
                    {
                        objectToOpen.SetActive(true); // Ensure the object is active
                        objectToOpen.transform.localScale = Vector3.zero; // Start from zero scale

                        // Scale back to the cached original scale
                        if (objectScales.TryGetValue(objectToOpen, out var originalScale))
                        {
                            LeanTween.scale(objectToOpen, originalScale, AnimationDuration)
                                .setEase(OpenEaseType)
                                .setIgnoreTimeScale(true);
                        }
                    }
                });
        }
    }
    
    private void HandleClawBehavior()
    {
        if (!isReturnButton)
        {
            clawController.SetChance0to1(clawChance);
            clawController.PopReturnButtonUp();
        }
        else
        {
            clawController.PopReturnButtonDown();
            clawController.RePosition();
            clawController.SetDefaultSprite();
        }
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
