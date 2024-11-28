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
        
        if (isGameOver) return;
        
        if (spawnCount >= maxSpawns && GameManager.Instance.currentHealth > 0)
        {
            WinGame();
            Debug.Log("Win");
            //GameManager.Instance.PauseGame();
            return;
        }
        
        if (GameManager.Instance.currentHealth <= 0)
        {
            GameOver();
        }
        
        HandleFoodAndTimer();
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
    
    private void HandleFoodAndTimer()
    {
        // Spawn new food if all food objects are gone
        GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("Food");

        if (foodObjects.Length == 0)
        {
            SpawnAllFood();
            timeLeft = countdownTime;
        }

        // Handle timer countdown
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
    
    private void GameOver()
    {
        if (isGameOver) return;

        timerText.gameObject.SetActive(false);
        spawnCountText.gameObject.SetActive(false);
        clockGameObject.SetActive(false);

        UITransitionUtility.Instance.MoveOut(panel, LeanTweenType.easeInOutQuad, 0.2f);
        
        isGameOver = true;
    }
    
    private void WinGame()
    {
        if (isGameOver) return;

        timerText.gameObject.SetActive(false);
        spawnCountText.gameObject.SetActive(false);
        clockGameObject.SetActive(false);

        foreach (var text in instructionText)
        {
            text.gameObject.SetActive(false);
        }

        UITransitionUtility.Instance.MoveOut(panel, LeanTweenType.easeInOutQuad, 0.2f);

        GameManager.Instance.WinGame();
        isGameOver = true;
    }
}
