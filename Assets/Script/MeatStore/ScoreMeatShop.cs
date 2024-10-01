using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreMeatShop : MonoBehaviour
{
    public static ScoreMeatShop Instance { get; private set; }
    
    [SerializeField] private int scoreValue = 0;
    [SerializeField] private GameObject WinScene;
    public int scoreMax;
    public int scoreMin;

    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        scoreValue = 0;
        if (scoreText == null)
            scoreText = GetComponent<TextMeshProUGUI>();
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = scoreValue.ToString() + $"/{scoreMax}";
    }

    void CheckWin()
    {
        if (scoreValue == scoreMax)
        {
            WinScene.gameObject.SetActive(true);
        }
    }
    
    public void ScoreUp(int amount)
    {
        scoreValue += amount;
        scoreValue = Mathf.Clamp(scoreValue, scoreMin, scoreMax);
        UpdateScoreText();
        CheckWin();
    }

    public int GetScore()
    {
        return scoreValue;
    }

    
}
