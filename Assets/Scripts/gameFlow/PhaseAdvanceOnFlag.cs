using UnityEngine;

/// <summary>
/// Gắn vào scene (ví dụ itroom).
/// Khi flag được chỉ định bật TRUE → tự động gọi DayManager.AdvancePhase().
/// </summary>
public class PhaseAdvanceOnFlag : MonoBehaviour
{
    [Tooltip("Tên flag cần lắng nghe (vd: it_toLobby)")]
    public string flagName = "it_toLobby";

    private bool hasTriggered = false;

    private void OnEnable()
    {
        GameFlagManager.OnFlagChanged += OnFlagChanged;
    }

    private void OnDisable()
    {
        GameFlagManager.OnFlagChanged -= OnFlagChanged;
    }

    private void OnFlagChanged(string name, bool value)
    {
        if (hasTriggered) return;
        if (name == flagName && value)
        {
            hasTriggered = true;
            if (DayManager.Instance != null)
            {
                DayManager.Instance.AdvancePhase();
                Debug.Log($"[PhaseAdvanceOnFlag] Flag '{flagName}' triggered → AdvancePhase()");
            }
        }
    }
}
