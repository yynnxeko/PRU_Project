using UnityEngine;
using UnityEngine.Events;

public class MissionManager : MonoBehaviour
{

    [Header("Mission Data")]
    public MissionData missionData;

    [Header("Runtime Steps (auto-populated hoặc kéo thủ công)")]
    public MissionStep[] steps;

    [Header("Events")]
    public UnityEvent onMissionStarted;
    public UnityEvent onMissionCompleted;
    public UnityEvent onMissionFailed;
    public UnityEvent<int> onStepCompleted;

    // Runtime state
    int currentStepIndex;
    MissionState state = MissionState.Locked;
    int retryCount;

    // Public properties
    public MissionState State => state;
    public bool IsActive => state == MissionState.Active;
    public bool IsCompleted => state == MissionState.Completed;
    public string MissionId => missionData != null ? missionData.missionId : "";
    public string MissionTitle => missionData != null ? missionData.title : "";

    void Start()
    {
        // Nếu có MissionData, lấy steps từ đó (nếu chưa gán thủ công)
        if (missionData != null && (steps == null || steps.Length == 0))
            steps = missionData.steps;

        // Sync state từ QuestLog nếu có
        if (QuestLog.Instance != null && missionData != null)
        {
            state = QuestLog.Instance.GetMissionState(missionData.missionId);
            retryCount = QuestLog.Instance.GetRetryCount(missionData.missionId);
        }
    }

    // ─────────────────── PUBLIC API ───────────────────

    /// <summary>
    /// Bắt đầu mission
    /// </summary>
    public void StartMission()
    {
        if (steps == null || steps.Length == 0)
        {
            Debug.LogWarning($"[Mission] {MissionTitle}: Không có steps!");
            return;
        }

        // Đăng ký với QuestLog
        if (QuestLog.Instance != null && missionData != null)
        {
            if (!QuestLog.Instance.StartMission(missionData.missionId))
                return; // Không thể start (đã completed hoặc locked)
        }

        state = MissionState.Active;
        currentStepIndex = 0;
        retryCount = QuestLog.Instance != null
            ? QuestLog.Instance.GetRetryCount(MissionId)
            : 0;

        // Reset tất cả steps
        foreach (var s in steps)
        {
            if (s != null) s.ResetStep();
        }

        steps[0].StartStep();
        onMissionStarted?.Invoke();

        Debug.Log($"[Mission] Started: {MissionTitle} ({MissionId})");
    }

    /// <summary>
    /// Thử lại mission sau khi fail (gọi từ MissionFailUI)
    /// </summary>
    public void RetryMission()
    {
        if (missionData != null && QuestLog.Instance != null)
            QuestLog.Instance.SetMissionAvailable(missionData.missionId);

        Debug.Log($"[Mission] Retrying: {MissionTitle}");
        StartMission();
    }

    /// <summary>
    /// Bỏ qua mission sau fail (gọi từ MissionFailUI)
    /// </summary>
    public void SkipAfterFail()
    {
        if (missionData != null && QuestLog.Instance != null)
            QuestLog.Instance.SetMissionAvailable(missionData.missionId);

        state = MissionState.Available;
        Debug.Log($"[Mission] Skipped: {MissionTitle} — vẫn Available để thử lại sau");
    }

    // ─────────────────── UPDATE LOOP ───────────────────

    void Update()
    {

        if (state != MissionState.Active) return;
        if (steps == null || currentStepIndex >= steps.Length) return;

        MissionStep step = steps[currentStepIndex];
        if (step == null) return;


        step.UpdateStep();

        if (step.IsCompleted)
        {

            HandleStepCompleted(step);
        }
        else if (step.IsFailed)
        {
            HandleStepFailed();
        }
    }

    // ─────────────────── INTERNAL ───────────────────

    void HandleStepCompleted(MissionStep step)
    {
        step.EndStep();
        onStepCompleted?.Invoke(currentStepIndex);
        Debug.Log($"[Mission] {MissionTitle} — Step {currentStepIndex} completed");

        currentStepIndex++;

        if (currentStepIndex < steps.Length)
        {
            // Chuyển sang step tiếp theo
            steps[currentStepIndex].StartStep();
        }
        else
        {
            // Hoàn thành toàn bộ mission
            CompleteMission();
        }
    }

    void CompleteMission()
    {
        state = MissionState.Completed;

        if (QuestLog.Instance != null && missionData != null)
            QuestLog.Instance.CompleteMission(missionData.missionId);

        onMissionCompleted?.Invoke();
        Debug.Log($"[Mission] Completed: {MissionTitle}");
    }

    void HandleStepFailed()
    {
        bool canFail = missionData != null ? missionData.canFail : true;

        if (!canFail)
        {
            // Không cho phép fail → reset step và tiếp tục
            steps[currentStepIndex].ResetStep();
            steps[currentStepIndex].StartStep();
            return;
        }

        state = MissionState.Failed;

        // Thông báo QuestLog
        if (QuestLog.Instance != null && missionData != null)
            QuestLog.Instance.FailMission(missionData.missionId);

        onMissionFailed?.Invoke();

        // Hiển thị Fail UI
        bool canRetry = missionData != null ? missionData.canRetry : true;
        int maxRetries = missionData != null ? missionData.maxRetries : 3;

        retryCount = QuestLog.Instance != null
            ? QuestLog.Instance.GetRetryCount(MissionId)
            : retryCount + 1;

        // Hết lượt retry
        if (canRetry && retryCount >= maxRetries)
            canRetry = false;

        if (MissionFailUI.Instance != null)
        {
            MissionFailUI.Instance.Show(this, MissionTitle, retryCount, maxRetries, canRetry);
        }
        else
        {
            // Không có UI → tự động retry
            Debug.LogWarning("[Mission] MissionFailUI not found — auto retrying");
            RetryMission();
        }

        Debug.Log($"[Mission] Failed: {MissionTitle} (retry {retryCount}/{maxRetries})");
    }
}
