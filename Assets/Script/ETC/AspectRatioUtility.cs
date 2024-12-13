using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioUtility : MonoBehaviour
{
    [SerializeField] private Camera[] cameras;
    private void Start()
    {
        Adjust();
    }

    private void Adjust()
    {
        float targetAspect = 16.0f / 9.0f;

        float windowAspect = (float)Screen.width / (float)Screen.height;

        float scaleHeight = windowAspect / targetAspect;
        

        if (scaleHeight < 1.0f)
        {
            foreach (var camera in cameras)
            {
                Rect rect = camera.rect;

                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;

                camera.rect = rect;
            }
        }
        else
        {
            foreach (var camera in cameras)
            {
                float scaleWidth = 1.0f / scaleHeight;

                Rect rect = camera.rect;
                rect.width = scaleWidth;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;

                camera.rect = rect;
            }
        }
    }
}
