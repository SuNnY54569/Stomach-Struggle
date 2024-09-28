using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodVisual : MonoBehaviour
{
    public Color rawSprite;
    public Color cookedSprite;
    public Color overcookedSprite;
    
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = rawSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.CompareTag("Cooked"))
        {
            spriteRenderer.color = cookedSprite;
        }
        else if (gameObject.CompareTag("Overcooked"))
        {
            spriteRenderer.color = overcookedSprite;
        }
    }
}
