using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MeatButton : MonoBehaviour
{
    [SerializeField] private Health health;

    private void Start()
    {
        if (health == null)
        {
            health = FindObjectOfType<Health>();
        }
    }

    public void OnMeatClicked()
    {
        if (gameObject.CompareTag("GoodMeat"))
        {
            Destroy(gameObject);
        }
        else
        {
            health.DecreaseHealth(1);
            Debug.Log("Oops! You picked a bad meat!");
        }
    }
}
