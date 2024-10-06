using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class spawnFoodRandom : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject foodPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Timer Settings")]
    [SerializeField] private float countdownTime = 30f;
    [SerializeField] private TextMeshProUGUI timerText;

    private float timeLeft;

    private void Start()
    {
        timeLeft = countdownTime;
        SpawnAllFood();
    }

    private void Update()
    {
        if (ScoreGuitar.scoreValue >= FindObjectOfType<ScoreGuitar>().ScoreMax)
        {
            return;
        }

        GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("Food");

        if (foodObjects.Length == 0)
        {
            SpawnAllFood();
            timeLeft = countdownTime;
        }

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timeLeft).ToString();
        }
        else
        {
            HandleTimeUp();
        }
    }

    private void SpawnAllFood()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                GameObject newFood = Instantiate(foodPrefab, spawnPoint.position, spawnPoint.rotation);
                FoodRandom foodRandomScript = newFood.GetComponent<FoodRandom>();

                if (foodRandomScript != null)
                {
                    foodRandomScript.RandomizeFoodAndTime();
                }
            }
        }
    }

    private void HandleTimeUp()
    {
        GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("Food");

        foreach (GameObject food in foodObjects)
        {
            Destroy(food);
        }

        Health playerHealth = FindObjectOfType<Health>();
        if (playerHealth != null)
        {
            playerHealth.DecreaseHealth(1);
        }

        timeLeft = countdownTime;
        SpawnAllFood();
    }
}
