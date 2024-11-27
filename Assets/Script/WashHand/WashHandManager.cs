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
    
    #region Serialized Fields with Tooltips

    [Header("Position and Object Settings")]
    [SerializeField, Tooltip("List of positions where the objects can be placed.")]
    private List<GameObject> positions;

    [SerializeField, Tooltip("Objects that need to be placed at random positions.")]
    private List<GameObject> objects;

    [Header("Movement Settings")]
    [SerializeField, Tooltip("The speed at which the objects move to their positions.")]
    private float moveSpeed = 2f;

    [Header("Central Image Settings")]
    [SerializeField, Tooltip("The image in the center that plays animations.")]
    private GameObject centralImage;

    [SerializeField, Tooltip("Animator component for the central image.")]
    private Animator centralAnimator;

    [Header("Game Settings")]
    [SerializeField, Tooltip("Index of the current object the player needs to click.")]
    private int currentObjectIndex = 1;

    [Header("UI Setting")] 
    [SerializeField, Tooltip("Start Button")]
    private GameObject startButton;

    [SerializeField, Tooltip("Warning Text")]
    private GameObject warningText;

    [SerializeField, Tooltip("Hand Gameobject")]
    private GameObject hand;

    [SerializeField, Tooltip("Canvas")] 
    private GameObject canvas;
    
    #endregion
    
    #region Private Fields

    private List<GameObject> startPositions = new List<GameObject>();
    private bool isBlinking;
    private bool isAppear;
    private bool isPause;
    private bool isUnpause = true;
    private bool isPopUpComplete;

    #endregion

    #region Unity Lifecycle
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
        
        UITransitionUtility.Instance.Initialize(startButton, Vector2.zero);
        UITransitionUtility.Instance.Initialize(warningText, Vector2.zero);
        GameManager.Instance.SetScoreTextActive(false);
    }

    private void Start()
    {
        GameManager.Instance.SetScoreTextActive(false);
        CloseAllColliders();
    }

    private void Update()
    {
        HandlePopUp();
        HandleObjectBlinking();
        HandlePauseState();
    }
    
    #endregion
    
    #region Game Start and Setup
    public void StartGame()
    {
        if (GameManager.Instance.isGamePaused) return;
        
        if (positions.Count != objects.Count)
        {
            Debug.LogError("The number of positions and objects must be equal.");
            return;
        }
        
        startPositions.Clear();
        List<GameObject> shuffledPositions = new List<GameObject>(positions);
        ShuffleList(shuffledPositions);
        
        for (int i = 0; i < objects.Count; i++)
        {
            startPositions.Add(shuffledPositions[i]);
            StartCoroutine(MoveObjectToPosition(objects[i], shuffledPositions[i]));
        }

        UITransitionUtility.Instance.PopDown(startButton);
        UITransitionUtility.Instance.PopDown(warningText);
        
        centralImage.SetActive(true);
        centralImage.transform.localScale = Vector3.zero;
        LeanTween.scale(centralImage, new Vector3(0.25f,0.25f,0.25f), 0.5f)
            .setEase(LeanTweenType.easeOutBack);
        
        LeanTween.scale(hand, Vector3.zero, 0.5f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() => { hand.SetActive(false); });
        
        SoundManager.PlaySound(SoundType.UIClick, VolumeType.SFX);
        centralAnimator = centralImage.GetComponent<Animator>();
    }
    #endregion
    
    #region Object Positioning and Movement
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
        objCollider?.Disable();

        float elapsedTime = 0f;
        Vector3 startingPosition = obj.transform.position;
        Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f);

        while (elapsedTime < 1f)
        {
            float t = elapsedTime * moveSpeed;
            obj.transform.position = Vector3.Lerp(startingPosition, targetPosition.transform.position, t);
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = targetPosition.transform.position;
        obj.transform.localScale = targetScale;

        objCollider?.Enable();
    }
    #endregion
    
    #region Game Actions
    public void OnObjectClicked(int objectIndex, string animationName, GameObject obj, Animator animator)
    {
        if (objectIndex == currentObjectIndex)
        {
            centralAnimator.SetTrigger(animationName);
            animator.SetTrigger("Explode");
            SoundManager.PlaySound(SoundType.BBExpolde, VolumeType.SFX);

            CloseAllColliders();
            StartCoroutine(WaitForAnimation(animator, "Explode", MoveToBondedPosition, obj));
            currentObjectIndex++;
        }
        else
        {
            GameManager.Instance.DecreaseHealth(1);
        }

        if (currentObjectIndex > objects.Count)
        {
            StartCoroutine(WaitToWin());
        }
    }
    #endregion
    
    #region Animation and Coroutines
    private IEnumerator WaitForAnimation(Animator animator, string animationName, Func<IEnumerator> coroutineToStart, GameObject obj)
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        yield return StartCoroutine(coroutineToStart());
        obj.SetActive(false);
    }
    
    private IEnumerator MoveToBondedPosition()
    {
        
        // Step 1: Move objects to their bonded positions simultaneously
        List<Coroutine> coroutines = new List<Coroutine>();
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject bondedPosition = startPositions[i].GetComponent<PositionBond>().bondedPosition;
            coroutines.Add(StartCoroutine(MoveObjectToPosition(objects[i], bondedPosition)));
        }

        // Wait for all coroutines to finish
        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }

        // Wait for a delay after all objects have reached their bonded positions
        yield return new WaitForSeconds(3f);

        // Step 2: Move objects back to their start positions simultaneously
        coroutines.Clear();
        for (int i = 0; i < objects.Count; i++)
        {
            coroutines.Add(StartCoroutine(MoveObjectToPosition(objects[i], startPositions[i])));
        }

        // Wait for all coroutines to finish
        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }

        isBlinking = false;
    }

    private IEnumerator WaitToWin()
    {
        yield return new WaitForSeconds(3f);
        GameManager.Instance.WinGame();
    }
    #endregion
    
    #region Helper Methods

    private void HandleObjectBlinking()
    {
        if (GameManager.Instance.currentHealth > 1)
        {
            isBlinking = false;
            return;
        }
        
        if (isBlinking) return;
        foreach (var objectClick in objects)
        {
            var objectClickComponent = objectClick.GetComponent<ObjectClick>();
            if (objectClickComponent != null && objectClickComponent.objectIndex == currentObjectIndex)
            {
                objectClickComponent.BlinkObject();
                isBlinking = true;
                break;
            }
        }
    }

    private void CloseAllColliders()
    {
        foreach (var obj in objects)
        {
            if (obj.TryGetComponent<Collider2D>(out Collider2D collider))
            {
                collider.enabled = false;
            }
        }
    }

    public void OpenAllColliders()
    {
        foreach (var obj in objects)
        {
            var collider = obj.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
                collider.isTrigger = true;
            }
        }
    }

    private IEnumerator PopUpUI()
    {
        UITransitionUtility.Instance.PopUp(warningText);
        yield return new WaitForSeconds(2f);
        UITransitionUtility.Instance.PopUp(startButton);
    }
    
    private void HandlePopUp()
    {
        if (!GameManager.Instance.tutorialPanel.activeSelf && !isAppear)
        {
            StartCoroutine(PopUpUI());
            hand.SetActive(true);
            isPopUpComplete = true;
            hand.transform.localScale = Vector3.zero;
            LeanTween.scale(hand, new Vector3(0.64f, 0.64f, 0.64f), 0.5f)
                .setEase(LeanTweenType.easeOutBack);
            isAppear = true;
        }
    }
    
    private void HandlePauseState()
    {
        if (GameManager.Instance.isGamePaused && isPopUpComplete)
        {
            if (!isPause)
            {
                UITransitionUtility.Instance.PopDown(startButton,LeanTweenType.easeInBack, 0.2f);
                UITransitionUtility.Instance.PopDown(warningText,LeanTweenType.easeInBack, 0.2f);
                isPause = true;
                isUnpause = false;
            }
        }
        else if (!GameManager.Instance.isGamePaused && isPopUpComplete)
        {
            if (!isUnpause)
            {
                UITransitionUtility.Instance.PopUp(warningText);
                UITransitionUtility.Instance.PopUp(startButton);
                isUnpause = true;
                isPause = false;
            }
        }
    }

    #endregion
}

public static class ColliderExtensions
{
    public static void Disable(this Collider2D collider) => collider.enabled = false;
    public static void Enable(this Collider2D collider) => collider.enabled = true;
}
