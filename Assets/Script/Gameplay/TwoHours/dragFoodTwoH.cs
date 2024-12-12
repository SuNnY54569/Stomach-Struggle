using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class dragFoodTwoH : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private FoodRandom _foodRandom;
    private Vector3 _startPosition;
    private bool isDragging;
    
    private Collider2D[] _colliderBuffer = new Collider2D[10];
    private const float DragRadius = 0.1f;

    private void Awake()
    {
        _foodRandom = GetComponent<FoodRandom>();
        _startPosition = transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused) return;

        LeanTween.move(gameObject, MouseWorldPosition(eventData), 0.05f);
        isDragging = true;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused || !isDragging) return;
        
        Vector3 mouseWorldPosition = MouseWorldPosition(eventData);
        mouseWorldPosition.z = transform.position.z;
        transform.position = mouseWorldPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.Instance.isGamePaused) return;
        
        isDragging = false;
        HandleDrop();
    }
    
    private void HandleDrop()
    {
        bool droppedInSlot = false;

        int colliderCount = Physics2D.OverlapCircleNonAlloc(transform.position, DragRadius, _colliderBuffer);

        for (int i = 0; i < colliderCount; i++)
        {
            var collider = _colliderBuffer[i];
            slotCanEat slotEat = collider.GetComponent<slotCanEat>();
            slotWarm slotWarm = collider.GetComponent<slotWarm>();

            if (slotEat != null)
            {
                slotEat.OnDrop(this, _foodRandom);
                droppedInSlot = true;
                OnDropInSlot(slotEat.transform);
                break;
            }
            else if (slotWarm != null)
            {
                slotWarm.OnDrop(this, _foodRandom);
                droppedInSlot = true;
                OnDropInSlot(slotWarm.transform);
                break;
            }
        }

        // If not dropped in any valid slot, return to starting position
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
                AnimateFoodDestruction();
            }));
    }
    
    private void AnimateFoodDestruction()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => { Destroy(gameObject); });

        LeanTween.rotateZ(gameObject, 5f, 0.15f)
            .setLoopPingPong(1);
    }
    
    private Vector3 MouseWorldPosition(PointerEventData eventData)
    {
        Vector3 inputPosition = eventData.position;
        return Camera.main.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, transform.position.z - Camera.main.transform.position.z));
    }
}
