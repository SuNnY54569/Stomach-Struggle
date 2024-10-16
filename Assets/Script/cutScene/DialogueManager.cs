using System.Collections;
using System;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Typewriter Settings")]
    [SerializeField] private float typingSpeed = 0.05f;

    [SerializeField]private string characterName;
    [SerializeField]private string dialogueContent;

    private void Start()
    {
        PauseGame();
        StartCoroutine(ShowDialogue(characterName, dialogueContent));
    }
    private void Update()
    {
        
    }

    private IEnumerator ShowDialogue(string name, string content)
    {
        dialogueText.text = name;

        yield return new WaitForSecondsRealtime(0.5f);

        foreach (char letter in content.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
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
