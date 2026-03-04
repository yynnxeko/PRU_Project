using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset;

    [Header("Camera Bounds")]
    [Tooltip("Bật giới hạn camera theo map")]
    public bool useBounds = true;
    public Collider2D boundsCollider;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Nếu chưa có target, tự tìm object player theo tag "Player"
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        FindBounds();
    }

    void LateUpdate()
    {
        // Nếu vẫn chưa có target thì thử tìm lại
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                return;
        }

        // Retry tìm bounds nếu chưa có
        if (useBounds && boundsCollider == null)
            FindBounds();

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = -10;

        Vector3 smoothed = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        if (useBounds && boundsCollider != null)
            smoothed = ClampCamera(smoothed);

        transform.position = smoothed;
    }

    private Vector3 ClampCamera(Vector3 pos)
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        Bounds b = boundsCollider.bounds;
        float clampedX = Mathf.Clamp(pos.x, b.min.x + halfWidth, b.max.x - halfWidth);
        float clampedY = Mathf.Clamp(pos.y, b.min.y + halfHeight, b.max.y - halfHeight);

        return new Vector3(clampedX, clampedY, pos.z);
    }

    void FindBounds()
    {
        GameObject boundsObj = GameObject.Find("CameraBounds");

        if (boundsObj == null)
        {
            Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
            foreach (var col in allColliders)
            {
                if (col.gameObject.name.ToLower().Contains("camerabounds") ||
                    col.gameObject.name.ToLower().Contains("camera bounds"))
                {
                    boundsObj = col.gameObject;
                    break;
                }
            }

            if (boundsObj == null)
            {
                foreach (var col in allColliders)
                {
                    if (col is BoxCollider2D box && box.isTrigger &&
                        col.bounds.size.x > 20f && col.bounds.size.y > 10f)
                    {
                        boundsObj = col.gameObject;
                        break;
                    }
                }
            }
        }

        if (boundsObj != null)
        {
            boundsCollider = boundsObj.GetComponent<Collider2D>();
            if (boundsCollider != null)
                Debug.Log("[CameraFollow] ✅ CameraBounds OK! bounds=" + boundsCollider.bounds);
        }
    }
}
