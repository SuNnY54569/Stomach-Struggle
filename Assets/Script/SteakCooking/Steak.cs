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
        if ((currentTool == Tools.ToolType.Tongs && !isCooking) ||
            (currentTool == Tools.ToolType.Spatula && isCooking && IsCooked()))
        {
            StopCooking();
            isDragging = true;
        }
        else if (currentTool == Tools.ToolType.Spatula && isCooking && IsOnPan())
        {
            FlipFood();
            StartCooking();
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

    private void OnMouseOver()
    {
        //spriteRenderer.color = Color.gray;
    }

    private void OnMouseExit()
    {
        //spriteRenderer.color = Color.white;
    }

    private void SnapToPanCenter()
    {
        // Assuming your pan has a Collider2D to find its position
        Collider2D panCollider = GameObject.FindWithTag("Pan").GetComponent<Collider2D>();
        if (panCollider != null)
        {
            transform.position = panCenter.position; // Snap to the center of the pan
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
            MarkAsCooked();
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
        isTopSideCooking = !isTopSideCooking;
        if (isTopSideCooking)
        {
            gameObject.transform.rotation = new Quaternion(0f, 0, 0,0 );
        }
        else
        {
            gameObject.transform.rotation = new Quaternion(180f, 0, 0,0 );
        }
    }
    
    public void ResetPosition()
    {
        transform.position = originalPosition;
    }
    
    public void PlaceOnPlate()
    {
        StopCooking();

        if (topSideCookingTimer >= cookingTime && bottomSideCookingTimer >= cookingTime &&
            topSideCookingTimer < overcookedTime && bottomSideCookingTimer < overcookedTime)
        {
            HandleCooked();
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
    
    private void MarkAsCooked()
    {
        gameObject.tag = "Cooked";
    }

    private void MarkAsOvercooked()
    {
        gameObject.tag = "Overcooked";
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
        Destroy(gameObject);
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
