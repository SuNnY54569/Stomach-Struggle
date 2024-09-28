using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    [SerializeField] private GameObject rawFoodPrefab; // Assign your raw food prefab in the Unity editor
    [SerializeField] private Transform[] spawnPoints; // Set multiple spawn points in the editor
    [SerializeField] private int foodCount = 5; // How many food items to spawn

    void Start()
    {
        SpawnFood();
    }

    void SpawnFood()
    {
        for (int i = 0; i < foodCount; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Instantiate(rawFoodPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
        }
    }
}
