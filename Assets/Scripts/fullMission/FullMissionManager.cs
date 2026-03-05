using UnityEngine;

/// <summary>
/// Quản lý tiến trình nhiệm vụ xuyên scene.
/// Singleton + DontDestroyOnLoad + PlayerPrefs.
/// Đặt trong scene đầu tiên, chỉ cần 1 lần.
/// </summary>
public class FullMissionManager : MonoBehaviour
{
    public static FullMissionManager Instance { get; private set; }

    private const string SAVE_KEY = "FullMissionProgress";

    [Header("Tiến trình")]
    public int currentMissionIndex;

    [Header("Tổng số nhiệm vụ (để biết khi nào xong hết)")]
    public int totalMissions = 3;

    [Header("Mô tả nhiệm vụ (Dùng cho mọi Scene)")]
    [TextArea(2, 4)]
    public string[] missionDescriptions = new string[]
    {
        "Hãy đến IT Room và lấy chìa khóa trong tủ đồ.",
        "Tìm mật khẩu trên máy tính để mở tủ phòng ngủ.",
        "Hãy trả lời các câu hỏi để chứng minh sự vô tội và tìm ra kẻ lừa đảo."
    };

    // Mission step đang active trong scene hiện tại
    private MissionStep activeStep;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load tiến trình từ ổ cứng
        currentMissionIndex = PlayerPrefs.GetInt(SAVE_KEY, 0);
        Debug.Log($"[FullMissionManager] Loaded progress: Mission {currentMissionIndex}");
    }

    /// <summary>
    /// Mỗi MissionStep gọi hàm này trong Start() để đăng ký.
    /// Nếu missionIndex khớp currentMissionIndex → StartStep().
    /// Nếu không → step tự disable.
    /// </summary>
    public void RegisterStep(int missionIndex, MissionStep step)
    {
        Debug.Log($"[FullMissionManager] RegisterStep: index={missionIndex}, current={currentMissionIndex}, step={step.gameObject.name}");

        if (missionIndex < currentMissionIndex)
        {
            // Nhiệm vụ này đã hoàn thành trước đó
            step.MarkAsCompleted();
            step.gameObject.SetActive(false);
            Debug.Log($"[FullMissionManager] Step {missionIndex} đã hoàn thành trước đó → disable");
            return;
        }

        if (missionIndex == currentMissionIndex)
        {
            // Đây là nhiệm vụ hiện tại → kích hoạt
            activeStep = step;
            step.StartStep();
            Debug.Log($"[FullMissionManager] Step {missionIndex} là nhiệm vụ hiện tại → StartStep()");
            return;
        }

        // missionIndex > currentMissionIndex → chưa đến lượt
        step.gameObject.SetActive(false);
        Debug.Log($"[FullMissionManager] Step {missionIndex} chưa đến lượt → disable");
    }

    /// <summary>
    /// Gọi khi MissionStep hoàn thành. Tăng index, lưu PlayerPrefs,
    /// và tìm step tiếp theo trong scene (nếu có).
    /// </summary>
    public void ReportComplete()
    {
        Debug.Log($"[FullMissionManager] Mission {currentMissionIndex} COMPLETED!");

        currentMissionIndex++;

        // Lưu vào ổ cứng
        PlayerPrefs.SetInt(SAVE_KEY, currentMissionIndex);
        PlayerPrefs.Save();
        Debug.Log($"[FullMissionManager] Saved progress: Mission {currentMissionIndex}");

        activeStep = null;

        if (currentMissionIndex >= totalMissions)
        {
            Debug.Log("[FullMissionManager] TẤT CẢ NHIỆM VỤ ĐÃ HOÀN THÀNH!");
            return;
        }

        // Không tự tìm step tiếp theo ở đây — khi player vào scene có step tiếp theo,
        // step đó sẽ tự RegisterStep() trong Start()
    }

    /// <summary>
    /// Lấy chỉ số nhiệm vụ hiện tại (dùng cho UI hoặc NPC dialogue).
    /// </summary>
    public int GetCurrentMissionIndex() => currentMissionIndex;

    /// <summary>
    /// Kiểm tra xem tất cả nhiệm vụ đã xong chưa.
    /// </summary>
    public bool AllMissionsCompleted() => currentMissionIndex >= totalMissions;

    /// <summary>
    /// Tìm mô tả nhiệm vụ hiện tại. 
    /// Lấy trực tiếp từ danh sách được cấu hình trên Inspector để dùng chung cho mọi Scene.
    /// </summary>
    public string GetActiveMissionDescription()
    {
        // Ưu tiên 1: Nếu đang có activeStep (nhiệm vụ nằm ngay trong scene này) thì lấy luôn
        if (activeStep != null) return activeStep.GetMissionDescription();

        // Ưu tiên 2: Lấy từ danh sách mô tả cấu hình sẵn trên FullMissionManager (hữu ích khi nhảy Scene)
        if (missionDescriptions != null && currentMissionIndex >= 0 && currentMissionIndex < missionDescriptions.Length)
        {
            return missionDescriptions[currentMissionIndex];
        }

        return "Không có nhiệm vụ hiện tại.";
    }

    /// <summary>
    /// Reset toàn bộ tiến trình (chơi lại từ đầu).
    /// </summary>
    public void ResetAllProgress()
    {
        currentMissionIndex = 0;
        PlayerPrefs.SetInt(SAVE_KEY, 0);
        PlayerPrefs.Save();
        Debug.Log("[FullMissionManager] RESET tất cả tiến trình về 0!");
    }
}
