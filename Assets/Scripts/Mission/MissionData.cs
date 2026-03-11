using UnityEngine;

[CreateAssetMenu(fileName = "NewMission", menuName = "Mission/MissionData")]
public class MissionData : ScriptableObject
{
    [Header("Info")]
    public string missionId;
    public string title;
    [TextArea(2, 4)]
    public string description;

    [Header("Steps")]
    [Tooltip("Kéo các MissionStep prefab hoặc GameObject chứa step vào đây")]
    public MissionStep[] steps;

    [Header("Prerequisites")]
    [Tooltip("Các mission cần hoàn thành trước khi mission này Available")]
    public MissionData[] prerequisites;

    [Header("Fail Settings")]
    public bool canFail = true;
    public bool canRetry = true;
    public int maxRetries = 3;
}
