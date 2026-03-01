using UnityEngine;
using UnityEngine.SceneManagement;

public enum DayPhase
{
    Morning,
    Noon,
    Night
}

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [Header("Day State")]
    public int currentDay = 1;
    public DayPhase currentPhase = DayPhase.Morning;

    public bool isNight => currentPhase == DayPhase.Night;

    [Header("Scene Settings")]
    public string bedroomSceneName = "Map_Bedroom"; // Scene sáng sớm

    [Header("UI References")]
    public GameObject dayStartUIPrefab; // Kéo Prefab hiện chữ đỏ vào đây (nếu có)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Hiện thông báo ngày 1 khi bắt đầu (hoặc trong Start)
    }

    private void Start()
    {
        ShowDayStartNotification();
    }

    /// <summary>
    /// Hiện chữ đỏ: "Ngày X: [Nhiệm vụ]"
    /// </summary>
    public void ShowDayStartNotification()
    {
        string message = $"Ngày {currentDay}: ";
        if (currentDay == 1) message += "Làm quen việc";
        // Thêm các ngày khác ở đây...

        Debug.Log("<color=red>" + message + "</color>");
        
        // Logic thực tế để hiện UI (ví dụ Instantiate prefab thông báo)
        if (dayStartUIPrefab != null)
        {
            Instantiate(dayStartUIPrefab);
        }
    }

    // ====================== GAME 2 HOOKS ======================

    /// <summary>
    /// Cửa IT Room gọi hàm này khi người chơi vào bàn làm việc Game 2
    /// </summary>
    public void OnWorkGameComplete(int score)
    {
        if (score >= 10)
        {
            if (currentPhase == DayPhase.Morning)
            {
                SetPhase(DayPhase.Noon);
                Debug.Log("Làm xong việc buổi sáng -> Đi ăn trưa!");
                // Hiện thông báo "Đi ăn trưa thôi"
            }
            else if (currentPhase == DayPhase.Noon)
            {
                SetPhase(DayPhase.Night);
                Debug.Log("Làm xong việc buổi chiều -> Về phòng ngủ!");
                // Hiện thông báo "Hết giờ làm, về ngủ thôi"
            }
        }
    }

    // ==========================================================

    /// <summary>
    /// Lấy tên scene chính xác dựa trên thời điểm trong ngày.
    /// Ví dụ: Nếu là Night và baseName là "Lobby", trả về "Lobby_Night".
    /// </summary>
    public string GetTargetSceneName(string baseName)
    {
        if (currentPhase == DayPhase.Night)
        {
            return baseName + "_Night";
        }
        
        // Hiện tại chỉ hỗ trợ suffix _Night, bạn có thể thêm _Noon nếu cần
        return baseName;
    }

    /// <summary>
    /// Chuyển sang một buổi cụ thể.
    /// </summary>
    public void SetPhase(DayPhase newPhase)
    {
        currentPhase = newPhase;
        Debug.Log("Time changed to: " + currentPhase);
    }

    /// <summary>
    /// Tiến lên buổi tiếp theo.
    /// </summary>
    public void AdvancePhase()
    {
        if (currentPhase == DayPhase.Morning) currentPhase = DayPhase.Noon;
        else if (currentPhase == DayPhase.Noon) currentPhase = DayPhase.Night;
        
        Debug.Log("Time advanced to: " + currentPhase);
    }

    /// <summary>
    /// Gọi khi người chơi bị bắt hoặc làm sai nhiệm vụ.
    /// Reset lại toàn bộ những gì làm trong ngày, về lại phòng ngủ đầu ngày hiện tại.
    /// </summary>
    public void FailDay()
    {
        Debug.Log("Day Failed! Resetting to start of Day " + currentDay);
        
        // 1. Reset nhiệm vụ (Nếu bạn có MissionManager)
        if (MissionManager.Instance != null)
        {
            // MissionManager logic
        }

        // 2. Reset Evidence (Khôi phục snapshot đầu ngày)
        if (EvidenceManager.Instance != null)
        {
            EvidenceManager.Instance.RestoreDayStart();
        }

        // 3. Spawn về phòng ngủ
        // Reset về sáng sớm
        currentPhase = DayPhase.Morning;
        SceneManager.LoadScene(bedroomSceneName);
    }

    /// <summary>
    /// Gọi khi đi ngủ vào ban đêm để sang ngày hôm sau.
    /// </summary>
    public void AdvanceDay()
    {
        currentDay++;
        currentPhase = DayPhase.Morning;
        Debug.Log("Day Advanced! Starting Day " + currentDay);

        // 1. Lưu snapshot mới cho đầu ngày mới
        if (EvidenceManager.Instance != null)
        {
            EvidenceManager.Instance.BackupDayStart();
        }

        // 2. Chuyển về scene phòng ngủ ban ngày
        SceneManager.LoadScene(bedroomSceneName);
    }
}
