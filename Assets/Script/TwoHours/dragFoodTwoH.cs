using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragFoodTwoH : MonoBehaviour
{
    private DragControllerGuitar _dragController;
    private bool _isDragging;
    private FoodRandom _foodRandom;

    private void Awake()
    {
        _dragController = FindObjectOfType<DragControllerGuitar>();
        _foodRandom = GetComponent<FoodRandom>();
    }

    private void Update()
    {
        if (_isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
            transform.position = new Vector2(worldPosition.x, worldPosition.y);
        }

        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            Drop();
        }
    }

    private void OnMouseDown()
    {
        _isDragging = true;
    }

    private void Drop()
    {
        _isDragging = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (Collider2D collider in colliders)
        {
            Debug.Log("Collider found: " + collider.gameObject.name);

            slotCanEat slotEat = collider.GetComponent<slotCanEat>();
            slotWarm slotWarm = collider.GetComponent<slotWarm>();
            if (slotEat != null)
            {
                FoodRandom foodRandom = GetComponent<FoodRandom>();
                slotEat.OnDrop(this, foodRandom);
                break;
            }
            else if (slotWarm != null)
            {
                FoodRandom foodRandom = GetComponent<FoodRandom>();
                slotWarm.OnDrop(this, foodRandom);
                break;
            }


        }
    }
}
