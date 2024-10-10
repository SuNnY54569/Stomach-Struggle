using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WashHandManager : MonoBehaviour
{
    public static WashHandManager Instance;
    
    [Tooltip("List of positions where the objects can be placed.")]
    public List<Transform> positions;  // Positions for the objects
    [Tooltip("Objects that need to be placed at random positions.")]
    public List<GameObject> objects;   // Objects to be placed
    [Tooltip("The speed at which the objects move to their positions.")]
    public float moveSpeed = 2f;
    
    [SerializeField] private int currentObjectIndex = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        if (positions.Count != objects.Count)
        {
            Debug.LogError("The number of positions and objects must be equal.");
            return;
        }

        // Shuffle the positions
        List<Transform> shuffledPositions = new List<Transform>(positions);
        ShuffleList(shuffledPositions);

        // Assign each object to a unique position
        for (int i = 0; i < objects.Count; i++)
        {
            StartCoroutine(MoveObjectToPosition(objects[i], shuffledPositions[i].position));
        }
    }

    // Shuffle algorithm (Fisher-Yates Shuffle)
    private void ShuffleList(List<Transform> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
    
    private IEnumerator MoveObjectToPosition(GameObject obj, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = obj.transform.position;

        while (elapsedTime < 1f)
        {
            // Lerp movement
            obj.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime);

            // Increment elapsed time by the fraction of the moveSpeed
            elapsedTime += Time.deltaTime * moveSpeed;

            yield return null;  // Wait for the next frame
        }

        // Ensure final position is exactly the target position
        obj.transform.position = targetPosition;
    }
    
    public void OnObjectClicked(int objectIndex, GameObject obj)
    {
        if (objectIndex == currentObjectIndex)
        {
            // Correct object clicked, destroy it
            Destroy(obj);
            currentObjectIndex++;  // Move to the next object in the sequence
        }
        else
        {
            // Incorrect object clicked, handle it (e.g., decrease health or show a message)
            Debug.Log("Wrong object clicked!");
            // You can add your logic here for incorrect clicks (like losing health)
        }

        // Check if all objects are destroyed (win condition)
        if (currentObjectIndex >= objects.Count + 1)
        {
            Debug.Log("All objects destroyed, you win!");
            // Trigger any win logic here
        }
    }
}
