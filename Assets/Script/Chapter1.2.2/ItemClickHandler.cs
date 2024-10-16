using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClickHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;
        
        if (gameObject.CompareTag("GoodVegetable"))
        {
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("BadVegetable"))
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
        if (gameObject.CompareTag("GoodVegetable"))
        {
            GameManager.Instance.DecreaseHealth(1);
        }
        else if (gameObject.CompareTag("BadVegetable"))
        {
            GameManager.Instance.DecreaseHealth(0);
        }

        Destroy(this.gameObject);
    }
}
