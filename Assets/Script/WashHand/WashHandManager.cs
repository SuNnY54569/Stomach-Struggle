using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class WashHandManager : MonoBehaviour
{
    public static WashHandManager Instance;
    
    [SerializeField,Tooltip("List of positions where the objects can be placed.")]
    private List<GameObject> positions;
    [SerializeField,Tooltip("Objects that need to be placed at random positions.")]
    private List<GameObject> objects;
    [SerializeField,Tooltip("The speed at which the objects move to their positions.")]
    private float moveSpeed = 2f;
    [SerializeField,Tooltip("The image in the center that plays animations.")]
    private GameObject centralImage;
    
    [SerializeField] private Animator centralAnimator;
    [SerializeField] private int currentObjectIndex = 1;
    private List<GameObject> startPositions = new List<GameObject>();

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
        
        GameManager.Instance.SetScoreTextActive(false);
    }

    public void StartGame()
    {
        if (positions.Count != objects.Count)
        {
            Debug.LogError("The number of positions and objects must be equal.");
            return;
        }
        
        List<GameObject> shuffledPositions = new List<GameObject>(positions);
        ShuffleList(shuffledPositions);
        
        for (int i = 0; i < objects.Count; i++)
        {
            startPositions.Add(shuffledPositions[i]);  // Store random start positions
            StartCoroutine(MoveObjectToPosition(objects[i], shuffledPositions[i]));
        }

        centralAnimator = centralImage.GetComponent<Animator>();
    }
    
    private void ShuffleList(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
    
    private IEnumerator MoveObjectToPosition(GameObject obj, GameObject targetPosition)
    {
        Collider2D objCollider = obj.GetComponent<Collider2D>();
        if (objCollider != null)
        {
            objCollider.enabled = false;
        }
        
        float elapsedTime = 0f;
        Vector3 startingPosition = obj.transform.position;

        while (elapsedTime < 1f)
        {
            obj.transform.position = Vector3.Lerp(startingPosition, targetPosition.transform.position, elapsedTime);
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;  // Wait for the next frame
        }
        
        obj.transform.position = targetPosition.transform.position;
        if (objCollider != null)
        {
            objCollider.enabled = true;
        }
    }
    
    public void OnObjectClicked(int objectIndex, string animationName, GameObject obj)
    {
        if (objectIndex == currentObjectIndex)
        {
            //centralAnimator.SetTrigger(animationName);
            
            obj.SetActive(false);
            foreach (var ob in objects)
            {
                ob.GetComponent<Collider2D>().enabled = false;
            }
            StartCoroutine(MoveToBondedPosition());
            currentObjectIndex++; 
        }
        else
        {
            GameManager.Instance.DecreaseHealth(1);
            foreach (var objectClick in objects)
            {
                var objectClickComponent = objectClick.GetComponent<ObjectClick>();
                if (objectClickComponent != null && objectClickComponent.objectIndex == currentObjectIndex)
                {
                    objectClickComponent.BlinkObject(); // Make the correct object blink
                    break;
                }
            }
        }
        
        if (currentObjectIndex >= objects.Count + 1)
        {
            StartCoroutine(WaitToWin());
        }
    }
    
    private IEnumerator MoveToBondedPosition()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject boundedPosition = startPositions[i].gameObject.GetComponent<PositionBond>().bondedPosition;
            StartCoroutine(MoveObjectToPosition(objects[i], boundedPosition));
        }
        
        //yield return new WaitForSeconds(centralAnimator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(3f);
        
        for (int i = 0; i < objects.Count; i++)
        {
            StartCoroutine(MoveObjectToPosition(objects[i], startPositions[i]));
        }
    }

    private IEnumerator WaitToWin()
    {
        //yield return new WaitForSeconds(centralAnimator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(3f);
        GameManager.Instance.WinGame();
    }
    
}
