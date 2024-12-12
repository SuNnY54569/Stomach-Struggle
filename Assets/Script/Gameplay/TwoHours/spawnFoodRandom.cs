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

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI spawnCountText;
    [SerializeField] private TextMeshProUGUI[] instructionText;
    [SerializeField] private GameObject clockGameObject;
    [SerializeField] private GameObject guideText;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject _canvas;

    private bool isGameOver = false;
    private bool isTickingSoundPlaying = false;
    private float timeLeft;
    private int spawnCount = 0;
    private const int maxSpawns = 4;

    private void Awake()
    {
        UITransitionUtility.Instance.Initialize(panel,Vector2.zero);
        
        Canvas canvas = _canvas.gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 1;
    }

    private void Start()
    {
        //GameManager.Instance.SetScoreTextActive(false);
        timeLeft = countdownTime;
        UpdateSpawnCountUI();
        UITransitionUtility.Instance.MoveIn(panel,LeanTweenType.easeInOutQuad, 0.2f);
    }
    
    private void OnEnable()
    {
        GameManager.OnGamePaused += HandlePause;
        GameManager.OnGameUnpaused += HandleUnpause;
    }

    private void OnDisable()
    {
        GameManager.OnGamePaused -= HandlePause;
        GameManager.OnGameUnpaused -= HandleUnpause;
    }

    private void Update()
    {
        if (isGameOver) return;

        if (spawnCount >= maxSpawns && GameManager.Instance.healthManager.currentHealth > 0)
        {
            WinGame();
            return;
        }

        if (GameManager.Instance.healthManager.currentHealth <= 0)
        {
            GameOver();
            return;
        }

        HandleFoodAndTimer();
    }
    
    private void HandlePause()
    {
        ScaleUI(Vector3.zero, 0.2f, true);
    }

    private void HandleUnpause()
    {
        ScaleUI(Vector3.one, 0.5f, true, 0.5f);
    }
    
    private void ScaleUI(Vector3 scale, float duration, bool ignoreTimeScale, float delay = 0f)
    {
        LeanTween.scale(clockGameObject, scale, duration).setEase(LeanTweenType.easeInOutQuad).setIgnoreTimeScale(ignoreTimeScale).setDelay(delay);
        LeanTween.scale(spawnCountText.gameObject, scale, duration).setEase(LeanTweenType.easeInOutQuad).setIgnoreTimeScale(ignoreTimeScale).setDelay(delay);
        LeanTween.scale(guideText, scale, duration).setEase(LeanTweenType.easeInOutQuad).setIgnoreTimeScale(ignoreTimeScale).setDelay(delay);
    }

    private void SpawnAllFood()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint == null) continue;

            var newFood = Instantiate(foodPrefab, spawnPoint.position, spawnPoint.rotation);
            newFood.GetComponent<FoodRandom>()?.RandomizeFoodAndTime();
        }

        spawnCount++;
        UpdateSpawnCountUI();
    }

    private void HandleTimeUp()
    {
        foreach (var food in GameObject.FindGameObjectsWithTag("Food"))
        {
            Destroy(food);
        }

        GameManager.Instance.healthManager.DecreaseHealth(1);

        timeLeft = countdownTime;

        if (spawnCount < maxSpawns)
        {
            spawnCount--;
            SpawnAllFood();
        }
    }

    private void UpdateSpawnCountUI()
    {
        spawnCountText.text = $"{spawnCount} / {maxSpawns - 1}";
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
        if (GameObject.FindGameObjectsWithTag("Food").Length == 0)
        {
            SpawnAllFood();
            timeLeft = countdownTime;
        }

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();

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

        HideUIElements();
        UITransitionUtility.Instance?.MoveOut(panel, LeanTweenType.easeInOutQuad, 0.2f);
        isGameOver = true;
    }
    
    private void WinGame()
    {
        if (isGameOver) return;

        HideUIElements();
        UITransitionUtility.Instance?.MoveOut(panel, LeanTweenType.easeInOutQuad, 0.2f);
        GameManager.Instance.healthManager.WinGame();
        isGameOver = true;
    }
    
    private void HideUIElements()
    {
        timerText.gameObject.SetActive(false);
        spawnCountText.gameObject.SetActive(false);
        clockGameObject.SetActive(false);

        foreach (var text in instructionText)
        {
            text.gameObject.SetActive(false);
        }
    }
}
