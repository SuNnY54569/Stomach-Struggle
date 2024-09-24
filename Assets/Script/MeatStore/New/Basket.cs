using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BadMeat"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Bad Meat");
        }
        else if (collision.CompareTag("GoodMeat"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Good Meat");
        }
    }
}
