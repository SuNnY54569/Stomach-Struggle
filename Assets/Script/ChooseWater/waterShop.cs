using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class waterShop : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject[] objectsToClose;
    [SerializeField] private GameObject[] objectsToOpen;
    [SerializeField] private bool isReturnButton;


    private void OnMouseOver()
    {
        if (GameManager.Instance.isGamePaused) return;
        sprite.color = Color.gray;
    }

    private void OnMouseExit()
    {
        if (GameManager.Instance.isGamePaused) return;
        sprite.color = Color.white;
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;
        foreach (var objectToClose in objectsToClose)
        {
            objectToClose.SetActive(false);
        }
        foreach (var objectToOpen in objectsToOpen)
        {
            objectToOpen.SetActive(true);
        }

        sprite.color = Color.white;
    }
}
