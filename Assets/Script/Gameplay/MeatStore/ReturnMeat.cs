using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnMeat : MonoBehaviour
{
    private ClawController clawController;

    private void Awake()
    {
        clawController = FindObjectOfType<ClawController>().GetComponent<ClawController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BadMeat") || collision.CompareTag("GoodMeat"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Return Meat");
        }
        
        SoundManager.PlaySound(SoundType.PickUpMeat,VolumeType.SFX);
        clawController.SetDefaultSprite();
    }
}
