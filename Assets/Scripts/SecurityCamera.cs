using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Mục tiêu")]
    public Transform player;
    public LayerMask obstacleMask;

    [Header("Cấu hình Camera")]
    public float viewDistance = 8f;
    public float viewAngle = 45f;
    public float rotationSpeed = 20f;
    public float rotationAngleMax = 60f;
    public float focusSpeed = 5f;

    [Header("Cấu hình Nghi ngờ")]
    public float suspicionLevel = 0f; // Thanh nghi ngờ (0-100)
    public float detectionRate = 50f; // Tốc độ phát hiện (50 -> 2 giây là đầy)
    public float cooldownRate = 30f;  // Tốc độ giảm khi mất dấu

    [Header("Cấu hình Nón")]
    public MeshFilter viewMeshFilter;
    public Color safeColor = new Color(0, 1, 0, 0.3f);
    public Color warningColor = new Color(1, 0.92f, 0.016f, 0.4f); // Vàng (Nghi ngờ)
    public Color alertColor = new Color(1, 0, 0, 0.6f); // Đỏ (Báo động)

    // Biến nội bộ
    private Mesh viewMesh;
    private MeshRenderer meshRenderer;
    private float startRotation;
    private bool isAlert = false;

    // Biến chống Spam lệnh
    private Vector3 lastAlertPosition;
    private float lastAlertTime;
    private float alertInterval = 0.5f; // Tối thiểu 0.5s mới báo lại 1 lần
    private float moveThreshold = 0.5f; // Player di chuyển > 0.5m mới báo lại ngay

    void Start()
    {
        startRotation = transform.eulerAngles.z;

        // Tự động setup Mesh nếu chưa có
        if (viewMeshFilter != null)
        {
            viewMesh = new Mesh();
            viewMeshFilter.mesh = viewMesh;
            meshRenderer = viewMeshFilter.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                if (meshRenderer.material == null)
                    meshRenderer.material = new Material(Shader.Find("Sprites/Default"));

                meshRenderer.material.color = safeColor;
            }
        }
        else
        {
            Debug.LogError("Chưa gán View Mesh Filter cho Camera! Hãy tạo object con và gán vào.");
        }
    }

    void Update()
    {
        // Kiểm tra an toàn trước khi chạy
        if (viewMeshFilter == null || meshRenderer == null) return;

        bool seeingPlayer = CheckPlayerVisible();

        // --- LOGIC SUSPICION ---
        if (seeingPlayer)
        {
            // Tăng nghi ngờ
            suspicionLevel += detectionRate * Time.deltaTime;

            // Focus vào player
            FocusOnPlayer();
        }
        else
        {
            // Giảm nghi ngờ
            suspicionLevel -= cooldownRate * Time.deltaTime;

            // Nếu chưa báo động đỏ thì quay lại đi tuần
            if (!isAlert) PatrolRotation();
        }

        // Kẹp giá trị 0-100
        suspicionLevel = Mathf.Clamp(suspicionLevel, 0f, 100f);

        // --- XỬ LÝ TRẠNG THÁI & MÀU SẮC ---
        if (suspicionLevel >= 100f)
        {
            // --- TRẠNG THÁI BÁO ĐỘNG (ĐỎ) ---
            isAlert = true;
            meshRenderer.material.color = alertColor;

            // Logic gửi lệnh thông minh (Chống Spam)
            if (player != null && seeingPlayer)
            {
                float distMoved = Vector3.Distance(player.position, lastAlertPosition);
                float timeSinceLast = Time.time - lastAlertTime;

                // Điều kiện gửi lệnh:
                // 1. Player di chuyển đủ xa (> 0.5m)
                // 2. HOẶC đã quá lâu chưa báo lại (> 0.5s)
                // 3. GameManager phải tồn tại
                if ((distMoved > moveThreshold || timeSinceLast > alertInterval) && GameManager.Instance != null)
                {
                    Debug.Log($"[Camera] Phát hiện xâm nhập tại {player.position}. Đang gọi hội...");
                    GameManager.Instance.AlertAllEnemies(player.position);

                    // Lưu lại trạng thái để so sánh lần sau
                    lastAlertPosition = player.position;
                    lastAlertTime = Time.time;
                }
            }

            // Camera vẫn tiếp tục Focus
            if (seeingPlayer) FocusOnPlayer();
        }
        else if (suspicionLevel > 0f)
        {
            // --- TRẠNG THÁI NGHI NGỜ (VÀNG) ---
            isAlert = false; // Chưa báo động hẳn
            meshRenderer.material.color = warningColor;
        }
        else
        {
            // --- TRẠNG THÁI AN TOÀN (XANH) ---
            isAlert = false;
            meshRenderer.material.color = safeColor;
        }

        DrawCone();
    }

    void PatrolRotation()
    {
        float angleOffset = Mathf.Sin(Time.time * (rotationSpeed / 10f)) * rotationAngleMax;
        Quaternion targetRot = Quaternion.Euler(0, 0, startRotation + angleOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 2f);
    }

    void FocusOnPlayer()
    {
        if (player == null) return;
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg - 90f;

        float angleDifference = Mathf.DeltaAngle(startRotation, targetAngle);
        if (Mathf.Abs(angleDifference) <= rotationAngleMax)
        {
            Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, focusSpeed * Time.deltaTime);
        }
    }

    bool CheckPlayerVisible()
    {
        if (player == null) return false;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > viewDistance) return false;

        Vector3 dir = (player.position - transform.position).normalized;
        if (Vector3.Angle(transform.up, dir) > viewAngle / 2) return false;

        if (Physics2D.Raycast(transform.position, dir, dist, obstacleMask)) return false;

        return true;
    }

    void DrawCone()
    {
        int rayCount = 50;
        float angle = viewAngle;
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewDistance, obstacleMask);

            if (hit.collider != null)
                vertex = transform.InverseTransformPoint(hit.point);
            else
                vertex = transform.InverseTransformPoint(transform.position + dir * viewDistance);

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

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.uv = uv;
        viewMesh.triangles = triangles;
    }

    Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
}
