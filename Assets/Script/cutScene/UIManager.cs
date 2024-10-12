using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI dialogueText;  // ช่องข้อความ
    [SerializeField] private GameObject dialoguePanel;      // แผงแสดงบทสนทนา
    [SerializeField] private float typeSpeed = 0.05f;       // ความเร็วในการพิมพ์

    [Header("Dialogue Settings")]  // บทสนทนาที่ตั้งใน Inspector
    [SerializeField] private string characterName = "ชื่อตัวละคร";
    [SerializeField] private string dialogueContent = "วันนี้จะทำอะไรดีนะ";

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // เรียกบทสนทนาที่ตั้งค่าไว้ใน Inspector เมื่อเริ่มเกม (สำหรับทดสอบ)
        ShowDialogue($"{characterName}: {dialogueContent}");
    }

    public void ShowDialogue(string message)
    {
        dialoguePanel.SetActive(true);  // เปิดแผง UI ข้อความ
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);  // หยุดการพิมพ์ที่ค้างอยู่
        }
        typingCoroutine = StartCoroutine(TypeTextEffect(message));
    }

    private IEnumerator TypeTextEffect(string message)
    {
        isTyping = true;
        dialogueText.text = "";  // ล้างข้อความก่อนเริ่มพิมพ์

        foreach (char letter in message)
        {
            dialogueText.text += letter;  // เพิ่มทีละตัวอักษร
            yield return new WaitForSeconds(typeSpeed);  // รอเวลา
        }

        isTyping = false;
    }

    private void Update()
    {
        // ถ้าผู้เล่นกด Space หรือคลิก ให้ข้ามการพิมพ์ทั้งหมดทันที
        if (isTyping && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            SkipTyping();
        }
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);  // หยุดการพิมพ์ที่กำลังทำงานอยู่
        }
        dialogueText.text = $"{characterName}: {dialogueContent}";  // แสดงข้อความทั้งหมดทันที
        isTyping = false;
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);  // ปิดแผง UI ข้อความ
        dialogueText.text = "";  // ล้างข้อความหลังปิด
    }
}
