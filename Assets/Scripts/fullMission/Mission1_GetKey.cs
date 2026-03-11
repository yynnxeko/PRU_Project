using UnityEngine;

public class Mission1_GetKey : MissionStep
{
    [Header("Mô tả nhiệm vụ")]
    [TextArea]
    public string missionDescription = "Hãy đến IT Room và lấy chìa khóa trong tủ đồ.";
    [Header("Thứ tự nhiệm vụ trong FullMissionManager")]
    public int missionIndex = 0;

    [Header("Cài đặt nhiệm vụ")]
    public string targetItemName = "Bedroom Key";

    void Start()
    {
        // Tự đăng ký với FullMissionManager
        if (FullMissionManager.Instance != null)
        {
            FullMissionManager.Instance.RegisterStep(missionIndex, this);
        }
        else
        {
            Debug.LogWarning("[Mission1_GetKey] FullMissionManager.Instance is null!");
        }
    }

    public override void StartStep()
    {
        base.StartStep();
        Debug.Log($"Nhiệm vụ 1: {missionDescription}");

        // Có thể hiện UI nhiệm vụ ở đây nếu muốn
    }

    // Hàm này sẽ được gọi từ Script của cái Tủ hoặc Item khi Player nhặt
    public void OnKeyCollected()
    {
        if (IsCompleted) return;

        Debug.Log("Đã lấy được chìa khóa!");
        CompleteStep();

        // Báo cho FullMissionManager
        if (FullMissionManager.Instance != null)
            FullMissionManager.Instance.ReportComplete();
    }

    // Hàm cho phép lấy mô tả nhiệm vụ này
    public override string GetMissionDescription()
    {
        return missionDescription;
    }
}
