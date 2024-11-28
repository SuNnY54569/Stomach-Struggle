using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class spawnWatertwo : MonoBehaviour
{
    [SerializeField] private GameObject goodWaterPrefab;
    [SerializeField] private GameObject badWaterPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private List<GameObject> waterBottles;

    private int badWaterCount = 5;

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
            bottle.GetComponent<itemClickWater>().spawnWatertwo = gameObject.GetComponent<spawnWatertwo>();
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
                waterBottle.GetComponent<itemClickWater>().col.enabled = false;
            }
        }
    }
}
