using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlternateSlotUi : MonoBehaviour, IDropHandler
{
    [SerializeField] private float minHour;
    [SerializeField] private float maxHour;
    [SerializeField] private Health playerHealth;
    [SerializeField] private SpawnUIManager spawnUIManager;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

            TextMeshProUGUI timeText = eventData.pointerDrag.transform.Find("Time").GetComponent<TextMeshProUGUI>();

            if (timeText != null)
            {
                string[] timeParts = timeText.text.Split(':');
                float hour = float.Parse(timeParts[0]);

                if (hour < minHour || hour > maxHour)
                {
                    ScoreGuitar.scoreValue += 1;
                    Debug.Log("Time outside the range! Score +1");
                }
                else
                {
                    if (playerHealth != null)
                    {
                        playerHealth.DecreaseHealth(1);
                        Debug.Log("Time within the range! Health -1");
                    }
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
