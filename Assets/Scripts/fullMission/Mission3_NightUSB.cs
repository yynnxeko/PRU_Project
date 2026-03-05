using UnityEngine;

public class Mission3_NightUSB : MissionStep
{
    [Header("Mô tả nhiệm vụ")]
    [TextArea]
    public string missionDescription = "Đợi đến tối, lẻn vào IT Room để lấy USB.";
    [Header("Thứ tự nhiệm vụ trong FullMissionManager")]
    public int missionIndex = 2;

    void Start()
    {
        // Tự đăng ký với FullMissionManager
        if (FullMissionManager.Instance != null)
        {
            FullMissionManager.Instance.RegisterStep(missionIndex, this);
        }
        else
        {
            Debug.LogWarning("[Mission3_NightUSB] FullMissionManager.Instance is null!");
        }
    }

    public override void StartStep()
    {
        base.StartStep();
        Debug.Log($"Nhiệm vụ 3: {missionDescription}");
    }

    public override void UpdateStep()
    {
        // Có thể thêm logic cảnh báo nếu người chơi vào phòng IT lúc ban ngày
    }

    // Gọi hàm này khi tương tác với USB
    public void OnUSBCollected()
    {
        // Kiểm tra xem có đúng là ban đêm không
        if (DayManager.Instance != null && DayManager.Instance.currentPhase == DayPhase.Night)
        {
            if (IsCompleted) return;

            Debug.Log("Đã lấy được USB thành công trong đêm!");
            CompleteStep();

            // Báo cho FullMissionManager
            if (FullMissionManager.Instance != null)
                FullMissionManager.Instance.ReportComplete();
        }
        else
        {
            Debug.Log("Bây giờ là ban ngày, lấy USB lúc này sẽ bị lộ!");
        }
    }

    // Hàm cho phép lấy mô tả nhiệm vụ này
    public string GetMissionDescription()
    {
        return missionDescription;
    }
}
