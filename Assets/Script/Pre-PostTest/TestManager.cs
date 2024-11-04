using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Proyecto26;
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
    public int currentQuestion;
    [SerializeField] private int totalQuestion;
    [SerializeField] private int score;
    
    public string firebaseURL = "https://stomachstruggle-default-rtdb.asia-southeast1.firebasedatabase.app/questions";

    private void Start()
    {
        if (QnA == null || QnA.Count == 0)
        {
            Debug.LogError("QnA list is not initialized or is empty.");
            return; // Prevents proceeding if there are no questions
        }
        
        totalQuestion = QnA.Count;
        goPanel.SetActive(false);
        quizPanel.SetActive(true);
        correctionPanel.SetActive(false);
        questionNumber = 0;
        qNumberText.text = $"{questionNumber}.";
        GenerateQuestion();
        scoreText.text = $"{score} / {totalQuestion}";
    }
    
    public void Next(string sceneName)
    {
        switch (sceneName)
        {
            case "PreTest":
                GameManager.Instance.preTestScore = score;
                break;
            case "PostTest":
                GameManager.Instance.postTestScore = score;
                break;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void Retry()
    {
        SceneManager.LoadScene("Start scene");
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
        IncrementWrongCount();
        corretOrNotText.text = "Wrong Answer";
        correctionText.text = QnA[currentQuestion].correction;
        correctionPanel.SetActive(true);
        
        StartCoroutine(HandleNextQuestion());
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
            Debug.Log($"Current Question Index: {currentQuestion}, Question Name: {QnA[currentQuestion].name}");
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
    
    private IEnumerator HandleNextQuestion()
    {
        yield return new WaitForSeconds(0.5f);

        QnA.RemoveAt(currentQuestion);
        GenerateQuestion();
    }
    
    private IEnumerator GetWrongCount(Action<int> onCallback)
    {
        string questionId = $"{QnA[currentQuestion].name}";

        // Wait for the response from RestClient
        yield return RestClient.Get($"{firebaseURL}/{questionId}/wrongCount.json").Then(response =>
        {
            if (int.TryParse(response.Text, out int wrongCount))
            {
                onCallback?.Invoke(wrongCount);
            }
            else
            {
                Debug.LogError("Failed to parse wrong count to an integer: " + response.Text);
            }
        }).Catch(error =>
        {
            Debug.LogError("Error fetching wrong count: " + error.Message);
        });
    }
    
    private void IncrementWrongCount()
    {
        // Start the coroutine to get the current wrong count
        StartCoroutine(GetWrongCount(currentCount =>
        {
            string questionId = $"{QnA[currentQuestion].name}";
            // Increment the wrong count
            int newCount = currentCount + 1;

            Question question = new Question(newCount);

            // Define the path to the question data
            string url = $"{firebaseURL}/{questionId}.json";

            // Update the wrong count in the database
            RestClient.Put(url, question).Then(response =>
            {
                Debug.Log($"Updated wrong count for {questionId}: {newCount}");
            }).Catch(error =>
            {
                Debug.LogError($"Error updating wrong count: {error}");
            });
        })); // Pass questionId directly
    }
}
