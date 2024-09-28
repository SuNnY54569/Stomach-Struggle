using System;
using UnityEngine;
public class DragFood : MonoBehaviour
{
    private Vector3 offset;
    private Collider2D collider2D;
    private bool isDragging;
    private Camera mainCamera;
    private Vector3 startPosition;
    private bool isOnGrill;
    private bool isOnPlate;
    private FoodCooking foodCooking; // Reference to the FoodCooking script
    public bool isInteractable = true; // Track if food can be interacted with

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        foodCooking = GetComponent<FoodCooking>();
        mainCamera = Camera.main;
        startPosition = transform.position;
    }
    
    private void Update()
    {
        if (foodCooking.isCooking && !isOnGrill)
        {
            foodCooking.StopCooking(); // Stop cooking if dragged away from the grill
        }
    }

    private void OnMouseDown()
    {
        if (isInteractable) // Only allow dragging if interactable
        {
            offset = transform.position - MouseWorldPosition();
            isDragging = true;
        }
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
        
        Vector2 rayOrigin = MouseWorldPosition();
        RaycastHit2D hitInfo = Physics2D.Raycast(rayOrigin, Vector2.zero);
        
        if (hitInfo.collider != null)
        {
            if (hitInfo.transform.CompareTag("Grill"))
            {
                transform.position += new Vector3(0, 0, -0.01f);
                foodCooking.StartCooking();
                isOnGrill = true;
                startPosition = transform.position;
            }
            else if (hitInfo.transform.CompareTag("Plate"))
            {
                foodCooking.PlaceOnPlate();
                startPosition = transform.position;
                isInteractable = false;
            }
            else
            {
                transform.position = startPosition;
            }
        }
        else
        {
            transform.position = startPosition;
        }

        collider2D.enabled = true;
    }

    private Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Grill"))
        {
            isOnGrill = false;
            foodCooking.StopCooking();
        }
        else if (other.CompareTag("Plate"))
        {
            isOnPlate = false;
        }
    }
}
