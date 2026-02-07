using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueGameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button choice1Button;
    [SerializeField] private Button choice2Button;
    [SerializeField] private Button choice3Button;
    [SerializeField] private Button choice4Button;

    [Header("Player Reference")]
    [SerializeField] private PlayerController2 playerController;

    private int correctIndex = -1;
    private DialogueMissionStep currentMissionStep;

    void Start()
    {
        // Tự động tìm PlayerController2 nếu chưa gán
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController2>();

        // Gán sự kiện cho 4 buttons
        if (choice1Button != null) choice1Button.onClick.AddListener(() => OnChoiceSelected(0));
        if (choice2Button != null) choice2Button.onClick.AddListener(() => OnChoiceSelected(1));
        if (choice3Button != null) choice3Button.onClick.AddListener(() => OnChoiceSelected(2));
        if (choice4Button != null) choice4Button.onClick.AddListener(() => OnChoiceSelected(3));
    }

    /// <summary>
    /// Được gọi từ DialogueMissionStep để start game
    /// </summary>
    public void StartGame(string question, string c1, string c2, string c3, string c4, int correct, DialogueMissionStep missionStep)
    {
        currentMissionStep = missionStep;
        correctIndex = correct;

        // Hiển thị câu hỏi
        if (questionText != null)
            questionText.text = question;

        // Hiển thị 4 đáp án
        SetButtonText(choice1Button, c1);
        SetButtonText(choice2Button, c2);
        SetButtonText(choice3Button, c3);
        SetButtonText(choice4Button, c4);

        // Set isInGame = true để block player movement
        if (playerController != null)
            playerController.isInGame = true;
    }

    void SetButtonText(Button btn, string text)
    {
        if (btn == null) return;
        var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = text;

        // Nếu text rỗng → disable button (optional)
        btn.interactable = !string.IsNullOrEmpty(text);
    }

    void OnChoiceSelected(int index)
    {
        if (currentMissionStep == null) return;

        // Check đúng/sai (silent - không hiện gì)
        if (index == correctIndex)
        {
            // Đúng → complete step
            currentMissionStep.OnDialogueCorrect();
        }
        else
        {
            // Sai → fail step
            currentMissionStep.OnDialogueFailed();
        }

        // Kết thúc game → enable player movement
        if (playerController != null)
            playerController.isInGame = false;
    }

    void OnDestroy()
    {
        // Đảm bảo enable player khi đóng game
        if (playerController != null)
            playerController.isInGame = false;
    }
}