using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnButton : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject[] objectsToClose;
    [SerializeField] private GameObject[] objectsToOpen;
    private void Awake()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }
    
    private void OnMouseOver()
    {
        sprite.color = Color.gray;
    }

    private void OnMouseExit()
    {
        sprite.color = Color.white;
    }
    
    private void OnMouseDown()
    {
        foreach (var objectToClose in objectsToClose)
        {
            objectToClose.SetActive(false);
        }
        foreach (var objectToOpen in objectsToOpen)
        {
            objectToOpen.SetActive(true);
        }
    }
}
