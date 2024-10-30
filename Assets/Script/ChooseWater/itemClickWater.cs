using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemClickWater : MonoBehaviour
{

    [SerializeField] private Sprite[] waterSprites;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        RandomizeFoodSprites();
    }

    private void RandomizeFoodSprites()
    {
        if(waterSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, waterSprites.Length);
            spriteRenderer.sprite = waterSprites[randomIndex];
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;

        if (GameManager.Instance.GetScore() == GameManager.Instance.scoreMax) return;

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
