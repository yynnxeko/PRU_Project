using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random; // Để dùng Random

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("UI Components")]
    public GameObject dialoguePanel;
    public Image avatarImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contextText;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public float autoCloseDelay = 2.0f;
    
    // Hiệu ứng Mã hóa
    [Header("Decoding Effect")]
    public string scrambleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*"; // Các ký tự rác
    public int scrambleLength = 3; // Số ký tự rác hiện ra trước khi hiện chữ thật

    private Coroutine typingCoroutine;
    private Coroutine autoCloseCoroutine;
    private bool isTyping = false;
    private string currentFullText = "";
    private Action currentOnCompleteCallback;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && dialoguePanel.activeSelf)
        {
            if (isTyping) CompleteTextImmediately();
            else HideDialogue();
        }
    }

    public void ShowDialogue(string content, string name, Sprite avatar = null, Action onComplete = null)
    {
        StopAllCoroutines();
        currentOnCompleteCallback = onComplete;

        dialoguePanel.SetActive(true);
        nameText.text = name;
        currentFullText = content;
        contextText.text = "";

        if (avatar != null)
        {
            avatarImage.sprite = avatar;
            avatarImage.gameObject.SetActive(true);
        }
        else
        {
            avatarImage.gameObject.SetActive(false);
        }

        typingCoroutine = StartCoroutine(TypeScrambleEffect());
    }

    public void HideDialogue()
    {
        StopAllCoroutines();
        dialoguePanel.SetActive(false);
        isTyping = false;

        if (currentOnCompleteCallback != null)
        {
            Action tempCallback = currentOnCompleteCallback;
            currentOnCompleteCallback = null;
            tempCallback.Invoke();
        }
    }

    // --- HIỆU ỨNG GÕ MÃ HÓA (SCRAMBLE) ---
    IEnumerator TypeScrambleEffect()
    {
        isTyping = true;
        contextText.text = "";

        string finalString = ""; // Chuỗi kết quả đang xây dựng

        for (int i = 0; i < currentFullText.Length; i++)
        {
            char targetChar = currentFullText[i];

            // Nếu là khoảng trắng thì hiện luôn, khỏi mã hóa
            if (targetChar == ' ')
            {
                finalString += " ";
                contextText.text = finalString;
                continue;
            }

            // Hiệu ứng nhảy số (Scramble) cho ký tự hiện tại
            for (int k = 0; k < scrambleLength; k++)
            {
                // Chọn ngẫu nhiên 1 ký tự rác
                char randomChar = scrambleChars[Random.Range(0, scrambleChars.Length)];
                
                // Hiển thị: Chuỗi đã xong + Ký tự rác đang nhảy
                contextText.text = finalString + randomChar;

                yield return new WaitForSeconds(typingSpeed / 2); // Nhảy nhanh hơn tốc độ gõ chút
            }

            // Chốt ký tự thật
            finalString += targetChar;
            contextText.text = finalString;
        }

        isTyping = false;
        autoCloseCoroutine = StartCoroutine(AutoCloseCountdown());
    }

    void CompleteTextImmediately()
    {
        StopCoroutine(typingCoroutine);
        contextText.text = currentFullText;
        isTyping = false;

        if (autoCloseCoroutine != null) StopCoroutine(autoCloseCoroutine);
        autoCloseCoroutine = StartCoroutine(AutoCloseCountdown());
    }

    IEnumerator AutoCloseCountdown()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        HideDialogue();
    }
}
