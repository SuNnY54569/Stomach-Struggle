using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlternateSlotUi : MonoBehaviour, IDropHandler
{
    [SerializeField] private int minHour;
    [SerializeField] private int maxHour;
    [SerializeField] private int minMinute;
    [SerializeField] private int maxMinute;

    [SerializeField] private Health playerHealth;
    [SerializeField] private SpawnUIManager spawnUIManager;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

            TextMeshProUGUI timeText = eventData.pointerDrag.transform.Find("Time").GetComponent<TextMeshProUGUI>();

            if (timeText != null)
            {
                string[] timeParts = timeText.text.Split(':');
                int hour = int.Parse(timeParts[0]);
                int minute = int.Parse(timeParts[1]);

                if ((hour > minHour || (hour == minHour && minute >= minMinute)) && (hour < maxHour || (hour == maxHour && minute <= maxMinute)))
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

                Destroy(eventData.pointerDrag.gameObject);

                if (spawnUIManager != null)
                {
                    spawnUIManager.SpawnRandomUI();
                }
            }
        }
    }
}
