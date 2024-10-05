using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClickHandler : MonoBehaviour
{
    public Health playerHealth;

    private void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<Health>();
        }
    }

    private void OnMouseDown()
    {
        if (gameObject.CompareTag("GoodMeat"))
        {
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("BadMeat"))
        {
            if (playerHealth != null)
            {
                playerHealth.DecreaseHealth(1);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Health playerHealth = FindObjectOfType<Health>();
        if (gameObject.CompareTag("GoodMeat"))
        {
            playerHealth.DecreaseHealth(1);
        }
        else if (gameObject.CompareTag("BadMeat"))
        {
            playerHealth.DecreaseHealth(0);
        }

        Destroy(this.gameObject);
    }
}
