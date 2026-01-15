using UnityEngine;
using UnityEngine.UI;

public class EnemyAi : MonoBehaviour
{
    [Header("Cài đặt Tầm nhìn Cơ bản")]
    public Transform player;
    public float baseDistance = 6f; // Tầm xa gốc
    public float baseAngle = 70f;   // Góc mở gốc
    public float innerViewDistance = 3f;
    public LayerMask obstacleMask;

    [Header("Nâng cấp 2: Góc nhìn động")]
    public float focusDistance = 7f; // Khi nghi ngờ nhìn xa hơn
    public float focusAngle = 40f;   // Khi nghi ngờ góc hẹp lại
    public float changeSpeed = 7.5f;   // Tốc độ biến đổi hình dạng nón

    [Header("Gán 2 cái Nón")]
    public MeshFilter meshFilterFar;
    public MeshFilter meshFilterNear;

    [Header("Màu sắc")]
    public Color colorFar = new Color(1, 0, 0, 0.2f);
    public Color colorNear = new Color(1, 0, 0, 0.6f);
    public Color colorAlert = new Color(1, 0, 0, 1f); // Màu báo động (Đỏ đặc)

    [Header("Logic Nghi ngờ")]
    public float suspicionLevel = 0f;
    public float cooldownRate = 15f;
    public float normalRate = 20f;
    public float fastRate = 50f;

    // Biến nội bộ
    private float currentViewDist;
    private float currentViewAngle;
    private bool isAlert = false;
    private float flashTimer = 0f;

    private Mesh meshFar;
    private Mesh meshNear;
    private SpriteRenderer myBodyColor;
    private MeshRenderer renderFar;
    private MeshRenderer renderNear;

    void Start()
    {
        myBodyColor = GetComponent<SpriteRenderer>();

        // Khởi tạo giá trị ban đầu
        currentViewDist = baseDistance;
        currentViewAngle = baseAngle;

        // Setup Nón Xa
        if (meshFilterFar != null)
        {
            meshFar = new Mesh();
            meshFilterFar.mesh = meshFar;
            renderFar = meshFilterFar.GetComponent<MeshRenderer>();
            if (renderFar)
            {
                renderFar.material = new Material(Shader.Find("Sprites/Default"));
                renderFar.material.color = colorFar;
                renderFar.sortingOrder = 1;
            }
        }

        // Setup Nón Gần
        if (meshFilterNear != null)
        {
            meshNear = new Mesh();
            meshFilterNear.mesh = meshNear;
            renderNear = meshFilterNear.GetComponent<MeshRenderer>();
            if (renderNear)
            {
                renderNear.material = new Material(Shader.Find("Sprites/Default"));
                renderNear.material.color = colorNear;
                renderNear.sortingOrder = 2;
            }
        }
    }

    void Update()
    {
        LogicSuspicion();

        // Vẽ nón với thông số động (biến đổi liên tục)
        DrawCone(meshFar, currentViewDist, currentViewAngle);
        DrawCone(meshNear, innerViewDistance, currentViewAngle); // Nón gần cũng thu hẹp góc theo
    }

    void LogicSuspicion()
    {
        if (player == null) return;

        // --- 1. XỬ LÝ NGHI NGỜ ---
        int zone = CheckZone();
        bool isSeeingPlayer = (zone > 0);

        if (isSeeingPlayer)
        {
            float rate = (zone == 2) ? fastRate : normalRate;
            suspicionLevel += rate * Time.deltaTime;

            // --- MỚI: TỰ ĐỘNG XOAY ĐẦU VỀ PHÍA MỤC TIÊU ---
            // Chỉ xoay khi đang nhìn thấy (Line of Sight không bị chặn)
            Vector3 dirToPlayer = (player.position - transform.position).normalized;

            // Tính góc xoay (sửa tên biến để không trùng)
            float rotationAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

            // Xoay từ từ về phía đó (cho mượt)
            // Trừ 90 độ vì Sprite mặc định thường hướng Lên (Up), còn Atan2 tính theo hướng Phải (Right)
            Quaternion targetRotation = Quaternion.Euler(0, 0, rotationAngle - 90);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
        else
        {
            suspicionLevel -= cooldownRate * Time.deltaTime;
            isAlert = false; // Mất dấu thì hết báo động

            // KHÔNG xoay ở đây -> Giữ nguyên hướng nhìn cuối cùng (Looking at last known position)
        }

        suspicionLevel = Mathf.Clamp(suspicionLevel, 0f, 100f);

        // --- 2. XỬ LÝ GÓC NHÌN ĐỘNG (Dynamic FOV) ---
        float targetDist = baseDistance;
        float targetAngle = baseAngle;

        if (suspicionLevel > 0)
        {
            // Càng nghi ngờ nhiều -> Càng biến đổi về dạng "Tập trung" (Focus)
            float factor = suspicionLevel / 100f;
            targetDist = Mathf.Lerp(baseDistance, focusDistance, factor);
            targetAngle = Mathf.Lerp(baseAngle, focusAngle, factor);
        }

        // Làm mượt chuyển động co giãn
        currentViewDist = Mathf.Lerp(currentViewDist, targetDist, changeSpeed * Time.deltaTime);
        currentViewAngle = Mathf.Lerp(currentViewAngle, targetAngle, changeSpeed * Time.deltaTime);


        // --- 3. XỬ LÝ MÀU SẮC & BÁO ĐỘNG (Alert Flash) ---
        if (suspicionLevel >= 100f)
        {
            isAlert = true;
            flashTimer += Time.deltaTime * 10f;

            // Nhấp nháy Đỏ - Đen
            if (Mathf.Sin(flashTimer) > 0)
            {
                // Khi sáng (Đỏ)
                myBodyColor.color = Color.red;
                if (renderNear) renderNear.material.color = colorAlert;
                if (renderFar) renderFar.material.color = colorAlert; // <-- THÊM DÒNG NÀY
            }
            else
            {
                // Khi tối (Đen) - Trả về màu gốc của từng nón
                myBodyColor.color = Color.black;
                if (renderNear) renderNear.material.color = colorNear;
                if (renderFar) renderFar.material.color = colorFar;   // <-- THÊM DÒNG NÀY
            }
        }
        else if (suspicionLevel > 0f)
        {
            // Mức nghi ngờ (Vàng)
            myBodyColor.color = Color.yellow;

            // Đảm bảo cả 2 nón không bị kẹt màu đỏ nếu thoát khỏi trạng thái báo động
            if (renderNear) renderNear.material.color = colorNear;
            if (renderFar) renderFar.material.color = colorFar;       // <-- THÊM DÒNG NÀY
        }
        else
        {
            // Bình thường (Trắng)
            myBodyColor.color = Color.white;
            if (renderNear) renderNear.material.color = colorNear;
            if (renderFar) renderFar.material.color = colorFar;       // <-- THÊM DÒNG NÀY
        }
    }

    // Hàm vẽ nón
    void DrawCone(Mesh meshToDraw, float dist, float angle)
    {
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

            if (hit.collider != null)
                vertex = transform.InverseTransformPoint(hit.point);
            else
                vertex = transform.InverseTransformPoint(transform.position + dir * dist);

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

    // Hàm check zone (Dùng currentViewDist/Angle động)
    int CheckZone()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        // Dùng biến currentViewDist thay vì maxViewDistance cố định
        if (dist > currentViewDist) return 0;

        Vector3 dir = (player.position - transform.position).normalized;

        // Dùng biến currentViewAngle động
        if (Vector3.Angle(transform.up, dir) > currentViewAngle / 2) return 0;

        if (Physics2D.Raycast(transform.position, dir, dist, obstacleMask)) return 0;

        if (dist <= innerViewDistance) return 2;
        return 1;
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
