using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public static Tools Instance { get; private set; }
    public enum ToolType
    {
        None,
        Tongs,
        Spatula
    }

    [Header("Tool Settings")]
    public ToolType currentTool = ToolType.None;
    
    [SerializeField] private Texture2D tongsCursor;
    [SerializeField] private Texture2D spatulaCursor;
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;

    public Steak currentlyCookingSteak;

    private Steak currentSteak;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Update()
    {
        if (GameManager.Instance.isGamePaused) 
        {
            DeselectTool();
        }
        if (GameManager.Instance.GetScore() == GameManager.Instance.scoreMax)
        {
            DeselectTool();
        }
    }

    public void SetCurrentTool(ToolType tool)
    {
        currentTool = tool;
        UpdateCursorIcon(tool);
    }

    private void UpdateCursorIcon(ToolType tool)
    {
        switch (tool)
        {
            case ToolType.Tongs:
                Cursor.SetCursor(tongsCursor, cursorHotspot, CursorMode.ForceSoftware);
                break;
            case ToolType.Spatula:
                Cursor.SetCursor(spatulaCursor, cursorHotspot, CursorMode.ForceSoftware);
                break;
            case ToolType.None:
            default:
                // Revert to the system's default cursor
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
        }
    }
    
    public void DeselectTool()
    {
        currentTool = ToolType.None;
        UpdateCursorIcon(ToolType.None);
    }
    
    public void SetCurrentlyCookingSteak(Steak steak)
    {
        // Set the current steak being cooked
        currentlyCookingSteak = steak;
    }
    
    public void ClearCurrentlyCookingSteak()
    {
        // Clear the reference when the steak is no longer being cooked
        currentlyCookingSteak = null;
    }
    
    public bool IsCurrentlyCookingSteak(Steak steak)
    {
        // Check if the provided steak is the one currently being cooked
        return currentlyCookingSteak == steak;
    }
}
