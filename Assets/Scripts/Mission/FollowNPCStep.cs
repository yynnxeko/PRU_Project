using UnityEngine;

public class FollowNPCStep : MissionStep
{
    public NPCPathFollower npc;
    public FollowCheckZone followZone;
    public SpeechBubble bubblePrefab;
    public Follower[] extraFollowers;
    public float maxLostTime = 3f;

    float lostTimer;
    bool npcWaiting;

    public override void StartStep()
    {
        base.StartStep();

        lostTimer = 0f;
        npcWaiting = false;

        npc.ResetPath();
        npc.Resume();
    }

    public override void UpdateStep()
    {
        if (IsCompleted || IsFailed) return;

        bool playerInside = followZone.PlayerInside;
        // NPC đi hết path
        if (npc.IsFinished)
        {
            npc.Pause();
            CompleteStep();
            return;
        }

        if (!playerInside)
        {
            if (!npcWaiting)
            {
                npc.Pause();
                foreach (var f in extraFollowers)
                    if (f != null) f.Pause();
                npcWaiting = true;
                Debug.Log("NPC WAITS");

                // Spawn bubble
                if (bubblePrefab != null)
                {
                    Debug.Log("Spawn bubble");
                    var bubble = Instantiate(
                        bubblePrefab,
                        npc.transform.position + Vector3.up * 1.5f,
                        Quaternion.identity
                    );
                    bubble.Init(npc.transform, Vector3.up * 1.5f);
                    bubble.Show("Đi lại đây nhanh lên", 2.5f);
                }
            }

            lostTimer += Time.deltaTime;
            if (lostTimer >= maxLostTime)
            {
                FailStep();
            }
        }
        else
        {
            lostTimer = 0f;

            if (npcWaiting)
            {
                npc.Resume();
                foreach (var f in extraFollowers)
                    if (f != null) f.Resume();
                npcWaiting = false;
            }
        }

    }



    public override void ResetStep()
    {
        base.ResetStep();

        lostTimer = 0f;
        npcWaiting = false;

        npc.ResetPath();
        npc.Pause();
    }
}
