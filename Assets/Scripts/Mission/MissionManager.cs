using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public MissionStep[] steps;
    int currentStepIndex;

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
        if (steps.Length > 0)
            steps[0].StartStep();
    }
}
