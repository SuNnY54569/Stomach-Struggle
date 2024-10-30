using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClickHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;
        if (GameManager.Instance.GetScore() == GameManager.Instance.scoreMax) return;
        
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

        Destroy(this.gameObject);
    }
}
