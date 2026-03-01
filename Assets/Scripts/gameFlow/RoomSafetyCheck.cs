using UnityEngine;
using System.Collections.Generic;

public class RoomSafetyCheck : MonoBehaviour
{
    [Header("Safety Settings")]
    public List<DayPhase> allowedPhases = new List<DayPhase>();
    public string roomName = "Unknown Room";

    private bool isPlayerInside = false;

    private void Update()
    {
        if (isPlayerInside)
        {
            CheckSafety();
        }
    }

    private void CheckSafety()
    {
        if (DayManager.Instance == null) return;

        // Nếu buổi hiện tại không nằm trong danh sách cho phép
        if (!allowedPhases.Contains(DayManager.Instance.currentPhase))
        {
            Debug.LogWarning($"Bị bảo vệ bắt! Bạn không được phép ở {roomName} vào buổi {DayManager.Instance.currentPhase}");
            
            // Tìm cách thông báo cho người chơi (visual) trước khi reset
            DayManager.Instance.FailDay();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            CheckSafety(); // Kiểm tra ngay khi vừa bước vào
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}
