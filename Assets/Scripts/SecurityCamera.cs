using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("--- MỤC TIÊU & CẤU HÌNH ---")]
    public Transform player;
    public LayerMask obstacleMask;
    public float viewDistance = 8f;
    public float viewAngle = 45f;
    public float wallIgnoreDist = 0.5f; // Khoảng cách tối thiểu để bắt đầu tính là vật cản (để nhìn xuyên tường đang đứng)

    [Header("--- ANIMATION & VISUAL ---")]
    public Animator animator;
    public string motionTimeParam = "CamAngle";
    public SpriteRenderer camSpriteRenderer;

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
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
            else
            {
                Debug.LogWarning("Không tìm thấy Player với tag 'Player'");
            }
        }
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

            // --- FIX LỖI ĐÈN BỊ CHE ---
            // Đặt Sorting Layer cho đèn nằm dưới Camera nhưng trên nền
            meshRenderer.sortingLayerName = "Decorations";
            meshRenderer.sortingOrder = -1;
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

        // --- B. ĐỒNG BỘ ANIMATION ---
        SyncAnimationToMeshAngle();

        // --- C. VẼ MESH ---
        DrawCone();
    }

    // --- HÀNH VI 1: ĐI TUẦN ---
    void PatrolRotation()
    {
        float angleOffset = Mathf.Sin(Time.time * rotationSpeed) * rotationAngleMax;
        currentMeshAngle = baseRotation + angleOffset;

        if (viewMeshFilter != null)
            viewMeshFilter.transform.rotation = Quaternion.Euler(0, 0, currentMeshAngle);
    }

    // --- HÀNH VI 2: FOCUS ---
    void FocusOnPlayer()
    {
        if (player == null) return;

        Vector3 dir = (player.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        Quaternion currentQ = Quaternion.Euler(0, 0, currentMeshAngle);
        Quaternion targetQ = Quaternion.Euler(0, 0, targetAngle);
        Quaternion resultQ = Quaternion.RotateTowards(currentQ, targetQ, focusSpeed * 100f * Time.deltaTime);

        currentMeshAngle = resultQ.eulerAngles.z;

        if (viewMeshFilter != null)
            viewMeshFilter.transform.rotation = resultQ;
    }

    // --- HÀM ĐỒNG BỘ ANIMATION ---
    void SyncAnimationToMeshAngle()
    {
        if (animator == null) return;

        float angleDifference = Mathf.DeltaAngle(baseRotation, currentMeshAngle);
        float normalizedTime = Mathf.InverseLerp(-rotationAngleMax, rotationAngleMax, angleDifference);

        float finalTime = 1f - normalizedTime;
        finalTime = Mathf.Clamp(finalTime, 0.05f, 0.95f); // Kẹp giá trị tránh lỗi frame

        animator.SetFloat(motionTimeParam, finalTime);
    }

    // --- CÁC HÀM PHỤ TRỢ (QUAN TRỌNG: ĐÃ SỬA RAYCAST) ---
    void UpdateStateColor(bool seeingPlayer)
    {
        Color targetColor = safeColor;

        if (suspicionLevel >= 100f)
        {
            targetColor = alertColor;
            isAlert = true;
            if (GameManager.Instance != null && player != null) GameManager.Instance.AlertAllEnemies(player.position);
        }
        else if (suspicionLevel > 0f)
        {
            targetColor = warningColor;
            isAlert = false;
        }
        else
        {
            isAlert = false;
            targetColor = safeColor;
        }

        if (meshRenderer != null) meshRenderer.material.color = targetColor;
    }

    // --- HÀM CHECK PLAYER: DÙNG RAYCAST ALL ĐỂ XUYÊN TƯỜNG ĐANG ĐỨNG ---
    bool CheckPlayerVisible()
    {
        if (player == null) return false;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > viewDistance) return false;

        Vector3 dir = (player.position - transform.position).normalized;

        // Check góc nhìn
        if (viewMeshFilter != null)
        {
            if (Vector3.Angle(viewMeshFilter.transform.up, dir) > viewAngle / 2) return false;
        }

        // Bắn Ray xuyên táo tất cả vật cản
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, dist, obstacleMask);

        foreach (var hit in hits)
        {
            // Nếu vật cản ở quá gần (< 0.5f) -> Coi như là bức tường Camera đang gắn vào -> BỎ QUA
            if (hit.distance < wallIgnoreDist) continue;

            // Nếu gặp vật cản ở xa (thật sự là chướng ngại vật) -> BỊ CHẶN
            return false;
        }

        // Không gặp vật cản hợp lệ nào -> NHÌN THẤY
        return true;
    }

    // --- HÀM VẼ CONE: DÙNG RAYCAST ALL ---
    void DrawCone()
    {
        if (viewMeshFilter == null) return;

        int rayCount = 50;
        float angleStep = viewAngle / rayCount;
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

            // Bắn Ray xuyên táo
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, viewDistance, obstacleMask);

            // Mặc định điểm cuối là xa tít (nếu không trúng gì)
            Vector3 worldHitPoint = transform.position + dir * viewDistance;
            bool hitSomethingValid = false;

            // Tìm điểm va chạm gần nhất NHƯNG phải xa hơn ngưỡng wallIgnoreDist
            // Vì RaycastAll trả về thứ tự ngẫu nhiên, ta cần tìm cái gần nhất thỏa mãn điều kiện
            float minValidDist = viewDistance + 1f;

            foreach (var hit in hits)
            {
                if (hit.distance < wallIgnoreDist) continue; // Bỏ qua tường tại chỗ

                if (hit.distance < minValidDist)
                {
                    minValidDist = hit.distance;
                    worldHitPoint = hit.point;
                    hitSomethingValid = true;
                }
            }

            vertex = viewMeshFilter.transform.InverseTransformPoint(worldHitPoint);
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
