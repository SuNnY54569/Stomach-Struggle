using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treatment : MonoBehaviour
{
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private Vector3 targetScale = Vector3.zero;
    [SerializeField] private GameObject Text;
    [SerializeField] private float showCoolDown = 1f;
    private SpriteRenderer sprite;
    private Color originalColor;
    private bool canShow = true;
    private Vector3 initialScale;

    private void Awake()
    {
        initialScale = Text.transform.localScale;
        UITransitionUtility.Instance.Initialize(Text, Vector2.zero);
        Text.SetActive(false);
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;

        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
        if (gameObject.CompareTag("GoodTreat"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IncreaseScore(1);
            }
        }
        else if (gameObject.CompareTag("BadTreat"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.DecreaseHealth(1);
            }
        }
        LeanTween.scale(gameObject, targetScale, scaleDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    private void OnMouseOver()
    {
        if (GameManager.Instance.isGamePaused) return;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.gray;
    }

    private void OnMouseExit()
    {
        if (GameManager.Instance.isGamePaused) return;
        UITransitionUtility.Instance.PopDown(Text, LeanTweenType.easeInBack, 0.1f);
        sprite.color = originalColor;
    }

    private void OnMouseEnter()
    {
        if (GameManager.Instance.isGamePaused) return;
        if (!canShow) return;
        
        Text.SetActive(true);
        Text.transform.localScale = Vector3.zero;
        LeanTween.scale(Text, initialScale, 0.2f)
            .setEase(LeanTweenType.easeOutBack)
            .setIgnoreTimeScale(true);
        LeanTween.rotateZ(gameObject, 5f, 0.1f)
            .setLoopPingPong(1).setOnComplete(() =>
            {
                StartCoroutine(ResetCanShow());
            });
    }

    private IEnumerator ResetCanShow()
    {
        canShow = false;
        yield return new WaitForSeconds(showCoolDown);
        canShow = true;
    }
}
