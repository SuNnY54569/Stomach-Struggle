using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        WashHandManager.Instance.StartGame();
    }
}
