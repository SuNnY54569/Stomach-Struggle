using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolButton : MonoBehaviour
{
    [SerializeField] private Tools.ToolType toolType;

    public void OnMouseDown()
    {
        Tools.Instance.SetCurrentTool(toolType);
    }
}
