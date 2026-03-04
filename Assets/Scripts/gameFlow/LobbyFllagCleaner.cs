using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyFlagCleaner : MonoBehaviour
{
    public List<string> flagsToClear = new List<string> { "npc_ready_in_lobby" };

    IEnumerator Start()
    {
        yield return null; // đợi 1 frame

        if (GameFlagManager.Instance != null)
        {
            foreach (string flag in flagsToClear)
            {
                GameFlagManager.Instance.SetFlag(flag, false);
            }
            Debug.Log($"Lobby flags cleared: {flagsToClear.Count} flag(s).");
        }
    }
}