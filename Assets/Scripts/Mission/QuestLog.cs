using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class QuestLog : MonoBehaviour
{
    public static QuestLog Instance;

    [Header("All Missions (kéo tất cả MissionData vào đây)")]
    public MissionData[] allMissions;

    [Header("Events")]
    public UnityEvent<string> onMissionUnlocked;    // missionId
    public UnityEvent<string> onMissionStarted;     // missionId
    public UnityEvent<string> onMissionCompleted;   // missionId
    public UnityEvent<string> onMissionFailed;      // missionId

    // Runtime tracking
    Dictionary<string, MissionState> missionStates = new Dictionary<string, MissionState>();
    Dictionary<string, int> retryCounts = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        LoadProgress();
        InitializeMissions();
    }

    /// <summary>
    /// Khởi tạo trạng thái ban đầu cho missions chưa có trong save
    /// </summary>
    void InitializeMissions()
    {
        foreach (var mission in allMissions)
        {
            if (mission == null) continue;

            if (!missionStates.ContainsKey(mission.missionId))
            {
                // Kiểm tra prerequisites
                bool prereqMet = CheckPrerequisites(mission);
                missionStates[mission.missionId] = prereqMet ? MissionState.Available : MissionState.Locked;
                retryCounts[mission.missionId] = 0;
            }
        }

        Debug.Log($"[QuestLog] Initialized {missionStates.Count} missions");
    }

    // ─────────────────── PUBLIC API ───────────────────

    /// <summary>
    /// Lấy trạng thái hiện tại của mission
    /// </summary>
    public MissionState GetMissionState(string missionId)
    {
        return missionStates.ContainsKey(missionId) ? missionStates[missionId] : MissionState.Locked;
    }

    /// <summary>
    /// Kiểm tra mission đã hoàn thành chưa
    /// </summary>
    public bool IsMissionCompleted(string missionId)
    {
        return GetMissionState(missionId) == MissionState.Completed;
    }

    /// <summary>
    /// Bắt đầu mission (chuyển từ Available → Active)
    /// </summary>
    public bool StartMission(string missionId)
    {
        MissionState state = GetMissionState(missionId);

        if (state != MissionState.Available && state != MissionState.Failed)
        {
            Debug.LogWarning($"[QuestLog] Cannot start mission '{missionId}' — state is {state}");
            return false;
        }

        missionStates[missionId] = MissionState.Active;
        onMissionStarted?.Invoke(missionId);
        SaveProgress();

        Debug.Log($"[QuestLog] Mission started: {missionId}");
        return true;
    }

    /// <summary>
    /// Đánh dấu mission hoàn thành (Active → Completed)
    /// </summary>
    public void CompleteMission(string missionId)
    {
        missionStates[missionId] = MissionState.Completed;
        onMissionCompleted?.Invoke(missionId);

        Debug.Log($"[QuestLog] Mission completed: {missionId}");

        // Tự động unlock missions tiếp theo
        UnlockNextMissions(missionId);
        SaveProgress();
    }

    /// <summary>
    /// Đánh dấu mission thất bại (Active → Failed)
    /// </summary>
    public void FailMission(string missionId)
    {
        missionStates[missionId] = MissionState.Failed;

        if (!retryCounts.ContainsKey(missionId))
            retryCounts[missionId] = 0;
        retryCounts[missionId]++;

        onMissionFailed?.Invoke(missionId);
        SaveProgress();

        Debug.Log($"[QuestLog] Mission failed: {missionId} (retry {retryCounts[missionId]})");
    }

    /// <summary>
    /// Đặt lại mission về Available (khi player chọn Retry)
    /// </summary>
    public void SetMissionAvailable(string missionId)
    {
        missionStates[missionId] = MissionState.Available;
        SaveProgress();
    }

    /// <summary>
    /// Lấy số lần retry của mission
    /// </summary>
    public int GetRetryCount(string missionId)
    {
        return retryCounts.ContainsKey(missionId) ? retryCounts[missionId] : 0;
    }

    /// <summary>
    /// Lấy MissionData theo missionId
    /// </summary>
    public MissionData GetMissionData(string missionId)
    {
        foreach (var m in allMissions)
        {
            if (m != null && m.missionId == missionId)
                return m;
        }
        return null;
    }

    /// <summary>
    /// Lấy tất cả missions và state (cho UI Quest Log)
    /// </summary>
    public List<(MissionData data, MissionState state)> GetAllMissions()
    {
        var result = new List<(MissionData, MissionState)>();
        foreach (var m in allMissions)
        {
            if (m == null) continue;
            MissionState state = GetMissionState(m.missionId);
            result.Add((m, state));
        }
        return result;
    }

    // ─────────────────── INTERNAL ───────────────────

    /// <summary>
    /// Kiểm tra tất cả prerequisites đã hoàn thành chưa
    /// </summary>
    bool CheckPrerequisites(MissionData mission)
    {
        if (mission.prerequisites == null || mission.prerequisites.Length == 0)
            return true;

        foreach (var prereq in mission.prerequisites)
        {
            if (prereq == null) continue;
            if (!IsMissionCompleted(prereq.missionId))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Khi 1 mission hoàn thành → kiểm tra và unlock missions mới
    /// </summary>
    void UnlockNextMissions(string completedMissionId)
    {
        foreach (var mission in allMissions)
        {
            if (mission == null) continue;
            if (GetMissionState(mission.missionId) != MissionState.Locked) continue;

            if (CheckPrerequisites(mission))
            {
                missionStates[mission.missionId] = MissionState.Available;
                onMissionUnlocked?.Invoke(mission.missionId);
                Debug.Log($"[QuestLog] Mission unlocked: {mission.missionId}");
            }
        }
    }

    // ─────────────────── SAVE / LOAD ───────────────────

    void SaveProgress()
    {
        MissionSaveManager.SaveProgress(missionStates, retryCounts);
    }

    void LoadProgress()
    {
        MissionSaveManager.LoadProgress(out missionStates, out retryCounts);
    }

    /// <summary>
    /// Reset toàn bộ progress (New Game)
    /// </summary>
    public void ResetAllProgress()
    {
        MissionSaveManager.ClearSaveData();
        missionStates.Clear();
        retryCounts.Clear();
        InitializeMissions();

        Debug.Log("[QuestLog] All progress reset");
    }
}
