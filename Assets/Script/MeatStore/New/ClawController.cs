using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawController : MonoBehaviour
{
    [Header("Claw Settings")]
    [SerializeField,Tooltip("The claw object that will pick up items.")]
    private GameObject claw; 
    [SerializeField,Tooltip("Prefab for good items that can be collected.")]
    private GameObject goodItemPrefab;  
    [SerializeField,Tooltip("Prefab for bad items that may decrease health.")]
    private GameObject badItemPrefab;  
    [SerializeField,Tooltip("The box where the claw returns the picked items.")]
    private GameObject returnBox; 
    [SerializeField,Tooltip("Spawn point for items that the claw can pick up.")]
    private GameObject itemSpawnPoint; 

    [Header("Movement Settings")]
    [SerializeField,Tooltip("Speed of horizontal movement for the claw.")]
    private float clawSpeed;  
    [SerializeField,Tooltip("Speed of vertical movement for the claw.")]
    private float verticalMoveSpeed;  
    [SerializeField,Tooltip("Y position where the claw reaches down to pick items.")]
    private float clawDownPositionY; 
    [SerializeField,Tooltip("Initial position of the claw when the game starts.")]
    private Vector2 startPosition; 
    
    [Header("Item Generation Settings")]
    [SerializeField,Tooltip("Chance of generating a good item (0 to 1).")]
    [Range(0f, 1f)]
    private float goodItemChance = 0.5f;
    
    private GameObject currentItem;  // Item currently attached to the claw
    private bool hasItem = false;  // Tracks if an item is held
    private bool isMovingDown = false;  // Tracks if claw is moving down
    private bool isReturning = false; // Tracks if claw is returning up

    void Start()
    {
        startPosition = claw.transform.position;
    }

    void Update()
    {
        HandleHorizontalMovement();
        HandleClawAction();

        if (currentItem == null)
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
            claw.transform.Translate(new Vector2(moveX, 0));
        }
    }
    
    private void HandleClawAction()
    {
        if (Input.GetKey(KeyCode.Space)) 
        {
            if (!isMovingDown && !isReturning) 
            {
                StartCoroutine(MoveClawDown());
            }
        }
    }
    
    IEnumerator MoveClawDown()
    {
        isMovingDown = true;
        
        while (claw.transform.position.y > clawDownPositionY)
        {
            claw.transform.Translate(Vector2.down * verticalMoveSpeed * Time.deltaTime);
            yield return null;
        }
        
        StartCoroutine(MoveClawUp());
    }
    
    IEnumerator MoveClawUp()
    {
        isReturning = true;

        while (claw.transform.position.y < startPosition.y)
        {
            claw.transform.Translate(Vector2.up * verticalMoveSpeed * Time.deltaTime);
            yield return null;
        }

        isMovingDown = false;
        isReturning = false;
        
    }
    
    void GenerateItem()
    {
        if (!hasItem)
        {
            
            bool isGoodItem = Random.Range(0f, 1f) < goodItemChance;
            currentItem = Instantiate(isGoodItem ? goodItemPrefab : badItemPrefab,
                itemSpawnPoint.transform.position, Quaternion.identity);

            currentItem.transform.SetParent(claw.transform);
            currentItem.transform.position = itemSpawnPoint.transform.position;
            
            Rigidbody2D itemRigidbody = currentItem.GetComponent<Rigidbody2D>();
            itemRigidbody.gravityScale = 0f;
            itemRigidbody.velocity = Vector2.zero;

            hasItem = true;
            StartCoroutine(WaitToReturnItem());
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

    IEnumerator WaitToReturnItem()
    {
        yield return new WaitForSeconds(0.5f);
        returnBox.SetActive(true);
    }
}
