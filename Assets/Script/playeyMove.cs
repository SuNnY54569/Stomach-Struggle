using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playeyMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public Health playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        movement = new Vector2(moveX, 0).normalized;
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("GoodMeat"))
        {
            ScoreGuitar.scoreValue += 1;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("BadMeat"))
        {
            playerHealth.DecreaseHealth(1);
            Destroy(other.gameObject);
        }
    }
}
