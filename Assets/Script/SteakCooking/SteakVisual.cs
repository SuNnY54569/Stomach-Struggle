using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteakVisual : MonoBehaviour
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
    private Steak steak;
    #endregion

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        steak = GetComponent<Steak>();
        spriteRenderer.color = rawSprite;
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
            spriteRenderer.color = overcookedSprite; 
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
