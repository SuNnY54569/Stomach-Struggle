using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodManager : MonoBehaviour
{
    [SerializeField] private GameObject[] rawFoodPrefabs; // Assign your raw food prefab in the Unity editor
    [SerializeField] private Transform[] spawnPoints; // Set multiple spawn points in the editor

    void Start()
    {
        SpawnFood();
    }

    void SpawnFood()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            GameObject randomFoodPrefab = rawFoodPrefabs[Random.Range(0, rawFoodPrefabs.Length)];
            Instantiate(randomFoodPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Raw"))
        {
            SpawnFood();
        }
    }
}
