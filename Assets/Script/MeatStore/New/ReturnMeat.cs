using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnMeat : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BadMeat") || collision.CompareTag("GoodMeat"))
        {
            Destroy(collision.gameObject);
            Debug.Log("Return Meat");
        }
    }
}
