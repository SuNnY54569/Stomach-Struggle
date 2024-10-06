using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreGuitar : MonoBehaviour
{
    public static int scoreValue = 0;
    [SerializeField] private int scoreMax;
    [SerializeField] private int scoreMin;
    [SerializeField] private GameObject WinScene;

    public TextMeshProUGUI scoreText;

    public int ScoreMax => scoreMax;

    void Start()
    {
        scoreValue = 0;
        if (scoreText == null)
            scoreText = GetComponent<TextMeshProUGUI>();
        UpdateScoreText();
    }

    void Update()
    {
        scoreValue = Mathf.Clamp(scoreValue, scoreMin, scoreMax);
        UpdateScoreText();
        CheckWin();
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

    
}
