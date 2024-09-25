using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Movement Settings")]
    [SerializeField,Tooltip("Speed of horizontal movement for the claw.")]
    private float clawSpeed;  
    [SerializeField,Tooltip("Speed of vertical movement for the claw.")]
    private float verticalMoveSpeed;  
    [SerializeField,Tooltip("Y position where the claw reaches down to pick items.")]
    private float clawDownPositionY; 
    [SerializeField,Tooltip("Initial position of the claw when the game starts.")]
    private Vector2 startPosition;
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
    
    // Button control variables
    private bool isMovingLeft = false;
    private bool isMovingRight = false;
    private bool isMovingDownByButton = false;

    void Start()
    {
        startPosition = claw.transform.position;
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
            float moveX = 0f;
            
            moveX += Input.GetAxis("Horizontal") * clawSpeed * Time.deltaTime;
            
            if (isMovingLeft)
            {
                moveX = -clawSpeed * Time.deltaTime;
            }
            else if (isMovingRight)
            {
                moveX = clawSpeed * Time.deltaTime;
            }
            
            Vector2 newPosition = claw.transform.position;
            newPosition.x += moveX;
            newPosition.x = Mathf.Clamp(newPosition.x, minMovementLimitX, maxMovementLimitX);
            claw.transform.position = newPosition;
        }
    }
    
    private void HandleClawAction()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || isMovingDownByButton) && !isMovingDown && !isReturning)
        {
            StartCoroutine(MoveClawDown());
        }
    }

    private IEnumerator MoveClawDown()
    {
        isMovingDown = true;
        isMovingDownByButton = false;
        
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

            currentItem.transform.SetParent(claw.transform);
            currentItem.transform.position = itemSpawnPoint.transform.position;
            
            Rigidbody2D itemRigidbody = currentItem.GetComponent<Rigidbody2D>();
            itemRigidbody.gravityScale = 0f;
            itemRigidbody.velocity = Vector2.zero;

            hasItem = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ItemBox"))
        {
            Debug.Log("Hit box");
            GenerateItem();
        }
    }
    
    public void SetChance0to1(float chance)
    {
        goodItemChance = chance;
    }
    
    public void MoveLeftButtonDown()
    {
        isMovingLeft = true;
    }

    public void MoveLeftButtonUp()
    {
        isMovingLeft = false;
    }

    public void MoveRightButtonDown()
    {
        isMovingRight = true;
    }

    public void MoveRightButtonUp()
    {
        isMovingRight = false;
    }

    public void MoveDownButtonPressed()
    {
        if (!isMovingDown && !isReturning)
        {
            isMovingDownByButton = true;
        }
    }
    
    
}
