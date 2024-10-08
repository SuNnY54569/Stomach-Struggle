using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject[] objectsToClose;
    [SerializeField] private GameObject[] objectsToOpen;
    [Range(0,1)] [SerializeField] private float clawChance;
    [SerializeField] private ClawController clawController;
    [SerializeField] private bool isReturnButton;

    private void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnMouseOver()
    {
        sprite.color = Color.gray;
    }

    private void OnMouseExit()
    {
        sprite.color = Color.white;
    }

    private void OnMouseDown()
    {
        foreach (var objectToClose in objectsToClose)
        {
            objectToClose.SetActive(false);
        }
        foreach (var objectToOpen in objectsToOpen)
        {
            objectToOpen.SetActive(true);
        }

        if (!isReturnButton)
        {
            clawController.SetChance0to1(clawChance);
        }
        else
        {
            clawController.RePosition();
        }
        
        sprite.color = Color.white;
    }
}
