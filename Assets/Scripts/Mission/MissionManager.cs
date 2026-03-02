using UnityEngine;

public class MissionManager : MonoBehaviour
{
    [Header("Mission List")]
    public MissionStep[] allMissions;
    public int currentStepIndex = 0;

    // Đã xóa Awake và DontDestroyOnLoad theo yêu cầu của bạn

    void Start()
    {
        // Không reset index ở Start để giữ tiến trình khi chuyển scene
        if (allMissions != null && allMissions.Length > 0 && currentStepIndex < allMissions.Length)
        {
            // Chỉ start nếu mission chưa active (đề phòng chuyển cảnh)
            if (!allMissions[currentStepIndex].IsCompleted && !allMissions[currentStepIndex].IsFailed)
                allMissions[currentStepIndex].StartStep();
        }
    }

    void Update()
    {
        if (allMissions == null || currentStepIndex >= allMissions.Length) return;

        MissionStep step = allMissions[currentStepIndex];
        if (step == null) return;

        // Cập nhật logic nhiệm vụ hiện tại
        if (Time.frameCount % 120 == 0)
            Debug.Log($"[MissionManager] Updating Step {currentStepIndex}: {step.gameObject.name}");
        
        step.UpdateStep();

        if (step.IsCompleted)
        {
            Debug.Log($"Mission Step {currentStepIndex} Completed!");
            currentStepIndex++;
            
            if (currentStepIndex < allMissions.Length)
            {
                if (allMissions[currentStepIndex] != null)
                    allMissions[currentStepIndex].StartStep();
            }
            else
            {
                Debug.Log("All Missions Completed!");
            }
        }
        else if (step.IsFailed)
        {
            Debug.Log($"Mission Step {currentStepIndex} Failed! Resetting...");
            if (DayManager.Instance != null)
                DayManager.Instance.FailDay("");
            else
                ResetCurrentMission(); // Fallback if no DayManager
        }
    }

    /// <summary>
    /// Reset lại đúng nhiệm vụ hiện tại (không nhảy index)
    /// </summary>
    public void ResetCurrentMission()
    {
        if (allMissions != null && currentStepIndex < allMissions.Length)
        {
            if (allMissions[currentStepIndex] != null)
            {
                allMissions[currentStepIndex].ResetStep();
                allMissions[currentStepIndex].StartStep();
            }
        }
    }
}
