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
    
    [SerializeField,Tooltip("Color for the cooked food state.")]
    private Color cookedSprite;
    
    [SerializeField,Tooltip("Color for the overcooked food state.")]
    private Color overcookedSprite;
    
    private SpriteRenderer spriteRenderer;
    #endregion

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = rawSprite;
    }

    private void Update()
    {
        UpdateFoodVisual();
    }
    
    #region Update Visuals
    private void UpdateFoodVisual()
    {
        // Change the sprite color based on the tag of the food
        if (gameObject.CompareTag("Cooked"))
        {
            spriteRenderer.color = cookedSprite;
        }
        else if (gameObject.CompareTag("Overcooked"))
        {
            spriteRenderer.color = overcookedSprite;
        }
    }
    #endregion
}
