using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    public MissionStep[] steps;
    int currentStepIndex;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentStepIndex = 0;
        if (steps.Length > 0)
            steps[0].StartStep();  // ✅ Start step đầu tiên
    }

    void Update()
    {
        if (currentStepIndex >= steps.Length) return;

        MissionStep step = steps[currentStepIndex];
        step.UpdateStep();

        if (step.IsCompleted)
        {
            currentStepIndex++;
            if (currentStepIndex < steps.Length)
                steps[currentStepIndex].StartStep();  // Start step mới
        }
        else if (step.IsFailed)
        {
            ResetMission();
        }
    }

    void ResetMission()
    {
        foreach (var s in steps)
            s.ResetStep();

        currentStepIndex = 0;
        
        // 👉 KHI NHIỆM VỤ THẤT BẠI THÌ RESET NGÀY
        if (DayManager.Instance != null)
        {
            DayManager.Instance.FailDay();
        }
        else if (steps.Length > 0)
        {
            steps[0].StartStep();
        }
    }
}
