using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour
{
    public static int scoreValue = 0;
    [SerializeField] private int scoreMax;
    [SerializeField] private int scoreMin;

    public TextMeshProUGUI scoreText;

    public GameObject goHospital;
    public TextMeshProUGUI timeCountText;
    public CountTime countTimeScript;

    void Start()
    {
        scoreValue = 0;
        if (scoreText == null)
            scoreText = GetComponent<TextMeshProUGUI>();
        UpdateScoreText();

        CheckScoreValue();
    }

    void Update()
    {
        scoreValue = Mathf.Clamp(scoreValue, scoreMin, scoreMax);
        UpdateScoreText();

        CheckScoreValue();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = scoreValue.ToString() + "/2";
    }

    void CheckScoreValue()
    {
        if (scoreValue >= 2)
        {
            scoreText.gameObject.SetActive(false);
            goHospital.SetActive(true);
            timeCountText.gameObject.SetActive(true);
            countTimeScript.StartCountdown();
        }
    }
}
