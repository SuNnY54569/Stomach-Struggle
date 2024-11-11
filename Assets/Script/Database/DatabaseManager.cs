using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Proyecto26;
using UnityEngine.SceneManagement;

public class DatabaseManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("UI References")]
    [SerializeField, Tooltip("Displays the user's name")] 
    private TextMeshProUGUI nameText;
    [SerializeField, Tooltip("Displays the user's pre-test score")] 
    private TextMeshProUGUI preTestScoreText;
    [SerializeField, Tooltip("Displays the user's post-test score")]
    private TextMeshProUGUI postTestScoreText;
    [SerializeField, Tooltip("Displays the user's total remaining hearts")]
    private TextMeshProUGUI totalHeartText;
    [SerializeField, Tooltip("Input field for entering the user's name")]
    private TMP_InputField nameInput;
    #endregion
    
    #region Private Fields
    private string newUserKey;
    private string userID;
    private const string firebaseURL = "https://stomachstruggle-default-rtdb.asia-southeast1.firebasedatabase.app/users";
    #endregion
    
    #region Unity Lifecycle
    void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
    }
    #endregion

    #region Firebase Methods
    public void CreateUser()
    {
        if (nameInput == null || GameManager.Instance == null)
        {
            Debug.LogError("One of the references is null: " +
                           $"nameInput: {nameInput}, GameManager.Instance: {GameManager.Instance}");
            return;
        }
        
        newUserKey = Guid.NewGuid().ToString();

        int totalHeart = GameManager.Instance.GetSumTotalHeart();
        int totalHeartLeft = GameManager.Instance.GetSumTotalHeartLeft();
        
        User newUser = new User(nameInput.text, $"{GameManager.Instance.preTestScore}/10", $"{GameManager.Instance.postTestScore}/10", $"{totalHeartLeft}/{totalHeart}");
        
        RestClient.Put($"{firebaseURL}/{userID}/{newUserKey}.json", newUser).Then(response =>
        {
            Debug.Log("User created successfully with key: " + newUserKey);
        }).Catch(error =>
        {
            Debug.LogError("Error creating user: " + error.Message);
        });
    }

    private IEnumerator GetName(Action<string> onCallback)
    {
        RestClient.Get($"{firebaseURL}/{userID}/{newUserKey}/name.json").Then(response =>
        {
            onCallback?.Invoke(response.Text);
        }).Catch(error =>
        {
            Debug.LogError("Error fetching name: " + error.Message);
        });

        yield return null;
    }

    private IEnumerator GetPreTestScore(Action<string> onCallback)
    {
        yield return RestClient.Get($"{firebaseURL}/{userID}/{newUserKey}/preTestScore.json").Then(response =>
        {
            string score = response.Text;
            Debug.Log($"Fetched Pre-Test Score Response: {score}");
            onCallback?.Invoke(score != "null" ? score : "No Score Found");
        }).Catch(error =>
        {
            Debug.LogError("Error fetching pre-test score: " + error.Message);
        });

        yield return null;
    }

    private IEnumerator GetPostTestScore(Action<string> onCallback)
    {
        yield return RestClient.Get($"{firebaseURL}/{userID}/{newUserKey}/postTestScore.json").Then(response =>
        {
            string score = response.Text;
            Debug.Log($"Fetched Post-Test Score Response: {score}");
            onCallback?.Invoke(score != "null" ? score : "No Score Found");
        }).Catch(error =>
        {
            Debug.LogError("Error fetching post-test score: " + error.Message);
        });

        yield return null;
    }

    private IEnumerator GetTotalHeart(Action<string> onCallback)
    {
        yield return RestClient.Get($"{firebaseURL}/{userID}/{newUserKey}/totalHeart.json").Then(response =>
        {
            string totalHeart = response.Text;
            Debug.Log($"Fetched totalHeart Response: {totalHeart}");
            onCallback?.Invoke(totalHeart != "null" ? totalHeart : "No totalHeart Found");
        }).Catch(error =>
        {
            Debug.LogError("Error fetching totalHeart: " + error.Message);
        });

        yield return null;
    }
    #endregion

    #region Public Methods
    public void GetUserInfo()
    {
        StartCoroutine(GetName((string name) =>
        {
            nameText.text = $"Name: {name}";
        }));

        StartCoroutine(GetPreTestScore((string preTestScore) =>
        {
            preTestScoreText.text = $"Pre-Test Score: {preTestScore}";
            if (preTestScoreText.text == "No Score Found")
            {
                preTestScoreText.text = $"Pre-Test Score: {GameManager.Instance.preTestScore}/10";
                Debug.LogWarning("preTestScoreText = null");
            }
        }));

        StartCoroutine(GetPostTestScore((string postTestScore) =>
        {
            postTestScoreText.text = $"Post-Test Score: {postTestScore}";
            if (postTestScoreText.text == "No Score Found")
            {
                postTestScoreText.text = $"Post-Test Score: {GameManager.Instance.postTestScore}/10";
                Debug.LogWarning("postTestScoreText = null");
            }
        }));
        
        StartCoroutine(GetTotalHeart((string totalHeart) =>
        {
            totalHeartText.text = $"TotalHeart: {totalHeart}";
            if (totalHeartText.text == "No Score Found")
            {
                totalHeartText.text = $"TotalHeart: {GameManager.Instance.GetSumTotalHeartLeft()}/{GameManager.Instance.GetSumTotalHeart()}";
                Debug.LogWarning("preTestScoreText = null");
            }
        }));
    }

    public void ToMenu()
    {
        GameManager.Instance.ResetTotalHeart();
        GameManager.Instance.ResetPrePostTest();
        SceneManagerClass.Instance.LoadMenuScene();
    }
    
    #endregion
}
