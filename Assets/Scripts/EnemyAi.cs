using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum EnemyState
{
    Idle,       // Đứng yên hoặc Đi tuần
    Suspicious, // Nghi ngờ
    Alert,      // Truy đuổi
    Searching,  // Tìm kiếm
    Return      // Quay về
}

public class EnemyAi : MonoBehaviour
{
    [Header("--- CÀI ĐẶT CHUNG ---")]
    public EnemyState currentState = EnemyState.Idle;
    public Transform player;
    public LayerMask obstacleMask;

    [Header("--- TUẦN TRA (PATROL) ---")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 4f;
    private int currentPatrolIndex = 0;
    private float patrolTimer = 0f;

    // Tùy chỉnh xoay đầu
    public float patrolRotationSpeed = 2f; // Tốc độ lắc
    public float patrolRotationAngle = 30f; // Góc lắc

    [Header("--- CẢM BIẾN (VISION) ---")]
    public float baseDistance = 6f;
    public float baseAngle = 70f;
    public float focusDistance = 8f;
    public float focusAngle = 40f;
    public float innerViewDistance = 3f;

    [Header("--- THÔNG SỐ LOGIC ---")]
    public float suspicionLevel = 0f;
    public float alertThreshold = 100f;

    public float rateFar = 15f;
    public float rateNear = 50f;
    public float cooldownRate = 10f;

    [Header("--- LỤC SOÁT (SEARCH) ---")]
    public float searchDuration = 5f;
    public float searchRadius = 4f;
    private float searchMoveTimer = 0f;

    [Header("--- VISUALS ---")]
    public MeshFilter meshFilterFar;
    public MeshFilter meshFilterNear;

    public Color colorFarIdle = new Color(1, 0, 0, 0.2f);
    public Color colorNearIdle = new Color(1, 0, 0, 0.6f);
    public Color colorSuspicious = new Color(1, 0.92f, 0.016f, 0.5f);
    public Color colorAlert = new Color(1, 0, 0, 1f);

    // --- BIẾN NỘI BỘ ---
    private NavMeshAgent agent;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 lastKnownPosition;
    private float currentViewDist;
    private float currentViewAngle;

    private Mesh meshFar;
    private Mesh meshNear;
    private MeshRenderer renderFar;
    private MeshRenderer renderNear;

    private float searchTimer;
    private PlayerInventory cachedPlayerInventory;

    private float alertBroadcastTimer = 0f;
    private float alertBroadcastInterval = 0.5f;

    // Biến riêng để lưu hướng gốc khi lắc đầu (Fix lỗi chỉ quay 1 bên)
    private Quaternion patrolBaseRotation;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.updatePosition = false;
        }
        if (patrolPoints == null) patrolPoints = new Transform[0];
    }

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        lastKnownPosition = transform.position;

        currentViewDist = baseDistance;
        currentViewAngle = baseAngle;

        if (player != null) cachedPlayerInventory = player.GetComponent<PlayerInventory>();

        SetupMesh(meshFilterFar, ref meshFar, 1);
        SetupMesh(meshFilterNear, ref meshNear, 2);

        renderFar = meshFilterFar.GetComponent<MeshRenderer>();
        renderNear = meshFilterNear.GetComponent<MeshRenderer>();

        if (renderFar) renderFar.material.color = colorFarIdle;
        if (renderNear) renderNear.material.color = colorNearIdle;

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            if (patrolPoints[0] != null)
                agent.SetDestination(patrolPoints[0].position);
        }
    }

    void Update()
    {
        HandleVisionSensors();

        switch (currentState)
        {
            case EnemyState.Idle: HandleIdleOrPatrol(); break;
            case EnemyState.Suspicious: HandleSuspicious(); break;
            case EnemyState.Alert: HandleAlert(); break;
            case EnemyState.Searching: HandleSearching(); break;
            case EnemyState.Return: HandleReturn(); break;
        }

        UpdateVisuals();
        SyncNavMeshAgent();
    }

    // ========================================================================
    // LOGIC TRẠNG THÁI
    // ========================================================================

    void HandleIdleOrPatrol()
    {
        if (suspicionLevel > 0) { ChangeState(EnemyState.Suspicious); return; }

        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.isStopped = false;

        // Đã đến điểm dừng
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            patrolTimer += Time.deltaTime;

            // --- FIX LOGIC XOAY ĐẦU ---
            // Chỉ lấy góc gốc MỘT LẦN ngay khi vừa dừng lại
            if (patrolTimer <= Time.deltaTime)
            {
                patrolBaseRotation = transform.rotation;
            }

            // Xoay qua lại quanh góc gốc đó
            float wave = Mathf.Sin(Time.time * patrolRotationSpeed) * patrolRotationAngle;
            transform.rotation = Quaternion.Euler(0, 0, patrolBaseRotation.eulerAngles.z + wave);
            // --------------------------

            if (patrolTimer >= patrolWaitTime)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                if (patrolPoints[currentPatrolIndex] != null)
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                patrolTimer = 0f;
            }
        }
    }

    void HandleSuspicious()
    {
        if (suspicionLevel >= alertThreshold) { ChangeState(EnemyState.Alert); return; }
        if (suspicionLevel <= 0) { ChangeState(EnemyState.Return); return; }
        if (CheckZone() > 0 && player != null) LookAtTarget(player.position);
    }

    void HandleAlert()
    {
        agent.isStopped = false;
        if (CheckZone() > 0 && player != null)
        {
            lastKnownPosition = player.position;
            agent.SetDestination(lastKnownPosition);
            LookAtTarget(player.position);

            alertBroadcastTimer -= Time.deltaTime;
            if (alertBroadcastTimer <= 0f)
            {
                if (GameManager.Instance != null) GameManager.Instance.AlertAllEnemies(player.position);
                alertBroadcastTimer = alertBroadcastInterval;
            }
        }
        else
        {
            agent.SetDestination(lastKnownPosition);
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
            {
                ChangeState(EnemyState.Searching);
            }
        }
    }

    void HandleSearching()
    {
        agent.isStopped = false;
        searchTimer -= Time.deltaTime;

        searchMoveTimer -= Time.deltaTime;
        if (searchMoveTimer <= 0)
        {
            Vector3 randomPoint = RandomNavSphere(lastKnownPosition, searchRadius, -1);
            agent.SetDestination(randomPoint);
            searchMoveTimer = 1.5f;
        }

        if (CheckZone() > 0) { ChangeState(EnemyState.Alert); return; }

        if (searchTimer <= 0)
        {
            suspicionLevel = 0;
            ChangeState(EnemyState.Return);
        }
    }

    void HandleReturn()
    {
        agent.isStopped = false;

        Vector3 targetReturn = startPosition;
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            if (patrolPoints[currentPatrolIndex] != null)
                targetReturn = patrolPoints[currentPatrolIndex].position;
        }

        agent.SetDestination(targetReturn);

        if (CheckZone() > 0) { ChangeState(EnemyState.Alert); return; }

        if (!agent.pathPending && agent.remainingDistance <= 0.2f)
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * 5f);
                if (Quaternion.Angle(transform.rotation, startRotation) < 5f) ChangeState(EnemyState.Idle);
            }
            else
            {
                ChangeState(EnemyState.Idle);
            }
        }
    }

    // ========================================================================
    // CÁC HÀM HỖ TRỢ
    // ========================================================================

    void HandleVisionSensors()
    {
        int zone = CheckZone(); bool canSee = (zone > 0);
        if (canSee)
        {
            float rate = (zone == 2) ? rateNear : rateFar;
            float mult = (cachedPlayerInventory && cachedPlayerInventory.handEvidence > 0) ? 1f + (cachedPlayerInventory.handEvidence * 2f) : 1f;
            suspicionLevel += rate * mult * Time.deltaTime;
        }
        else if (currentState != EnemyState.Alert && currentState != EnemyState.Searching)
        {
            suspicionLevel -= cooldownRate * Time.deltaTime;
        }
        suspicionLevel = Mathf.Clamp(suspicionLevel, 0f, 100f);
    }

    void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        switch (newState)
        {
            case EnemyState.Alert: suspicionLevel = 100f; if (GameManager.Instance && player) GameManager.Instance.AlertAllEnemies(player.position); alertBroadcastTimer = alertBroadcastInterval; break;
            case EnemyState.Searching: searchTimer = searchDuration; searchMoveTimer = 0f; break;
            case EnemyState.Idle: patrolTimer = 0f; break;
        }
    }

    public void GoToLocation(Vector3 targetPos)
    {
        if (CheckZone() > 0) return;
        lastKnownPosition = targetPos;
        if (currentState != EnemyState.Alert) { suspicionLevel = 100f; ChangeState(EnemyState.Alert); }
        if (agent && agent.isOnNavMesh) agent.SetDestination(lastKnownPosition);
    }

    void SyncNavMeshAgent()
    {
        if (agent && agent.isOnNavMesh)
        {
            Vector3 nextPos = agent.nextPosition; transform.position = new Vector3(nextPos.x, nextPos.y, 0); agent.nextPosition = transform.position;
            if (agent.velocity.sqrMagnitude > 0.1f) LookAtTarget(transform.position + agent.velocity);
        }
    }

    void LookAtTarget(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle - 90), Time.deltaTime * 10f);
    }

    void UpdateVisuals()
    {
        float targetDist = (currentState == EnemyState.Alert) ? focusDistance : baseDistance;
        float targetAngle = (currentState == EnemyState.Alert) ? focusAngle : baseAngle;
        currentViewDist = Mathf.Lerp(currentViewDist, targetDist, Time.deltaTime * 5f);
        currentViewAngle = Mathf.Lerp(currentViewAngle, targetAngle, Time.deltaTime * 5f);
        DrawCone(meshFar, currentViewDist, currentViewAngle); DrawCone(meshNear, innerViewDistance, currentViewAngle);
        Color cN = colorNearIdle; Color cF = colorFarIdle;
        if (currentState == EnemyState.Suspicious) { cN = colorSuspicious; cF = new Color(colorSuspicious.r, colorSuspicious.g, colorSuspicious.b, 0.2f); }
        else if (currentState == EnemyState.Alert) { cN = colorAlert; cF = new Color(colorAlert.r, colorAlert.g, colorAlert.b, 0.5f); }
        else if (currentState == EnemyState.Searching) { cN = Color.yellow; cF = new Color(1, 0.92f, 0.016f, 0.1f); }
        if (renderNear) renderNear.material.color = cN; if (renderFar) renderFar.material.color = cF;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var inv = collision.gameObject.GetComponent<PlayerInventory>();
            if (inv != null && inv.TotalEvidence() > 0) { Debug.Log("GAME OVER!"); Time.timeScale = 0; }
        }
    }

    void SetupMesh(MeshFilter mf, ref Mesh mesh, int order)
    {
        if (!mf) return; mesh = new Mesh(); mf.mesh = mesh;
        var mr = mf.GetComponent<MeshRenderer>(); if (!mr) mr = mf.gameObject.AddComponent<MeshRenderer>();
        if (!mr.material) mr.material = new Material(Shader.Find("Sprites/Default")); mr.sortingOrder = order;
    }
    int CheckZone()
    {
        if (!player) return 0;
        float d = Vector3.Distance(transform.position, player.position);
        if (d > currentViewDist) return 0;
        Vector3 dir = (player.position - transform.position).normalized;
        if (Vector3.Angle(transform.up, dir) > currentViewAngle / 2) return 0;
        if (Physics2D.Raycast(transform.position, dir, d, obstacleMask)) return 0;
        return (d <= innerViewDistance) ? 2 : 1;
    }
    void DrawCone(Mesh m, float d, float a)
    {
        if (!m) return; int cnt = 50; float startA = GetAngleFromVectorFloat(transform.up) + a / 2f;
        float curA = startA; float step = a / cnt;
        Vector3[] v = new Vector3[cnt + 2]; Vector2[] uv = new Vector2[cnt + 2]; int[] t = new int[cnt * 3];
        v[0] = Vector3.zero; int vIdx = 1; int tIdx = 0;
        for (int i = 0; i <= cnt; i++)
        {
            Vector3 dir = GetVectorFromAngle(curA);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, d, obstacleMask);
            v[vIdx] = hit.collider ? transform.InverseTransformPoint(hit.point) : transform.InverseTransformPoint(transform.position + dir * d);
            if (i > 0) { t[tIdx] = 0; t[tIdx + 1] = vIdx - 1; t[tIdx + 2] = vIdx; tIdx += 3; }
            vIdx++; curA -= step;
        }
        m.Clear(); m.vertices = v; m.uv = uv; m.triangles = t;
    }
    Vector3 GetVectorFromAngle(float a) { return new Vector3(Mathf.Cos(a * Mathf.Deg2Rad), Mathf.Sin(a * Mathf.Deg2Rad)); }
    float GetAngleFromVectorFloat(Vector3 d) { d.Normalize(); float n = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg; return n < 0 ? n + 360 : n; }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}
