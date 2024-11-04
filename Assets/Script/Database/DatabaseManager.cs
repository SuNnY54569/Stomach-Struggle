using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Proyecto26;

public class DatabaseManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI preTestScoreText;
    [SerializeField] private TextMeshProUGUI postTestScoreText;
    [SerializeField] private TMP_InputField nameInput;
    
    string newUserKey;
    private string userID;
    private string firebaseURL = "https://stomachstruggle-default-rtdb.asia-southeast1.firebasedatabase.app/users";
    
    void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
    }

    public void CreateUser()
    {
        if (nameInput == null || GameManager.Instance == null)
        {
            Debug.LogError("One of the references is null: " +
                           $"nameInput: {nameInput}, GameManager.Instance: {GameManager.Instance}");
            return;
        }
        
        newUserKey = Guid.NewGuid().ToString();
        
        User newUser = new User(nameInput.text, $"{GameManager.Instance.preTestScore}/10", $"{GameManager.Instance.postTestScore}/10");
        
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
    }
}