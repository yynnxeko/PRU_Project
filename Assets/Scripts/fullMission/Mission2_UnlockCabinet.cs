using UnityEngine;

public class Mission2_UnlockCabinet : MissionStep
{
    [Header("Mô tả nhiệm vụ")]
    [TextArea]
    public string missionDescription = "Tìm mật khẩu trên máy tính để mở tủ phòng ngủ.";
    [Header("Thứ tự nhiệm vụ trong FullMissionManager")]
    public int missionIndex = 1;

    private bool hasPassword = false;

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
        hasPassword = false;
        Debug.Log($"Nhiệm vụ 2: {missionDescription}");
    }

    // Gọi hàm này khi tương tác với máy tính
    public void OnComputerInteracted()
    {
        if (hasPassword) return;

        hasPassword = true;
        Debug.Log("Đã tìm thấy mật khẩu! Giờ hãy đi mở tủ.");
    }

    // Gọi hàm này khi tương tác với tủ sau khi đã có pass
    public void OnCabinetOpened()
    {
        if (!hasPassword)
        {
            Debug.Log("Tủ đã khóa, bạn cần mật khẩu từ máy tính.");
            return;
        }

        if (IsCompleted) return;

        Debug.Log("Đã mở tủ thành công!");
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
