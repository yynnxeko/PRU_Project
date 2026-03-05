using UnityEngine;

/// <summary>
/// Script đơn giản để ẩn/hiện NPC dựa theo số ngày trong DayManager.
/// </summary>
public class NpcDayActivator : MonoBehaviour
{
    [Header("Cài đặt")]
    [Tooltip("Ngày bắt đầu xuất hiện NPC này (Ví dụ: 2 nghĩa là từ ngày 2 mới hiện)")]
    public int appearFromDay = 2;

    void Start()
    {
        CheckAppearance();
    }

    private void CheckAppearance()
    {
        if (DayManager.Instance == null)
        {
            Debug.LogWarning("[NpcDayActivator] Không tìm thấy DayManager.Instance!");
            return;
        }

        // Nếu ngày hiện tại nhỏ hơn ngày quy định -> Ẩn NPC đi
        if (DayManager.Instance.currentDay < appearFromDay)
        {
            gameObject.SetActive(false);
            Debug.Log($"[NpcDayActivator] {gameObject.name} bị ẩn vì chưa tới ngày {appearFromDay} (Hiện tại: {DayManager.Instance.currentDay})");
        }
        else
        {
            gameObject.SetActive(true);
            Debug.Log($"[NpcDayActivator] {gameObject.name} xuất hiện (Ngày {DayManager.Instance.currentDay} >= {appearFromDay})");
        }
    }
}
