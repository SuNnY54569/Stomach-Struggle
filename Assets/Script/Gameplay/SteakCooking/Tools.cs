using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public static Tools Instance { get; private set; }
    
    #region Enum & Fields
    public enum ToolType
    {
        None,
        Tongs,
        Spatula
    }

    [Header("Tool Settings")]
    public ToolType currentTool = ToolType.None;
    private ToolType lastTool = ToolType.None;

    [SerializeField] private Texture2D tongsCursor;
    [SerializeField] private Texture2D spatulaCursor;
    [SerializeField] private Vector2 tongsCursorHotspot;
    [SerializeField] private Vector2 spatulaCursorHotspot;

    [Header("Warning Settings")]
    [SerializeField] private TextMeshProUGUI warningMessageText;
    [SerializeField] private string defaultWarningMessage = "โปรดใช้เครื่องมือที่เหมาะสม!";
    [SerializeField] private string tongsWarning = "อันนี้ต้องใช้ตะหลิวนะ";
    [SerializeField] private string spatulaWarning = "อันนี้ต้องใช้ทีคีบนะ";
    [SerializeField] private float warningFadeDuration = 1.5f;
    [SerializeField] private float warningCooldown = 1.5f;

    private float lastWarningTime = -Mathf.Infinity;

    [Header("Cooking Management")]
    public Steak currentlyCookingSteak;
    #endregion
    
    #region Unity Lifecycle
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
    
    private void OnEnable()
    {
        GameManager.OnGamePaused += HandleGamePaused;
        GameManager.OnGameUnpaused += HandleGameUnpaused;
    }

    private void OnDisable()
    {
        GameManager.OnGamePaused -= HandleGamePaused;
        GameManager.OnGameUnpaused -= HandleGameUnpaused;
    }

    private void Update()
    {
        if (GameManager.Instance.scoreManager.GetScore() != GameManager.Instance.scoreManager.scoreMax) return;
        lastTool = ToolType.None;
        currentTool = ToolType.None;
    }
    #endregion
    
    #region Tool Management
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

    private void StoreAndDeselectTool()
    {
        lastTool = currentTool;
        DeselectTool();
    }
    
    private void ResumeLastTool()
    {
        if (currentTool == ToolType.None)
        {
            SetCurrentTool(lastTool);
        }
    }

    private void DeselectTool()
    {
        currentTool = ToolType.None;
        UpdateCursorIcon(ToolType.None);
    }
    #endregion
    
    #region Cooking Management
    public void SetCurrentlyCookingSteak(Steak steak)
    {
        currentlyCookingSteak = steak;
    }

    public void ClearCurrentlyCookingSteak()
    {
        currentlyCookingSteak = null;
    }

    public bool IsCurrentlyCookingSteak(Steak steak)
    {
        return currentlyCookingSteak == steak;
    }
    #endregion
    
    #region Warning System
    public void ShowWarning(ToolType toolType)
    {
        if (Time.time - lastWarningTime < warningCooldown || warningMessageText == null) return;

        lastWarningTime = Time.time;

        switch (toolType)
        {
            case ToolType.Spatula:
                warningMessageText.text = spatulaWarning;
                break;
            case ToolType.Tongs:
                warningMessageText.text = tongsWarning;
                break;
            default:
                warningMessageText.text = defaultWarningMessage;
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
    #endregion
    
    #region Game State Handlers
    private void HandleGamePaused()
    {
        StoreAndDeselectTool();
    }

    private void HandleGameUnpaused()
    {
        ResumeLastTool();
    }
    #endregion
    
}
