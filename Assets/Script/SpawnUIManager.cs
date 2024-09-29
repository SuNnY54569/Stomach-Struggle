using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnUIManager : MonoBehaviour
{
    [Header("UI Prefab References")]
    [SerializeField] private GameObject uiPrefab; // Prefab ของ UI ที่จะ spawn
    [SerializeField] private Transform uiParent; // Parent ที่จะวาง UI (เช่น Canvas)

    [Header("Image Settings")]
    [SerializeField] private Sprite[] randomImages; // รูปภาพที่สามารถสุ่มได้

    [Header("Time Settings")]
    [SerializeField] private int minHour = 0; // ชั่วโมงต่ำสุด
    [SerializeField] private int maxHour = 23; // ชั่วโมงสูงสุด
    [SerializeField] private int minMinute = 0; // นาทีต่ำสุด
    [SerializeField] private int maxMinute = 59; // นาทีสูงสุด

    private void Start()
    {
        SpawnRandomUI(); // เรียกใช้เมื่อเริ่มเกม
    }

    public void SpawnRandomUI()
    {
        // สร้าง UI ใหม่
        GameObject newUI = Instantiate(uiPrefab, uiParent);

        // สุ่มรูปภาพ
        Image imageComponent = newUI.transform.Find("Food").GetComponent<Image>();
        if (imageComponent != null && randomImages.Length > 0)
        {
            int randomIndex = Random.Range(0, randomImages.Length);
            imageComponent.sprite = randomImages[randomIndex];
        }

        // สุ่มเวลา
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
