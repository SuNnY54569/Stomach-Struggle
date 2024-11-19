using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteakSpawner : MonoBehaviour
{
    [Header("Steak Settings")]
    [SerializeField] private GameObject steakPrefab; // Prefab to spawn
    [SerializeField] private Transform[] spawnPoints; // Where to spawn the new steak
    [SerializeField] private int maxSteakCount = 4; // Total number of steaks to spawn
    [SerializeField] private int previousMaxScore; // Max score to win the game
    [SerializeField] private int maxScore = 3;
    
    private int remainingSteaks;

    private void Start()
    {
        previousMaxScore = GameManager.Instance.scoreMax;
        GameManager.Instance.SetMaxScore(maxScore);
        GameManager.Instance.SetScoreTextActive(true);
        GameManager.Instance.UpdateScoreText();
        remainingSteaks = maxSteakCount;
        SpawnInitialSteaks();
        //SoundManager.PlaySound();
    }

    private void SpawnInitialSteaks()
    {
        // Spawn steaks at each spawn point, up to the maxSteakCount
        for (int i = 0; i < maxSteakCount && i < spawnPoints.Length; i++)
        {
            Instantiate(steakPrefab, spawnPoints[i].position, Quaternion.identity);
        }
    }

    public void HandleSteakLost()
    {
        remainingSteaks--;
        if (remainingSteaks <= 0)
        {
            if (GameManager.Instance.GetScore() < GameManager.Instance.scoreMax)
            {
                GameManager.Instance.GameOver();
            }
            
            GameManager.Instance.scoreMax = previousMaxScore;
        }
    }
}
