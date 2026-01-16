using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    [Header("Cài đặt Tầm nhìn Cơ bản")]
    public Transform player;
    public float baseDistance = 6f;
    public float baseAngle = 70f;
    public float innerViewDistance = 3f;
    public LayerMask obstacleMask;

    [Header("Nâng cấp 2: Góc nhìn động")]
    public float focusDistance = 7f;
    public float focusAngle = 40f;
    public float changeSpeed = 6.5f;

    [Header("Gán 2 cái Nón")]
    public MeshFilter meshFilterFar;
    public MeshFilter meshFilterNear;

    [Header("Màu sắc")]
    public Color colorFar = new Color(1, 0, 0, 0.2f);
    public Color colorNear = new Color(1, 0, 0, 0.6f);
    public Color colorAlert = new Color(1, 0, 0, 1f);

    [Header("Logic Nghi ngờ")]
    public float suspicionLevel = 0f;
    public float cooldownRate = 10f;
    public float normalRate = 15f;
    public float fastRate = 50f;
    public float alertCooldownThreshold = 50f; // <--- MỚI: Dưới mức này mới bỏ cuộc

    [Header("Logic Tang vật (Evidence)")]
    public float evidenceSensitivity = 0.5f;

    // Biến nội bộ
    private float currentViewDist;
    private float currentViewAngle;
    private bool isAlert = false;
    private float flashTimer = 0f;

    // Logic báo động & di chuyển
    private float alertTimer = 0f;
    private float alertDuration = 5f;
    private Vector3 lastKnownPosition; // Vị trí cuối cùng nhận được từ Cam hoặc Tự nhìn thấy
    private bool hasTarget = false;    // Đang có mục tiêu truy đuổi?
    private Vector3 startPosition;     // <--- MỚI: Vị trí trực ban đầu
    private Quaternion startRotation;  // <--- MỚI: Hướng nhìn ban đầu

    private Mesh meshFar;
    private Mesh meshNear;
    private SpriteRenderer myBodyColor;
    private MeshRenderer renderFar;
    private MeshRenderer renderNear;
    private PlayerInventory cachedPlayerInventory;
    private NavMeshAgent agent;

    void Start()
    {
        myBodyColor = GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.updatePosition = false;
        }

        // Lưu vị trí & hướng ban đầu để lát nữa quay về
        startPosition = transform.position;
        startRotation = transform.rotation;

        currentViewDist = baseDistance;
        currentViewAngle = baseAngle;

        if (player != null) cachedPlayerInventory = player.GetComponent<PlayerInventory>();

        // Setup 2 nón (giữ nguyên code cũ)
        SetupMesh(meshFilterFar, ref meshFar, ref renderFar, colorFar, 1);
        SetupMesh(meshFilterNear, ref meshNear, ref renderNear, colorNear, 2);
    }

    void SetupMesh(MeshFilter mf, ref Mesh mesh, ref MeshRenderer mr, Color color, int order)
    {
        if (mf != null)
        {
            mesh = new Mesh();
            mf.mesh = mesh;
            mr = mf.GetComponent<MeshRenderer>();
            if (mr)
            {
                if (mr.material == null) mr.material = new Material(Shader.Find("Sprites/Default"));
                mr.material.color = color;
                mr.sortingOrder = order;
            }
        }
    }

    void Update()
    {
        LogicSuspicion();
        LogicMovement();

        DrawCone(meshFar, currentViewDist, currentViewAngle);
        DrawCone(meshNear, innerViewDistance, currentViewAngle);

        // Fix Z
        if (agent != null && agent.isOnNavMesh)
        {
            Vector3 nextPos = agent.nextPosition;
            transform.position = new Vector3(nextPos.x, nextPos.y, 0);
            agent.nextPosition = transform.position;
        }
    }

    void LogicMovement()
    {
        if (player == null || agent == null || !agent.isOnNavMesh) return;

        // --- TRƯỜNG HỢP 1: TỰ NHÌN THẤY PLAYER (ƯU TIÊN CAO NHẤT) ---
        if (isAlert && CheckZone() > 0)
        {
            // Cập nhật liên tục vị trí mới nhất
            lastKnownPosition = player.position;
            hasTarget = true;

            // Đuổi theo ngay
            agent.SetDestination(lastKnownPosition);
        }
        // --- TRƯỜNG HỢP 2: MẤT DẤU NHƯNG VẪN CAY (Suspicion > 50) ---
        else if (isAlert && suspicionLevel >= alertCooldownThreshold)
        {
            if (hasTarget)
            {
                // Chạy đến điểm cuối cùng nhìn thấy/được báo
                agent.SetDestination(lastKnownPosition);

                // Nếu đã đến nơi rồi mà không thấy ai
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    // Đứng im quan sát, chờ hạ nhiệt
                    hasTarget = false;
                }
            }
            // Nếu không có target thì đứng im tại chỗ cảnh giác (hoặc bạn có thể code thêm hành vi đi tuần quanh đó)
        }
        // --- TRƯỜNG HỢP 3: HẠ NHIỆT (Suspicion < 50) -> VỀ VỊ TRÍ CŨ ---
        else
        {
            // Nếu đã hạ nhiệt
            isAlert = false;
            hasTarget = false;

            // Quay về vị trí trực ban đầu
            agent.SetDestination(startPosition);

            // Xoay về hướng cũ khi đã về đến nơi
            if (!agent.pathPending && agent.remainingDistance <= 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * 2f);
            }
        }

        // Xoay hướng nhìn theo hướng di chuyển
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 dir = agent.velocity.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    void LogicSuspicion()
    {
        if (player == null) return;

        int zone = CheckZone();
        bool isSeeingPlayer = (zone > 0);

        // --- LOGIC ALERT TIMER ---
        if (isAlert)
        {
            // Nếu đang Alert, đếm ngược
            alertTimer -= Time.deltaTime;

            // Nếu còn thời gian Alert, Suspicion luôn max 100
            if (alertTimer > 0) suspicionLevel = 100f;
        }

        if (isSeeingPlayer)
        {
            // Nhìn thấy -> Reset timer -> Suspicion tăng
            if (isAlert) alertTimer = alertDuration;

            float evidenceMultiplier = 1f;
            if (cachedPlayerInventory == null) cachedPlayerInventory = player.GetComponent<PlayerInventory>();
            if (cachedPlayerInventory != null && cachedPlayerInventory.IsCarryingEvidence())
            {
                evidenceMultiplier = 1f + (cachedPlayerInventory.carriedEvidence * evidenceSensitivity);
            }

            float baseRate = (zone == 2) ? fastRate : normalRate;
            suspicionLevel += baseRate * evidenceMultiplier * Time.deltaTime;

            // Xoay đầu nhìn Player (chỉ khi chưa chạy, để nhìn cho tự nhiên)
            if (!isAlert)
            {
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                float rotationAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, rotationAngle - 90), 5f * Time.deltaTime);
            }
        }
        else
        {
            // Không nhìn thấy -> Giảm từ từ
            // Chỉ giảm khi hết timer "cay cú"
            if (alertTimer <= 0)
            {
                suspicionLevel -= cooldownRate * Time.deltaTime;
            }
        }

        suspicionLevel = Mathf.Clamp(suspicionLevel, 0f, 100f);

        // Kích hoạt Alert nếu đầy cây
        if (suspicionLevel >= 100f && !isAlert)
        {
            isAlert = true;
            alertTimer = alertDuration;
        }

        // --- LOGIC FOV & MÀU SẮC ---
        float targetDist = (suspicionLevel > 0) ? Mathf.Lerp(baseDistance, focusDistance, suspicionLevel / 100f) : baseDistance;
        float targetAngle = (suspicionLevel > 0) ? Mathf.Lerp(baseAngle, focusAngle, suspicionLevel / 100f) : baseAngle;

        currentViewDist = Mathf.Lerp(currentViewDist, targetDist, changeSpeed * Time.deltaTime);
        currentViewAngle = Mathf.Lerp(currentViewAngle, targetAngle, changeSpeed * Time.deltaTime);

        // Màu sắc
        if (suspicionLevel >= 100f) // Alert (Đỏ)
        {
            flashTimer += Time.deltaTime * 10f;
            SetColor(Mathf.Sin(flashTimer) > 0 ? Color.red : Color.black, colorAlert);
        }
        else if (suspicionLevel > 0f) // Nghi ngờ (Vàng)
        {
            SetColor(Color.yellow, colorNear);
        }
        else // An toàn (Trắng)
        {
            SetColor(Color.white, colorNear);
        }
    }

    void SetColor(Color body, Color cone)
    {
        myBodyColor.color = body;
        if (renderNear) renderNear.material.color = cone;
    }

    // Hàm gọi từ Camera (Nhận tin báo)
    public void GoToLocation(Vector3 pos)
    {
        // Chỉ nhận tin báo mới nếu tin đó khác vị trí hiện tại một chút (tránh spam)
        if (Vector3.Distance(pos, lastKnownPosition) > 1.0f)
        {
            lastKnownPosition = pos;
            hasTarget = true;

            // Kích hoạt trạng thái báo động
            isAlert = true;
            suspicionLevel = 100f;
            alertTimer = alertDuration;
        }
    }

    // ... (Giữ nguyên các hàm DrawCone, CheckZone, GetVector, LateUpdate cũ) ...
    // Copy lại y chang các hàm đó vào đây
    void DrawCone(Mesh meshToDraw, float dist, float angle)
    { /* Code cũ */
        if (meshToDraw == null) return;
        int rayCount = 50;
        float startAngle = GetAngleFromVectorFloat(transform.up) + angle / 2f;
        float currentAngle = startAngle;
        float angleStep = angle / rayCount;
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];
        vertices[0] = Vector3.zero;
        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            Vector3 dir = GetVectorFromAngle(currentAngle);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, obstacleMask);
            if (hit.collider != null) vertex = transform.InverseTransformPoint(hit.point);
            else vertex = transform.InverseTransformPoint(transform.position + dir * dist);
            vertices[vertexIndex] = vertex;
            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex++;
            currentAngle -= angleStep;
        }
        meshToDraw.Clear();
        meshToDraw.vertices = vertices;
        meshToDraw.uv = uv;
        meshToDraw.triangles = triangles;
    }
    int CheckZone()
    { /* Code cũ */
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > currentViewDist) return 0;
        Vector3 dir = (player.position - transform.position).normalized;
        if (Vector3.Angle(transform.up, dir) > currentViewAngle / 2) return 0;
        if (Physics2D.Raycast(transform.position, dir, dist, obstacleMask)) return 0;
        if (dist <= innerViewDistance) return 2;
        return 1;
    }
    Vector3 GetVectorFromAngle(float angle)
    { /* Code cũ */
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    float GetAngleFromVectorFloat(Vector3 dir)
    { /* Code cũ */
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
    void LateUpdate()
    { /* Code cũ */
        if (transform.position.z != 0) transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
