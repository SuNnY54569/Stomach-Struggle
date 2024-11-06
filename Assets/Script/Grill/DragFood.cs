using System;
using UnityEngine;

public class DragFood : MonoBehaviour
{
    #region Drag Settings
    [Header("Settings")]
    [SerializeField, Tooltip("Array of food spawners.")]
    private GameObject foodSpawners;

    [Tooltip("Can the player interact with this food item?")]
    public bool isInteractable = true;
    
    private Collider2D col;
    private Vector3 offset;
    private bool isDragging;
    private Camera mainCamera;
    private Vector3 startPosition;
    private bool isOnGrill;
    private bool wasOnGrillBeforeDrag;
    private FoodCooking foodCooking;
    #endregion

    private void Awake()
    {
        foodSpawners = transform.parent.gameObject;
        col = GetComponent<Collider2D>();
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

    #region Mouse Events
    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;
        if (!isInteractable) return;
        
        offset = transform.position - MouseWorldPosition();
        
        if (isOnGrill)
        {
            foodCooking.FlipFood();
            isDragging = true;
            wasOnGrillBeforeDrag = isOnGrill;
            foodCooking.StopCooking();
        }
        else
        {
            isDragging = true;
            wasOnGrillBeforeDrag = isOnGrill;
            foodCooking.StopCooking();
        }
    }

    private void OnMouseDrag()
    {
        if (GameManager.Instance.isGamePaused) return;
        if (isDragging && isInteractable)
        {
            transform.position = MouseWorldPosition() + offset;
        }
    }
    
    private void OnMouseUp()
    {
        if (GameManager.Instance.isGamePaused) return;
        if (!isInteractable) return;
        isDragging = false;
        col.enabled = false;
        
        RaycastHit2D hitInfo = Physics2D.Raycast(MouseWorldPosition(), Vector2.zero);
        
        if (hitInfo.collider != null)
        {
            HandleDrop(hitInfo);
            foodSpawners.gameObject.GetComponent<FoodSpawner>().SpawnFood();
        }
        else
        {
            ResetPosition();
        }

        col.enabled = true;
    }
    #endregion
    
    #region Drop Handling
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
        if (foodCooking.IsBottomSideCooked() && foodCooking.IsTopSideCooked())
        {
            isInteractable = false;
            Destroy(col);
        }
    }

    private void PlaceOnTrash()
    {
        foodCooking.PlaceOnTrash();
    }

    private void ResetPosition()
    {
        transform.position = startPosition;
        if (wasOnGrillBeforeDrag)
        {
            isOnGrill = true;
            foodCooking.StartCooking();
        }
    }
    #endregion
    
    #region Utility Methods
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
    #endregion
}
