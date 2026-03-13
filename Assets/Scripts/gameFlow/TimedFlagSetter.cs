using UnityEngine;
using System.Collections;

/// <summary>
/// Tự động bật một cờ (flag) sau một khoảng thời gian chờ.
/// </summary>
public class TimedFlagSetter : MonoBehaviour
{
    [Header("Cấu hình")]
    public string flagToSet = "canteen_ToLobby";
    public float delayInSeconds = 10f;
    public bool setOnStart = true;
    public int targetDay = 2; // Chỉ chạy vào ngày này

    void Start()
    {
        // Chỉ chạy nếu được tích chọn và đúng ngày mục tiêu
        if (setOnStart && DayManager.Instance != null && DayManager.Instance.currentDay == targetDay)
        {
            StartCoroutine(SetFlagRoutine());
        }
    }

    private IEnumerator SetFlagRoutine()
    {
        // Nếu cờ đã bật từ trước rồi thì thôi, không đếm ngược nữa cho đỡ tốn tài nguyên
        if (GameFlagManager.Instance != null && GameFlagManager.Instance.GetFlag(flagToSet))
        {
            Debug.Log($"[TimedFlagSetter] Cờ '{flagToSet}' đã bật sẵn, không cần chạy nữa.");
            yield break;
        }

        Debug.Log($"[TimedFlagSetter] Đang đợi {delayInSeconds} giây để bật cờ '{flagToSet}' cho ngày {targetDay}...");

        yield return new WaitForSeconds(delayInSeconds);

        if (GameFlagManager.Instance != null)
        {
            // Kiểm tra lại lần nữa trước khi bật (đề phòng người chơi làm xong việc trong lúc chờ)
            if (!GameFlagManager.Instance.GetFlag(flagToSet))
            {
                GameFlagManager.Instance.SetFlag(flagToSet, true);
                Debug.Log($"[TimedFlagSetter] Đã tự động bật cờ '{flagToSet}'!");
            }
        }
    }
}
