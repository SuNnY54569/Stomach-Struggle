using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodVisual : MonoBehaviour
{
    #region Visual Settings
    [Header("Food Colors")]
    [SerializeField,Tooltip("Sprite for the raw food state.")]
    private Sprite rawSprite;
    
    [SerializeField, Tooltip("Sprite for the cooking food state.")]
    private Sprite cookingSprite;
    
    [SerializeField, Tooltip("Sprite for the fully cooked state.")]
    private Sprite cookedSprite;
    
    [SerializeField, Tooltip("Sprite for the overcooked state.")]
    private Sprite overcookedSprite;
    
    private SpriteRenderer spriteRenderer;
    private FoodCooking foodCooking;
    #endregion

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        foodCooking = GetComponent<FoodCooking>();
        spriteRenderer.sprite = rawSprite;
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
        
        if (isTopOvercooked || isBottomOvercooked)
        {
            spriteRenderer.sprite = overcookedSprite; 
            return;
        }
        
        // If both sides are fully cooked
        if (isTopCooked && isBottomCooked)
        {
            spriteRenderer.sprite = cookedSprite;
        }
        // If one side is fully cooked
        else if (isTopCooked)
        {
            spriteRenderer.sprite = isTopCooking ? cookingSprite : cookedSprite; 
        }
        else if (isBottomCooked)
        {
            spriteRenderer.sprite = isTopCooking ? cookedSprite : cookingSprite; 
        }
        // Neither side is cooked
        else
        {
            spriteRenderer.sprite = rawSprite;
        }
    }
    #endregion
}
