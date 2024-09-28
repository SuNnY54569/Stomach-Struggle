using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreMeatShop : MonoBehaviour
{
    [SerializeField] private int scoreValue = 0;
    [SerializeField] private int scoreMax;
    [SerializeField] private int scoreMin;
    [SerializeField] private GameObject WinScene;

    public TextMeshProUGUI scoreText;

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

    
}
