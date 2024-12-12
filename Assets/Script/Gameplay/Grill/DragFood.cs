using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragFood : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
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
    
    [SerializeField] private float flipCooldownDuration = 0.3f;

    private Collider2D col;
    private Vector3 offset;
    private bool isDragging;
    private Camera mainCamera;
    private Vector3 startPosition;
    private bool wasOnGrillBeforeDrag;
    private FoodCooking foodCooking;
    private Tools.ToolType currentTool;
    private Vector3 initialScale;
    private bool isFirstTimeOnGrill = true;
    private bool isFirstTimeDrag = true;
    private float lastFlipTime = -Mathf.Infinity;
    private bool canFlip => Time.time >= lastFlipTime + flipCooldownDuration;
    
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
        initialScale = transform.localScale;
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
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused || !isInteractable) return;
        
        if (isFirstTimeDrag)
        {
            transform.position = MouseWorldPosition();
        }
        else
        {
            offset = transform.position - MouseWorldPosition();
        }
        if (!ValidateToolForCookingState()) return;
        isDragging = true;
        wasOnGrillBeforeDrag = isOnGrill;
        
        if (isOnGrill)
        {
            foodCooking.StopCooking();
            isOnGrill = false;
        }
        else
        {
            spriteRenderer.enabled = true;
            PopUpFood();
            mainCollider.enabled = true;
            spawnCollider.enabled = false;
        }
        
        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || GameManager.Instance.isGamePaused || !isInteractable) return;
        if (!ValidateToolForCookingState()) return;

        if (!isFirstTimeDrag)
        {
            transform.position = MouseWorldPosition() + offset;
        }
        else
        {
            transform.position = MouseWorldPosition();
        }
        
        
    }
    
    public void OnPointerUp(PointerEventData eventData)
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
        }
        col.enabled = true;
        
        if (isOnGrill)
        {
            if (isFirstTimeOnGrill)
            {
                isFirstTimeOnGrill = false;
                return;
            }
            
            if (canFlip)
            {
                if (!foodCooking.IsBottomSideOvercooked() && !foodCooking.IsTopSideOvercooked())
                {
                    foodCooking.FlipFood();
                    lastFlipTime = Time.time;
                    SoundManager.PlaySound(SoundType.flipMeat, VolumeType.SFX);
                }
            }
        }
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
                isFirstTimeDrag = false;
                break;
            case "Plate":
                PlaceOnPlate();
                SoundManager.PlaySound(SoundType.PlaceOnPlate,VolumeType.SFX);
                foodSpawners.gameObject.GetComponent<FoodSpawner>().SpawnFood();
                isFirstTimeDrag = false;
                break;
            case "Trash":
                PlaceOnTrash();
                SoundManager.PlaySound(SoundType.PlaceOnTrash,VolumeType.SFX);
                foodSpawners.gameObject.GetComponent<FoodSpawner>().SpawnFood();
                isFirstTimeDrag = false;
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
        hasBeenActivate = true;
        startPosition = transform.position;
    }

    private void PlaceOnPlate()
    {
        isOnGrill = false;
        hasBeenActivate = true;
        foodCooking.PlaceOnPlate();
        startPosition = transform.position;
        isInteractable = false;
        Destroy(col);
    }

    private void PlaceOnTrash()
    {
        isOnGrill = false;
        foodCooking.PlaceOnTrash();
    }

    private void ResetPosition()
    {
        LeanTween.move(gameObject, startPosition, 0.1f) // 0.5 seconds for the animation
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                if (wasOnGrillBeforeDrag)
                {
                    isOnGrill = true;
                    foodCooking?.StartCooking();
                    LeanTween.scale(gameObject, initialScale, 0.2f).setEase(LeanTweenType.easeOutBounce);
                }
                if (!hasBeenActivate)
                {
                    LeanTween.scale(gameObject, Vector3.zero, 0.2f)
                        .setEase(LeanTweenType.easeOutBack) // Set easing type
                        .setIgnoreTimeScale(true)
                        .setOnComplete(() =>
                        {
                            spriteRenderer.enabled = false;
                            mainCollider.enabled = false;
                            spawnCollider.enabled = true;
                            transform.localScale = initialScale;
                        });
                }
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            });
        LeanTween.rotateZ(gameObject, 5f, 0.1f)
            .setLoopPingPong(1)
            .setOnComplete(() =>
            {
                // Reset the rotation to 0 when the animation finishes
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            });;
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
        
        bool isOvercooked = foodCooking.IsBottomSideOvercooked() || foodCooking.IsTopSideOvercooked();
        
        if (isOvercooked)
        {
            return true;
        }

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
    
    private void PopUpFood()
    {
        gameObject.SetActive(true);
        gameObject.transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, initialScale, 0.2f)
            .setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true);
        LeanTween.rotateZ(gameObject, 5f, 0.1f).setLoopPingPong(1);
    }
    
    #endregion
}
