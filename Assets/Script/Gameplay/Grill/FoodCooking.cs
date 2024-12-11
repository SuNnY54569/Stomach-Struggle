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
    
    [SerializeField] private ScoreVisual scoreVisual;

    private SpriteRenderer spriteRenderer;
    private Color rawColor = Color.red;
    private Color cookedColor = Color.green;
    private Color overcookedColor = Color.black;
    #endregion

    #region Unity Life Cycle
    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        scoreVisual = GameObject.FindGameObjectWithTag("Plate").GetComponent<ScoreVisual>();
    }

    private void Start()
    {
        InitializeProgressBar();
    }

    private void Update()
    {
        
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
    #endregion
    
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
        isTopSideCooking = !isTopSideCooking;
        cookingProgressBar.value = isTopSideCooking ? topSideCookingTimer : bottomSideCookingTimer;
        progressBarFill.color = isTopSideCooking ? CalculateProgressColor(topSideCookingTimer) : CalculateProgressColor(bottomSideCookingTimer);

        PerformFlipAnimation();
        
        SoundManager.PlaySound(SoundType.flipMeat,VolumeType.SFX);
    }
    private void PerformFlipAnimation()
    {
        float bounceHeight = 0.2f;
        float bounceDuration = 0.2f;
        float halfwayDuration = bounceDuration / 2f;
        float delayBeforeBounce = 0.11f;

        LeanTween.delayedCall(gameObject, delayBeforeBounce, () =>
        {
            float originalY = gameObject.transform.position.y;
            // Move up
            LeanTween.moveY(gameObject, originalY + bounceHeight, halfwayDuration)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() =>
                {
                    spriteRenderer.flipY = !spriteRenderer.flipY;

                    LeanTween.moveY(gameObject, originalY, halfwayDuration)
                        .setEase(LeanTweenType.easeInQuad);
                });
        });
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
        GameManager.Instance.healthManager.DecreaseHealth(1);
    }

    private void HandleCooked()
    {
        GameManager.Instance.scoreManager.IncreaseScore(1);
        scoreVisual.SetScore(GameManager.Instance.scoreManager.GetScore());
    }

    private void HandleOvercooked()
    {
        GameManager.Instance.healthManager.DecreaseHealth(1);
    }
    #endregion

    #region Utility Method
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
    #endregion
    
}
