using UnityEngine;

/// <summary>
/// Gắn vào 1 GameObject trong scene Hospital.
/// Khi CorpseSearchable phát event OnUSBFound → hoàn thành Mission 2
/// và tắt cờ để lần sau bị chích điện không vào Hospital nữa.
/// </summary>
public class Mission2_HospitalComplete : MonoBehaviour
{
    private bool isListening = false;

    void Start()
    {
        // Chỉ kích hoạt nếu đang ở Mission 2
        if (FullMissionManager.Instance != null && FullMissionManager.Instance.GetCurrentMissionIndex() == 1)
        {
            isListening = true;
            CorpseSearchable.OnUSBFound += OnUSBFoundHandler;
            Debug.Log("[Mission2_HospitalComplete] Đang đợi player tìm USB...");
        }
        else
        {
            Debug.Log("[Mission2_HospitalComplete] Không phải Mission 2 → disable");
            enabled = false;
        }
    }

    void OnDestroy()
    {
        if (isListening)
        {
            CorpseSearchable.OnUSBFound -= OnUSBFoundHandler;
        }
    }

    private void OnUSBFoundHandler()
    {
        Debug.Log("[Mission2_HospitalComplete] USB đã tìm thấy! Hoàn thành Mission 2.");

        // Hủy đăng ký event
        CorpseSearchable.OnUSBFound -= OnUSBFoundHandler;
        isListening = false;

        // === TẮT CỜ để lần sau bị chích điện KHÔNG vào Hospital nữa ===
        if (GameFlagManager.Instance != null)
        {
            // Tắt mission_accepted → FailedRoutine sẽ không bật go_to_medical nữa
            GameFlagManager.Instance.SetFlag("mission_accepted", false);

            // Tắt go_to_medical phòng trường hợp còn sót
            GameFlagManager.Instance.SetFlag("go_to_medical", false);

            Debug.Log("[Mission2_HospitalComplete] Đã tắt cờ mission_accepted + go_to_medical");
        }

        // Hoàn thành Mission 2
        CompleteStep();
    }

    private void CompleteStep()
    {
        if (FullMissionManager.Instance != null)
        {
            FullMissionManager.Instance.ReportComplete();
            Debug.Log("[Mission2_HospitalComplete] FullMissionManager.ReportComplete() → Mission 3!");
        }
    }
}
