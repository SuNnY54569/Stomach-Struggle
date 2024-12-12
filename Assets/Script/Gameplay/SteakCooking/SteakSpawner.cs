using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteakSpawner : MonoBehaviour
{
    [Header("Steak Settings")]
    [SerializeField] private GameObject steakPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int maxSteakCount = 4;
    
    private int remainingSteaks;
    private bool isGameOverTriggered = false;

    private void Start()
    {
        GameManager.Instance.scoreManager.UpdateScoreText();
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
        if (isGameOverTriggered) return;

        if (GameManager.Instance.healthManager.currentHealth == 0)
        {
            isGameOverTriggered = true;
        }

        if (remainingSteaks < GameManager.Instance.scoreManager.scoreMax && 
            (GameManager.Instance.scoreManager.GetScore() < GameManager.Instance.scoreManager.scoreMax || 
             GameManager.Instance.healthManager.currentHealth == 0))
        {
            TriggerGameOver();
        }
    }
    
    private void TriggerGameOver()
    {
        if (isGameOverTriggered) return;

        isGameOverTriggered = true;
        GameManager.Instance.healthManager.GameOver();
    }
}
