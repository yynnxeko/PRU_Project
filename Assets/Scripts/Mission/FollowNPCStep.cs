using UnityEngine;

public class FollowNPCStep : MissionStep
{
    [Header("Main")]
    public NPCPathFollower npc;
    public FollowCheckZone followZone;

    [Header("Feedback")]
    public SpeechBubble bubblePrefab;
    public string waitText = "Đi lại đây nhanh lên";
    public float bubbleOffsetY = 1.5f;

    [Header("Followers")]
    public Follower[] extraFollowers;

    [Header("FullMission Settings (Optional)")]
    [Tooltip("Set to >= 0 to register with FullMissionManager")]
    public int missionIndex = -1;

    [Header("Fail Condition")]
    public float maxLostTime = 3f;

    float lostTimer;
    bool npcWaiting;
    SpeechBubble currentBubble;

    private void Start()
    {
        // Chỉ đăng ký nếu có index hợp lệ (>= 0)
        if (missionIndex >= 0 && FullMissionManager.Instance != null)
        {
            FullMissionManager.Instance.RegisterStep(missionIndex, this);
        }
    }

    public override void StartStep()
    {
        base.StartStep();

        lostTimer = 0f;
        npcWaiting = false;

        if (npc != null)
        {
            npc.ResetPath();
            npc.Resume();
        }
    }

    public void OnFollowComplete()
    {
        if (IsCompleted) return;
        CompleteStep();

        // Báo cho FullMissionManager nếu là nhiệm vụ chính
        if (missionIndex >= 0 && FullMissionManager.Instance != null)
        {
            FullMissionManager.Instance.ReportComplete();
        }
    }

    public override void UpdateStep()
    {
        //  KHÓA CUTSCENE BUS (nếu có)
        if (GameFlow.BusCutscene) return;

        // KHOÁ CUTSCENE LOBBY (Nếu đang diễn Cutscene đầu tiên ở sảnh)
        if (LoppyCutscene.isPlaying) return;

        if (IsCompleted || IsFailed) return;
        if (npc == null || !npc.gameObject.activeInHierarchy || followZone == null)
        {
            HideBubble();
            return;
        }

        bool playerInside = followZone.PlayerInside;

        // NPC đi hết path
        if (npc.IsFinished)
        {
            if (playerInside)
            {
                // Player đã trong zone → hoàn thành
                HideBubble();
                npc.Pause();
                OnFollowComplete();
            }
            else
            {
                // Player chưa đến → NPC dừng chờ + hiện bubble
                HandlePlayerLost();
            }
            return;
        }

        // NPC đang đi, kiểm tra player
        if (!playerInside)
        {
            HandlePlayerLost();
        }
        else
        {
            HandlePlayerBack();
        }
    }

    void HandlePlayerLost()
    {
        if (!npcWaiting)
        {
            npcWaiting = true;
            npc.Pause();

            foreach (var f in extraFollowers)
                if (f != null) f.Pause();

            ShowBubble();
            Debug.Log("[FollowStep] NPC WAITS");
        }

        // Chỉ đợi + hiện bubble, không fail
        lostTimer += Time.deltaTime;
    }

    void HandlePlayerBack()
    {
        lostTimer = 0f;

        if (!npcWaiting) return;

        npcWaiting = false;
        npc.Resume();

        foreach (var f in extraFollowers)
            if (f != null) f.Resume();

        HideBubble();
        Debug.Log("[FollowStep] NPC RESUMED");
    }

    void ShowBubble()
    {
        if (bubblePrefab == null || currentBubble != null) return;

        currentBubble = Instantiate(
            bubblePrefab,
            npc.transform.position + Vector3.up * bubbleOffsetY,
            Quaternion.identity
        );

        currentBubble.Init(npc.transform, Vector3.up * bubbleOffsetY);
        currentBubble.Show(waitText, maxLostTime);
    }

    void HideBubble()
    {
        if (currentBubble != null)
        {
            Destroy(currentBubble.gameObject);
            currentBubble = null;
        }
    }

    public override void ResetStep()
    {
        base.ResetStep();

        lostTimer = 0f;
        npcWaiting = false;

        HideBubble();

        if (npc != null)
        {
            npc.ResetPath();
            npc.Pause();
        }
    }

    private void OnDisable()
    {
        HideBubble();
    }

    private void OnDestroy()
    {
        HideBubble();
    }
}