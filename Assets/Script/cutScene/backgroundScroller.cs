using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundScroller : MonoBehaviour
{
    [Header("Scroll Settings")]
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float resetPosition;
    [SerializeField] private float startPosition;

    private void Update()
    {
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        if (transform.position.x <= resetPosition)
        {
            Vector3 newPosition = new Vector3(startPosition, transform.position.y, transform.position.z);
            transform.position = newPosition;
        }
    }
}
