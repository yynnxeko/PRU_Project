using UnityEngine;

[System.Serializable]
public class HoldAction
{
    public float requiredHoldTime = 1.5f;
    public bool resetOnStart = true;

    float timer;
    bool holding;

    public void StartHold()
    {
        holding = true;
        if (resetOnStart) timer = 0f;
    }

    public bool UpdateHold(float deltaTime)
    {
        if (!holding) return false;

        timer += deltaTime;
        return timer >= requiredHoldTime;
    }

    public void CancelHold()
    {
        holding = false;
    }

    public float GetProgress()
    {
        return Mathf.Clamp01(timer / requiredHoldTime);
    }
}
