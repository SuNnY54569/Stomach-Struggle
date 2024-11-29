using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Steak : MonoBehaviour
{
    #region Cooking Settings
    [Header("Cooking Settings")]
    [SerializeField] private float cookingTime = 5f;
    [SerializeField] private float overcookedTime = 10f;
    [SerializeField] private float flipCooldownDuration = 0.5f;
    
    private float topSideCookingTimer = 0f;
    private float bottomSideCookingTimer = 0f;
    private float flipCooldownTimer = 0f;

    public bool isTopSideCooking = true;
    private bool isCooking = false;
    private bool isDragging = false;

    [SerializeField] private Transform panCenter;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 originalPosition;
    private Tools.ToolType currentTool;
    private SteakSpawner steakSpawner;
    private Camera mainCamera;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        steakSpawner = FindObjectOfType<SteakSpawner>().GetComponent<SteakSpawner>();
        panCenter = GameObject.FindGameObjectWithTag("PanCenter").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        originalPosition = transform.position;
    }
    
    private void Update()
    {
        if (flipCooldownTimer > 0)
        {
            flipCooldownTimer -= Time.deltaTime;
        }
        
        currentTool = Tools.Instance.currentTool;
        
        if (isCooking)
        {
            UpdateCookingProgress();
        }
    }
    #endregion
    
    #region Mouse Interaction
    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;
        
        if (IsTopSideOvercooked() || IsBottomSideOvercooked())
        {
            if (currentTool == Tools.ToolType.Tongs || currentTool == Tools.ToolType.Spatula)
            {
                StopCooking();
                isDragging = true;
                return;
            }
        }
        
        if ((currentTool == Tools.ToolType.Tongs && !isCooking) || (currentTool == Tools.ToolType.Spatula && isCooking && IsCooked()))
        {
            StopCooking();
            LeanTween.move(gameObject, MouseWorldPosition(), 0.05f);
            isDragging = true;
        }
        else if (currentTool == Tools.ToolType.Spatula && isCooking && IsDroppedOnLayer("PanLayer"))
        {
            if (!IsBottomSideOvercooked() && !IsTopSideOvercooked())
            {
                FlipFood();
            }
            StartCooking();
        }
        else
        {
            Tools.Instance.ShowWarning(currentTool);
        }
    }
    
    private void OnMouseDrag()
    {
        if (GameManager.Instance.isGamePaused || !isDragging) return;
        
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }
    
    private void OnMouseUp()
    {
        if (GameManager.Instance.isGamePaused || !isDragging) return;
        
        isDragging = false;

        if (currentTool == Tools.ToolType.Tongs || currentTool == Tools.ToolType.Spatula)
        {
            HandleDrop();
        }
    }
    
    private void HandleDrop()
    {
        if (IsDroppedOnLayer("TrashLayer"))
        {
            DestroySteak();
            SoundManager.PlaySound(SoundType.PlaceOnTrash, VolumeType.SFX);
            steakSpawner.HandleSteakLost();
            Tools.Instance.DeselectTool();
        }
        else if (currentTool == Tools.ToolType.Tongs)
        {
            HandleTongsDrop();
        }
        else if (currentTool == Tools.ToolType.Spatula)
        {
            HandleSpatulaDrop();
        }
    }

    private void HandleTongsDrop()
    {
        if (IsDroppedOnLayer("PanLayer"))
        {
            if (Tools.Instance.currentlyCookingSteak != null)
            {
                ResetPosition();
                return;
            }
            SnapToPanCenter();
            originalPosition = transform.position;
            StartCooking();
            Tools.Instance.DeselectTool();
        }
        else if (IsDroppedOnLayer("PlateLayer"))
        {
            PlaceOnPlate();
        }
        else
        {
            ResetPosition();
        }
    }
    
    private void HandleSpatulaDrop()
    {
        if (IsDroppedOnLayer("PlateLayer"))
        {
            PlaceOnPlate();
        }
        else if (IsDroppedOnLayer("PanLayer"))
        {
            SnapToPanCenter();
            originalPosition = transform.position;
            StartCooking();
        }
        else
        {
            SnapToPanCenter();
            ResetPosition();
            StartCooking();
        }
    }
    #endregion

    #region Cooking Logic
    private void UpdateCookingProgress()
    {
        float currentTimer = isTopSideCooking ? topSideCookingTimer : bottomSideCookingTimer;
        currentTimer += Time.deltaTime;

        if (isTopSideCooking)
        {
            topSideCookingTimer = currentTimer;
        }
        else
        {
            bottomSideCookingTimer = currentTimer;
        }

        if (currentTimer >= overcookedTime)
        {
            MarkAsOvercooked();
        }
    }

    private void StartCooking()
    {
        if (Tools.Instance.currentlyCookingSteak == null)
        {
            Tools.Instance.SetCurrentlyCookingSteak(this);
            isCooking = true;
        }
        else if(Tools.Instance.IsCurrentlyCookingSteak(this) && isCooking == false)
        {
            ResetPosition();
            isCooking = true;
        }
    }

    private void StopCooking()
    {
        isCooking = false;
    }
    
    public void FlipFood()
    {
        if (flipCooldownTimer > 0)
        {
            Debug.Log("Flip is on cooldown. Please wait.");
            return;
        }
        
        isTopSideCooking = !isTopSideCooking;
        flipCooldownTimer = flipCooldownDuration;
        SoundManager.PlaySound(SoundType.flipMeat,VolumeType.SFX);
        
        float originalY = transform.position.y;
        float bounceHeight = 0.2f;
        float bounceDuration = 0.2f;
        float halfwayDuration = bounceDuration / 2f;
        
        LeanTween.moveY(gameObject, originalY + bounceHeight, halfwayDuration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
                
                LeanTween.moveY(gameObject, originalY, halfwayDuration)
                    .setEase(LeanTweenType.easeInQuad);
            });
    }
    
    private void PlaceOnPlate()
    {
        StopCooking();

        if (topSideCookingTimer >= cookingTime && bottomSideCookingTimer >= cookingTime &&
            topSideCookingTimer < overcookedTime && bottomSideCookingTimer < overcookedTime)
        {
            HandleCooked();
            SoundManager.PlaySound(SoundType.PlaceOnPlate,VolumeType.SFX);
        }
        else if (topSideCookingTimer < cookingTime || bottomSideCookingTimer < cookingTime)
        {
            HandleUndercooked();
        }
        else
        {
            HandleOvercooked();
        }
        
        Tools.Instance.ClearCurrentlyCookingSteak();
        Tools.Instance.DeselectTool();
    }
    
    private void HandleUndercooked()
    {
        GameManager.Instance.DecreaseHealth(1);
        ResetPosition();
        if (Tools.Instance.currentlyCookingSteak == gameObject && IsDroppedOnLayer("PanLayer"))
        {
            StartCooking();
        }
    }

    private void HandleCooked()
    {
        GameManager.Instance.IncreaseScore(1);
        Tools.Instance.ClearCurrentlyCookingSteak();
        gameObject.GetComponent<Collider2D>().enabled = false;
        steakSpawner.HandleSteakLost();
    }

    private void HandleOvercooked()
    {
        GameManager.Instance.DecreaseHealth(1);
        ResetPosition();
    }

    private void MarkAsOvercooked()
    {
        isCooking = false;
    }

    private bool IsCooked() => topSideCookingTimer >= cookingTime && bottomSideCookingTimer >= cookingTime;
    
    private void DestroySteak()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.2f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => Destroy(gameObject));
        LeanTween.rotateZ(gameObject, 5f, 0.1f).setLoopPingPong(1);
    }
    #endregion
    
    #region Helper Methods
    private void SnapToPanCenter()
    {
        if (panCenter != null)
        {
            LeanTween.move(gameObject, panCenter.position, 0.5f)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() => Debug.Log("Steak reset to original position."));
        }
    }
    
    private void ResetPosition()
    {
        LeanTween.move(gameObject, originalPosition, 0.5f) // 0.5 seconds for the animation
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Debug.Log("Steak reset to original position.");
            });
        LeanTween.rotateZ(gameObject, 5f, 0.1f).setLoopPingPong(1);
    }
    
    private bool IsDroppedOnLayer(string layerName) => 
        Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask(layerName)) != null;
    
    private Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }
    
    public float GetTotalCookingProgress()
    {
        float currentTimer = isTopSideCooking ? topSideCookingTimer : bottomSideCookingTimer;
        return Mathf.Clamp01(currentTimer / overcookedTime);
    }
    
    public float CookingTimeElapsed()
    {
        float elapsedTime = isTopSideCooking ? topSideCookingTimer : bottomSideCookingTimer;
        return elapsedTime;
    }
    
    public bool IsCooking()
    {
        return isCooking;
    }
    
    public bool IsTopSideCooked() => topSideCookingTimer >= cookingTime && topSideCookingTimer < overcookedTime;
    public bool IsBottomSideCooked() => bottomSideCookingTimer >= cookingTime && bottomSideCookingTimer < overcookedTime;
    public bool IsTopSideOvercooked() => topSideCookingTimer >= overcookedTime;
    public bool IsBottomSideOvercooked() => bottomSideCookingTimer >= overcookedTime;
    #endregion
}
