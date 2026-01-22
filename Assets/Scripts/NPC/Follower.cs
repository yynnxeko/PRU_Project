using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target;
    public float followDistance = 0.8f;
    public float followSpeed = 2f;

    public int indexInLine = 0;        // 0,1,2,... cho từng NPC
    public float spacing = 0.6f;       // khoảng cách giữa mỗi NPC

    public bool isPaused;

    void Update()
    {
        if (target == null || isPaused) return;

        // mỗi NPC lùi thêm spacing * indexInLine
        Vector2 offsetBehind = new Vector2(0f, -spacing * (indexInLine + 1));
        Vector2 desiredPos = (Vector2)target.position + offsetBehind;

        float dist = Vector2.Distance(transform.position, desiredPos);
        if (dist > followDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                desiredPos,
                followSpeed * Time.deltaTime
            );
        }
    }

    public void Pause() => isPaused = true;
    public void Resume() => isPaused = false;
}
