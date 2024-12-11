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
    
    private static readonly Color DefaultColor = Color.white;
    private static readonly Color HoverColor = Color.gray;

    private void Start()
    {
        //GameManager.Instance.SetScoreTextActive(false);
    }

    // Called when the pointer clicks or taps the object
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused == true) return;

        ToggleObjects();
        PlayClickSound();
        SetSpriteColor(DefaultColor);
    }

    // Called when the pointer enters the object (hover effect)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused == true) return;

        SetSpriteColor(HoverColor);
    }

    // Called when the pointer exits the object
    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused == true) return;

        SetSpriteColor(DefaultColor);
    }

    private void ToggleObjects()
    {
        // Use a single method to toggle object states for clarity and maintainability
        SetActiveState(objectsToClose, false);
        SetActiveState(objectsToOpen, true);
    }
    
    private void SetActiveState(IEnumerable<GameObject> objects, bool state)
    {
        foreach (var obj in objects)
        {
            if (obj.activeSelf != state)
                obj.SetActive(state);
        }
    }

    private void SetSpriteColor(Color color)
    {
        if (sprite != null)
            sprite.color = color;
    }

    private void PlayClickSound()
    {
        SoundManager.PlaySound(SoundType.UIClick, VolumeType.SFX);
    }
}
