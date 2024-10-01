using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodVisual : MonoBehaviour
{
    #region Visual Settings
    [Header("Food Colors")]
    [SerializeField,Tooltip("Color for the raw food state.")]
    private Color rawSprite;
    
    [SerializeField,Tooltip("Color for the cooked food state (for either side).")]
    private Color cookedSprite;
    
    [SerializeField,Tooltip("Color for the overcooked food state.")]
    private Color overcookedSprite;
    
    private SpriteRenderer spriteRenderer;
    private FoodCooking foodCooking;
    #endregion

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        foodCooking = GetComponent<FoodCooking>();
        spriteRenderer.color = rawSprite;
    }

    private void Update()
    {
        UpdateFoodVisual();
    }
    
    #region Update Visuals
    private void UpdateFoodVisual()
    {
        bool isTopCooked = foodCooking.IsTopSideCooked();
        bool isBottomCooked = foodCooking.IsBottomSideCooked();
        bool isTopCooking = foodCooking.isTopSideCooking;
        bool isTopOvercooked = foodCooking.IsTopSideOvercooked();
        bool isBottomOvercooked = foodCooking.IsBottomSideOvercooked();
        
        if (isTopOvercooked && isBottomOvercooked)
        {
            spriteRenderer.color = overcookedSprite; 
            return;
        }
        
        if (isTopOvercooked)
        {
            if (isBottomCooked) 
            {
                spriteRenderer.color = isTopCooking ? cookedSprite : overcookedSprite; 
            }
            else 
            {
                spriteRenderer.color = isTopCooking ? rawSprite : overcookedSprite; 
            }
            return;
        }
        
        if (isBottomOvercooked)
        {
            if (isTopCooked) 
            {
                spriteRenderer.color = isTopCooking ? overcookedSprite : cookedSprite; 
            }
            else 
            {
                spriteRenderer.color = !isTopCooking ? overcookedSprite : rawSprite; 
            }
            return;
        }
        
        if (isTopCooked && isBottomCooked)
        {
            spriteRenderer.color = cookedSprite; 
        }
        else if (isTopCooked)
        {
            spriteRenderer.color = isTopCooking ? rawSprite : cookedSprite; 
        }
        else if (isBottomCooked)
        {
            spriteRenderer.color = isTopCooking ? cookedSprite : rawSprite; 
        }
        else
        {
            spriteRenderer.color = rawSprite; 
        }
    }
    #endregion
}
