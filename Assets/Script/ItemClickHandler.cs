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
            ScoreGuitar.scoreValue += 1;
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
}
