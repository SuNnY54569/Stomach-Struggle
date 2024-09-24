using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
        else
        {
            Debug.Log("Wrong Answer");
            testManager.Wrong();
        }
    }
}
