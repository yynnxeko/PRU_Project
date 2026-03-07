using UnityEngine;

public class Mission2_UnlockCabinet : MissionStep
{
    [Header("Mô tả nhiệm vụ")]
    [TextArea]
    public string missionDescription = "Hãy đến IT Room làm nhiệm vụ trả lời câu hỏi hằng ngày.";
    [Header("Thứ tự nhiệm vụ trong FullMissionManager")]
    public int missionIndex = 1;

    void Start()
    {
        // Tự đăng ký với FullMissionManager
        if (FullMissionManager.Instance != null)
        {
            FullMissionManager.Instance.RegisterStep(missionIndex, this);
        }
        else
        {
            Debug.LogWarning("[Mission2_UnlockCabinet] FullMissionManager.Instance is null!");
        }
    }

    public override void StartStep()
    {
        base.StartStep();
        Debug.Log($"Nhiệm vụ 2: {missionDescription}");

        // Bật cờ mission_accepted để khi trả lời sai trong DialogueMissionStep
        // → FailedRoutine() sẽ bật cờ go_to_medical → bị chích điện → đi phòng y tế
        if (GameFlagManager.Instance != null)
        {
            GameFlagManager.Instance.SetFlag("mission_accepted", true);
            Debug.Log("[Mission2] Đã bật cờ mission_accepted → sẵn sàng cho luồng chích điện");
        }
    }

    /// <summary>
    /// Gọi khi player đã đến phòng y tế (Hospital scene).
    /// Script ở scene Hospital sẽ gọi hàm này để hoàn thành nhiệm vụ.
    /// (Chưa sử dụng - chờ implement phần Hospital)
    /// </summary>
    public void OnArrivedAtMedicalRoom()
    {
        if (IsCompleted) return;

        Debug.Log("[Mission2] Đã đến phòng y tế!");
        CompleteStep();

        // Tắt cờ mission_accepted vì nhiệm vụ đã xong
        if (GameFlagManager.Instance != null)
        {
            GameFlagManager.Instance.SetFlag("mission_accepted", false);
        }

        // Báo cho FullMissionManager
        if (FullMissionManager.Instance != null)
            FullMissionManager.Instance.ReportComplete();
    }

    public override string GetMissionDescription()
    {
        return missionDescription;
    }
}
