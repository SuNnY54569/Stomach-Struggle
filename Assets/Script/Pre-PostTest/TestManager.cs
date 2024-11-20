using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Proyecto26;
using Random = UnityEngine.Random;
using SimpleJSON;

public class TestManager : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField, Tooltip("List of questions and answers for the quiz")]
    private List<QandA> QnA;
    private List<QandA> originalQnA;

    [Header("References")]
    [SerializeField, Tooltip("Array of answer option buttons")]
    private GameObject[] options;
    [SerializeField, Tooltip("Panel for displaying the quiz")]
    private GameObject quizPanel;
    [SerializeField, Tooltip("Panel for displaying game over screen")]
    private GameObject goPanel;
    [SerializeField, Tooltip("Panel for displaying answer corrections")]
    private GameObject correctionPanel;
    [SerializeField, Tooltip("Panel for displaying correct answer feedback")]
    private GameObject correctPanel;
    [SerializeField, Tooltip("Panel for displaying Start game")]
    private GameObject startPanel;
    [SerializeField, Tooltip("Text for indicating if the answer was correct or incorrect")]
    private TMP_Text corretOrNotText;
    [SerializeField, Tooltip("Text for showing the correction of the answer")]
    private TMP_Text correctionText;
    [SerializeField, Tooltip("Text for displaying the question")]
    private TMP_Text questionText;
    [SerializeField, Tooltip("Text for displaying the current score")]
    private TMP_Text scoreText;
    [SerializeField, Tooltip("Text for displaying the final score on game over screen")]
    private TMP_Text finalscoreText;
    [SerializeField, Tooltip("Text for displaying the question number")]
    private TMP_Text qNumberText;

    [Header("Infos")]
    [SerializeField, Tooltip("Current question number")]
    private int questionNumber;
    [SerializeField, Tooltip("Index of the current question in the QnA list")]
    private int currentQuestion;
    [SerializeField, Tooltip("Total number of questions in the quiz")]
    private int totalQuestion;
    [SerializeField, Tooltip("Player's score")]
    private int score;
    
    [Header("Quiz Settings")]
    [SerializeField, Tooltip("Minimum score needed to pass the post-test")]
    private int minimumPassingScore = 5;
    [SerializeField, Tooltip("Button to proceed to the next scene")]
    private GameObject nextButton;
    [SerializeField, Tooltip("Minimum Score Text GameObject")]
    private GameObject minimumScoreText;
    [SerializeField, Tooltip("Time Untill Correct panel to close")]
    private float disablePanelTime;

    [Tooltip("Firebase database URL for questions")]
    string firebaseURL = "https://stomachstruggle-default-rtdb.asia-southeast1.firebasedatabase.app/questions";

    private void Start()
    {
        if (QnA == null || QnA.Count == 0)
        {
            Debug.LogError("QnA list is not initialized or is empty.");
            return;
        }
        
        originalQnA = new List<QandA>(QnA);

        Initialize();
    }

    private void LateUpdate()
    {
        if (goPanel.activeSelf)
        {
            quizPanel.SetActive(false);
        }
    }

    private void Initialize()
    {
        QnA = new List<QandA>(originalQnA);
        UpdateMostWrongCountQuestionInDatabase();
        totalQuestion = QnA.Count;
        goPanel.SetActive(false);
        quizPanel.SetActive(false);
        startPanel.SetActive(true);
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

        SceneManagerClass.Instance.LoadNextScene();
    }

    public void Retry()
    {
        score = 0;
        questionNumber = 0;
        SceneManagerClass.Instance.ReloadScene();
    }

    public void ToMenu()
    {
        SceneManagerClass.Instance.LoadMenuScene();
    }

    public void StartTest()
    {
        quizPanel.SetActive(true);
        startPanel.SetActive(false);
        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
    }

    private void GameOver()
    {
        quizPanel.SetActive(false);
        goPanel.SetActive(true);
        finalscoreText.text = $"{score} / {totalQuestion}";

        if (SceneManager.GetActiveScene().name == "PostTest")
        {
            if (score >= minimumPassingScore)
            {
                minimumScoreText.GetComponent<TMP_Text>().text =
                    $"คะเเนนผ่านเกณฑ์";
                finalscoreText.color = Color.green;
                nextButton.SetActive(true);
                minimumScoreText.SetActive(true);
            }
            else
            {
                finalscoreText.color = Color.red;
                nextButton.SetActive(false);
                minimumScoreText.GetComponent<TMP_Text>().text =
                    $"ต้องได้คะแนนมากกว่า {minimumPassingScore} ถึงจะผ่านนะ";
                minimumScoreText.SetActive(true);
            }
        }
        else
        {
            finalscoreText.color = Color.black; 
            nextButton.SetActive(true); 
        }
    }

    public void Correct()
    {
        score += 1;
        scoreText.text = $"{score} / {totalQuestion}";
        correctPanel.SetActive(true);
    }

    public void Wrong()
    {
        IncrementWrongCount();
        corretOrNotText.text = "ผิดนะครับ";
        correctionText.text = QnA[currentQuestion].correction;
        correctionPanel.SetActive(true);
    }

    private void SetAnswer()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<TMP_Text>().text = QnA[currentQuestion].answers[i];
            if (QnA[currentQuestion].correctAnswers == i + 1)
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
            UpdateMostWrongCountQuestionInDatabase();
            GameOver();
        }
    }

    public void NextQuestion()
    {
        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
        QnA.RemoveAt(currentQuestion);
        GenerateQuestion();
    }

    private IEnumerator GetWrongCount(Action<int> onCallback)
    {
        string questionId = $"{QnA[currentQuestion].name}";

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
        }).Catch(error => { Debug.LogError("Error fetching wrong count: " + error.Message); });
    }

    private void IncrementWrongCount()
    {
        StartCoroutine(GetWrongCount(currentCount =>
        {
            string questionId = $"{QnA[currentQuestion].name}";
            int newCount = currentCount + 1;

            Question question = new Question(newCount);

            string url = $"{firebaseURL}/{questionId}.json";

            RestClient.Put(url, question).Then(response =>
            {
                Debug.Log($"Updated wrong count for {questionId}: {newCount}");
            }).Catch(error => { Debug.LogError($"Error updating wrong count: {error}"); });
        }));
    }

    public void UpdateMostWrongCountQuestionInDatabase()
    {
        StartCoroutine(FetchQuestionsAndUpdateMostWrongCount());
    }

    private IEnumerator FetchQuestionsAndUpdateMostWrongCount()
    {
        yield return RestClient.Get($"{firebaseURL}.json").Then(response =>
        {
            // Deserialize response into a dictionary
            Debug.Log("Response Text: " + response.Text);

            var jsonData = JSON.Parse(response.Text);

            if (jsonData == null)
            {
                Debug.LogError("Failed to parse JSON data.");
                return;
            }
            string maxWrongCountQuestionId = null;
            int maxWrongCount = -1;

            // Find the question with the highest wrong count
            foreach (var questionKey in jsonData.Keys)
            {
                int wrongCount = jsonData[questionKey]["wrongCount"].AsInt;

                if (wrongCount > maxWrongCount)
                {
                    maxWrongCount = wrongCount;
                    maxWrongCountQuestionId = questionKey;
                }
            }

            if (maxWrongCountQuestionId != null)
            {
                MostWrongQuestion mostWrong = new MostWrongQuestion(maxWrongCountQuestionId, maxWrongCount);
                string mostWrongURL = $"https://stomachstruggle-default-rtdb.asia-southeast1.firebasedatabase.app/mostWrongQuestion.json";

                RestClient.Put(mostWrongURL, mostWrong).Then(_ =>
                {
                    Debug.Log($"Updated most wrong question in database: {maxWrongCountQuestionId} with count {maxWrongCount}");
                }).Catch(error =>
                {
                    Debug.LogError($"Failed to update most wrong question: {error}");
                });
            }
            else
            {
                Debug.Log("No questions with wrong counts found.");
            }
        }).Catch(error =>
        {
            Debug.LogError("Error fetching questions: " + error.Message);
        });
    }
}

[Serializable]
public class MostWrongQuestion
{
    public string questionId;
    public int wrongCount;

    public MostWrongQuestion(string questionId, int wrongCount)
    {
        this.questionId = questionId;
        this.wrongCount = wrongCount;
    }
}
