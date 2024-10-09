using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField] private ClawController clawController;

    private void Awake()
    {
        clawController = FindObjectOfType<ClawController>().GetComponent<ClawController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BadMeat"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance.DecreaseHealth(1);
        }
        else if (collision.CompareTag("GoodMeat"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance.IncreaseScore(1);
        }
        
        clawController.SetDefaultSprite();
    }
}
