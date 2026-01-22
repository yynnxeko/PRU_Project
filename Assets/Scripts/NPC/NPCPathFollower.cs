using UnityEngine;

public class NPCPathFollower : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;

    int currentIndex;
    bool isPaused;

    public bool IsFinished => currentIndex >= waypoints.Length;

    void Update()
    {
        Debug.Log("NPC Update: IsFinished=" + IsFinished + " isPaused=" + isPaused);
        if (IsFinished || isPaused) return;

        Move();
    }

    void Move()
    {
        Transform target = waypoints[currentIndex];
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
            currentIndex++;
    }

    // 🔹 NPC DỪNG
    public void Pause()
    {
        Debug.Log("NPC PAUSED");
        isPaused = true;
    }

    // 🔹 NPC ĐI TIẾP
    public void Resume()
    {
        Debug.Log("NPC RESUMED");
        isPaused = false;
    }


    public void ResetPath()
    {
        currentIndex = 0;
        isPaused = false;
    }
}
