using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class waterShop : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Water Shop Button Settings")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject[] objectsToClose;
    [SerializeField] private GameObject[] objectsToOpen;

    private void Start()
    {
        GameManager.Instance.SetScoreTextActive(false);
    }

    // Called when the pointer clicks or taps the object
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused) return;

        ToggleObjects();

        SoundManager.PlaySound(SoundType.UIClick, VolumeType.SFX);
        sprite.color = Color.white;
    }

    // Called when the pointer enters the object (hover effect)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused) return;

        sprite.color = Color.gray;
    }

    // Called when the pointer exits the object
    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused) return;

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
