using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NpcMoveNav : MonoBehaviour
{
    [System.Serializable]
    public class WaypointInfo
    {
        public Transform point;
        public bool isChair = false;
        public Vector2 sitFaceDirection = new Vector2(0, -1);
    }

    [Header("Path Settings")]
    public List<WaypointInfo> waypoints;
    public float arriveDistance = 0.15f;

    [Header("Walk Flag")]
    [Tooltip("Nếu flag này ON → đi path bình thường rồi tự tắt. Nếu OFF → ngồi thẳng vào ghế.")]
    public string walkFlag = "";

    [Header("Hide On Flag")]
    [Tooltip("Khi flag này bật TRUE → ẩn NPC (ví dụ: it_toLobby)")]
    public string hideOnFlag = "";

    [Header("Status")]
    public bool canMove = true;
    public bool IsSitting = false;

    int currentIndex = 0;
    NavMeshAgent agent;
    Animator anim;
    bool isFinished = false;
    bool isSittingProcessStarted = false;

    // Cache flag ở Awake để tất cả NPC đọc TRƯỚC khi bất kỳ ai clear trong Start
    bool cachedShouldWalk = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Đọc flag sớm (Awake chạy trước tất cả Start)
        if (!string.IsNullOrEmpty(walkFlag)
            && GameFlagManager.Instance != null)
        {
            cachedShouldWalk = GameFlagManager.Instance.GetFlag(walkFlag);
            Debug.Log($"[NpcMoveNav] Awake – cached '{walkFlag}' = {cachedShouldWalk}");
        }
    }

    void OnEnable()
    {
        GameFlagManager.OnFlagChanged += OnFlagChanged;
    }

    void OnDisable()
    {
        GameFlagManager.OnFlagChanged -= OnFlagChanged;
    }

    void OnFlagChanged(string flagName, bool value)
    {
        if (!string.IsNullOrEmpty(hideOnFlag) && flagName == hideOnFlag && value)
        {
            Debug.Log($"[NpcMoveNav] Flag '{hideOnFlag}' = TRUE → ẨN NPC '{name}'");
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        // Dùng giá trị đã cache từ Awake (tránh race-condition giữa các NPC)
        if (!string.IsNullOrEmpty(walkFlag))
        {
            if (cachedShouldWalk)
            {
                // Tự tắt flag → lần sau vào scene sẽ ngồi thẳng
                if (GameFlagManager.Instance != null)
                    GameFlagManager.Instance.SetFlag(walkFlag, false);

                Debug.Log($"[NpcMoveNav] Start – walking path (flag was ON)");
            }
            else
            {
                Debug.Log($"[NpcMoveNav] Start – flag OFF → seated instantly");
                SkipToChair();
                return;
            }
        }

        if (waypoints.Count > 0 && agent.isOnNavMesh)
            agent.SetDestination(waypoints[currentIndex].point.position);
    }

    /// <summary>
    /// Warp NPC thẳng đến ghế cuối cùng trong path, ngồi ngay.
    /// </summary>
    public void SkipToChair()
    {
        WaypointInfo chairWP = null;
        for (int i = waypoints.Count - 1; i >= 0; i--)
        {
            if (waypoints[i].isChair)
            {
                chairWP = waypoints[i];
                break;
            }
        }

        if (chairWP == null || chairWP.point == null) return;

        agent.Warp(chairWP.point.position);
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        IsSitting = true;
        isFinished = true;

        if (anim)
        {
            anim.SetBool("IsMoving", false);
            anim.SetBool("IsSitting", true);
            anim.SetFloat("LastInputX", chairWP.sitFaceDirection.x);
            anim.SetFloat("LastInputY", chairWP.sitFaceDirection.y);
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputY", 0);
        }

        Debug.Log($"[NpcMoveNav] Flag OFF → seated instantly");
    }

    void Update()
    {
        if (IsSitting || isSittingProcessStarted) return;

        if (!agent.isOnNavMesh) return;

        if (!canMove || isFinished)
        {
            agent.isStopped = true;
            UpdateAnimation(false);
            return;
        }

        agent.isStopped = false;

        Move();
        CheckDestination();
        UpdateAnimation(true);
    }

    void Move()
    {
        if (currentIndex < waypoints.Count && agent.isOnNavMesh)
            agent.SetDestination(waypoints[currentIndex].point.position);
    }

    void CheckDestination()
    {
        if (!agent.isOnNavMesh) return;
        if (agent.pathPending) return;
        if (agent.remainingDistance > agent.stoppingDistance + arriveDistance) return;

        WaypointInfo wp = waypoints[currentIndex];

        if (wp.isChair)
        {
            if (!isSittingProcessStarted)
                StartCoroutine(SitDownSmoothly(wp));
        }
        else
        {
            currentIndex++;
            if (currentIndex >= waypoints.Count)
                isFinished = true;
        }
    }

    IEnumerator SitDownSmoothly(WaypointInfo wp)
    {
        isSittingProcessStarted = true;

        // Giảm tốc cho mượt
        float originalSpeed = agent.speed;
        agent.speed = 1.2f;

        agent.SetDestination(wp.point.position);

        // ← CHỈ ĐỂ NPC ĐI TỚI CUỐI CÙNG (KHÔNG TẮT NAVMESH SỚM)
        while (agent.isOnNavMesh && (agent.pathPending || agent.remainingDistance > 0.02f))
        {
            UpdateAnimation(true);
            yield return null;
        }

        // ← CHỈ TẮT NAVMESH KHI ĐÃ NGỒI THẬT
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        IsSitting = true;
        isFinished = true;
        agent.speed = originalSpeed;

        if (anim)
        {
            UpdateAnimation(false);
            anim.SetBool("IsSitting", true);
            anim.SetFloat("LastInputX", wp.sitFaceDirection.x);
            anim.SetFloat("LastInputY", wp.sitFaceDirection.y);
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputY", 0);
        }

        isSittingProcessStarted = false;
    }

    void UpdateAnimation(bool isMoving)
    {
        if (!anim) return;

        anim.SetBool("IsMoving", isMoving);

        if (isMoving && agent.hasPath)
        {
            Vector3 v = agent.velocity;
            if (v.sqrMagnitude > 0.01f)
            {
                anim.SetFloat("InputX", v.x);
                anim.SetFloat("InputY", v.y);
                anim.SetFloat("LastInputX", v.x);
                anim.SetFloat("LastInputY", v.y);
            }
        }
    }
}
