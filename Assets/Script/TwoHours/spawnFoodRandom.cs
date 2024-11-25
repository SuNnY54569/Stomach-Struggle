using System;
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

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI spawnCountText;
    [SerializeField] private GameObject clockGameObject;
    [SerializeField] private GameObject guideText;
    [SerializeField] private GameObject panel;

    private bool isGameOver = false;
    public bool isTickingSoundPlaying = false;

    private float timeLeft;
    private int spawnCount = 0;
    private const int maxSpawns = 4;

    private void Awake()
    {
        UITransitionUtility.Instance.Initialize(panel,Vector2.zero);
    }

    private void Start()
    {
        GameManager.Instance.SetScoreTextActive(false);
        timeLeft = countdownTime;
        UpdateSpawnCountUI();
        UITransitionUtility.Instance.MoveIn(panel,LeanTweenType.easeInOutQuad, 0.2f);
    }

    private void Update()
    {
        clockGameObject.SetActive(!GameManager.Instance.isGamePaused);
        spawnCountText.gameObject.SetActive(!GameManager.Instance.isGamePaused);
        guideText.SetActive(!GameManager.Instance.isGamePaused);
        
        if (spawnCount >= maxSpawns)
        {
            timerText.gameObject.SetActive(false);
            clockGameObject.SetActive(false);
            UITransitionUtility.Instance.MoveOut(panel,LeanTweenType.easeInOutQuad, 0.2f);
            
            if (isGameOver) return;
                
            if (GameManager.Instance.currentHealth <= 0)
            {
                GameManager.Instance.GameOver();
            }
            else 
            {
                GameManager.Instance.WinGame();
            }
            spawnCount = 3;
            timerText.gameObject.SetActive(false);
            clockGameObject.SetActive(false);
            spawnCountText.gameObject.SetActive(false);

            foreach (var text in instructionText)
            {
                text.gameObject.SetActive(false);
            }

            isGameOver = true;

            return;
        }

        if (GameManager.Instance.currentHealth <= 0)
        {
            timerText.gameObject.SetActive(false);
            spawnCountText.gameObject.SetActive(false);
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
            if (!isTickingSoundPlaying)
            {
                StartCoroutine(PlayClockTickingSound());
            }
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
        spawnCount++;
        
        UpdateSpawnCountUI();
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

        if (spawnCount < maxSpawns)
        {
            spawnCount -= 1;
            SpawnAllFood();
        }
    }

    private void UpdateSpawnCountUI()
    {
        if (spawnCountText != null)
        {
            spawnCountText.text = $"{spawnCount} / 3";
        }
    }

    IEnumerator PlayClockTickingSound()
    {
        isTickingSoundPlaying = true;
        SoundManager.PlaySound(SoundType.ClockTicking, VolumeType.SFX);

        yield return new WaitForSeconds(1f);
        isTickingSoundPlaying = false;
    }
}
