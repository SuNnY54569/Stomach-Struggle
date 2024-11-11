using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolButton : MonoBehaviour
{
    
    [SerializeField] private Tools.ToolType toolType;
    [SerializeField] private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;
        Tools.Instance.SetCurrentTool(toolType);
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
}
