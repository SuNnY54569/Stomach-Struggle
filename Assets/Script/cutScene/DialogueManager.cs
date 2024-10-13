using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Typewriter Settings")]
    [SerializeField] private float typingSpeed = 0.05f;  // Speed at which characters appear

    [SerializeField]private string characterName;
    [SerializeField]private string dialogueContent;

    private void Start()
    {
        StartCoroutine(ShowDialogue(characterName, dialogueContent));
    }

    // Coroutine to display text with typing effect
    private IEnumerator ShowDialogue(string name, string content)
    {
        dialogueText.text = name;  // Show the character name immediately

        yield return new WaitForSeconds(0.5f); // Optional: small delay before starting the typing effect

        foreach (char letter in content.ToCharArray())
        {
            dialogueText.text += letter;  // Add each letter one by one
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
