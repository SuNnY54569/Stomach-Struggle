using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragFoodTwoH : MonoBehaviour
{
    private DragControllerGuitar _dragController;
    private bool _isDragging;
    private FoodRandom _foodRandom;
    private Vector3 _startPosition;

    private void Awake()
    {
        _dragController = FindObjectOfType<DragControllerGuitar>();
        _foodRandom = GetComponent<FoodRandom>();
        _startPosition = transform.position;
    }

    private void Update()
    {
        if (GameManager.Instance.isGamePaused) return;
        
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
        if (GameManager.Instance.isGamePaused) return;
        _isDragging = true;
    }

    private void Drop()
    {
        _isDragging = false;
        bool droppedInSlot = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (Collider2D collider in colliders)
        {

            slotCanEat slotEat = collider.GetComponent<slotCanEat>();
            slotWarm slotWarm = collider.GetComponent<slotWarm>();
            if (slotEat != null)
            {
                FoodRandom foodRandom = GetComponent<FoodRandom>();
                slotEat.OnDrop(this, foodRandom);
                droppedInSlot = true;
                OnDropInSlot(slotEat.transform);
                break;
            }
            else if (slotWarm != null)
            {
                FoodRandom foodRandom = GetComponent<FoodRandom>();
                slotWarm.OnDrop(this, foodRandom);
                droppedInSlot = true;
                OnDropInSlot(slotWarm.transform);
                break;
            }
        }
        if (!droppedInSlot)
        {
            LeanTween.move(gameObject, _startPosition, 0.3f).setEase(LeanTweenType.easeInOutQuad);
        }
    }

    private void OnDropInSlot(Transform slot)
    {
        LeanTween.move(gameObject, slot.position, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete((() =>
            {
                transform.position = slot.position;
                LeanTween.scale(gameObject, Vector3.zero, 0.3f)
                    .setEase(LeanTweenType.easeInOutQuad)
                    .setOnComplete(() =>
                    {
                        Destroy(gameObject);
                    });
                LeanTween.rotateZ(gameObject, 5f, 0.15f)
                    .setLoopPingPong(1);
            }));
    }
}
