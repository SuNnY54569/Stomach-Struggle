using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SteakProgressBar : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private RectTransform arrowIndicator; // The arrow that points to the progress
    [SerializeField] private Image progressBarBackground;  // Background image of the progress bar
    private Steak currentlyCookingSteak;

    private void Update()
    {
        // Get the currently cooking steak
        currentlyCookingSteak = Tools.Instance.currentlyCookingSteak;

        if (currentlyCookingSteak != null)
        {
            UpdateProgressIndicator();
        }
        else
        {
            arrowIndicator.gameObject.SetActive(false); // Hide the arrow if no steak is being cooked
        }
    }

    private void UpdateProgressIndicator()
    {
        arrowIndicator.gameObject.SetActive(true); // Show the arrow

        float cookingProgress = currentlyCookingSteak.GetTotalCookingProgress(); // Get progress as a 0-1 value
        float barWidth = progressBarBackground.rectTransform.rect.width;

        // Calculate the arrow's new position based on progress
        Vector2 newPosition = arrowIndicator.anchoredPosition;
        newPosition.x = cookingProgress * barWidth;
        arrowIndicator.anchoredPosition = newPosition;
    }
}
