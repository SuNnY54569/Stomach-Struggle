using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Typewriter Settings")]
    [SerializeField] private float typingSpeed = 0.015f;

    [SerializeField] private string characterName;
    [SerializeField] private string dialogueContent;

    private bool isDialogueDisplayed = false;
    private bool hasClicked = false;

    private Coroutine currentDialogueCoroutine = null;

    private void Start()
    {
        currentDialogueCoroutine = StartCoroutine(ShowDialogue(characterName, dialogueContent));
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !hasClicked)
        {
            ShowFullDialogue();
            hasClicked = true;
        }
    }

    private IEnumerator ShowDialogue(string name, string content)
    {
        if (isDialogueDisplayed)
        {
            yield break; 
        }

        string currentText = name + "";
        dialogueText.text = currentText;

        yield return new WaitForSecondsRealtime(0.5f);

        foreach (char letter in content.ToCharArray())
        {
            currentText += letter; 
            dialogueText.text = currentText;

            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isDialogueDisplayed = true;
    }

    private void ShowFullDialogue()
    {
        if (!isDialogueDisplayed)
        {
            dialogueText.text = characterName + "" + dialogueContent;
            isDialogueDisplayed = true;

            if (currentDialogueCoroutine != null)
            {
                StopCoroutine(currentDialogueCoroutine);
                currentDialogueCoroutine = null;
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
