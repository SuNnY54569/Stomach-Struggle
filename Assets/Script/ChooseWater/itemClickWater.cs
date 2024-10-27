using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemClickWater : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;

        if (gameObject.CompareTag("GoodWater"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DecreaseHealth(1);
            }
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("BadWater"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IncreaseScore(1);
            }
            Destroy(gameObject);
        }
    }
}
