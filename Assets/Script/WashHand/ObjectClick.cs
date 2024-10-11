using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClick : MonoBehaviour
{
    public int objectIndex; // The order of this object
    [SerializeField] private GameObject picture;
    [SerializeField] private float amp;
    [SerializeField] private float freq;
    [SerializeField] private string animationName;
    [SerializeField] private SpriteRenderer objectRenderer; // Renderer to control object color
    [SerializeField] private Color blinkColor = Color.red; // Color to blink
    [SerializeField] private float blinkDuration = 1f; // Duration of each blink cycle
    [SerializeField] private int blinkCount = 3;
    [SerializeField] private float scaleAmount = 1.25f;

    private Color originalColor;
    private Vector3 initPos;

    private void Start()
    {
        initPos = picture.transform.localPosition;
        if (objectRenderer == null)
        {
            objectRenderer = picture.GetComponent<SpriteRenderer>();
        }

        originalColor = objectRenderer.material.color;
    }

    private void Update()
    {
        picture.transform.localPosition = new Vector3(initPos.x, Mathf.Sin(Time.time * freq) * amp + initPos.y, 0);
    }

    private void OnMouseDown()
    {
        WashHandManager.Instance.OnObjectClicked(objectIndex, animationName, gameObject);
    }

    public void BlinkObject()
    {
        StartCoroutine(BlinkAndScaleObject());
    }

    private IEnumerator BlinkAndScaleObject()
    {
        Vector3 originalScale = gameObject.transform.localScale;
        Color originalColor = objectRenderer.material.color;

        float blinkTime = 0; // Time tracker for smooth scaling

        for (int i = 0; i < blinkCount; i++)
        {
            float duration = blinkDuration / 2f; // Duration for each scaling up and down cycle

            // Smoothly scale up
            while (blinkTime < duration)
            {
                // Smooth scaling using Mathf.Sin() for smooth transitions
                float scaleFactor =
                    1 + Mathf.Sin(blinkTime * Mathf.PI / duration) * (scaleAmount - 1); // Smooth scaling
                gameObject.transform.localScale = originalScale * scaleFactor;

                // Blink to the hint color while scaling
                objectRenderer.material.color = blinkColor;

                blinkTime += Time.deltaTime;
                yield return null;
            }

            // Reset the scale and color
            gameObject.transform.localScale = originalScale;
            objectRenderer.material.color = originalColor;

            // Wait before starting the next blink cycle
            blinkTime = 0; // Reset time tracker
            yield return new WaitForSeconds(blinkDuration / 2f);

            // Ensure the object returns to its original state
            objectRenderer.material.color = originalColor;
            gameObject.transform.localScale = originalScale;
        }
    }
}
