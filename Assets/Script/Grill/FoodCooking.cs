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
    
    private float cookingTimer;
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
        
        cookingTimer += Time.deltaTime;
        UpdateProgressBar();
        
        if (cookingTimer >= overcookedTime)
        {
            MarkAsOvercooked();
        }
        else if (cookingTimer >= cookingTime && cookingTimer < overcookedTime)
        {
            MarkAsCooked();
        }
        else
        {
            MarkAsRaw();
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

    private void UpdateProgressBar()
    {
        if (cookingProgressBar == null) return;
        
        cookingProgressBar.value = cookingTimer; 
        progressBarFill.color = CalculateProgressColor();
    }

    private Color CalculateProgressColor()
    {
        if (cookingTimer <= cookingTime)
        {
            float t = cookingTimer / cookingTime;
            return Color.Lerp(rawColor, cookedColor, t);
        }
        else 
        {
            float t = (cookingTimer - cookingTime) / (overcookedTime - cookingTime);
            return Color.Lerp(cookedColor, overcookedColor, t);
        }
    }
    #endregion
    
    #region Cooking State Methods
    private void MarkAsRaw()
    {
        gameObject.tag = "Raw";
    }

    private void MarkAsCooked()
    {
        gameObject.tag = "Cooked";
    }

    private void MarkAsOvercooked()
    {
        gameObject.tag = "Overcooked";
        StopCooking();
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
        
        if (cookingTimer < cookingTime)
        {
            HandleUndercooked();
        }
        else if (cookingTimer < overcookedTime)
        {
            HandleCooked();
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
}
