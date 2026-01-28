using UnityEngine;

public class NPCPathFollower : MonoBehaviour
{
    [Header("Path")]
    public Transform[] waypoints;
    public float speed = 2f;
    public float arriveDistance = 0.1f;

    int currentIndex;
    bool isPaused;

    // Debug trạng thái (xem trong Inspector)
    [Header("Debug (Read Only)")]
    [SerializeField] bool isFinished;

    public bool IsFinished => isFinished;

    void Update()
    {
        // 🔒 Khóa khi đang bus cutscene
        if (GameFlow.BusCutscene) return;

        UpdateFinishedState();

        if (isFinished || isPaused) return;

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

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) <= arriveDistance)
        {
            currentIndex++;
        }
    }

    // ======================
    // CONTROL
    // ======================
    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;
        Debug.Log("[NPC] PAUSED");
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;
        Debug.Log("[NPC] RESUMED");
    }

    public void ResetPath()
    {
        currentIndex = 0;
        isPaused = false;
        UpdateFinishedState();
        Debug.Log("[NPC] RESET PATH");
    }
}
