using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonNext : MonoBehaviour
{
    [Header("Button Color Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.cyan;

    [SerializeField]private Button button;
    [SerializeField]private Image buttonImage;

    private void Start()
    {
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();

        buttonImage.color = normalColor;
    }

    public void OnPointerEnter()
    {
        buttonImage.color = hoverColor;
    }

    public void OnPointerExit()
    {
        buttonImage.color = normalColor;
    }

}
