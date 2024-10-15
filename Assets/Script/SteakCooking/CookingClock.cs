using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CookingClock : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text cookingTimeText; // For Unity UI
    // [SerializeField] private TextMeshProUGUI cookingTimeText; // For TextMeshPro
    private Steak currentlyCookingSteak;

    // Update is called once per frame
    void Update()
    {
        currentlyCookingSteak = Tools.Instance.currentlyCookingSteak;

        if (currentlyCookingSteak != null)
        {
            UpdateCookingTime();
        }
        else
        {
            cookingTimeText.text = "00:00";
        }
    }
    
    private void UpdateCookingTime()
    {
        if (currentlyCookingSteak != null)
        {
            float timeElapsed = currentlyCookingSteak.CookingTimeElapsed();
            
            int minutes = Mathf.FloorToInt(timeElapsed); // Treat elapsed seconds as minutes
            int seconds = 0;
            cookingTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            cookingTimeText.text = "00:00";
        }
    }
}
