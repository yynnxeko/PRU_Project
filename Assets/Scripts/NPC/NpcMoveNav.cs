using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NpcMoveNav : MonoBehaviour
{
    // Cấu trúc dữ liệu cho Waypoint xịn hơn
    [System.Serializable]
    public class WaypointInfo
    {
        public Transform point;      // Kéo cái GameObject Waypoint vào đây
        public bool isChair = false; // Tick vào nếu đây là điểm ngồi
        public Vector2 sitFaceDirection = new Vector2(0, -1); // Hướng mặt khi ngồi (Mặc định quay xuống)
    }

    [Header("Path Settings")]
    public List<WaypointInfo> waypoints; // Dùng List thay vì mảng Transform thường
    public float arriveDistance = 0.1f;

    [Header("Status")]
    public bool canMove = true;
    public bool IsSitting = false;

    // ... (Các biến cũ giữ nguyên)
    int currentIndex;
    NavMeshAgent agent;
    Animator anim;
    bool isFinished;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (IsSitting) return; // Đang ngồi thì nghỉ khỏe

        if (!canMove || isFinished)
        {
            if (agent.enabled) agent.isStopped = true;
            UpdateAnimation(false);
            return;
        }

        if (agent.enabled) agent.isStopped = false;

        CheckDestination();
        Move();
        UpdateAnimation(true);
    }

    void CheckDestination()
    {
        if (waypoints == null || waypoints.Count == 0) return;
        if (currentIndex >= waypoints.Count) return;

        // Logic check đến nơi
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + arriveDistance)
        {
            // Đã đến điểm hiện tại!
            WaypointInfo currentWP = waypoints[currentIndex];

            // 1. Kiểm tra xem điểm này có phải là GHẾ không?
            if (currentWP.isChair)
            {
                // Thực hiện hành động NGỒI ngay tại điểm này
                SitDown(currentWP);
            }
            else
            {
                // Nếu không phải ghế, đi tiếp điểm sau
                currentIndex++;
                if (currentIndex >= waypoints.Count)
                {
                    isFinished = true;
                }
            }
        }
    }

    void Move()
    {
        if (isFinished || IsSitting) return;
        if (currentIndex >= waypoints.Count) return;

        agent.SetDestination(waypoints[currentIndex].point.position);
    }

    // ==========================================
    // HÀM NGỒI (Sửa lại theo ý ông)
    // ==========================================
    void SitDown(WaypointInfo wpData)
    {
        IsSitting = true;
        canMove = false;
        isFinished = true;

        agent.isStopped = true;
        agent.enabled = false;
        transform.position = wpData.point.position;

        // GỌI TRỰC TIẾP (đừng gọi qua hàm trung gian nào cả cho chắc)
        if (anim)
        {
            anim.SetBool("IsSitting", true);

            // Ép hướng ngồi vào Animator
            anim.SetFloat("LastInputX", wpData.sitFaceDirection.x);
            anim.SetFloat("LastInputY", wpData.sitFaceDirection.y);

            // Reset mấy biến cũ
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputY", 0);
        }
    }


    // ... (Các hàm UpdateAnimation, SetAnimDirection, StartWork giữ nguyên như cũ)

    void UpdateAnimation(bool isMoving) { /* ... Code cũ ... */ }
    void SetAnimDirection(float x, float y) { /* ... Code cũ ... */ }
}
