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
    
    [Header("UI Setting")]
    [SerializeField] private GameObject _canvas;
    public GameObject buttonPanel;
    [SerializeField] private GameObject clawGameObject;
    
    private GameObject currentItem;  
    private bool hasItem = false;  
    private bool isMovingLeft = false;
    private bool isMovingRight = false;
    private bool isMovingDown = false;
    private bool isReturning = false;
    public bool isInGame;

    private void Awake()
    {
        Canvas canvas = _canvas.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 1;
        initialScale = returnButton.transform.localScale;
        clawGameObject.SetActive(false);
    }

    private void Start()
    {
        claw.transform.position = startPosition;
    }

    void Update()
    {
        HandlePCInput();
        HandleHorizontalMovement();
        
        if (currentItem == null && !isMovingDown && !isReturning)
        {
            hasItem = false;
            returnBox.SetActive(false);
        }
    }
    
    private void HandlePCInput()
    {
        if (!isMovingDown && !isReturning)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                isMovingLeft = true;
                isMovingRight = false;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                isMovingRight = true;
                isMovingLeft = false;
            }

            // Detect Key Release for Instant Stop
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                isMovingLeft = false;
            }
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                isMovingRight = false;
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartClawAction();
            }
        }
    }
    
    private void HandleHorizontalMovement()
    {
        if (!isMovingDown && !isReturning)
        {
            float moveX = 0;
            if (isMovingLeft)
                moveX = -clawSpeed * Time.deltaTime;
            if (isMovingRight)
                moveX = clawSpeed * Time.deltaTime;

            Vector2 newPosition = claw.transform.position;
            newPosition.x += moveX;
            newPosition.x = Mathf.Clamp(newPosition.x, minMovementLimitX, maxMovementLimitX);
            claw.transform.position = newPosition;
        }
    }
    
    public void MoveLeft(bool move)
    {
        isMovingLeft = move;
    }

    public void MoveRight(bool move)
    {
        isMovingRight = move;
    }
    
    public void StartClawAction()
    {
        if (!isMovingDown && !isReturning && claw.activeInHierarchy)
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
            GameObject itemPrefab = isGoodItem
                ? goodItemPrefabs[Random.Range(0, goodItemPrefabs.Length)]
                : badItemPrefabs[Random.Range(0, badItemPrefabs.Length)];

            currentItem = Instantiate(itemPrefab, itemSpawnPoint.transform.position, Quaternion.identity);
            if (claw.activeInHierarchy)
            {
                StartCoroutine(SetItemParentAfterFrame());
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
        returnButton.SetActive(true);
        returnButton.transform.localScale = Vector3.zero; 
        LeanTween.scale(returnButton, initialScale, 0.5f)
            .setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true);

    }
    
    public void PopReturnButtonDown()
    {
        LeanTween.scale(returnButton, Vector3.zero, 0.5f)
            .setEase(LeanTweenType.easeInBack)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                returnButton.SetActive(false);
            });
    }
}
