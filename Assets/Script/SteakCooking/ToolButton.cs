using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolButton : MonoBehaviour
{
    [SerializeField] private Tools.ToolType toolType;

    public void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;
        Tools.Instance.SetCurrentTool(toolType);
    }
}
