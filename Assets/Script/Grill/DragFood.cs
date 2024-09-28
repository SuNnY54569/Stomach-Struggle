using UnityEngine;
public class DragFood : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 startPosition;
    private bool isOnGrill = false;
    private bool isOnPlate = false;

    void Start()
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
