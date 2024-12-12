using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Steak : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
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
    [SerializeField] private Collider2D col;

    private Vector3 originalPosition;
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
    
    #region Pointer Events
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused) return;
        
        if (Tools.Instance.currentlyCookingSteak != null && Tools.Instance.currentlyCookingSteak != this)
        {
            return;
        }

        OverCookedHandle();
        ToolsHandle();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused || !isDragging) return;
        
        transform.position = MouseWorldPosition();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused || !isDragging) return;
        
        isDragging = false;

        if (currentTool == Tools.ToolType.Tongs || currentTool == Tools.ToolType.Spatula)
        {
            HandleDrop();
        }
    }

    private void OverCookedHandle()
    {
        if (IsTopSideOvercooked() || IsBottomSideOvercooked())
        {
            if (currentTool == Tools.ToolType.Tongs || currentTool == Tools.ToolType.Spatula)
            {
                StopCooking();
                isDragging = true;
                return;
            }
        }
    }

    private void ToolsHandle()
    {
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
    
    private void HandleDrop()
    {
        if (IsDroppedOnLayer("TrashLayer"))
        {
            DestroySteak();
            SoundManager.PlaySound(SoundType.PlaceOnTrash, VolumeType.SFX);
            steakSpawner.HandleSteakLost();
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
            if (Tools.Instance.currentlyCookingSteak != null && Tools.Instance.currentlyCookingSteak != this)
            {
                ResetPosition();
                return;
            }
            SnapToPanCenter();
            StartCooking();
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
            isCooking = true;
        }
    }

    private void StopCooking()
    {
        isCooking = false;
    }

    private void FlipFood()
    {
        if (flipCooldownTimer > 0)
        {
            Debug.Log("Flip is on cooldown. Please wait.");
            return;
        }
        
        switch (isTopSideCooking)
        {
            case true when !IsTopSideCooked():
                return;
            case false when !IsBottomSideCooked():
                return;
        }
        
        isTopSideCooking = !isTopSideCooking;
        flipCooldownTimer = flipCooldownDuration;
        SoundManager.PlaySound(SoundType.flipMeat,VolumeType.SFX);
        FlipAnimation();
    }

    private void FlipAnimation()
    {
        float originalY = transform.position.y;
        float bounceHeight = 0.2f;
        float bounceDuration = 0.2f;
        float halfwayDuration = bounceDuration / 2f;
        
        LeanTween.sequence()
            .append(LeanTween.moveY(gameObject, originalY + bounceHeight, halfwayDuration).setEaseOutQuad())
            .append(() => spriteRenderer.flipX = !spriteRenderer.flipX)
            .append(LeanTween.moveY(gameObject, originalY, halfwayDuration).setEaseInQuad());
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
        else if (topSideCookingTimer < cookingTime && bottomSideCookingTimer < cookingTime)
        {
            HandleUndercooked();
        }
        else
        {
            HandleOvercooked();
        }
        
        Tools.Instance.ClearCurrentlyCookingSteak();
    }
    
    private void HandleUndercooked()
    {
        GameManager.Instance.healthManager.DecreaseHealth(1);
        ResetPosition();
    }

    private void HandleCooked()
    {
        GameManager.Instance.scoreManager.IncreaseScore(1);
        Tools.Instance.ClearCurrentlyCookingSteak();
        col.enabled = false;
    }

    private void HandleOvercooked()
    {
        isCooking = false;
        Tools.Instance.ClearCurrentlyCookingSteak();
        GameManager.Instance.healthManager.DecreaseHealth(1);
        steakSpawner.HandleSteakLost();
        col.enabled = false;
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
                .setOnComplete(() =>
                {
                    originalPosition = transform.position;
                    SoundManager.PlaySound(SoundType.flipMeat, VolumeType.SFX);
                });
        }
    }
    
    private void ResetPosition()
    {
        LeanTween.move(gameObject, originalPosition, 0.5f) // 0.5 seconds for the animation
            .setEase(LeanTweenType.easeInOutQuad);
        LeanTween.rotateZ(gameObject, 5f, 0.1f).setLoopPingPong(1);
    }
    
    private bool IsDroppedOnLayer(string layerName) => 
        Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask(layerName)) != null;
    
    private Vector3 MouseWorldPosition()
    {
        Vector3 inputPosition;
        if (Application.isMobilePlatform)
        {
            inputPosition = Input.GetTouch(0).position;
        }
        else
        {
            inputPosition = Input.mousePosition;
        }
        return Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, Camera.main.nearClipPlane));
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
