using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("--- MỤC TIÊU & CẤU HÌNH ---")]
    public Transform player;
    public LayerMask obstacleMask;
    public float viewDistance = 8f;
    public float viewAngle = 45f;

    [Header("--- ANIMATION & VISUAL ---")]
    public Animator animator;
    public string motionTimeParam = "CamAngle";
    public SpriteRenderer camSpriteRenderer; // Để đổi màu khi báo động

    [Header("--- VIEW MESH (TẦM NHÌN) ---")]
    public MeshFilter viewMeshFilter;
    public Color safeColor = new Color(0, 1, 0, 0.3f);
    public Color alertColor = new Color(1, 0, 0, 0.6f);
    public Color warningColor = new Color(1, 0.92f, 0.016f, 0.4f);

    [Header("--- CHUYỂN ĐỘNG ---")]
    public float rotationSpeed = 2f;      // Tốc độ đi tuần (nhịp Sin)
    public float rotationAngleMax = 60f;  // Góc quay tối đa khi đi tuần
    public float focusSpeed = 5f;         // Tốc độ xoay khi phát hiện Player

    [Header("--- TRẠNG THÁI GAME ---")]
    public float suspicionLevel = 0f;
    public float detectionRate = 50f;
    public float cooldownRate = 30f;

    // Biến nội bộ
    private Mesh viewMesh;
    private MeshRenderer meshRenderer;
    private float baseRotation; // Góc gốc ban đầu
    private float currentMeshAngle; // Góc hiện tại của Mesh (để sync)
    private bool isAlert = false;

    void Start()
    {
        // 1. Lưu góc gốc để làm chuẩn
        baseRotation = transform.eulerAngles.z;
        currentMeshAngle = baseRotation;

        // 2. Setup Mesh (Tạo lưới tầm nhìn)
        if (viewMeshFilter != null)
        {
            viewMesh = new Mesh();
            viewMesh.name = "ViewCone";
            viewMeshFilter.mesh = viewMesh;

            meshRenderer = viewMeshFilter.GetComponent<MeshRenderer>();
            if (meshRenderer == null) meshRenderer = viewMeshFilter.gameObject.AddComponent<MeshRenderer>();

            // Tạo material tạm nếu chưa có
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        // 3. Tự tìm Animator
        if (animator == null) animator = GetComponent<Animator>();
        if (camSpriteRenderer == null) camSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        bool seeingPlayer = CheckPlayerVisible();

        // --- A. LOGIC HÀNH VI ---
        if (seeingPlayer)
        {
            // 1. Tăng nghi ngờ
            suspicionLevel += detectionRate * Time.deltaTime;

            // 2. Chế độ FOCUS: Xoay về phía Player
            FocusOnPlayer();
        }
        else
        {
            // 1. Giảm nghi ngờ
            suspicionLevel -= cooldownRate * Time.deltaTime;

            // 2. Chế độ PATROL: Đi tuần tự động nếu chưa báo động đỏ
            if (suspicionLevel < 100f)
            {
                PatrolRotation();
            }
        }

        suspicionLevel = Mathf.Clamp(suspicionLevel, 0f, 100f);
        UpdateStateColor(seeingPlayer);

        // --- B. ĐỒNG BỘ ANIMATION (QUAN TRỌNG NHẤT) ---
        // Dù đang Patrol hay Focus, ta luôn tính toán dựa trên góc lệch hiện tại của Mesh so với góc gốc
        SyncAnimationToMeshAngle();

        // --- C. VẼ MESH ---
        DrawCone();
    }

    // --- HÀNH VI 1: ĐI TUẦN (Dùng Sin cho mượt) ---
    void PatrolRotation()
    {
        // Tính góc lệch theo hàm Sin
        float angleOffset = Mathf.Sin(Time.time * rotationSpeed) * rotationAngleMax;

        // Cập nhật góc mục tiêu
        currentMeshAngle = baseRotation + angleOffset;

        // Áp dụng vào ViewMesh
        if (viewMeshFilter != null)
            viewMeshFilter.transform.rotation = Quaternion.Euler(0, 0, currentMeshAngle);
    }

    // --- HÀNH VI 2: FOCUS (Xoay theo Player) ---
    void FocusOnPlayer()
    {
        if (player == null) return;

        Vector3 dir = (player.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f; // -90 vì sprite gốc thường hướng lên

        // Xoay từ từ (Lerp) về phía Player
        Quaternion currentQ = Quaternion.Euler(0, 0, currentMeshAngle);
        Quaternion targetQ = Quaternion.Euler(0, 0, targetAngle);

        Quaternion resultQ = Quaternion.RotateTowards(currentQ, targetQ, focusSpeed * 100f * Time.deltaTime);

        // Cập nhật lại biến góc hiện tại
        currentMeshAngle = resultQ.eulerAngles.z;

        // Áp dụng
        if (viewMeshFilter != null)
            viewMeshFilter.transform.rotation = resultQ;
    }

    // --- HÀM ĐỒNG BỘ CHUNG (Dùng cho cả 2 trường hợp) ---
    // lam on di, met lam roi
    void SyncAnimationToMeshAngle()
    {
        if (animator == null) return;

        float angleDifference = Mathf.DeltaAngle(baseRotation, currentMeshAngle);
        float normalizedTime = Mathf.InverseLerp(-rotationAngleMax, rotationAngleMax, angleDifference);

        // Đảo ngược (như nãy đã fix)
        float finalTime = 1f - normalizedTime;

        // --- THÊM DÒNG NÀY: Kẹp giá trị lại ---
        // Thay vì chạy từ 0 đến 1, ta chỉ cho chạy từ 0.05 đến 0.95
        // Nó sẽ cắt bỏ một tí tẹo ở 2 đầu mút -> Tránh bị frame lỗi hoặc trùng
        finalTime = Mathf.Clamp(finalTime, 0.05f, 0.95f);
        // -------------------------------------

        animator.SetFloat(motionTimeParam, finalTime);
    }


    // --- CÁC HÀM PHỤ TRỢ (MÀU SẮC, RAYCAST) ---
    void UpdateStateColor(bool seeingPlayer)
    {
        Color targetColor = safeColor;

        if (suspicionLevel >= 100f)
        {
            // BÁO ĐỘNG ĐỎ
            targetColor = alertColor;
            isAlert = true;

            // Gọi Game Manager nếu có
            if (GameManager.Instance != null && player != null)
                GameManager.Instance.AlertAllEnemies(player.position);
        }
        else if (suspicionLevel > 0f)
        {
            // NGHI NGỜ VÀNG
            targetColor = warningColor;
            isAlert = false;
        }
        else
        {
            // AN TOÀN XANH (QUAN TRỌNG: Phải có nhánh này để reset về false)
            isAlert = false;
            targetColor = safeColor;
        }

        if (meshRenderer != null) meshRenderer.material.color = targetColor;
        // if (camSpriteRenderer != null) camSpriteRenderer.color = targetColor; 
    }



    bool CheckPlayerVisible()
    {
        if (player == null) return false;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > viewDistance) return false;

        Vector3 dir = (player.position - transform.position).normalized;
        // Check góc nhìn dựa trên hướng hiện tại của ViewMesh
        if (viewMeshFilter != null)
        {
            if (Vector3.Angle(viewMeshFilter.transform.up, dir) > viewAngle / 2) return false;
        }

        if (Physics2D.Raycast(transform.position, dir, dist, obstacleMask)) return false;

        return true;
    }

    void DrawCone()
    {
        if (viewMeshFilter == null) return;

        int rayCount = 50;
        float angleStep = viewAngle / rayCount;
        // Lấy góc bắt đầu dựa trên hướng hiện tại của Mesh
        float startAngle = GetAngleFromVectorFloat(viewMeshFilter.transform.up) + viewAngle / 2f;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            float currentAngle = startAngle - angleStep * i;
            Vector3 dir = GetVectorFromAngle(currentAngle);
            Vector3 vertex;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewDistance, obstacleMask);

            // Convert World point sang Local point để vẽ Mesh đúng
            if (hit.collider != null)
                vertex = viewMeshFilter.transform.InverseTransformPoint(hit.point);
            else
                vertex = viewMeshFilter.transform.InverseTransformPoint(transform.position + dir * viewDistance);

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex++;
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.uv = uv;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateBounds();
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
