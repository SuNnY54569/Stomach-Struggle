using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI preTestScoreText;
    [SerializeField] private TextMeshProUGUI postTestScoreText;
    [SerializeField] private TMP_InputField nameInput;
    
    string newUserKey;
    private string userID;
    private DatabaseReference dbReference;
    
    void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    
        if (dbReference == null)
        {
            Debug.LogError("Database reference not initialized!");
        }
    }

    public void CreateUser()
    {
        if (nameInput == null || GameManager.Instance == null || dbReference == null)
        {
            Debug.LogError("One of the references is null: " +
                           $"nameInput: {nameInput}, GameManager.Instance: {GameManager.Instance}, dbReference: {dbReference}");
            return;
        }
        
        User newUser = new User(nameInput.text, $"{GameManager.Instance.preTestScore}/10", $"{GameManager.Instance.postTestScore}/10");
        string json = JsonUtility.ToJson(newUser);

        newUserKey = dbReference.Child("users").Push().Key;
        dbReference.Child("users").Child(userID).Child(newUserKey).SetRawJsonValueAsync(json);
    }

    public IEnumerator GetName(Action<string> onCallback)
    {
        var userNameData = dbReference.Child("users").Child(userID).Child(newUserKey).Child("name").GetValueAsync();

        yield return new WaitUntil(predicate: () => userNameData.IsCompleted);

        if (userNameData != null)
        {
            DataSnapshot snapshot = userNameData.Result;
            
            onCallback.Invoke(snapshot.Value.ToString());
        }
    }
    
    public IEnumerator GetPreTestScore(Action<string> onCallback)
    {
        var userPreTestScoreData = dbReference.Child("users").Child(userID).Child(newUserKey).Child("preTestScore").GetValueAsync();

        yield return new WaitUntil(predicate: () => userPreTestScoreData.IsCompleted);

        if (userPreTestScoreData != null)
        {
            DataSnapshot snapshot = userPreTestScoreData.Result;
            
            onCallback.Invoke(snapshot.Value.ToString());
        }
    }
    
    public IEnumerator GetPostTestScore(Action<string> onCallback)
    {
        var userPostTestScoreData = dbReference.Child("users").Child(userID).Child(newUserKey).Child("postTestScore").GetValueAsync();

        yield return new WaitUntil(predicate: () => userPostTestScoreData.IsCompleted);

        if (userPostTestScoreData != null)
        {
            DataSnapshot snapshot = userPostTestScoreData.Result;
            
            onCallback.Invoke(snapshot.Value.ToString());
        }
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

        }));
        
        StartCoroutine(GetPostTestScore((string postTestScore) =>
        {
            postTestScoreText.text = $"Post-Test Score: {postTestScore}";

        }));
    }
}
