using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] rawFoodPrefabs;
    [SerializeField] private float spawnDelay;
    [SerializeField] private float raycastDistance = 1.0f;
    
    // Start is called before the first frame update
    private void Start()
    {
        SpawnFood();
        StartCoroutine(SpawnFoodWithDelay());
    }

    private void SpawnFood()
    {
        if (!IsObjectAbove())
        {
            GameObject randomFoodPrefab = rawFoodPrefabs[Random.Range(0, rawFoodPrefabs.Length)];
            Instantiate(randomFoodPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("Cannot spawn food; an object is above the spawn point.");
        }
    }
    
    private bool IsObjectAbove()
    {
        // Perform a raycast from the spawn point upwards
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, raycastDistance);
        return hit.collider != null; // Returns true if an object is detected
    }
    
    private IEnumerator SpawnFoodWithDelay()
    {
        while (true) // Repeat indefinitely
        {
            yield return new WaitForSeconds(spawnDelay); // Wait for the specified delay
            SpawnFood(); // Attempt to spawn food
        }
    }
}
