using UnityEngine;
using System.Collections.Generic;

public class GameFlagManager : MonoBehaviour
{
    public static GameFlagManager Instance { get; private set; }

    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Bật hoặc tắt một cờ (flag).
    /// </summary>
    public void SetFlag(string flagName, bool value)
    {
        if (flags.ContainsKey(flagName))
            flags[flagName] = value;
        else
            flags.Add(flagName, value);
        
        Debug.Log($"[GameFlagManager] Flag '{flagName}' set to: {value}");
    }

    /// <summary>
    /// Kiểm tra trạng thái của một cờ. Trả về false nếu cờ chưa tồn tại.
    /// </summary>
    public bool GetFlag(string flagName)
    {
        if (flags.TryGetValue(flagName, out bool value))
            return value;
        return false;
    }
}
