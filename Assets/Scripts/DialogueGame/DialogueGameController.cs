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

    /// <summary>
    /// Instance hiện tại để DialogueMissionStep tìm nhanh không cần FindObjectOfType.
    /// </summary>
    public static DialogueGameController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController2>();

        if (choice1Button != null) choice1Button.onClick.AddListener(() => OnChoiceSelected(0));
        if (choice2Button != null) choice2Button.onClick.AddListener(() => OnChoiceSelected(1));
        if (choice3Button != null) choice3Button.onClick.AddListener(() => OnChoiceSelected(2));
        if (choice4Button != null) choice4Button.onClick.AddListener(() => OnChoiceSelected(3));

        // Tự báo cho DialogueMissionStep biết mình đã sẵn sàng
        DialogueMissionStep.NotifyControllerReady(this);
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

        if (index == correctIndex)
        {
            currentMissionStep.OnDialogueCorrect();
            // Không tắt isInGame ở đây — OnDialogueCorrect sẽ tự quyết định
            // (nếu xong 10/20 câu thì nó tự tắt, còn lại thì tiếp câu mới)
        }
        else
        {
            currentMissionStep.OnDialogueFailed();
            // Fail → FailedRoutine sẽ tự tắt isInGame và ForceStandUp
        }
    }

    void OnDisable()
    {
        // Khi bị tắt (Quit/CloseDesktop) → clear Instance để lần sau tạo mới được
        if (Instance == this) Instance = null;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;

        if (playerController != null)
            playerController.isInGame = false;
    }
}