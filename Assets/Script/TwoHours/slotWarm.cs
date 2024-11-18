using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slotWarm : MonoBehaviour
{
    [SerializeField] private int startHour = 14;
    [SerializeField] private int startMinute = 0;
    [SerializeField] private int endHour = 18;
    [SerializeField] private int endMinute = 0;

    public void OnDrop(dragFoodTwoH foodObject, FoodRandom foodRandom)
    {

        string[] timeParts = foodRandom.GetTimeText().Split(':');
        int foodHour = int.Parse(timeParts[0]);
        int foodMinute = int.Parse(timeParts[1]);

        bool isWithinTimeRange = foodRandom.IsTimeInRange(foodHour, foodMinute, startHour, startMinute, endHour, endMinute);

        if (isWithinTimeRange)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DecreaseHealth(1);
            }
        }
        else
        {
            SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
            GameManager.Instance.IncreaseScore(0);
        }

        Destroy(foodObject.gameObject);
    }
}
