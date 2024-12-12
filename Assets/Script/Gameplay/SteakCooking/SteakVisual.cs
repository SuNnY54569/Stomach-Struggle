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
    private Sprite currentSprite;
    #endregion

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        steak = GetComponent<Steak>();
        SetSprite(rawSprite);
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
        
        Sprite newSprite;
        
        if (isTopOvercooked || isBottomOvercooked)
        {
            newSprite = overcookedSprite;
        }
        else if (isTopCooked && isBottomCooked)
        {
            newSprite = cookedSprite;
        }
        else if (isTopCooked)
        {
            newSprite = isTopCooking ? rawSprite : almostCookedSprite;
        }
        else if (isBottomCooked)
        {
            newSprite = isTopCooking ? almostCookedSprite : rawSprite;
        }
        else
        {
            newSprite = rawSprite;
        }
        
        SetSprite(newSprite);
    }
    
    private void SetSprite(Sprite newSprite)
    {
        if (currentSprite != newSprite)
        {
            currentSprite = newSprite;
            spriteRenderer.sprite = newSprite;
        }
    }
    
    #endregion
}
