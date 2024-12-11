using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClickHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (GameManager.Instance.isGamePaused) return;
        if (GameManager.Instance.scoreManager.GetScore() == GameManager.Instance.scoreManager.scoreMax) return;
        else if (gameObject.CompareTag("BadVegetable"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.healthManager.DecreaseHealth(1);
            }
        }
        
        LeanTween.scale(gameObject, Vector3.zero, 0.2f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Destroy(gameObject);
            });
        SoundManager.PlaySound(SoundType.UIClick,VolumeType.SFX);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("GoodVegetable"))
        {
            GameManager.Instance.healthManager.DecreaseHealth(1);
        }

        LeanTween.scale(gameObject, Vector3.zero, 0.2f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                Destroy(gameObject);
            });
    }
}
