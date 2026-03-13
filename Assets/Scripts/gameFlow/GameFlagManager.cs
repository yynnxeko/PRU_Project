using UnityEngine;
using System;
using System.Collections.Generic;

public class GameFlagManager : MonoBehaviour
{
    public static GameFlagManager Instance { get; private set; }

    /// <summary>
    /// Event phát khi bất kỳ flag nào thay đổi. Param: (flagName, newValue)
    /// </summary>
    public static event Action<string, bool> OnFlagChanged;

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
        OnFlagChanged?.Invoke(flagName, value);
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

    /// <summary>
    /// Xóa toàn bộ các cờ hiện có (Dùng khi Chơi Mới).
    /// </summary>
    public void ResetAllFlags()
    {
        flags.Clear();
        Debug.Log("[GameFlagManager] Đã xóa toàn bộ flags trong bộ nhớ!");
    }
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            flags.Clear();
            Debug.Log("All flags manually cleared!");
        }
    }
#endif
}
