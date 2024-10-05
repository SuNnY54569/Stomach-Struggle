using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slotCanEat : MonoBehaviour
{
    public void OnDrop(dragFoodTwoH draggedFood, FoodRandom foodRandom)
    {
        string timeText = foodRandom.GetTimeText();
        int hour = int.Parse(timeText.Split(':')[0]);
        int minute = int.Parse(timeText.Split(':')[1]);

        if (IsTimeInRange(hour, minute, 14, 0, 18, 0))
        {
            ScoreGuitar.scoreValue += 1;
        }
        else
        {
            Health playerHealth = FindObjectOfType<Health>();
            if (playerHealth != null)
            {
                playerHealth.DecreaseHealth(1);
            }
        }

        Destroy(draggedFood.gameObject);
    }

    private bool IsTimeInRange(int hour, int minute, int startHour, int startMinute, int endHour, int endMinute)
    {
        int timeInMinutes = hour * 60 + minute;
        int startTimeInMinutes = startHour * 60 + startMinute;
        int endTimeInMinutes = endHour * 60 + endMinute;

        return timeInMinutes >= startTimeInMinutes && timeInMinutes <= endTimeInMinutes;
    }
}
