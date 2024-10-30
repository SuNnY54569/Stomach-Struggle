using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TreatmentSpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] treatments; // Combine good and bad treatments into one array
    [SerializeField] private Transform[] spawnpoints;

    private void Start()
    {
        SpawnTreatments();
        GameManager.Instance.SetMaxScore(3);
        GameManager.Instance.UpdateScoreText();
    }
    
    private void SpawnTreatments()
    {
        // Shuffle the spawnpoints array to randomize treatment placement
        Transform[] shuffledSpawnpoints = (Transform[])spawnpoints.Clone();
        ShuffleArray(shuffledSpawnpoints);

        // Make sure we don't exceed the number of spawnpoints
        int maxSpawnCount = Mathf.Min(treatments.Length, shuffledSpawnpoints.Length);

        for (int i = 0; i < maxSpawnCount; i++)
        {
            Instantiate(treatments[i], shuffledSpawnpoints[i].position, Quaternion.identity);
        }
    }

    private void ShuffleArray(Transform[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Transform temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
