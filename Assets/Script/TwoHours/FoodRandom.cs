using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodRandom : MonoBehaviour
{
    #region UI Components
    [Header("UI Components")]
    [SerializeField, Tooltip("Food Image")]
    private Image foodImage;

    [SerializeField, Tooltip("Time 00:00")]
    private TextMeshProUGUI timeText;


    #endregion

    #region Randomization Settings
    [Header("Randomization Settings")]
    [SerializeField, Tooltip("List of possible food images to spawn")]
    private Sprite[] foodSprites;

    [SerializeField, Tooltip("Minimum hour for random time (0-23)")]
    private int minHour;

    [SerializeField, Tooltip("Maximum hour for random time (0-23)")]
    private int maxHour;

    [SerializeField, Tooltip("Minimum minute for random time (0-59)")]
    private int minMinute;

    [SerializeField, Tooltip("Maximum minute for random time (0-59)")]
    private int maxMinute;

    #endregion

    private void Start()
    {
        RandomizeFoodAndTime();
    }

    public void RandomizeFoodAndTime()
    {
        if (foodSprites.Length > 0 && foodImage != null)
        {
            int randomIndex = UnityEngine.Random.Range(0, foodSprites.Length);
            foodImage.sprite = foodSprites[randomIndex];
        }

        int randomHour = UnityEngine.Random.Range(minHour, maxHour + 1);
        int randomMinute = UnityEngine.Random.Range(minMinute, maxMinute + 1);

        if (timeText != null)
        {
            string formattedTime = $"{randomHour:00}:{randomMinute:00}";
            timeText.text = formattedTime;
        }

        if (IsTimeInRange(randomHour, randomMinute, 14, 0, 18, 0))
        {
            gameObject.tag = "CanEat";
        }
        else
        {
            gameObject.tag = "WarmBeforeEat";
        }
    }

    private bool IsTimeInRange(int hour, int minute, int startHour, int startMinute, int endHour, int endMinute)
    {
        int timeInMinutes = hour * 60 + minute;
        int startTimeInMinutes = startHour * 60 + startMinute;
        int endTimeInMinutes = endHour * 60 + endMinute;

        return timeInMinutes >= startTimeInMinutes && timeInMinutes <= endTimeInMinutes;
    }

    public string GetTimeText() // New method to get the time text
    {
        if (timeText != null)
        {
            return timeText.text; // Return the current time text
        }
        return "00:00"; // Default return if something goes wrong
    }

}