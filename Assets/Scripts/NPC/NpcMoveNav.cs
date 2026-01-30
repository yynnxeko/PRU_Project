using UnityEngine;
using UnityEngine.AI;

public class NpcMoveNav : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] waypoints;
    public float arriveDistance = 0.1f;

    // Biến này để kiểm soát "Đúng giờ mới đi"
    [Header("Status")]
    public bool canMove = true; // Mặc định là FALSE (đứng yên chờ lệnh)

    int currentIndex;
    NavMeshAgent agent;

    // Debug xem đến nơi chưa
    [SerializeField] bool isFinished;
    public bool IsFinished => isFinished;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Setup chuẩn cho 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Random né nhau
        agent.avoidancePriority = Random.Range(30, 60);
    }

    void Update()
    {
        // Nếu chưa đến giờ (canMove = false) hoặc đã đi xong -> Đứng im
        if (!canMove || isFinished)
        {
            if (!agent.isStopped) agent.isStopped = true;
            UpdateAnimation(false); // Chạy anim Idle
            return;
        }
        else
        {
            if (agent.isStopped) agent.isStopped = false;
        }

        CheckIfFinished();
        Move();
        UpdateAnimation(true); // Chạy anim Walk
    }

    void CheckIfFinished()
    {
        // Kiểm tra xem đã đi hết danh sách điểm chưa
        if (waypoints == null || waypoints.Length == 0 || currentIndex >= waypoints.Length)
        {
            isFinished = true;
        }
    }

    void Move()
    {
        if (isFinished) return;

        Transform target = waypoints[currentIndex];
        agent.SetDestination(target.position);

        // Kiểm tra đến điểm hiện tại chưa
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + arriveDistance)
        {
            currentIndex++; // Chuyển sang điểm tiếp theo
        }
    }

    // Hàm xử lý Animation (Quay mặt + Chạy/Đứng)
    void UpdateAnimation(bool isMoving)
    {
        var anim = GetComponent<Animator>();
        if (anim == null) return;

        if (isMoving && agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            anim.SetFloat("InputX", direction.x);
            anim.SetFloat("InputY", direction.y);
            anim.SetBool("IsMoving", true);

            // Lưu hướng quay mặt
            anim.SetFloat("LastInputX", direction.x);
            anim.SetFloat("LastInputY", direction.y);
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }
    }

    // ==========================================
    // HÀM GỌI TỪ HỆ THỐNG GIỜ (TIME MANAGER)
    // ==========================================

    // Gọi hàm này khi đến giờ làm việc (ví dụ 8:00 AM)
    public void StartWork()
    {
        canMove = true;
        isFinished = false;
        currentIndex = 0;
        Debug.Log("NPC bắt đầu đi làm!");
    }

    // Gọi hàm này khi hết giờ (nếu cần dừng lại ngay lập tức)
    public void StopWork()
    {
        canMove = false;
        agent.isStopped = true;
    }
}
