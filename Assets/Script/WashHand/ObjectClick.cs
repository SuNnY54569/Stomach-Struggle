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

        while (GameManager.Instance.currentHealth == 1) // Continue blinking while health == 1
        {
            float duration = blinkDuration / 2f;

            // Smoothly scale up and down during the blink
            while (blinkTime < duration)
            {
                // Use Mathf.Sin() for smooth transitions
                float scaleFactor = 1 + Mathf.Sin(blinkTime * Mathf.PI / duration) * (scaleAmount - 1);
                gameObject.transform.localScale = originalScale * scaleFactor;

                // Change color to the hint color while scaling
                objectRenderer.material.color = blinkColor;

                blinkTime += Time.deltaTime;
                yield return null;
            }

            // Reset the scale and color after blinking
            gameObject.transform.localScale = originalScale;
            objectRenderer.material.color = originalColor;

            // Reset the time tracker
            blinkTime = 0;
            yield return new WaitForSeconds(blinkDuration / 2f);
        }

        // Ensure object returns to original state if health is no longer 1
        objectRenderer.material.color = originalColor;
        gameObject.transform.localScale = originalScale;
    }
}
