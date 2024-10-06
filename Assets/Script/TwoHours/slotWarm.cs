using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slotWarm : MonoBehaviour
{
    [SerializeField] private int startHour = 14;
    [SerializeField] private int startMinute = 0;
    [SerializeField] private int endHour = 16;
    [SerializeField] private int endMinute = 0;

    public void OnDrop(dragFoodTwoH foodObject, FoodRandom foodRandom)
    {
        Debug.Log("slotWarm OnDrop called");

        string[] timeParts = foodRandom.GetTimeText().Split(':');
        int foodHour = int.Parse(timeParts[0]);
        int foodMinute = int.Parse(timeParts[1]);

        bool isWithinTimeRange = foodRandom.IsTimeInRange(foodHour, foodMinute, startHour, startMinute, endHour, endMinute);

        Health playerHealth = FindObjectOfType<Health>();

        if (isWithinTimeRange)
        {
            if (playerHealth != null)
            {
                playerHealth.DecreaseHealth(1);
            }
        }
        else
        {
            ScoreGuitar.scoreValue += 1;
        }

        Destroy(foodObject.gameObject);
    }
}
