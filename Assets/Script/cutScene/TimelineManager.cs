using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    public void TriggerDialogue(string characterName, string dialogueContent)
    {
        string fullDialogue = $"{characterName}: {dialogueContent}";
        UIManager.Instance.ShowDialogue(fullDialogue);
    }
}
