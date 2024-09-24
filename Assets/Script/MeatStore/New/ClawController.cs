using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawController : MonoBehaviour
{
    public GameObject claw;  // The claw object
    public GameObject goodItemPrefab;  // Prefab for good items
    public GameObject badItemPrefab;  // Prefab for bad items
    public GameObject returnBox; // The box where the claw gets items
    public GameObject itemSpawnPoint;

    private GameObject currentItem;  // Item currently attached to the claw
    private bool hasItem = false;  // Tracks if an item is held
    private bool isMovingDown = false;  // Tracks if claw is moving down
    private bool isReturning = false; // Tracks if claw is returning up
    private bool canReturnItem = false;

    public float clawSpeed = 5f;  // Speed of horizontal movement
    public float verticalMoveSpeed = 2f;  // Speed of vertical claw movement
    public float clawDownPositionY = -3f;  // Y position where the claw reaches down
    public Vector2 startPosition;  // Initial position of the claw (set this to the starting position)

    void Start()
    {
        // Set claw's initial position
        startPosition = claw.transform.position;
    }

    void Update()
    {
        // Horizontal movement (left and right)
        if (!isMovingDown && !isReturning)  // Only allow horizontal movement when claw isn't moving vertically
        {
            float moveX = Input.GetAxis("Horizontal") * clawSpeed * Time.deltaTime;
            claw.transform.Translate(new Vector2(moveX, 0));
        }

        // Button press to lower the claw or drop the item
        if (Input.GetKeyDown(KeyCode.Space))  // Use "Space" key or another button for claw action
        {
            if (!hasItem && !isMovingDown && !isReturning)  // Move down if no item is held
            {
                StartCoroutine(MoveClawDown());
            }
            else if (hasItem && !isMovingDown && !isReturning)  // Drop item if holding one and over the basket
            {
                StartCoroutine(MoveClawDown());
            }
        }

        if (currentItem == null)
        {
            hasItem = false;
            returnBox.SetActive(false);
        }
    }

    // Move the claw down to try and pick up an item
    IEnumerator MoveClawDown()
    {
        isMovingDown = true;

        // Move claw down until it reaches the defined Y position
        while (claw.transform.position.y > clawDownPositionY)
        {
            claw.transform.Translate(Vector2.down * verticalMoveSpeed * Time.deltaTime);
            yield return null;
        }

        // Move the claw back up
        StartCoroutine(MoveClawUp());
    }

    // Move the claw back up to its starting position
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

    // Generate either a good or bad item
    void GenerateItem()
    {
        if (!hasItem)
        {
            // Randomly choose a good or bad item
            bool isGoodItem = Random.Range(0, 2) == 0;  // 50% chance for good or bad item
            if (isGoodItem)
            {
                currentItem = Instantiate(goodItemPrefab, itemSpawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                currentItem = Instantiate(badItemPrefab, itemSpawnPoint.transform.position, Quaternion.identity);
            }

            // Attach the generated item to the claw
            currentItem.transform.SetParent(claw.transform);
            currentItem.transform.position = itemSpawnPoint.transform.position;

            // Disable gravity while attached to the claw
            currentItem.GetComponent<Rigidbody2D>().gravityScale = 0f;
            currentItem.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            hasItem = true;
            StartCoroutine(WaitToReturnItem());
        }
    }

    // OnTriggerEnter2D is called when the claw collides with the item box
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ItemBox"))
        {
            Debug.Log("Hit box");
            // Claw touched the item box, generate an item
            GenerateItem();
        }
    }

    IEnumerator WaitToReturnItem()
    {
        yield return new WaitForSeconds(0.5f);
        returnBox.SetActive(true);
    }
}
