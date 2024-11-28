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
    
    [SerializeField] private float topSideCookingTimer = 0f;
    [SerializeField] private float bottomSideCookingTimer = 0f;
    public bool isTopSideCooking = true;
    [SerializeField] private Transform panCenter;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField, Tooltip("Cooldown time in seconds before the food can be flipped again.")]
    private float flipCooldownDuration = 0.5f; // Example: 2 seconds cooldown
    private float flipCooldownTimer = 0f;
    
    private bool isCooking = false;
    private bool isDragging = false;
    private Vector2 originalPosition;
    private Tools.ToolType currentTool;
    private SteakSpawner steakSpawner;
    #endregion

    private void Awake()
    {
        steakSpawner = FindObjectOfType<SteakSpawner>().GetComponent<SteakSpawner>();
        panCenter = GameObject.FindGameObjectWithTag("PanCenter").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        
        // Update current tool
        currentTool = Tools.Instance.currentTool;

        // Update cooking progress if cooking
        if (isCooking)
        {
            UpdateCookingProgress();
        }
    }
    
    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;
        
        if (IsTopSideOvercooked() || IsBottomSideOvercooked())
        {
            if (currentTool == Tools.ToolType.Tongs || currentTool == Tools.ToolType.Spatula)
            {
                StopCooking();
                isDragging = true;
                return; // Skip the rest of the logic
            }
        }
        
        if ((currentTool == Tools.ToolType.Tongs && !isCooking) ||
            (currentTool == Tools.ToolType.Spatula && isCooking && IsCooked()))
        {
            StopCooking();
            isDragging = true;
        }
        else if (currentTool == Tools.ToolType.Spatula && isCooking && IsOnPan())
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
        if (GameManager.Instance.isGamePaused) return;
        if (!isDragging) return;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }
    
    private void OnMouseUp()
    {
        if (GameManager.Instance.isGamePaused) return;
        if (!isDragging) return;
        isDragging = false;

        if (currentTool == Tools.ToolType.Tongs || currentTool == Tools.ToolType.Spatula)
        {
            if (IsDroppedOnTrashCan())
            {
                DestroySteak();
                SoundManager.PlaySound(SoundType.PlaceOnTrash,VolumeType.SFX);
                steakSpawner.HandleSteakLost();
                Tools.Instance.DeselectTool();
            }
            else if (currentTool == Tools.ToolType.Tongs)
            {
                if (IsDroppedOnPan())
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
                else if (IsDroppedOnPlate())
                {
                    PlaceOnPlate();
                }
                else
                {
                    ResetPosition();
                }
            }
            else if (currentTool == Tools.ToolType.Spatula)
            {
                if (IsDroppedOnPlate())
                {
                    PlaceOnPlate();
                }
                else if (IsOnPan()) // Check if it's still on the pan
                {
                    SnapToPanCenter();
                    originalPosition = transform.position;
                    StartCooking(); // Resume cooking if still on the pan
                }
                else
                {
                    SnapToPanCenter();
                    ResetPosition();
                    StartCooking();
                }
            }
        }
    }

    private void SnapToPanCenter()
    {
        // Assuming your pan has a Collider2D to find its position
        Collider2D panCollider = GameObject.FindWithTag("Pan").GetComponent<Collider2D>();
        if (panCollider != null)
        {
            LeanTween.move(gameObject, panCenter.position, 0.5f) // 0.5 seconds for the animation
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() =>
                {
                    Debug.Log("Steak reset to original position.");
                });
        }
    }
    
    private bool IsDroppedOnPan()
    {
        return Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("PanLayer")) != null;;
    }
    private bool IsDroppedOnPlate()
    {
        return Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("PlateLayer")) != null;
    }
    
    private bool IsOnPan()
    {
        return Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("PanLayer")) != null;
    }
    
    private bool IsDroppedOnTrashCan()
    {
        return Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("TrashLayer")) != null;
    }
    
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
        else if (currentTimer >= cookingTime)
        {
            //MarkAsCooked();
        }
    }
    
    public void StartCooking()
    {
        if (Tools.Instance.currentlyCookingSteak == null) // Only start cooking if no steak is currently cooking
        {
            Tools.Instance.SetCurrentlyCookingSteak(this); // Set this steak as currently cooking
            isCooking = true;
        }
        else if(Tools.Instance.IsCurrentlyCookingSteak(this) && isCooking == false)
        {
            ResetPosition();
            isCooking = true;
        }
    }

    public void StopCooking()
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

        // Move up
        LeanTween.moveY(gameObject, originalY + bounceHeight, halfwayDuration)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                spriteRenderer.flipY = !spriteRenderer.flipY;
                
                LeanTween.moveY(gameObject, originalY, halfwayDuration)
                    .setEase(LeanTweenType.easeInQuad);
            });
    }
    
    public void ResetPosition()
    {
        LeanTween.move(gameObject, originalPosition, 0.5f) // 0.5 seconds for the animation
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Debug.Log("Steak reset to original position.");
            });
        LeanTween.rotateZ(gameObject, 5f, 0.1f).setLoopPingPong(1);
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
        if (Tools.Instance.currentlyCookingSteak == gameObject && IsOnPan())
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
    
    public bool IsCooked()
    {
        return topSideCookingTimer >= cookingTime && bottomSideCookingTimer >= cookingTime;
    }
    
    public float CookingTimeElapsed()
    {
        float elapsedTime = isTopSideCooking ? topSideCookingTimer : bottomSideCookingTimer;
        return elapsedTime;
    }
    
    private void DestroySteak()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.2f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Destroy(gameObject);
            });
        LeanTween.rotateZ(gameObject, 5f, 0.1f).setLoopPingPong(1);
    }
    
    public float GetTotalCookingProgress()
    {
        float currentTimer = isTopSideCooking ? topSideCookingTimer : bottomSideCookingTimer;
        return Mathf.Clamp01(currentTimer / overcookedTime);
    }
    
    public bool IsCooking()
    {
        return isCooking; // Return whether the steak is currently cooking
    }
    
    public bool IsTopSideCooked() => topSideCookingTimer >= cookingTime && topSideCookingTimer < overcookedTime;
    public bool IsBottomSideCooked() => bottomSideCookingTimer >= cookingTime && bottomSideCookingTimer < overcookedTime;
    public bool IsTopSideOvercooked() => topSideCookingTimer >= overcookedTime;
    public bool IsBottomSideOvercooked() => bottomSideCookingTimer >= overcookedTime;
}
