using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodCooking : MonoBehaviour
{
    #region Cooking Settings
    [Header("Cooking Settings")]
    [SerializeField, Tooltip("Time it takes to cook the food.")]
    public float cookingTime;
    
    [SerializeField, Tooltip("Time after which the food is considered overcooked.")]
    private float overcookedTime;
    
    public float topSideCookingTimer;
    public float bottomSideCookingTimer;
    
    public bool isTopSideCooking = true;
    
    [SerializeField, Tooltip("Cooldown time in seconds before the food can be flipped again.")]
    private float flipCooldownDuration = 0.5f; // Example: 2 seconds cooldown
    private float flipCooldownTimer = 0f;
    
    private bool isFlipped;
    #endregion
    
    #region UI Components
    [Header("UI Components")]
    [SerializeField, Tooltip("Slider representing the cooking progress.")]
    private Slider cookingProgressBar;
    
    [SerializeField, Tooltip("Image component to change the color of the progress bar.")]
    private Image progressBarFill;
    #endregion
    
    #region Cooking State
    [Header("Cooking State")]
    [Tooltip("Is the food currently cooking?")]
    public bool isCooking;

    private SpriteRenderer spriteRenderer;
    private Color rawColor = Color.red;
    private Color cookedColor = Color.green;
    private Color overcookedColor = Color.black;
    #endregion

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        InitializeProgressBar();
    }

    private void Update()
    {
        if (flipCooldownTimer > 0)
        {
            flipCooldownTimer -= Time.deltaTime;
        }
        
        if (!isCooking) return;
        
        if (isTopSideCooking)
        {
            topSideCookingTimer += Time.deltaTime;
            UpdateProgressBar(topSideCookingTimer);
            
            if (topSideCookingTimer >= overcookedTime)
            {
                MarkAsOvercooked();
            }
        }
        else
        {
            bottomSideCookingTimer += Time.deltaTime;
            UpdateProgressBar(bottomSideCookingTimer);
            
            if (bottomSideCookingTimer >= overcookedTime)
            {
                MarkAsOvercooked();
            }
        }
    }
    
    #region Progress Bar Methods
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

    private void UpdateProgressBar(float timer)
    {
        if (cookingProgressBar == null) return;
        
        cookingProgressBar.value = timer; 
        progressBarFill.color = CalculateProgressColor(timer);
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
    #endregion
    
    #region Cooking State Methods

    private void MarkAsOvercooked()
    {
        StopCooking();
    }
    
    public void FlipFood()
    {
        if (flipCooldownTimer > 0)
        {
            Debug.Log("Flip is on cooldown. Please wait.");
            return;
        }
        
        isTopSideCooking = !isTopSideCooking;
        cookingProgressBar.value = isTopSideCooking ? topSideCookingTimer : bottomSideCookingTimer;
        progressBarFill.color = isTopSideCooking ? CalculateProgressColor(topSideCookingTimer) : CalculateProgressColor(bottomSideCookingTimer);
        
        flipCooldownTimer = flipCooldownDuration;
        
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
        SoundManager.PlaySound(SoundType.flipMeat,VolumeType.SFX);
    }
    

    public void StartCooking()
    {
        isCooking = true;
        ShowProgressBar(true);
    }

    public void StopCooking()
    {
        isCooking = false;
        ShowProgressBar(false);
    }

    public void ShowProgressBar(bool show)
    {
        if (cookingProgressBar != null)
        {
            cookingProgressBar.gameObject.SetActive(show);
        }
    }
    #endregion
    
    #region Interaction Methods
    public void PlaceOnPlate()
    {
        StopCooking(); 
        
        if (topSideCookingTimer >= cookingTime && bottomSideCookingTimer >= cookingTime && topSideCookingTimer < overcookedTime && bottomSideCookingTimer < overcookedTime)
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
    }

    public void PlaceOnTrash()
    {
        StopCooking();
        PopDownFood();
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
    #endregion
    
    public bool IsTopSideCooked() => topSideCookingTimer >= cookingTime && topSideCookingTimer < overcookedTime;
    public bool IsBottomSideCooked() => bottomSideCookingTimer >= cookingTime && bottomSideCookingTimer < overcookedTime;
    public bool IsTopSideOvercooked() => topSideCookingTimer >= overcookedTime;
    public bool IsBottomSideOvercooked() => bottomSideCookingTimer >= overcookedTime;

    private void PopDownFood()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.2f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Destroy(gameObject);
            });
    }
    
}
