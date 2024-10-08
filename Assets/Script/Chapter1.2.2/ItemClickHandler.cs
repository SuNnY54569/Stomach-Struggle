using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClickHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;
        
        if (gameObject.CompareTag("GoodMeat"))
        {
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("BadMeat"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DecreaseHealth(1);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("GoodMeat"))
        {
            GameManager.Instance.DecreaseHealth(1);
        }
        else if (gameObject.CompareTag("BadMeat"))
        {
            GameManager.Instance.DecreaseHealth(0);
        }

        Destroy(this.gameObject);
    }
}
