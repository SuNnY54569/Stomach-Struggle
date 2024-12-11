using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class WashHandManager : MonoBehaviour
{
    
    [Serializable]
    public class BondedPosition
    {
        public Transform position;
        public Transform bondedPosition;
    }
    
    public static WashHandManager Instance;
    
    #region Serialized Fields with Tooltips
    [Header("Position and Object Settings")]

    public List<BondedPosition> bondedPositions;
    
    [SerializeField, Tooltip("Objects that need to be placed at random positions.")]
    private List<GameObject> objects;

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
    private List<BondedPosition> startPositions = new List<BondedPosition>();
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
    }

    private void Start()
    {
        CloseAllColliders();
    }

    private void Update()
    {
        HandlePopUp();
        HandleObjectBlinking();
    }
    
    private void OnEnable()
    {
        GameManager.OnGamePaused += HandleGamePaused;
        GameManager.OnGameUnpaused += HandleGameUnpaused;
    }

    private void OnDisable()
    {
        GameManager.OnGamePaused -= HandleGamePaused;
        GameManager.OnGameUnpaused -= HandleGameUnpaused;
    }
    #endregion
    
    #region Game Start and Setup
    public void StartGame()
    {
        if (GameManager.Instance.isGamePaused) return;
        
        if (bondedPositions.Count != objects.Count)
        {
            Debug.LogError("The number of positions and objects must be equal.");
            return;
        }
        
        startPositions.Clear();
        List<BondedPosition> shuffledPositions = new List<BondedPosition>();
        for (int i = 0; i < bondedPositions.Count; i++)
        {
            
            shuffledPositions.Add(bondedPositions[i]);
        }
        ShuffleList(shuffledPositions);

        for (int i = 0; i < objects.Count; i++)
        {
            startPositions.Add(shuffledPositions[i]);
            MoveObjectToPosition(objects[i], shuffledPositions[i].position);
            objects[i].GetComponent<ObjectClick>().MovingObjectWithLeanTween();
        }

        StartCoroutine(PopDownAfterStart());
        
        centralImage.SetActive(true);
        centralImage.transform.localScale = Vector3.zero;
        LeanTween.scale(centralImage, new Vector3(0.25f,0.25f,0.25f), 0.5f)
            .setEase(LeanTweenType.easeOutBack);
        
        LeanTween.scale(hand, Vector3.zero, 0.5f)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() =>
            {
                hand.SetActive(false);
            });
        
        SoundManager.PlaySound(SoundType.UIClick, VolumeType.SFX);
        centralAnimator = centralImage.GetComponent<Animator>();
    }
    #endregion
    
    #region Object Positioning and Movement Methods
    private void ShuffleList(List<BondedPosition> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
    
    private void MoveObjectToPosition(GameObject obj, Transform targetPosition)
    {
        Collider2D objCollider = obj.GetComponent<Collider2D>();
        objCollider?.Disable();

        // Use LeanTween to move the object to the target position and scale it
        LeanTween.move(obj, targetPosition.position, 1f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => 
            {
                // Once the movement is complete, scale the object
                LeanTween.scale(obj, new Vector3(1.5f, 1.5f, 1.5f), 1f)
                    .setEase(LeanTweenType.easeInOutQuad)
                    .setOnComplete(() => objCollider?.Enable());
            });
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
            GameManager.Instance.healthManager.DecreaseHealth(1);
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
        float timeout = 1.5f; // Maximum time to wait
        float timer = 0f;

        yield return new WaitUntil(() =>
        {
            timer += Time.deltaTime;
            return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) || timer >= timeout;
        });

        if (timer < timeout)
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            obj.SetActive(false);
            yield return StartCoroutine(coroutineToStart());
        }
        else
        {
            Debug.LogWarning($"Animation '{animationName}' timed out.");
        }
    }
    
    private IEnumerator MoveToBondedPosition()
    {
        // Step 1: Move objects to their bonded positions simultaneously
        List<LTDescr> tweens = new List<LTDescr>();
        for (int i = 0; i < objects.Count; i++)
        {
            Vector3 bondedPosition = startPositions[i].bondedPosition.position;

            // Disable collider during movement
            Collider2D objCollider = objects[i].GetComponent<Collider2D>();
            objCollider?.Disable();

            // Use LeanTween to move the object and scale it
            tweens.Add(LeanTween.move(objects[i], bondedPosition, 3f)
                .setEase(LeanTweenType.easeInOutQuad));
            tweens.Add(LeanTween.scale(objects[i], new Vector3(1f, 1f, 1f), 3f)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() => objCollider?.Enable()));
        }

        // Wait for a delay after all objects have reached their bonded positions
        yield return new WaitForSeconds(3f);

        tweens.Clear();
        for (int i = 0; i < objects.Count; i++)
        {
            Collider2D objCollider = objects[i].GetComponent<Collider2D>();
            objCollider?.Disable();

            // Use LeanTween to move the object and reset its scale
            tweens.Add(LeanTween.move(objects[i], startPositions[i].position.position, 3f)
                .setEase(LeanTweenType.easeInOutQuad));
            tweens.Add(LeanTween.scale(objects[i], new Vector3(1.5f, 1.5f, 1.5f), 3f)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() => objCollider?.Enable()));
        }

        // Wait for all tweens to complete
        yield return new WaitForSeconds(3f);

        isBlinking = false;
    }

    private IEnumerator WaitToWin()
    {
        yield return new WaitForSeconds(3f);
        centralAnimator.SetTrigger("WashWater");
        yield return new WaitForSeconds(3f);
        GameManager.Instance.healthManager.WinGame();
    }
    #endregion
    
    #region Helper Methods

    private void HandleObjectBlinking()
    {
        if (GameManager.Instance.healthManager.currentHealth > 1)
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

    private IEnumerator PopDownAfterStart()
    {
        UITransitionUtility.Instance.PopDown(startButton);
        UITransitionUtility.Instance.PopDown(warningText);

        yield return new WaitForSecondsRealtime(0.75f);
        
        Destroy(startButton);
        Destroy(warningText);
    }
    
    private void HandleGamePaused()
    {
        if (warningText == null && startButton == null) return;
        if (isPause || !isPopUpComplete) return;
        UITransitionUtility.Instance.PopDown(startButton, LeanTweenType.easeInBack, 0.2f);
        UITransitionUtility.Instance.PopDown(warningText, LeanTweenType.easeInBack, 0.2f);
                
        isPause = true;
        isUnpause = false;
    }
    
    private void HandleGameUnpaused()
    {
        if (warningText == null && startButton == null) return;
        if (isUnpause || !isPopUpComplete) return;
        StartCoroutine(WaitToPopUp());
        
        isUnpause = true;
        isPause = false;
    }

    #endregion

    private IEnumerator WaitToPopUp()
    {
        yield return new WaitForSecondsRealtime(1f);
        UITransitionUtility.Instance.PopUp(warningText);
        UITransitionUtility.Instance.PopUp(startButton);
    }
}

public static class ColliderExtensions
{
    public static void Disable(this Collider2D collider) => collider.enabled = false;
    public static void Enable(this Collider2D collider) => collider.enabled = true;
}
