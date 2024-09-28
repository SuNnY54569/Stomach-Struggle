using System;
using UnityEngine;
public class DragFood : MonoBehaviour
{
    private Vector3 offset;
    private Collider2D collider2D;
    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 startPosition;
    private bool isOnGrill = false;
    private bool isOnPlate = false;

    /*void Start()
    {
        mainCamera = Camera.main; // Get the main camera reference
        startPosition = transform.position;
    }
    
    private void OnMouseDown()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f; // Ensure it's a 2D movement
        offset = transform.position - mouseWorldPosition;
        isDragging = true;
    }
    
    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Update the food's position based on the mouse position
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f; // Ensure it's a 2D movement
            transform.position = mouseWorldPosition + offset;
        }
    }

    void OnMouseUp()
    {
        var newPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.01f);
        transform.position = newPosition;
        isDragging = false;
        // Check if food is dropped on grill
        if (isOnGrill)
        {
            CookingManager.Instance.StartCooking(this);
            startPosition = transform.position;
        }
        else if (isOnPlate)
        {
            CookingManager.Instance.PlaceOnPlate(this);
            startPosition = transform.position;
        }
        else
        {
            // Reset to original position if dropped elsewhere
            transform.position = startPosition;
        }
    }*/

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        mainCamera = Camera.main;
        startPosition = transform.position;
    }

    private void OnMouseDown()
    {
        offset = transform.position - MouseWorldPosition();
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = MouseWorldPosition() + offset;
        }
    }
    
    private void OnMouseUp()
    {
        isDragging = false;
        collider2D.enabled = false;
        var rayOrigin = Camera.main.transform.position;
        var rayDirection = MouseWorldPosition() - Camera.main.transform.position;
        RaycastHit2D hitInfo;
        if (hitInfo = Physics2D.Raycast(rayOrigin,rayDirection))
        {
            if (hitInfo.transform.CompareTag("Grill"))
            {
                transform.position += new Vector3(0, 0, -0.01f);
                CookingManager.Instance.StartCooking(this);
                startPosition = transform.position;
            }
            else if (hitInfo.transform.CompareTag("Plate"))
            {
                CookingManager.Instance.PlaceOnPlate(this);
                startPosition = transform.position;
            }
            else
            {
                transform.position = startPosition;
            }
        }

        collider2D.enabled = true;
    }

    private Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Grill"))
        {
            isOnGrill = true;
        }
        else if (other.CompareTag("Plate"))
        {
            isOnPlate = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Grill"))
        {
            isOnGrill = false;
        }
        else if (other.CompareTag("Plate"))
        {
            isOnPlate = false;
        }
    }
}
