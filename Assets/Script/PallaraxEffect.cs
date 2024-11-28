using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PallaraxEffect : MonoBehaviour
{

    [SerializeField] private RawImage _image;
    [SerializeField] private float x, y;

    private void Update()
    {
        _image.uvRect = new Rect(_image.uvRect.position + new Vector2(x, y) * Time.unscaledDeltaTime, _image.uvRect.size);
    }
}
