using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class slotUi : MonoBehaviour, IDropHandler
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

                if (hour >= minHour && hour <= maxHour)
                {
                    ScoreGuitar.scoreValue += 1;
                }
                else
                {
                    playerHealth.DecreaseHealth(1);
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
