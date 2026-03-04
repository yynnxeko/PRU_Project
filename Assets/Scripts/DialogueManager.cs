using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;
    public GameObject staticEffectImage;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public float autoNextDelay = 0.5f; // Thời gian chờ tự động chuyển (giây)

    [System.Serializable]
    public struct DialogueLine
    {
        public string characterName;
        [TextArea(3, 10)] public string content;
        public bool isEncrypted;
    }

    public List<DialogueLine> dialogueLines;
    private Queue<DialogueLine> linesQueue;
    private bool isTyping = false; // Biến kiểm tra đang gõ chữ hay không
    private Coroutine autoNextCoroutine; // Lưu coroutine đếm giờ tự động

    void Start()
    {
        linesQueue = new Queue<DialogueLine>();
        dialoguePanel.SetActive(false);
        if (staticEffectImage) staticEffectImage.SetActive(false);
    }

    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        linesQueue.Clear();
        foreach (DialogueLine line in dialogueLines)
        {
            linesQueue.Enqueue(line);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // Nếu có hẹn giờ tự động chuyển từ câu trước thì hủy đi
        if (autoNextCoroutine != null) StopCoroutine(autoNextCoroutine);

        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = linesQueue.Dequeue();

        nameText.text = currentLine.characterName;
        isTyping = true; // Bắt đầu gõ

        if (currentLine.isEncrypted)
        {
            nameText.color = Color.red;
            if (staticEffectImage) staticEffectImage.SetActive(true);
            StartCoroutine(TypeEncryptedText(currentLine.content));
        }
        else
        {
            nameText.color = Color.white;
            if (staticEffectImage) staticEffectImage.SetActive(false);
            StartCoroutine(TypeSentence(currentLine.content));
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        contentText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            contentText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Gõ xong
        isTyping = false;
        // Bắt đầu đếm ngược để tự qua câu tiếp
        autoNextCoroutine = StartCoroutine(AutoNextTimer());
    }

    IEnumerator TypeEncryptedText(string sentence)
    {
        contentText.text = "";
        string glitchChars = "!@#$%^&*()_+-=[]{}|;':,./<>?";

        for (int i = 0; i < sentence.Length; i++)
        {
            contentText.text += glitchChars[Random.Range(0, glitchChars.Length)];
            yield return new WaitForSeconds(typingSpeed / 2);
            contentText.text = contentText.text.Remove(contentText.text.Length - 1);
            contentText.text += sentence[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        // Gõ xong
        isTyping = false;
        // Bắt đầu đếm ngược để tự qua câu tiếp
        autoNextCoroutine = StartCoroutine(AutoNextTimer());
    }

    // Hàm đếm ngược thời gian chờ
    IEnumerator AutoNextTimer()
    {
        yield return new WaitForSeconds(autoNextDelay);
        DisplayNextSentence(); // Hết giờ thì tự qua câu mới
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        if (staticEffectImage) staticEffectImage.SetActive(false);
    }

    void Update()
    {
        // Nếu người chơi sốt ruột bấm chuột thì qua luôn
        if (dialoguePanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Nếu đang gõ mà bấm -> Hiện hết chữ ngay lập tức (skip effect)
                StopAllCoroutines();
                isTyping = false;

                // Lưu ý: Đoạn này hơi phức tạp nếu muốn hiện đủ chữ ngay
                // Nên đơn giản nhất là cho qua câu tiếp luôn
                DisplayNextSentence();
            }
            else
            {
                // Nếu đã gõ xong mà bấm -> Qua câu tiếp luôn (không chờ timer nữa)
                if (autoNextCoroutine != null) StopCoroutine(autoNextCoroutine);
                DisplayNextSentence();
            }
        }
    }
}
