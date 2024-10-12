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
    private bool isTopSideCooking = true;
    private bool isCooking = false;
    private bool isDragging = false;
    private Vector2 originalPosition;
    private Tools.ToolType currentTool;
    #endregion
    
    #region UI Components
    [Header("UI Components")]
    [SerializeField] private Slider cookingProgressBar;
    [SerializeField] private Image progressBarFill;
    [SerializeField] private TMP_Text cookingClockText;
    
    private Color rawColor = Color.red;
    private Color cookedColor = Color.green;
    private Color overcookedColor = Color.black;
    #endregion
    
    private void Start()
    {
        originalPosition = transform.position;
        InitializeProgressBar();
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
        if (!isDragging) return;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }
    
    private void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;

            if (currentTool == Tools.ToolType.Tongs)
            {
                if (IsDroppedOnPan())
                {
                    SnapToPanCenter();
                    originalPosition = transform.position;
                    StartCooking();
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
            transform.position = panCollider.bounds.center; // Snap to the center of the pan
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
    
    private void InitializeProgressBar()
    {
        if (cookingProgressBar != null)
        {
            cookingProgressBar.maxValue = cookingTime;
            cookingProgressBar.value = 0;
            progressBarFill.color = rawColor;
            ShowProgressBar(false);
        }
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

        UpdateProgressBar(currentTimer);

        if (currentTimer >= overcookedTime)
        {
            MarkAsOvercooked();
        }
        else if (currentTimer >= cookingTime)
        {
            MarkAsCooked();
        }
    }
    
    private void UpdateProgressBar(float timer)
    {
        if (cookingProgressBar != null)
        {
            cookingProgressBar.value = timer;
            progressBarFill.color = CalculateProgressColor(timer);
        }
    }
    
    private Color CalculateProgressColor(float timer)
    {
        if (timer <= cookingTime)
        {
            float t = timer / cookingTime;
            return Color.Lerp(rawColor, cookedColor, t);
        }
        else
        {
            float t = (timer - cookingTime) / (overcookedTime - cookingTime);
            return Color.Lerp(cookedColor, overcookedColor, t);
        }
    }
    
    public void StartCooking()
    {
        if (Tools.Instance.currentlyCookingSteak == null) // Only start cooking if no steak is currently cooking
        {
            Tools.Instance.SetCurrentlyCookingSteak(this); // Set this steak as currently cooking
            isCooking = true;
            ShowProgressBar(true);
        }
        else if(Tools.Instance.IsCurrentlyCookingSteak(this) && isCooking == false)
        {
            ResetPosition();
            isCooking = true;
            ShowProgressBar(true);
        }
    }

    public void StopCooking()
    {
        isCooking = false;
        ShowProgressBar(false);
    }
    
    public void FlipFood()
    {
        isTopSideCooking = !isTopSideCooking;
        cookingProgressBar.value = isTopSideCooking ? topSideCookingTimer : bottomSideCookingTimer;
        progressBarFill.color = isTopSideCooking ? CalculateProgressColor(topSideCookingTimer) : CalculateProgressColor(bottomSideCookingTimer);
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
        gameObject.GetComponent<Collider2D>().enabled = false;
    }

    private void HandleUndercooked()
    {
        GameManager.Instance.DecreaseHealth(1);
    }

    private void HandleCooked()
    {
        GameManager.Instance.IncreaseScore(1);
    }

    private void HandleOvercooked()
    {
        GameManager.Instance.DecreaseHealth(1);
    }
    
    public void ShowProgressBar(bool show)
    {
        if (cookingProgressBar != null)
        {
            cookingProgressBar.gameObject.SetActive(show);
        }
    }
    
    private void MarkAsCooked()
    {
        gameObject.tag = "Cooked";
    }

    private void MarkAsOvercooked()
    {
        gameObject.tag = "Overcooked";
        isCooking = false;
        ShowProgressBar(false);
    }
    
    public bool IsCooked()
    {
        return topSideCookingTimer >= cookingTime && bottomSideCookingTimer >= cookingTime;
    }
    
    public float CookingTimeRemaining()
    {
        float remainingTime = 0f;
        if (isTopSideCooking)
        {
            remainingTime = cookingTime - topSideCookingTimer;
        }
        else
        {
            remainingTime = cookingTime - bottomSideCookingTimer;
        }
        
        return Mathf.Max(remainingTime, 0);
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
