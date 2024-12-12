using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodSpawner : MonoBehaviour
{
    #region Spawner Settings
    [Header("Spawner Settings")]
    [SerializeField, Tooltip("Array of raw food prefabs to spawn randomly.")]
    private GameObject[] rawFoodPrefabs;
    
    [SerializeField, Tooltip("Delay between food spawns.")]
    private float spawnDelay;
    
    [SerializeField, Tooltip("Distance for raycast to check if object is above the spawn point.")]
    private float raycastDistance = 1.0f;
    #endregion
    
    // Start is called before the first frame update
    private void Start()
    {
        SpawnFood();
    }

    public void SpawnFood()
    {
        if (!IsObjectAbove())
        {
            Instantiate(rawFoodPrefabs[Random.Range(0, rawFoodPrefabs.Length)], transform.position, Quaternion.identity, transform);
        }
    }
    
    private bool IsObjectAbove()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, raycastDistance);
        return hit.collider != null;
    }
}
