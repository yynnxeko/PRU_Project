using UnityEngine;

public class FollowTargetStep : MissionStep
{
    public Transform player;
    public Transform targetNPC;

    public float maxDistance = 3f;
    public float requiredTime = 5f;

    float followTimer;

    public override void UpdateStep()
    {
        float dist = Vector2.Distance(player.position, targetNPC.position);

        if (dist <= maxDistance)
        {
            followTimer += Time.deltaTime;

            if (followTimer >= requiredTime)
            {
                IsCompleted = true;
            }
        }
        else
        {
            // Đi quá xa → FAIL
            FailStep();
        }
    }

    public override void ResetStep()
    {
        base.ResetStep();
        followTimer = 0f;
    }
}
