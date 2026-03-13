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

        // 2. Xóa đặc thù của Evidence
        if (EvidenceManager.Instance != null)
        {
            EvidenceManager.Instance.DeleteAllSave();
        }

        Debug.Log("[GameSaveManager] Đã xóa TẤT CẢ Save Data! Sẵn sàng cho New Game.");
    }
}
