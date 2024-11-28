using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slotCanEat : MonoBehaviour
{
    [SerializeField] private int startHour = 14;
    [SerializeField] private int startMinute = 0;
    [SerializeField] private int endHour = 18;
    [SerializeField] private int endMinute = 0;
    [SerializeField] private float scaleUpFactor = 1.2f; // Amount to scale up
    [SerializeField] private float animationDuration = 0.3f;

    public void OnDrop(dragFoodTwoH foodObject, FoodRandom foodRandom)
    {
        string[] timeParts = foodRandom.GetTimeText().Split(':');
        int foodHour = int.Parse(timeParts[0]);
        int foodMinute = int.Parse(timeParts[1]);

        bool isWithinTimeRange = foodRandom.IsTimeInRange(foodHour, foodMinute, startHour, startMinute, endHour, endMinute);

        if (isWithinTimeRange)
        {
            SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
        }
        else
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DecreaseHealth(1);
            }
        }

        ScaleAnimation();
    }
    
    private void ScaleAnimation()
    {
        Vector3 originalScale = gameObject.transform.localScale;
        Vector3 scaledUp = originalScale * scaleUpFactor;

        // Scale up and down using LeanTween
        LeanTween.scale(gameObject, scaledUp, animationDuration / 2)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(gameObject, originalScale, animationDuration / 2)
                    .setEase(LeanTweenType.easeInQuad);
            });
        
        LeanTween.rotateZ(gameObject, 5f, 0.15f)
            .setLoopPingPong(1);
    }
}
