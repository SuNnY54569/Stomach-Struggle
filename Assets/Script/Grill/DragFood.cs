using System;
using UnityEngine;

public class DragFood : MonoBehaviour
{
    #region Drag Settings
    [Header("Food and Spawner Settings")]
    [SerializeField, Tooltip("Parent GameObject containing all food spawners.")]
    private GameObject foodSpawners;

    [Tooltip("Determines if the player can interact with this food item.")]
    public bool isInteractable = true;

    [SerializeField, Tooltip("Collider for the spawn area.")]
    private Collider2D spawnCollider;

    [SerializeField, Tooltip("Main collider for the food object.")]
    private Collider2D mainCollider;
    
    [SerializeField, Tooltip("spriteRenderer for the food object spirte.")]
    private SpriteRenderer spriteRenderer;

    [Header("Food Drag and Cooking")]
    [SerializeField, Tooltip("Whether the food item is on the grill.")]
    private bool isOnGrill;

    [SerializeField, Tooltip("Tracks if the food has been activated or placed.")]
    private bool hasBeenActivate;

    private Collider2D col;
    private Vector3 offset;
    private bool isDragging;
    private Camera mainCamera;
    private Vector3 startPosition;
    private bool wasOnGrillBeforeDrag;
    private FoodCooking foodCooking;
    private Tools.ToolType currentTool;
    #endregion

    private void Awake()
    {
        isOnGrill = false;
        spriteRenderer.enabled = false;
        mainCollider.enabled = false;
        foodSpawners = transform.parent.gameObject;
        col = GetComponent<Collider2D>();
        foodCooking = GetComponent<FoodCooking>();
        mainCamera = Camera.main;
        startPosition = transform.position;
    }
    
    private void Update()
    {
        currentTool = Tools.Instance.currentTool;
        
        if (foodCooking.isCooking && !isOnGrill)
        {
            foodCooking.StopCooking();
        }
    }

    #region Mouse Events
    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused || !isInteractable) return;
        
        offset = transform.position - MouseWorldPosition();
        
        if (!ValidateToolForCookingState()) return;
        
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
            spriteRenderer.enabled = true;
            mainCollider.enabled = true;
            spawnCollider.enabled = false;
            foodCooking.StopCooking();
        }
        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
    }

    private void OnMouseDrag()
    {
        if (!isDragging || GameManager.Instance.isGamePaused || !isInteractable) return;
        if (!ValidateToolForCookingState()) return;
        
        transform.position = MouseWorldPosition() + offset;
        
    }
    
    private void OnMouseUp()
    {
        if (GameManager.Instance.isGamePaused || !isInteractable) return;
        if (!ValidateToolForCookingState()) return;
        
        isDragging = false;
        col.enabled = false;
        
        RaycastHit2D hitInfo = Physics2D.Raycast(MouseWorldPosition(), Vector2.zero);
        
        if (hitInfo.collider != null)
        {
            HandleDrop(hitInfo);
        }
        else
        {
            ResetPosition();
            if (!hasBeenActivate)
            {
                spriteRenderer.enabled = false;
                mainCollider.enabled = false;
                spawnCollider.enabled = true;
            }
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
                SoundManager.PlaySound(SoundType.flipMeat,VolumeType.SFX);
                foodSpawners.gameObject.GetComponent<FoodSpawner>().SpawnFood();
                break;
            case "Plate":
                PlaceOnPlate();
                SoundManager.PlaySound(SoundType.PlaceOnPlate,VolumeType.SFX);
                foodSpawners.gameObject.GetComponent<FoodSpawner>().SpawnFood();
                break;
            case "Trash":
                PlaceOnTrash();
                SoundManager.PlaySound(SoundType.PlaceOnTrash,VolumeType.SFX);
                foodSpawners.gameObject.GetComponent<FoodSpawner>().SpawnFood();
                break;
            default:
                ResetPosition();
                if (!hasBeenActivate)
                {
                    spriteRenderer.enabled = false;
                    mainCollider.enabled = false;
                    spawnCollider.enabled = true;
                }

                break;
        }
    }
    
    private void PlaceOnGrill()
    {
        transform.position += new Vector3(0, 0, -0.01f);
        foodCooking.StartCooking();
        isOnGrill = true;
        hasBeenActivate = true;
        startPosition = transform.position;
    }

    private void PlaceOnPlate()
    {
        hasBeenActivate = true;
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
    
    private bool ValidateToolForCookingState()
    {
        if (Tools.Instance.currentTool == Tools.ToolType.None)
        {
            Tools.Instance.ShowWarning(currentTool);
            return false;
        }

        bool isBothSidesCooked = foodCooking.IsBottomSideCooked() && foodCooking.IsTopSideCooked();

        if (Tools.Instance.currentTool == Tools.ToolType.Spatula)
        {
            if (!isBothSidesCooked)
            {
                Tools.Instance.ShowWarning(currentTool);
                return false;
            }
        }
        else if (Tools.Instance.currentTool == Tools.ToolType.Tongs)
        {
            if (isBothSidesCooked)
            {
                Tools.Instance.ShowWarning(currentTool);
                return false;
            }
        }

        return true;
    }
    #endregion
}
