using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClick : MonoBehaviour
{
    [SerializeField] private int objectIndex; // The order of this object
    [SerializeField] private GameObject picture;
    [SerializeField] private float moveDistance = 0.5f;  // Distance to move up and down
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float extraDownDistance = 0.2f;
    
    private Vector3 originalPosition;
    
    private void Start()
    {
        originalPosition = picture.transform.localPosition; // Store the starting position
        StartCoroutine(MoveUpAndDownLoop());
    }

    private void OnMouseDown()
    {
        // Notify the manager when the object is clicked
        WashHandManager.Instance.OnObjectClicked(objectIndex, gameObject);
    }
    
    private IEnumerator MoveUpAndDownLoop()
    {
        while (true)
        {
            // Move up
            Vector3 targetPosition = originalPosition + Vector3.up * moveDistance; // Move up
            float elapsedTime = 0f;

            while (elapsedTime < 1f)
            {
                picture.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, elapsedTime);
                elapsedTime += Time.deltaTime * moveSpeed;
                yield return null;
            }

            /*// Move back down (extra down distance)
            Vector3 downTargetPosition = originalPosition - Vector3.up * extraDownDistance; // Additional down position
            elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                picture.transform.localPosition = Vector3.Lerp(targetPosition, downTargetPosition, elapsedTime);
                elapsedTime += Time.deltaTime * moveSpeed;
                yield return null;
            }*/

            // Return to original position
            elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                picture.transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, elapsedTime);
                elapsedTime += Time.deltaTime * moveSpeed;
                yield return null;
            }
        }
    }
    
    
}
