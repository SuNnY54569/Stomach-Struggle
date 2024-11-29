using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class spawnWatertwo : MonoBehaviour
{
    [Header("Water Bottle Prefabs")]
    [SerializeField] private GameObject goodWaterPrefab;  
    [SerializeField] private GameObject badWaterPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private List<GameObject> waterBottles;

    [Header("Spawn Control")]
    [SerializeField] private int badWaterCount = 5;

    void Start()
    {
        SpawnWater();
    }
    private void SpawnWater()
    {
        var spawnList = new List<Transform>(spawnPoints);
        spawnList = spawnList.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < badWaterCount && i < spawnList.Count; i++)
        {
            GameObject bottle = Instantiate(badWaterPrefab, spawnList[i].position, Quaternion.identity);
            waterBottles.Add(bottle);
            itemClickWater itemClick = bottle.GetComponent<itemClickWater>();
            itemClick.spawnWatertwo = this;
        }

        for (int i = badWaterCount; i < spawnList.Count; i++)
        {
            GameObject bottle = Instantiate(goodWaterPrefab, spawnList[i].position, Quaternion.identity);
            waterBottles.Add(bottle);
        }
    }

    public void DisableAllCollider()
    {
        foreach (var waterBottle in waterBottles)
        {
            if (waterBottle.activeSelf)
            {
                itemClickWater itemClick = waterBottle.GetComponent<itemClickWater>();
                if (itemClick != null)
                {
                    itemClick.col.enabled = false;
                }
            }
        }
    }
}
