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
    [SerializeField] private TextMeshProUGUI[] instructionText;

    private float timeLeft;

    private void Start()
    {
        timeLeft = countdownTime;
        SpawnAllFood();
    }

    private void Update()
    {
        if (GameManager.Instance.GetScore() >= GameManager.Instance.scoreMax || GameManager.Instance.currentHealth <= 0 || GameManager.Instance.GetScore() >= GameManager.Instance.scoreMax)
        {
            timerText.gameObject.SetActive(false);
            foreach (var text in instructionText)
            {
                text.gameObject.SetActive(false);
            }
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
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DecreaseHealth(1);
        }

        timeLeft = countdownTime;
        SpawnAllFood();
    }
}
