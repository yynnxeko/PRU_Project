using UnityEngine;

/// <summary>
/// Quản lý việc kiểm tra và xóa toàn bộ Save Data của game.
/// Dùng chủ yếu ở màn hình Menu (Startup Scene).
/// </summary>
public static class GameSaveManager
{
    // Kiểm tra xem ổ cứng có dữ liệu lưu chưa (Dấu hiệu là đã từng vào BedRoom lưu game)
    public static bool HasSaveData()
    {
        return PlayerPrefs.GetInt("HasSavedGame", 0) == 1;
    }

    // Ghi nhận game đã được lưu (Gọi khi vào BedRoom)
    public static void MarkGameAsSaved()
    {
        PlayerPrefs.SetInt("HasSavedGame", 1);
        PlayerPrefs.Save();
        Debug.Log("[GameSaveManager] Flag HasSavedGame = 1");
    }

    // Xóa sạch mọi thứ liên quan đến tiến trình (Dùng cho Chơi Mới)
    public static void DeleteAllSaveData()
    {
        // 1. Xóa trong PlayerPrefs (Mission, Day, Flag...)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // 2. Xóa đặc thù của Evidence trong RAM
        if (EvidenceManager.Instance != null)
        {
            EvidenceManager.Instance.DeleteAllSave();
        }

        // 3. Xóa dữ liệu Ngày và Buổi trong RAM (DayManager)
        if (DayManager.Instance != null)
        {
            DayManager.Instance.currentDay = 0;
            DayManager.Instance.currentPhase = DayPhase.Night; // Bắt đầu game là buổi Tối
        }

        // 4. Xóa nhiệm vụ hiện tại trong RAM
        if (FullMissionManager.Instance != null)
        {
            FullMissionManager.Instance.ResetAllProgress();
        }

        // 5. Reset các cờ (Bao gồm Cutscene, v.v.)
        if (GameFlagManager.Instance != null)
        {
            GameFlagManager.Instance.ResetAllFlags();
        }

        // 6. Reset tiến độ câu hỏi (DialogueMissionStep)
        DialogueMissionStep.ResetSavedIndex();

        // Xóa sạch cờ Chơi Tiếp để chắc chắn đây là ván mới
        PlayerPrefs.SetInt("IsContinueFlow", 0);
        PlayerPrefs.Save();

        Debug.Log("[GameSaveManager] Đã dọn dẹp sạch sẽ toàn bộ Ổ cứng và RAM! Sẵn sàng cho New Game.");
    }
}
