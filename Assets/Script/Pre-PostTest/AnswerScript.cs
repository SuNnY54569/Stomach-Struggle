using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class AnswerScript : MonoBehaviour
{
    public bool isCorrect = false;
    [SerializeField] private TestManager testManager;
     
    public void Answer()
    {
        if (isCorrect)
        {
            Debug.Log("Correct Answer");
            testManager.Correct();
            SoundManager.PlaySound(SoundType.CorrectAnswer,VolumeType.SFX);
        }
        else
        {
            Debug.Log("Wrong Answer");
            testManager.Wrong();
            SoundManager.PlaySound(SoundType.WrongAnswer,VolumeType.SFX);
        }
    }
}
