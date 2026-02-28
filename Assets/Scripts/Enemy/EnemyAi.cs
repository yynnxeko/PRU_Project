using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public enum EnemyState
{
    Idle, Suspicious, Alert, Searching, Return
}

public class EnemyAi : MonoBehaviour
{
    [Header("--- COMPONENTS ---")]
    public Animator anim;
    public Transform rotatingPart;

    [Header("--- CÀI ĐẶT CHUNG ---")]
    public EnemyState currentState = EnemyState.Idle;
    public Transform player;
    public LayerMask obstacleMask;

    [Header("--- UI ---")]
    public Image suspicionBarFill;
    public GameObject suspicionCanvas;

    [Header("--- TUẦN TRA ---")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 4f;
    private int currentPatrolIndex = 0;
    private float patrolTimer = 0f;
    public float patrolRotationSpeed = 5f;
    public float patrolRotationAngle = 60f;

    [Header("--- CẢM BIẾN ---")]
    public float baseDistance = 5f;
    public float focusDistance = 6.5f;
    public float baseAngle = 70f;
    public float focusAngle = 60f;
    public float innerViewDistance = 3f;

    [Header("--- LOGIC SUSPICION ---")]
    public float suspicionLevel = 0f;
    public float alertThreshold = 100f;
    public float rateFar = 20f;
    public float rateNear = 35f;
    public float cooldownRate = 30f;

    [Header("--- SEARCH ---")]
    public float searchDuration = 5f;
    public float searchRadius = 4f;
    private float searchMoveTimer = 0f;

    [Header("--- FLASHLIGHT ---")]
    public Light2D flashlight;
    public float intensityNormal = 1.2f;
    public float intensityBoost = 2.0f;

    // --- BIẾN NỘI BỘ ---
    private NavMeshAgent agent;
    private Vector3 startPosition;
    private Vector3 lastKnownPosition;
    private float currentViewDist;
    private float currentViewAngle;
    private float searchTimer;
    private PlayerInventory cachedPlayerInventory;
    private float alertBroadcastTimer = 0f;
    private float alertBroadcastInterval = 0.5f;

    private Quaternion patrolBaseRotation;
    private bool hasSetBaseRotation = false;

    // Biến lưu hướng nhìn cuối cùng (để cập nhật LastInput)
    private Vector2 lastFaceDir = Vector2.down;
    [SerializeField] private PlayerController2 playerController;

    void Awake()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
        if (playerController == null && player != null)
            playerController = player.GetComponent<PlayerController2>();

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
        if (anim == null) anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        startPosition = transform.position;
        lastKnownPosition = transform.position;
        currentViewDist = baseDistance;
        currentViewAngle = baseAngle;

        if (player != null) cachedPlayerInventory = player.GetComponent<PlayerInventory>();

        if (flashlight != null)
        {
            flashlight.pointLightOuterRadius = currentViewDist;
            flashlight.pointLightOuterAngle = currentViewAngle;
            flashlight.pointLightInnerAngle = currentViewAngle * 0.7f;
        }

        if (patrolPoints != null && patrolPoints.Length > 0 && patrolPoints[0] != null)
            agent.SetDestination(patrolPoints[0].position);

        if (rotatingPart != null) patrolBaseRotation = rotatingPart.rotation;
        else patrolBaseRotation = transform.rotation;
    }

    void Update()
    {
        HandleVisionSensors();
        HandleUI();

        if (suspicionLevel >= alertThreshold && currentState != EnemyState.Alert)
            ChangeState(EnemyState.Alert);

        if (currentState == EnemyState.Alert && suspicionLevel <= 0)
            ChangeState(EnemyState.Idle);

        CheckCapturePlayer();

        switch (currentState)
        {
            case EnemyState.Idle: HandleIdleOrPatrol(); break;
            case EnemyState.Suspicious: HandleSuspicious(); break;
            case EnemyState.Alert: HandleAlert(); break;
            case EnemyState.Searching: HandleSearching(); break;
            case EnemyState.Return: HandleReturn(); break;
        }

        UpdateVisuals();

        // CẬP NHẬT ANIMATION 5 BIẾN
        UpdateAnimation();

        // Xử lý xoay đèn khi di chuyển
        bool isArrivedAtPatrolPoint = (currentState == EnemyState.Idle && !agent.pathPending && agent.remainingDistance <= 0.5f);
        if (agent.velocity.sqrMagnitude > 0.1f && !isArrivedAtPatrolPoint)
        {
            LookAtTarget(transform.position + agent.velocity);
            hasSetBaseRotation = false;
        }
    }

    // --- HÀM ANIMATION CHUẨN (GIỐNG PLAYER) ---
    void UpdateAnimation()
    {
        if (anim == null) return;

        bool isMoving = agent.velocity.sqrMagnitude > 0.1f;
        anim.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            // --- TRƯỜNG HỢP 1: ĐANG DI CHUYỂN ---
            Vector3 moveDir = agent.velocity.normalized;

            // Gửi InputX/Y để chạy animation Walk
            anim.SetFloat("InputX", moveDir.x);
            anim.SetFloat("InputY", moveDir.y);

            // Lưu lại hướng này vào LastInput để dùng khi dừng lại
            lastFaceDir = new Vector2(moveDir.x, moveDir.y);
            anim.SetFloat("LastInputX", lastFaceDir.x);
            anim.SetFloat("LastInputY", lastFaceDir.y);
        }
        else
        {
            // --- TRƯỜNG HỢP 2: ĐANG ĐỨNG YÊN ---

            // InputX/Y về 0 (vì không di chuyển)
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputY", 0);

            // Kiểm tra xem có đang ở chế độ "Lắc Đèn" (Patrol Wait) không?
            bool isWaitingAtPoint = (currentState == EnemyState.Idle && !agent.pathPending && agent.remainingDistance <= 0.5f);

            if (isWaitingAtPoint && rotatingPart != null)
            {
                // Nếu đang lắc đèn: Cập nhật LastInput theo hướng đèn
                // Để nhân vật quay mặt theo đèn
                Vector3 lightDir = rotatingPart.up;

                if (Mathf.Abs(lightDir.x) > 0.3f)
                    lastFaceDir = new Vector2(Mathf.Sign(lightDir.x), 0);
                else
                    lastFaceDir = new Vector2(0, Mathf.Sign(lightDir.y));
            }
            // Nếu không lắc đèn: Giữ nguyên lastFaceDir cũ (hướng nhìn cuối cùng khi dừng)

            // Gửi LastInput vào Animator để nó hiện đúng hình Idle
            anim.SetFloat("LastInputX", lastFaceDir.x);
            anim.SetFloat("LastInputY", lastFaceDir.y);
        }
    }

    void HandleIdleOrPatrol()
    {
        if (suspicionLevel > 0) { ChangeState(EnemyState.Suspicious); return; }

        if (patrolPoints == null || patrolPoints.Length == 0) return;
        agent.isStopped = false;

        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            patrolTimer += Time.deltaTime;

            if (!hasSetBaseRotation)
            {
                float currentZ = (rotatingPart != null) ? rotatingPart.eulerAngles.z : transform.eulerAngles.z;
                patrolBaseRotation = Quaternion.Euler(0, 0, currentZ);
                hasSetBaseRotation = true;
            }

            float wave = Mathf.Sin(patrolTimer * patrolRotationSpeed) * patrolRotationAngle;

            if (rotatingPart != null)
            {
                rotatingPart.rotation = Quaternion.Euler(0, 0, patrolBaseRotation.eulerAngles.z + wave);
            }

            if (patrolTimer >= patrolWaitTime)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                if (patrolPoints[currentPatrolIndex] != null)
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                patrolTimer = 0f;
            }
        }
    }

    // ... CÁC HÀM PHÍA DƯỚI GIỮ NGUYÊN (Copy lại từ code cũ) ...
    void HandleSuspicious()
    {
        if (suspicionLevel <= 0) { ChangeState(EnemyState.Return); return; }
        if (CheckZone() > 0 && player != null) LookAtTarget(player.position);
    }

    void HandleAlert()
    {
        agent.isStopped = false;
        if (CheckZone() > 0 && player != null)
        {
            lastKnownPosition = player.position;
            LookAtTarget(player.position);
            alertBroadcastTimer -= Time.deltaTime;
            if (alertBroadcastTimer <= 0f)
            {
                alertBroadcastTimer = alertBroadcastInterval;
            }
        }
        agent.SetDestination(lastKnownPosition);
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            if (CheckZone() == 0) ChangeState(EnemyState.Searching);
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
        if (patrolPoints != null && patrolPoints.Length > 0 && patrolPoints[currentPatrolIndex] != null)
            targetReturn = patrolPoints[currentPatrolIndex].position;
        agent.SetDestination(targetReturn);
        if (CheckZone() > 0) { ChangeState(EnemyState.Alert); return; }
        if (!agent.pathPending && agent.remainingDistance <= 0.2f)
        {
            ChangeState(EnemyState.Idle);
        }
    }

    void HandleVisionSensors()
    {
        int zone = CheckZone();
        bool canSee = (zone > 0);
        if (canSee)
        {
            float rate = (zone == 2) ? rateNear : rateFar;
            float mult = (cachedPlayerInventory != null && cachedPlayerInventory.handEvidences.Count > 0) ? 1f + (cachedPlayerInventory.handEvidences.Count * 2f) : 1f;
            suspicionLevel += rate * mult * Time.deltaTime;
        }
        else
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
            case EnemyState.Alert: alertBroadcastTimer = alertBroadcastInterval; break;
            case EnemyState.Searching: searchTimer = searchDuration; searchMoveTimer = 0f; break;
            case EnemyState.Idle: patrolTimer = 0f; suspicionLevel = 0f; break;
        }
    }

    public void GoToLocation(Vector3 targetPos)
    {
        lastKnownPosition = targetPos;
        if (currentState != EnemyState.Alert)
        {
            suspicionLevel = 100f;
            ChangeState(EnemyState.Alert);
        }
        if (agent && agent.isOnNavMesh) agent.SetDestination(lastKnownPosition);
    }

    void LookAtTarget(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (rotatingPart != null)
        {
            rotatingPart.rotation = Quaternion.Lerp(rotatingPart.rotation, Quaternion.Euler(0, 0, angle - 90), Time.deltaTime * 10f);
        }
    }

    void UpdateVisuals()
    {
        bool isLongRange = (currentState == EnemyState.Alert || currentState == EnemyState.Suspicious || currentState == EnemyState.Searching);
        float targetDist = isLongRange ? focusDistance : baseDistance;
        float targetAngle = (currentState == EnemyState.Alert) ? focusAngle : baseAngle;
        currentViewDist = Mathf.Lerp(currentViewDist, targetDist, Time.deltaTime * 5f);
        currentViewAngle = Mathf.Lerp(currentViewAngle, targetAngle, Time.deltaTime * 5f);
        if (flashlight != null)
        {
            flashlight.pointLightOuterRadius = currentViewDist;
            flashlight.pointLightOuterAngle = currentViewAngle;
            flashlight.pointLightInnerAngle = currentViewAngle * 0.7f;
            bool isPlayerInNearZone = (CheckZone() == 2);
            float targetIntensity = isPlayerInNearZone ? intensityBoost : intensityNormal;
            flashlight.intensity = Mathf.Lerp(flashlight.intensity, targetIntensity, Time.deltaTime * 10f);
        }
    }

    void HandleUI()
    {
        if (suspicionBarFill == null) return;
        suspicionBarFill.fillAmount = suspicionLevel / 100f;
        if (suspicionLevel >= 100f) suspicionBarFill.color = Color.red;
        else if (suspicionLevel > 50f) suspicionBarFill.color = new Color(1f, 0.64f, 0f);
        else suspicionBarFill.color = Color.yellow;
        if (suspicionCanvas != null)
        {
            suspicionCanvas.SetActive(suspicionLevel > 0);
            suspicionCanvas.transform.rotation = Quaternion.identity;
            suspicionCanvas.transform.position = transform.position + Vector3.up * 1.5f;
        }
    }

    void CheckCapturePlayer()
    {
        if (player != null && CheckZone() > 0)
        {
            float d = Vector3.Distance(transform.position, player.position);

            if (d <= 1.2f)
            {
                GameManager.Instance.HandlePlayerCaught(cachedPlayerInventory);
            }
        }
    }

    int CheckZone()
    {
        if (!player) return 0;

        if (playerController != null && playerController.IsSitting)
            return 0;

        float d = Vector3.Distance(transform.position, player.position);

        if (d > currentViewDist) return 0;

        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 forwardDir = (rotatingPart != null) ? rotatingPart.up : transform.up;
        if (Vector3.Angle(forwardDir, dir) > currentViewAngle / 2) return 0;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, d, obstacleMask);
        if (hit.collider != null)
        {
            if (hit.collider.transform == player || hit.collider.transform.IsChildOf(player)) { }
            else return 0;
        }
        return (d <= innerViewDistance) ? 2 : 1;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}
