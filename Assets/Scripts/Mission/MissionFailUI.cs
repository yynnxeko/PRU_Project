using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class MissionFailUI : MonoBehaviour
{
    public static MissionFailUI Instance;

    [Header("UI Elements")]
    public CanvasGroup panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public Button retryButton;
    public Button skipButton;

    [Header("Events")]
    public UnityEvent onRetry;
    public UnityEvent onSkip;

    MissionManager currentMission;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        Hide();
    }

    /// <summary>
    /// Hiển thị UI thất bại
    /// </summary>
    public void Show(MissionManager mission, string missionTitle, int retryCount, int maxRetries, bool canRetry)
    {
        currentMission = mission;

        if (panel != null)
        {
            panel.alpha = 1f;
            panel.blocksRaycasts = true;
            panel.interactable = true;
        }

        if (titleText != null)
            titleText.text = "Nhiệm vụ thất bại!";

        if (messageText != null)
        {
            if (canRetry)
                messageText.text = $"{missionTitle}\nSố lần thử: {retryCount}/{maxRetries}";
            else
                messageText.text = $"{missionTitle}\nKhông thể thử lại";
        }

        // Nút Retry
        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(canRetry);
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(OnRetryClicked);
        }

        // Nút Skip
        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(OnSkipClicked);
        }

        // Pause game
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        if (panel != null)
        {
            panel.alpha = 0f;
            panel.blocksRaycasts = false;
            panel.interactable = false;
        }

        Time.timeScale = 1f;
    }

    void OnRetryClicked()
    {
        Hide();
        onRetry?.Invoke();

        if (currentMission != null)
            currentMission.RetryMission();
    }

    void OnSkipClicked()
    {
        Hide();
        onSkip?.Invoke();

        if (currentMission != null)
            currentMission.SkipAfterFail();
    }
}
