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
            spriteRenderer.color = overcookedSprite; // If overcooked
            return;
        }
        
        if (isTopOvercooked)
        {
            if (isBottomCooked) // Top is overcooked, bottom is cooked
            {
                spriteRenderer.color = isTopCooking ? cookedSprite : overcookedSprite; // Display cooked color for both sides
            }
            else // Top is overcooked, bottom is raw
            {
                spriteRenderer.color = isTopCooking ? rawSprite : overcookedSprite; // Show the correct color
            }
            return;
        }
        
        if (isBottomOvercooked)
        {
            if (isTopCooked) // Bottom is overcooked, top is cooked
            {
                spriteRenderer.color = isTopCooking ? overcookedSprite : cookedSprite; // Display cooked color for both sides
            }
            else // Bottom is overcooked, top is raw
            {
                spriteRenderer.color = !isTopCooking ? overcookedSprite : rawSprite; // Show the correct color
            }
            return;
        }
        
        // Determine the color based on the cooking status if neither side is overcooked
        if (isTopCooked && isBottomCooked)
        {
            spriteRenderer.color = cookedSprite; // Both sides cooked
        }
        else if (isTopCooked)
        {
            spriteRenderer.color = isTopCooking ? rawSprite : cookedSprite; // Top cooked, bottom raw
        }
        else if (isBottomCooked)
        {
            spriteRenderer.color = isTopCooking ? cookedSprite : rawSprite; // Bottom cooked, top raw
        }
        else
        {
            spriteRenderer.color = rawSprite; // Both sides raw
        }
    }
    #endregion
}
