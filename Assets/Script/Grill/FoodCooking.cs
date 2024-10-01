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
    private float cookingTime;
    
    [SerializeField, Tooltip("Time after which the food is considered overcooked.")]
    private float overcookedTime;
    
    private float topSideCookingTimer;
    private float bottomSideCookingTimer;
    
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

    private Color rawColor = Color.red;
    private Color cookedColor = Color.green;
    private Color overcookedColor = Color.black;
    #endregion

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
            else if (topSideCookingTimer >= cookingTime && topSideCookingTimer < overcookedTime)
            {
                MarkAsCooked();
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
            else if (bottomSideCookingTimer >= cookingTime && bottomSideCookingTimer < overcookedTime)
            {
                MarkAsCooked();
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
    
    private void MarkAsCooked()
    {
        if (isTopSideCooking)
        {
            gameObject.tag = "TopCooked";
        }
        else
        {
            gameObject.tag = "BottomCooked";
        }
    }

    private void MarkAsOvercooked()
    {
        gameObject.tag = "Overcooked";
        StopCooking();
    }
    
    public void FlipFood()
    {
        isTopSideCooking = !isTopSideCooking;
        cookingProgressBar.value = isTopSideCooking ? topSideCookingTimer : bottomSideCookingTimer;
        progressBarFill.color = isTopSideCooking ? CalculateProgressColor(topSideCookingTimer) : CalculateProgressColor(bottomSideCookingTimer);
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
        Destroy(gameObject);
    }
    
    private void HandleUndercooked()
    {
        Health.Instance.DecreaseHealth(1);
    }

    private void HandleCooked()
    {
        ScoreMeatShop.Instance.ScoreUp(1);
    }

    private void HandleOvercooked()
    {
        Health.Instance.DecreaseHealth(1);
    }
    #endregion
    
    public bool IsTopSideCooked() => topSideCookingTimer >= cookingTime && topSideCookingTimer < overcookedTime;
    public bool IsBottomSideCooked() => bottomSideCookingTimer >= cookingTime && bottomSideCookingTimer < overcookedTime;
    public bool IsTopSideOvercooked() => topSideCookingTimer >= overcookedTime;
    public bool IsBottomSideOvercooked() => bottomSideCookingTimer >= overcookedTime;
    
}
