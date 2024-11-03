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
    private bool isBlinking = false;

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

    private void Start()
    {
        GameManager.Instance.SetScoreTextActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.currentHealth > 1)
        {
            isBlinking = false; // Reset when health is more than 1
            return;
        }
        
        if (isBlinking) return;
        foreach (var objectClick in objects)
        {
            var objectClickComponent = objectClick.GetComponent<ObjectClick>();
            if (objectClickComponent != null && objectClickComponent.objectIndex == currentObjectIndex)
            {
                objectClickComponent.BlinkObject(); // Make the correct object blink
                isBlinking = true; // Set to true so it doesn't repeatedly trigger
                break;
            }
        }
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
        Vector3 startingScale = obj.transform.localScale;
        Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f);

        while (elapsedTime < 1f)
        {
            obj.transform.position = Vector3.Lerp(startingPosition, targetPosition.transform.position, elapsedTime);
            obj.transform.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime);
            
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;  // Wait for the next frame
        }
        
        obj.transform.position = targetPosition.transform.position;
        obj.transform.localScale = targetScale;
        
        if (objCollider != null)
        {
            objCollider.enabled = true;
        }
    }
    
    public void OnObjectClicked(int objectIndex, string animationName, GameObject obj, Animator animator)
    {
        if (objectIndex == currentObjectIndex)
        {
            centralAnimator.SetTrigger(animationName);
            animator.SetTrigger("Explode");
            foreach (var ob in objects)
            {
                ob.GetComponent<Collider2D>().enabled = false;
            }
            StartCoroutine(WaitForAnimation(animator, "Explode", MoveToBondedPosition, obj));
            currentObjectIndex++;
        }
        else
        {
            GameManager.Instance.DecreaseHealth(1);
        }
        
        if (currentObjectIndex >= objects.Count + 1)
        {
            StartCoroutine(WaitToWin());
        }
    }
    
    private IEnumerator WaitForAnimation(Animator animator, string animationName, Func<IEnumerator> coroutineToStart, GameObject obj)
    {
        // Wait until the current animation state is the one specified
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            yield return null;
        }

        // Wait until the animation has finished playing
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        // Start the specified coroutine after the animation has finished
        yield return StartCoroutine(coroutineToStart());
        obj.SetActive(false);
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
        isBlinking = false;
    }

    private IEnumerator WaitToWin()
    {
        //yield return new WaitForSeconds(centralAnimator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(3f);
        GameManager.Instance.WinGame();
    }
    
}
