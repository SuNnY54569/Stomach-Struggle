using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private ScoreMeatShop scoreMeatShop;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BadMeat"))
        {
            Destroy(collision.gameObject);
            health.DecreaseHealth(1);
        }
        else if (collision.CompareTag("GoodMeat"))
        {
            Destroy(collision.gameObject);
            scoreMeatShop.ScoreUp(1);
        }
    }
}
