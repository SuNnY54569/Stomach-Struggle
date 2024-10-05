using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class slotUi : MonoBehaviour, IDropHandler
{
    [SerializeField] private float minHour;
    [SerializeField] private float maxHour;
    [SerializeField] private float minMinute;
    [SerializeField] private float maxMinute;
    
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
                float hour = float.Parse(timeParts[0]);
                int minute = int.Parse(timeParts[1]);

                if ((hour > minHour || (hour == minHour && minute >= minMinute)) && (hour < maxHour || (hour == maxHour && minute <= maxMinute)))
                {
                    GameManager.Instance.IncreaseScore(1);
                }
                else
                {
                    GameManager.Instance.DecreaseHealth(1);
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
