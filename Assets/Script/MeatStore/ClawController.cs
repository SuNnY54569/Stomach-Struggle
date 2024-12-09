using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ClawController : MonoBehaviour
{
    [Header("Claw Settings")]
    [SerializeField,Tooltip("The claw object that will pick up items.")]
    private GameObject claw;
    [SerializeField,Tooltip("The box where the claw returns the picked items.")]
    private GameObject returnBox; 
    [SerializeField,Tooltip("Spawn point for items that the claw can pick up.")]
    private GameObject itemSpawnPoint; 
    [SerializeField,Tooltip("Array of prefab for good items that can be collected.")]
    private GameObject[] goodItemPrefabs;  
    [SerializeField,Tooltip("Array of prefab for bad items that may decrease health.")]
    private GameObject[] badItemPrefabs;
    [SerializeField, Tooltip("Claw Sprite")]
    private SpriteRenderer clawSprite;
    [SerializeField, Tooltip("Default Claw Sprite")]
    private Sprite defaultClawSprite;
    [SerializeField, Tooltip("Max Score for this scene")]
    private int maxScore;
    [SerializeField, Tooltip("return button")]
    private GameObject returnButton;

    private Vector3 initialScale;

    [Header("Movement Settings")]
    [SerializeField,Tooltip("Speed of horizontal movement for the claw.")]
    private float clawSpeed;  
    [SerializeField,Tooltip("Speed of vertical movement for the claw.")]
    private float verticalMoveSpeed;  
    [SerializeField,Tooltip("Y position where the claw reaches down to pick items.")]
    private float clawDownPositionY; 
    [SerializeField,Tooltip("Initial position of the claw when the game starts.")]
    private Vector2 startPosition = new Vector2(0, 1.95f);
    [SerializeField, Tooltip("The minimum X position the claw can reach.")]
    private float minMovementLimitX;
    [SerializeField, Tooltip("The maximum X position the claw can reach.")]
    private float maxMovementLimitX;
    
    [Header("Item Generation Settings")]
    [SerializeField,Tooltip("Chance of generating a good item (0 to 1).")]
    [Range(0f, 1f)]
    private float goodItemChance = 0.5f;
    
    private GameObject currentItem;  // Item currently attached to the claw
    private bool hasItem = false;  // Tracks if an item is held
    private bool isMovingDown = false;  // Tracks if claw is moving down
    private bool isReturning = false; // Tracks if claw is returning up

    private void Awake()
    {
        initialScale = returnButton.transform.localScale;
    }

    void Start()
    {
        //GameManager.Instance.scoreManager.SetScoreTextActive(true);
    }

    void Update()
    {
        HandleHorizontalMovement();
        HandleClawAction();

        if (currentItem == null && !isMovingDown && !isReturning)//test
        {
            hasItem = false;
            returnBox.SetActive(false);
        }
    }
    
    private void HandleHorizontalMovement()
    {
        if (!isMovingDown && !isReturning) 
        {
            float moveX = Input.GetAxis("Horizontal") * clawSpeed * Time.deltaTime;

            // Get current position and apply movement
            Vector2 newPosition = claw.transform.position;
            newPosition.x += moveX;

            // Clamp the X position to stay within the minX and maxX limits
            newPosition.x = Mathf.Clamp(newPosition.x, minMovementLimitX, maxMovementLimitX);

            // Apply the clamped position back to the claw
            claw.transform.position = newPosition;
        }
    }
    
    private void HandleClawAction()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMovingDown && !isReturning)
        {
            StartCoroutine(MoveClawDown());
        }
    }

    private IEnumerator MoveClawDown()
    {
        isMovingDown = true;
        
        while (claw.transform.position.y > clawDownPositionY)
        {
            claw.transform.Translate(Vector2.down * verticalMoveSpeed * Time.deltaTime);
            yield return null;
        }
        
        StartCoroutine(MoveClawUp());
    }

    private IEnumerator MoveClawUp()
    {
        isReturning = true;

        while (claw.transform.position.y < startPosition.y)
        {
            claw.transform.Translate(Vector2.up * verticalMoveSpeed * Time.deltaTime);
            yield return null;
        }

        isMovingDown = false;
        isReturning = false;
        
        if (hasItem)
        {
            returnBox.SetActive(true);
        }
    }
    
    private void GenerateItem()
    {
        if (!hasItem)
        {
            
            bool isGoodItem = Random.Range(0f, 1f) < goodItemChance;
            if (isGoodItem)
            {
                int randomIndex = Random.Range(0, goodItemPrefabs.Length);
                currentItem = Instantiate(goodItemPrefabs[randomIndex], itemSpawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                int randomIndex = Random.Range(0, badItemPrefabs.Length);
                currentItem = Instantiate(badItemPrefabs[randomIndex], itemSpawnPoint.transform.position, Quaternion.identity);
            }

            if (claw.activeInHierarchy)
            {
                StartCoroutine(SetItemParentAfterFrame());
            }
            else
            {
                Debug.LogWarning("Claw is inactive, cannot start coroutine.");
            }
            
            MeatObject meat = currentItem.GetComponent<MeatObject>();

            clawSprite.sprite = meat.meatSprite;
        }
    }
    
    private IEnumerator SetItemParentAfterFrame()
    {
        yield return null;  

        currentItem.transform.SetParent(claw.transform);
        currentItem.transform.position = itemSpawnPoint.transform.position;

        Rigidbody2D itemRigidbody = currentItem.GetComponent<Rigidbody2D>();
        itemRigidbody.gravityScale = 0f;
        itemRigidbody.velocity = Vector2.zero;

        hasItem = true;
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ItemBox"))
        {
            Debug.Log("Hit box");
            GenerateItem();
            SoundManager.PlaySound(SoundType.PickUpMeat,VolumeType.SFX);
        }
    }
    
    public void SetChance0to1(float chance)
    {
        goodItemChance = chance;
    }

    public void RePosition()
    {
        StopAllCoroutines();
         
        isMovingDown = false;
        isReturning = false;
        
        if (currentItem != null)
        {
            Destroy(currentItem);  
            currentItem = null;    
            hasItem = false; 
        }
        
        claw.transform.position = startPosition;
        SetDefaultSprite();
    }

    public void SetDefaultSprite()
    {
        clawSprite.sprite = defaultClawSprite;
    }

    public void PopReturnButtonUp()
    {
        returnButton.SetActive(true); // Ensure the panel is active
        returnButton.transform.localScale = Vector3.zero; // Start from zero scale
        LeanTween.scale(returnButton, initialScale, 0.5f)
            .setEase(LeanTweenType.easeOutBack) // Set easing type
            .setIgnoreTimeScale(true); // Use unscaled time

    }
    
    public void PopReturnButtonDown()
    {
        LeanTween.scale(returnButton, Vector3.zero, 0.5f)
            .setEase(LeanTweenType.easeInBack) // Set easing type
            .setIgnoreTimeScale(true) // Use unscaled time
            .setOnComplete(() =>
            {
                returnButton.SetActive(false);
            });
    }
}
