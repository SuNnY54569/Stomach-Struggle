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
    
    private bool isPointerOver = false;

    private void Start()
    {
        GameManager.Instance.SetScoreTextActive(false);
    }
    
    private void Update()
    {
        if (GameManager.Instance.isGamePaused) return;

        // Detect touch or mouse click
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            HandlePointerDown();
        }

        // Check hover-like behavior
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            HandleMouseHover();
        }
    }

    private void HandlePointerDown()
    {
        // Check if the pointer is over this object
        Vector2 pointerPosition = GetPointerPosition();
        RaycastHit2D hit = Physics2D.Raycast(pointerPosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            ToggleObjects();
            SoundManager.PlaySound(SoundType.UIClick, VolumeType.SFX);
            sprite.color = Color.white;
        }
    }

    private void HandleMouseHover()
    {
        Vector2 pointerPosition = GetPointerPosition();
        RaycastHit2D hit = Physics2D.Raycast(pointerPosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (!isPointerOver)
            {
                sprite.color = Color.gray;
                isPointerOver = true;
            }
        }
        else
        {
            if (isPointerOver)
            {
                sprite.color = Color.white;
                isPointerOver = false;
            }
        }
    }
    
    private Vector2 GetPointerPosition()
    {
        if (Input.touchCount > 0)
        {
            return Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
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
