using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TestManager : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] private List<QandA> QnA;
    
    [Header("References")]
    [SerializeField] private GameObject[] options;
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private GameObject goPanel;
    [SerializeField] private GameObject correctionPanel;
    [SerializeField] private TMP_Text corretOrNotText;
    [SerializeField] private TMP_Text correctionText;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text finalscoreText;
    [SerializeField] private TMP_Text qNumberText;

    [Header("Infos")] 
    [SerializeField] private int questionNumber;
    [SerializeField] private int currentQuestion;
    [SerializeField] private int totalQuestion;
    [SerializeField] private int score;

    private void Start()
    {
        totalQuestion = QnA.Count;
        goPanel.SetActive(false);
        quizPanel.SetActive(true);
        correctionPanel.SetActive(false);
        questionNumber = 0;
        qNumberText.text = $"{questionNumber}.";
        GenerateQuestion();
        scoreText.text = $"{score} / {totalQuestion}";
    }

    public void Next()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    private void GameOver()
    {
        quizPanel.SetActive(false);
        goPanel.SetActive(true);
        finalscoreText.text = $"{score} / {totalQuestion}";
    }
    public void Correct()
    {
        corretOrNotText.text = "Correct Answer";
        correctionText.text = "Good Job !";
        score += 1;
        scoreText.text = $"{score} / {totalQuestion}";
        correctionPanel.SetActive(true);
        QnA.RemoveAt(currentQuestion);
        GenerateQuestion();
    }

    public void Wrong()
    {
        corretOrNotText.text = "Wrong Answer";
        correctionText.text = QnA[currentQuestion].correction;
        correctionPanel.SetActive(true);
        QnA.RemoveAt(currentQuestion);
        GenerateQuestion();
    }
    
    private void SetAnswer()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<TMP_Text>().text = QnA[currentQuestion].answers[i];
            if (QnA[currentQuestion].correctAnswers == i+1)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = true;
            }
        }
    }

    private void GenerateQuestion()
    {
        if (QnA.Count > 0)
        {
            currentQuestion = Random.Range(0, QnA.Count);

            questionText.text = QnA[currentQuestion].question;
            SetAnswer();
            questionNumber++;
            qNumberText.text = $"{questionNumber}.";
        }
        else
        {
            Debug.Log("Out of Questions");
            GameOver();
        }
    }
}
