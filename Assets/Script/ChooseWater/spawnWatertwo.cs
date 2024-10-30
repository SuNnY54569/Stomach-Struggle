using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnWatertwo : MonoBehaviour
{
    [SerializeField] private GameObject goodWaterPrefab;
    [SerializeField] private GameObject badWaterPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private int badWaterCount = 5;

    void Start()
    {
        SpawnWater();
    }
    private void SpawnWater()
    {
        List<Transform> spawnList = new List<Transform>(spawnPoints);
        ShuffleList(spawnList);

        for (int i = 0; i < badWaterCount && i < spawnList.Count; i++)
        {
            Instantiate(badWaterPrefab, spawnList[i].position, Quaternion.identity);
        }

        for (int i = badWaterCount; i < spawnList.Count; i++)
        {
            Instantiate(goodWaterPrefab, spawnList[i].position, Quaternion.identity);
        }
    }
    private void ShuffleList(List<Transform> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Transform temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
