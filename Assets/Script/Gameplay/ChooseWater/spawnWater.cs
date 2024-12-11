using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnWater : MonoBehaviour
{
    [SerializeField] private GameObject[] waterPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    void Start()
    {
        SpawnAllWater();
    }

    private void SpawnAllWater()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject randomWater = waterPrefabs[Random.Range(0, waterPrefabs.Length)];

            Instantiate(randomWater, spawnPoints[i].position, spawnPoints[i].rotation);
        }
    }
}
