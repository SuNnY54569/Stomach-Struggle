using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class checkRawFood : MonoBehaviour, IDropHandler
{
    public string RawFoodTag = "RawFood";

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (eventData.pointerDrag != null)
        {
            GameObject draggedObject = eventData.pointerDrag;

            if (draggedObject.CompareTag(RawFoodTag))
            {
                draggedObject.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                Destroy(draggedObject);
            }
            else
            {
                draggedObject.GetComponent<RectTransform>().anchoredPosition = eventData.pointerDrag.GetComponent<DargDrop>().originalPosition;
            }
        }
    }
}
