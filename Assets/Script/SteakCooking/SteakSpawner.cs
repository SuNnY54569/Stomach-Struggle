using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteakSpawner : MonoBehaviour
{
    [Header("Steak Settings")]
    [SerializeField] private GameObject steakPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int maxSteakCount = 4;
    [SerializeField] private int maxScore = 3;
    
    private int remainingSteaks;
    private bool isGameOverTriggered = false;

    private void Start()
    {
        GameManager.Instance.SetMaxScore(maxScore);
        GameManager.Instance.SetScoreTextActive(true);
        GameManager.Instance.UpdateScoreText();
        remainingSteaks = maxSteakCount;
        SpawnInitialSteaks();
    }

    private void SpawnInitialSteaks()
    {
        for (int i = 0; i < maxSteakCount && i < spawnPoints.Length; i++)
        {
            Instantiate(steakPrefab, spawnPoints[i].position, Quaternion.identity);
        }
    }

    public void HandleSteakLost()
    {
        if (isGameOverTriggered) return;

        remainingSteaks--;
        CheckGameOverConditions();
    }
    
    private void CheckGameOverConditions()
    {
        // Centralized game-over logic
        if (isGameOverTriggered) return;

        if (GameManager.Instance.currentHealth == 0)
        {
            isGameOverTriggered = true;
        }

        if (remainingSteaks < maxScore && 
            (GameManager.Instance.GetScore() < GameManager.Instance.scoreMax || 
             GameManager.Instance.currentHealth == 0))
        {
            TriggerGameOver();
        }
    }
    
    private void TriggerGameOver()
    {
        if (isGameOverTriggered) return;

        isGameOverTriggered = true;
        GameManager.Instance.GameOver();
    }
}
