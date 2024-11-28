using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private Vector2 tongsCursorHotspot;
    [SerializeField] private Vector2 spatulaCursorHotspot;
    
    [Header("Warning Setting")]
    [SerializeField] private TextMeshProUGUI warningMessageText;
    [SerializeField] private string warningMessage;
    [SerializeField] private string tongsWarning = "อันนี้ต้องใช้ตะหลิวนะ";
    [SerializeField] private string spatulaWarning = "อันนี้ต้องใช้ทีคีบนะ";
    [SerializeField] private float warningFadeDuration = 1.5f;
    [SerializeField] private float warningCooldown = 1.5f;
    private float lastWarningTime = -Mathf.Infinity; 

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
        UITransitionUtility.Instance.Initialize(warningMessageText.gameObject,Vector2.zero);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGamePaused) 
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
                Cursor.SetCursor(tongsCursor, tongsCursorHotspot, CursorMode.ForceSoftware);
                break;
            case ToolType.Spatula:
                Cursor.SetCursor(spatulaCursor, spatulaCursorHotspot, CursorMode.ForceSoftware);
                break;
            case ToolType.None:
            default:
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
    
    public void ShowWarning(ToolType toolType)
    {
        if (Time.time - lastWarningTime < warningCooldown) return;
        if (warningMessageText == null) return;
        
        lastWarningTime = Time.time;
        switch (toolType)
        {
            case ToolType.Spatula :
                warningMessageText.text = spatulaWarning;
                break;
            case ToolType.Tongs:
                warningMessageText.text = tongsWarning;
                break;
            default:
                warningMessageText.text = warningMessage;
                break;
        }
        UITransitionUtility.Instance.PopUp(warningMessageText.gameObject);

        StartCoroutine(FadeOutWarning());
    }
    
    private IEnumerator FadeOutWarning()
    {
        yield return new WaitForSeconds(warningFadeDuration);
        UITransitionUtility.Instance.PopDown(warningMessageText.gameObject);
    }
}
