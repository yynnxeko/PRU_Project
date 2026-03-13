using UnityEngine;

/// <summary>
/// Gắn script này vào một GameObject trống trong Scene "BedRoom".
/// Khi người chơi load vào phòng ngủ, game sẽ tiến hành lưu cứng lại mọi thứ:
/// 1. Số Ngày (Day)
/// 2. Danh sách Evidence hiện tại
/// 3. Đánh dấu HasSavedGame = 1 để nút Chơi Tiếp nhận diện được.
/// </summary>
public class CheckpointSaveRoot : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("[CheckpointSaveRoot] Đã vào phòng ngủ. Tiến hành ghi đè TẤT CẢ Save Data...");

        // 1. Phím tắt lưu cờ "Đã có save"
        GameSaveManager.MarkGameAsSaved();

        // 2. Lưu Ngày hiện tại
        if (DayManager.Instance != null)
        {
            // Bảo vệ: Nếu mới ngủ dậy ở BedRoom thì chắc chắn là buổi sáng
            // Gán luôn Morning trước khi lưu để khỏi bị lệch pha
            DayManager.Instance.currentPhase = DayPhase.Morning;
            DayManager.Instance.SaveDayData();
        }

        // 3. Khôi phục/Lưu Lại danh sách Bằng Chứng của đúng thời điểm đầu ngày ngủ dậy
        if (EvidenceManager.Instance != null)
        {
            // Do DayManager.FailDay hoặc AdvanceDay đã tạo snapshot, ta Backup phát nữa cho chắc
            EvidenceManager.Instance.BackupDayStart(); 
            EvidenceManager.Instance.SaveAllData();
        }
    }
}
