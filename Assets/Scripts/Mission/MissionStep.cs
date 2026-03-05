using UnityEngine;

public abstract class MissionStep : MonoBehaviour
{
    public bool IsCompleted { get; protected set; }
    public bool IsFailed { get; protected set; }

    public virtual void StartStep()
    {
        IsCompleted = false;
        IsFailed = false;
    }

    public virtual void UpdateStep()
    {
    }

    public virtual void EndStep()
    {
    }
    protected void CompleteStep()
    {
        IsCompleted = true;
    }
    protected void FailStep()
    {
        IsFailed = true;
    }

    public virtual void ResetStep()
    {
        IsCompleted = false;
        IsFailed = false;
    }

    /// <summary>
    /// Đánh dấu đã hoàn thành (dùng bởi FullMissionManager cho các step cũ).
    /// </summary>
    public void MarkAsCompleted()
    {
        IsCompleted = true;
    }

    /// <summary>
    /// Trả về mô tả nhiệm vụ. Các subclass nên override hàm này.
    /// </summary>
    public virtual string GetMissionDescription()
    {
        return "Nhiệm vụ hiện tại không có mô tả chi tiết.";
    }
}
