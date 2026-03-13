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

    [Header("Tổng số nhiệm vụ")]
    public int totalMissions = 3;

    [Header("Mô tả nhiệm vụ")]
    [TextArea(2, 4)]
    public string[] missionDescriptions = new string[]
    {
        "Hãy đến IT Room và lấy chìa khóa trong tủ đồ.",
        "Hãy đến IT Room làm nhiệm vụ trả lời câu hỏi hằng ngày.",
        "Hãy trả lời các câu hỏi để chứng minh sự vô tội và tìm ra kẻ lừa đảo."
    };

    [Header("Mission Voice (index = mission index)")]
    public AudioSource voiceAudioSource;
    public AudioClip[] missionVoices;

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

        // Load tiến trình
        currentMissionIndex = PlayerPrefs.GetInt(SAVE_KEY, 0);

        Debug.Log($"[FullMissionManager] Loaded progress: Mission {currentMissionIndex}");

        if (currentMissionIndex == 1 && GameFlagManager.Instance != null)
        {
            GameFlagManager.Instance.SetFlag("mission_accepted", true);
            Debug.Log("[FullMissionManager] Mission 2 active → bật cờ mission_accepted");
        }
    }

    /// <summary>
    /// MissionStep gọi hàm này để đăng ký.
    /// </summary>
    public void RegisterStep(int missionIndex, MissionStep step)
    {
        Debug.Log($"[FullMissionManager] RegisterStep: index={missionIndex}, current={currentMissionIndex}");

        if (missionIndex < currentMissionIndex)
        {
            step.MarkAsCompleted();
            step.gameObject.SetActive(false);
            return;
        }

        if (missionIndex == currentMissionIndex)
        {
            activeStep = step;
            step.StartStep();

            PlayMissionVoice(currentMissionIndex);

            if (currentMissionIndex == 1 && GameFlagManager.Instance != null)
            {
                GameFlagManager.Instance.SetFlag("mission_accepted", true);
            }

            return;
        }

        step.gameObject.SetActive(false);
    }

    /// <summary>
    /// Khi nhiệm vụ hoàn thành
    /// </summary>
    public void ReportComplete()
    {
        Debug.Log($"[FullMissionManager] Mission {currentMissionIndex} COMPLETED!");

        currentMissionIndex++;

        PlayerPrefs.SetInt(SAVE_KEY, currentMissionIndex);
        PlayerPrefs.Save();

        string nextMissionDesc = GetActiveMissionDescription();

        Debug.Log($"<color=green>[FullMission] TIẾN TRÌNH MỚI: Mission {currentMissionIndex}</color>");
        Debug.Log($"<color=cyan>[FullMission] HIỆN TẠI: {nextMissionDesc}</color>");

        activeStep = null;

        if (currentMissionIndex >= totalMissions)
        {
            Debug.Log("[FullMissionManager] TẤT CẢ NHIỆM VỤ ĐÃ HOÀN THÀNH!");
        }
    }

    /// <summary>
    /// Phát audio tương ứng với nhiệm vụ
    /// </summary>
    void PlayMissionVoice(int index)
    {
        if (voiceAudioSource == null) return;

        if (missionVoices != null && index < missionVoices.Length && missionVoices[index] != null)
        {
            voiceAudioSource.Stop();
            voiceAudioSource.clip = missionVoices[index];
            voiceAudioSource.Play();
        }
    }

    public int GetCurrentMissionIndex() => currentMissionIndex;

    public bool AllMissionsCompleted() => currentMissionIndex >= totalMissions;

    /// <summary>
    /// Lấy mô tả nhiệm vụ hiện tại + phát âm thanh
    /// </summary>
    public string GetActiveMissionDescription()
    {
        if (activeStep != null)
            return activeStep.GetMissionDescription();

        if (missionDescriptions != null &&
            currentMissionIndex >= 0 &&
            currentMissionIndex < missionDescriptions.Length)
        {
            PlayMissionVoice(currentMissionIndex);

            return missionDescriptions[currentMissionIndex];
        }

        return "Không có nhiệm vụ hiện tại.";
    }

    /// <summary>
    /// Reset tiến trình
    /// </summary>
    public void ResetAllProgress()
    {
        currentMissionIndex = 0;

        PlayerPrefs.SetInt(SAVE_KEY, 0);
        PlayerPrefs.Save();

        Debug.Log("[FullMissionManager] RESET tất cả tiến trình!");
    }
}