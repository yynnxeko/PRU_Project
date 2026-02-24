using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FollowCheckZone : MonoBehaviour
{
    [Header("Assign Player Here")]
    [Tooltip("Kéo Player ROOT hoặc Empty Player vào đây")]
    public Transform player;

    [Header("Debug (Read Only)")]
    [SerializeField] bool playerInside;
    public bool PlayerInside => playerInside;

    int insideCount = 0;
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
    }
    void Reset()
    {
        // Tự bật IsTrigger cho đỡ quên
        var col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;

        insideCount++;
        playerInside = true;

        Debug.Log("[FollowZone] Player ENTER | count = " + insideCount);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;

        insideCount--;
        if (insideCount <= 0)
        {
            insideCount = 0;
            playerInside = false;
            Debug.Log("[FollowZone] Player EXIT");
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (IsPlayer(other))
            playerInside = true;
    }

    bool IsPlayer(Collider2D other)
    {
        if (player == null) return false;

        // Nhận collider ở root hoặc con của player
        return other.transform == player
            || other.transform.IsChildOf(player);
    }
}
