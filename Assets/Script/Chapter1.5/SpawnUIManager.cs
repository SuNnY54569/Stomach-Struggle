using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnUIManager : MonoBehaviour
{
    [Header("UI Prefab References")]
    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private Transform uiParent;

    [Header("Image Settings")]
    [SerializeField] private Sprite[] randomImages;

    [Header("Time Settings")]
    [SerializeField] private int minHour;
    [SerializeField] private int maxHour;
    [SerializeField] private int minMinute;
    [SerializeField] private int maxMinute;

    private void Start()
    {
        SpawnRandomUI();
    }

    public void SpawnRandomUI()
    {
        if (GameManager.Instance.GetScore() >= GameManager.Instance.scoreMax || GameManager.Instance.currentHealth <= 0)
        {
            return;
        }


        GameObject newUI = Instantiate(uiPrefab, uiParent);

        Image imageComponent = newUI.transform.Find("Food").GetComponent<Image>();
        if (imageComponent != null && randomImages.Length > 0)
        {
            int randomIndex = Random.Range(0, randomImages.Length);
            imageComponent.sprite = randomImages[randomIndex];
        }

        TextMeshProUGUI timeText = newUI.transform.Find("Time").GetComponent<TextMeshProUGUI>();
        if (timeText != null)
        {
            int randomHour = Random.Range(minHour, maxHour + 1);
            int randomMinute = Random.Range(minMinute, maxMinute + 1);

            string formattedTime = $"{randomHour:00}:{randomMinute:00}";
            timeText.text = formattedTime;
        }
    }
}
