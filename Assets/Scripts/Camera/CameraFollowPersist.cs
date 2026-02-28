using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollowPersist : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private float smooth = 10f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("Persist")]
    [SerializeField] private bool dontDestroyOnLoad = true;

    [Header("Camera Bounds")]
    [Tooltip("Bật giới hạn camera theo map")]
    [SerializeField] private bool useBounds = true;

    [Tooltip("Kéo Collider2D dùng làm giới hạn vào đây, hoặc để trống để tự tìm object tên 'CameraBounds'")]
    [SerializeField] private Collider2D boundsCollider;

    [Tooltip("Nếu không dùng Collider2D, set giới hạn thủ công")]
    [SerializeField] private Vector2 minBounds = new Vector2(-50f, -50f);
    [SerializeField] private Vector2 maxBounds = new Vector2(50f, 50f);

    private Transform target;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        gameObject.tag = "MainCamera";
        if (dontDestroyOnLoad)
        {
            var cams = FindObjectsOfType<Camera>();
            foreach (var c in cams)
            {
                if (c != cam) Destroy(c.gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        FindPlayer();
        FindBounds();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
        // Reset bounds để tìm lại cho scene mới
        boundsCollider = null;
        FindBounds();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Nếu chưa tìm được bounds, thử tìm lại
        if (useBounds && boundsCollider == null)
        {
            FindBounds();
        }

        Vector3 desired = target.position + offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, smooth * Time.deltaTime);

        // Clamp camera trong giới hạn map
        if (useBounds && boundsCollider != null)
        {
            smoothed = ClampCamera(smoothed);
        }

        transform.position = smoothed;
    }

    /// <summary>
    /// Clamp vị trí camera sao cho viewport không vượt ra ngoài giới hạn map.
    /// Tính toán dựa trên half-size của camera để mép camera không bị tràn.
    /// </summary>
    private Vector3 ClampCamera(Vector3 pos)
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        Bounds b = boundsCollider.bounds;
        float clampedX = Mathf.Clamp(pos.x, b.min.x + halfWidth, b.max.x - halfWidth);
        float clampedY = Mathf.Clamp(pos.y, b.min.y + halfHeight, b.max.y - halfHeight);

        return new Vector3(clampedX, clampedY, pos.z);
    }

    void FindPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        target = player != null ? player.transform : null;
    }

    /// <summary>
    /// Tự tìm object có tên "CameraBounds" trong scene để làm giới hạn.
    /// Object đó cần có Collider2D (BoxCollider2D được khuyên dùng).
    /// </summary>
    void FindBounds()
    {
        // Phương pháp 1: Tìm bằng tên chính xác
        GameObject boundsObj = GameObject.Find("CameraBounds");

        // Phương pháp 2: Nếu không tìm thấy, tìm tất cả Collider2D trong scene
        if (boundsObj == null)
        {
            Debug.Log("[CameraFollowPersist] GameObject.Find('CameraBounds') không tìm thấy. Đang tìm bằng cách khác...");

            Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
            foreach (var col in allColliders)
            {
                // Tìm object có tên chứa "CameraBounds" hoặc "camerabounds" (không phân biệt hoa thường)
                if (col.gameObject.name.ToLower().Contains("camerabounds") ||
                    col.gameObject.name.ToLower().Contains("camera bounds"))
                {
                    boundsObj = col.gameObject;
                    Debug.Log("[CameraFollowPersist] Tìm thấy qua fallback: '" + col.gameObject.name + "'");
                    break;
                }
            }

            // Phương pháp 3: Tìm bất kỳ BoxCollider2D nào là trigger
            if (boundsObj == null)
            {
                foreach (var col in allColliders)
                {
                    if (col is BoxCollider2D box && box.isTrigger &&
                        col.bounds.size.x > 20f && col.bounds.size.y > 10f)
                    {
                        boundsObj = col.gameObject;
                        Debug.Log("[CameraFollowPersist] Tìm thấy bounds qua BoxCollider2D trigger lớn: '" + col.gameObject.name + "' bounds=" + col.bounds);
                        break;
                    }
                }
            }
        }

        if (boundsObj != null)
        {
            boundsCollider = boundsObj.GetComponent<Collider2D>();
            if (boundsCollider != null)
                Debug.Log("[CameraFollowPersist] ✅ CameraBounds OK! bounds=" + boundsCollider.bounds);
            else
                Debug.LogWarning("[CameraFollowPersist] ⚠ Object '" + boundsObj.name + "' có nhưng không có Collider2D!");
        }
        else
        {
            Debug.LogWarning("[CameraFollowPersist] ❌ Không tìm thấy CameraBounds trong scene!");
        }
    }
}
