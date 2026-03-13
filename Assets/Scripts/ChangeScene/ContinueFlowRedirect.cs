using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gắn script này vào 1 object trong Scene mà bạn mượn Camera (ví dụ Buss hoặc Internal Area).
/// Script sẽ check nếu ta đi từ dòng chữ "Chơi Tiếp" thì sẽ ép nhảy sang BedRoom ngay lập tức
/// Cùng với Spawn ID để khỏi lỗi vị trí.
/// </summary>
public class ContinueFlowRedirect : MonoBehaviour
{
    private void Start()
    {
        // Kiểm tra xem có đang trên chuyến xe Load "Chơi Tiếp" hay không
        if (PlayerPrefs.GetInt("IsContinueFlow", 0) == 1)
        {
            Debug.Log("[ContinueFlowRedirect] Phát hiện cờ chơi tiếp! Đang chuyển hướng qua BedRoom...");
            
            // Tắt cờ ngay lập tức để không bị lặp lại vòng lặp vô tận
            PlayerPrefs.SetInt("IsContinueFlow", 0);
            PlayerPrefs.Save();

            // Cho người chơi có 1 frame để mấy cái GameManager, Camera khởi tạo kịp
            StartCoroutine(RedirectToBedRoom());
        }
    }

    private IEnumerator RedirectToBedRoom()
    {
        yield return new WaitForSeconds(0.1f);

        // Theo yêu cầu: Chơi tiếp thì luôn ép về buổi sáng
        if (DayManager.Instance != null)
        {
            DayManager.Instance.currentPhase = DayPhase.Morning;
            DayManager.Instance.SaveDayData(); // Lưu lại luôn để đồng bộ
        }

        // Gắn ID Spawn để người chơi thức dậy ở giường
        DoorSceneChange.NextSpawnId = "to_bedroomDay";

        // Tải Scene phòng ngủ
        SceneManager.LoadScene("BedRoom");
    }
}
