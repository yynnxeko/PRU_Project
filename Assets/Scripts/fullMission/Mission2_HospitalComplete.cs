using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gắn vào 1 GameObject trong scene Hospital.
/// Khi CorpseSearchable phát event OnUSBFound → hoàn thành Mission 2.
/// Cờ go_to_medical sẽ giữ TRUE cho đến khi player rời Hospital scene.
/// </summary>
public class Mission2_HospitalComplete : MonoBehaviour
{
    private bool isListening = false;
    private bool usbFound = false;

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

    void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
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
        usbFound = true;

        // Tắt mission_accepted → FailedRoutine sẽ không bật go_to_medical nữa
        if (GameFlagManager.Instance != null)
        {
            GameFlagManager.Instance.SetFlag("mission_accepted", false);
            Debug.Log("[Mission2_HospitalComplete] Đã tắt cờ mission_accepted");
        }

        // GIỮ go_to_medical = true để RoomSafetyCheck không bắt khi còn ở Hospital
        // Sẽ tắt khi player rời scene (OnSceneUnloaded)

        // Hoàn thành Mission 2
        CompleteStep();
    }

    /// <summary>
    /// Khi scene Hospital bị unload (player ra khỏi) → tắt go_to_medical
    /// </summary>
    private void OnSceneUnloaded(Scene scene)
    {
        
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
