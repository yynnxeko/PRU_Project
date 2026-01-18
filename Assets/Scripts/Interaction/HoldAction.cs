using UnityEngine;

[System.Serializable]
public class HoldAction
{
    public float requiredHoldTime = 1.5f;

    float timer;
    bool holding;

    public void StartHold()
    {
        holding = true;
        timer = 0f;
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
        timer = 0f;
    }

    public float GetProgress()
    {
        return Mathf.Clamp01(timer / requiredHoldTime);
    }
}
