using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class waterShop : MonoBehaviour
{
    [Header("Water Shop Button Settings")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject[] objectsToClose;
    [SerializeField] private GameObject[] objectsToOpen;
    [SerializeField] private bool isReturnButton;

    private void Start()
    {
        GameManager.Instance.SetScoreTextActive(false);
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
        
        ToggleObjects();

        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
        sprite.color = Color.white;
    }
    
    private void ToggleObjects()
    {
        // Close all specified objects
        foreach (var objectToClose in objectsToClose)
        {
            objectToClose.SetActive(false);
        }

        // Open all specified objects
        foreach (var objectToOpen in objectsToOpen)
        {
            objectToOpen.SetActive(true);
        }
    }
}
