using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteakVisual : MonoBehaviour
{
    #region Visual Settings
    [Header("Food Sprites")]
    [SerializeField, Tooltip("Sprite for the raw food state.")]
    private Sprite rawSprite;

    [SerializeField, Tooltip("Sprite for the almost cooked food state.")]
    private Sprite almostCookedSprite;
    
    [SerializeField, Tooltip("Sprite for the cooked food state.")]
    private Sprite cookedSprite;

    [SerializeField, Tooltip("Sprite for the overcooked food state.")]
    private Sprite overcookedSprite;
    
    private SpriteRenderer spriteRenderer;
    private Steak steak;
    #endregion

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        steak = GetComponent<Steak>();
        spriteRenderer.sprite = rawSprite;
    }

    private void Update()
    {
        UpdateFoodVisual();
    }
    
    #region Update Visuals
    private void UpdateFoodVisual()
    {
        bool isTopCooked = steak.IsTopSideCooked();
        bool isBottomCooked = steak.IsBottomSideCooked();
        bool isTopCooking = steak.IsCooking() && steak.isTopSideCooking;
        bool isTopOvercooked = steak.IsTopSideOvercooked();
        bool isBottomOvercooked = steak.IsBottomSideOvercooked();
        
        if (isTopOvercooked || isBottomOvercooked)
        {
            spriteRenderer.sprite = overcookedSprite; 
            return;
        }
        
        if (isTopCooked && isBottomCooked)
        {
            spriteRenderer.sprite = cookedSprite; 
        }
        else if (isTopCooked)
        {
            spriteRenderer.sprite = isTopCooking ? rawSprite : almostCookedSprite;
        }
        else if (isBottomCooked)
        {
            spriteRenderer.sprite = isTopCooking ? almostCookedSprite : rawSprite; 
        }
        else
        {
            spriteRenderer.sprite = rawSprite; 
        }
    }
    #endregion
}
