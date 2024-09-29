using System;
using UnityEngine;

public class DragFood : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Can the player interact with this food item?")]
    public bool isInteractable = true;
    
    private Vector3 offset;
    private Collider2D collider2D;
    private bool isDragging;
    private Camera mainCamera;
    private Vector3 startPosition;
    private bool isOnGrill;
    private FoodCooking foodCooking;

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
            foodCooking.StopCooking();
        }
    }

    private void OnMouseDown()
    {
        if (!isInteractable) return;
        FoodPickUp foodPickUp = gameObject.GetComponent<FoodPickUp>();
        //foodPickUp.PickUp();
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
        
        RaycastHit2D hitInfo = Physics2D.Raycast(MouseWorldPosition(), Vector2.zero);
        
        if (hitInfo.collider != null)
        {
            HandleDrop(hitInfo);
        }
        else
        {
            ResetPosition();
        }

        collider2D.enabled = true;
    }
    
    private void HandleDrop(RaycastHit2D hitInfo)
    {
        switch (hitInfo.transform.tag)
        {
            case "Grill":
                PlaceOnGrill();
                break;
            case "Plate":
                PlaceOnPlate();
                break;
            case "Trash":
                PlaceOnTrash();
                break;
            default:
                ResetPosition();
                break;
        }
    }
    
    private void PlaceOnGrill()
    {
        transform.position += new Vector3(0, 0, -0.01f);
        foodCooking.StartCooking();
        isOnGrill = true;
        startPosition = transform.position;
    }

    private void PlaceOnPlate()
    {
        foodCooking.PlaceOnPlate();
        startPosition = transform.position;
        isInteractable = false;
        collider2D.enabled = false;
    }

    private void PlaceOnTrash()
    {
        foodCooking.PlaceOnTrash();
    }

    private void ResetPosition()
    {
        transform.position = startPosition;
    }


    private Vector3 MouseWorldPosition()
    {
        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Grill"))
        {
            isOnGrill = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Grill"))
        {
            isOnGrill = false;
            foodCooking.StopCooking();
        }
    }
}
