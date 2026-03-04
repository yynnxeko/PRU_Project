using UnityEngine;

public class Mission1_GetKey : MissionStep
{
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
        Debug.Log("Nhiệm vụ 1: Hãy đến IT Room và lấy chìa khóa trong tủ đồ.");
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
}
