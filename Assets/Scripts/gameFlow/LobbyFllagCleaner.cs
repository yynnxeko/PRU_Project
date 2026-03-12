using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyFlagCleaner : MonoBehaviour
{
    public List<string> flagsToClear = new List<string> { "npc_ready_in_lobby" };

    // Flags sẽ KHÔNG bị xóa (luôn giữ nguyên dù có trong danh sách)
    // private static readonly HashSet<string> protectedFlags = new HashSet<string>
    // {
    //     "go_to_medical"
    // };

    IEnumerator Start()
    {
        yield return null; // đợi 1 frame

        if (GameFlagManager.Instance != null)
        {
            foreach (string flag in flagsToClear)
            {
                // if (protectedFlags.Contains(flag))
                // {
                //     Debug.Log($"[LobbyFlagCleaner] Bỏ qua flag được bảo vệ: '{flag}'");
                //     continue;
                // }
                GameFlagManager.Instance.SetFlag(flag, false);
            }
            Debug.Log($"Lobby flags cleared: {flagsToClear.Count} flag(s).");
        }
    }
}