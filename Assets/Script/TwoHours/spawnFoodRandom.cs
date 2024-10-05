using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class spawnFoodRandom : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject foodPrefab;

    [Header("Spawn Points Food")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Timer Settings")]
    [SerializeField] private float countdownTime = 30f;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Start()    
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        while (true)
        {
            SpawnAllFood();
            yield return StartCoroutine(CountdownTimer());
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

    private IEnumerator CountdownTimer()
    {
        float timeLeft = countdownTime;

        while (timeLeft > 0)
        {
            timerText.text = $"{timeLeft:F1}";
            yield return new WaitForSeconds(0.1f);
            timeLeft -= 0.1f;
        }

        HandleTimeUp();
    }

    private void HandleTimeUp()
    {
        GameObject[] foodWarm = GameObject.FindGameObjectsWithTag("WarmBeforeEat");
        GameObject[] foodEat = GameObject.FindGameObjectsWithTag("CanEat");

        List<GameObject> foodsToDestroy = new List<GameObject>();
        foodsToDestroy.AddRange(foodWarm);
        foodsToDestroy.AddRange(foodEat);

        foreach (GameObject food in foodsToDestroy)
        {
            Destroy(food);
        }


        Health playerHealth = FindObjectOfType<Health>();
        if (playerHealth != null)
        {
            playerHealth.DecreaseHealth(1);
        }
    }

}
