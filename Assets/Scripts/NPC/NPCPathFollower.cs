using UnityEngine;

public class NPCPathFollower : MonoBehaviour
{
    [Header("Path")]
    public Transform[] waypoints;
    public float speed = 2f;
    public float arriveDistance = 0.1f;

    int currentIndex;
    bool isPaused;

    // Cache Animator
    Animator anim;

    // Debug trạng thái (xem trong Inspector)
    [Header("Debug (Read Only)")]
    [SerializeField] bool isFinished;

    public bool IsFinished => isFinished;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //  Khóa khi đang bus cutscene
        if (GameFlow.BusCutscene) return;

        UpdateFinishedState();

        if (isFinished || isPaused)
        {
            SetAnimInput(Vector2.zero, false);
            return;
        }

        Move();
    }

    void UpdateFinishedState()
    {
        isFinished = waypoints == null
                     || waypoints.Length == 0
                     || currentIndex >= waypoints.Length;
    }

    void Move()
    {
        if (currentIndex < 0 || currentIndex >= waypoints.Length) return;

        Transform target = waypoints[currentIndex];

        // Tính hướng di chuyển
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;

        // Set Animator input theo hướng di chuyển
        SetAnimInput(direction, true);

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) <= arriveDistance)
        {
            currentIndex++;

            // Nếu hết waypoint → dừng anim
            UpdateFinishedState();
            if (isFinished)
            {
                SetAnimInput(Vector2.zero, false);
            }
        }
    }

    // ======================
    // ANIMATOR INPUT
    // ======================
    void SetAnimInput(Vector2 dir, bool moving)
    {
        if (anim == null) return;

        anim.SetFloat("InputX", dir.x);
        anim.SetFloat("InputY", dir.y);

        if (dir != Vector2.zero)
        {
            anim.SetFloat("LastInputX", dir.x);
            anim.SetFloat("LastInputY", dir.y);
        }

        anim.SetBool("IsMoving", moving);
    }

    // ======================
    // CONTROL
    // ======================
    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;
        SetAnimInput(Vector2.zero, false);
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;
    }

    public void ResetPath()
    {
        currentIndex = 0;
        isPaused = false;
        UpdateFinishedState();
    }
}
