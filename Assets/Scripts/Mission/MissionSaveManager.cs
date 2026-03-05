using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionSaveManager : MonoBehaviour
{
    const string SAVE_KEY = "MissionSaveData";

    [Serializable]
    class SaveEntry
    {
        public string missionId;
        public int state; // MissionState as int
        public int retryCount;
    }

    [Serializable]
    class SaveData
    {
        public List<SaveEntry> entries = new List<SaveEntry>();
    }

    /// <summary>
    /// Lưu toàn bộ trạng thái missions vào PlayerPrefs
    /// </summary>
    public static void SaveProgress(Dictionary<string, MissionState> states, Dictionary<string, int> retryCounts)
    {
        SaveData data = new SaveData();

        foreach (var kvp in states)
        {
            SaveEntry entry = new SaveEntry
            {
                missionId = kvp.Key,
                state = (int)kvp.Value,
                retryCount = retryCounts.ContainsKey(kvp.Key) ? retryCounts[kvp.Key] : 0
            };
            data.entries.Add(entry);
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"[MissionSave] Saved {data.entries.Count} missions");
    }

    /// <summary>
    /// Load trạng thái missions từ PlayerPrefs
    /// </summary>
    public static void LoadProgress(
        out Dictionary<string, MissionState> states,
        out Dictionary<string, int> retryCounts)
    {
        states = new Dictionary<string, MissionState>();
        retryCounts = new Dictionary<string, int>();

        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("[MissionSave] No save data found");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null || data.entries == null) return;

        foreach (var entry in data.entries)
        {
            states[entry.missionId] = (MissionState)entry.state;
            retryCounts[entry.missionId] = entry.retryCount;
        }

        Debug.Log($"[MissionSave] Loaded {data.entries.Count} missions");
    }

    /// <summary>
    /// Xóa toàn bộ save data (dùng khi New Game)
    /// </summary>
    public static void ClearSaveData()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
        Debug.Log("[MissionSave] Save data cleared");
    }
}
