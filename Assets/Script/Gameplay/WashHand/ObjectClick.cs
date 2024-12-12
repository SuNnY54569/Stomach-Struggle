using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClick : MonoBehaviour
{
    public int objectIndex;
    [SerializeField] private GameObject picture;
    [SerializeField] private float amp;
    [SerializeField] private float freq;
    [SerializeField] private string animationName;
    [SerializeField] private SpriteRenderer objectRenderer; 
    [SerializeField] private Color blinkColor = Color.red;
    [SerializeField] private float blinkDuration = 1f;
    [SerializeField] private float scaleAmount = 1.25f;
    [SerializeField] private Animator animator;
    
    private Color originalColor;
    private Vector3 initPos;

    private void Awake()
    {
        initPos = picture.transform.localPosition;
        originalColor = objectRenderer.material.color;
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;
        
        WashHandManager.Instance.OnObjectClicked(objectIndex, animationName, gameObject, animator);
    }

    public void BlinkObject()
    {
        BlinkAndScaleObject();
    }

    private void BlinkAndScaleObject()
    {
        LeanTween.scale(gameObject, gameObject.transform.localScale * scaleAmount, blinkDuration / 2f)
            .setLoopPingPong()
            .setEaseInOutSine();

        LeanTween.value(gameObject, UpdateColor, originalColor, blinkColor, blinkDuration / 2f)
            .setLoopPingPong()
            .setEaseInOutSine()
            .setOnUpdate((float t) =>
            {
                // Play sound at the start of each color transition
                if (Mathf.Approximately(t, 1f))
                {
                    if (gameObject.activeSelf)
                    {
                        SoundManager.PlaySound(SoundType.BBWarning, VolumeType.SFX, 0.1f);
                    }
                }
            });;
    }
    
    private void UpdateColor(Color color)
    {
        objectRenderer.material.color = color;
    }
    
    public void StopBlinkWithLeanTween()
    {
        LeanTween.cancel(gameObject);
        gameObject.transform.localScale = Vector3.one;
        objectRenderer.material.color = originalColor;
    }
    
    public void MovingObjectWithLeanTween()
    {
        LeanTween.moveLocalY(picture, initPos.y + amp, freq)
            .setEaseInOutSine()
            .setLoopPingPong();
    }
}
