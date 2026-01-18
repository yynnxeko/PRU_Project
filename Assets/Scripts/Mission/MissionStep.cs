using UnityEngine;

public abstract class MissionStep : MonoBehaviour
{
    public bool IsCompleted { get; protected set; }

    public virtual void StartStep()
    {
        IsCompleted = false;
    }

    public virtual void UpdateStep()
    {
    }

    public virtual void EndStep()
    {
    }

    public virtual void ResetStep()
    {
        IsCompleted = false;
    }
}
