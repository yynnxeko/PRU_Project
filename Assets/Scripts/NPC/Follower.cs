using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target;
    public float followDistance = 0.8f;
    public float followSpeed = 2f;

    public int indexInLine = 0;        // 0,1,2,... cho từng NPC
    public float spacing = 0.6f;       // khoảng cách giữa mỗi NPC

    public bool isPaused;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (target == null || isPaused)
        {
            SetAnimInput(Vector2.zero, false);
            return;
        }

        // mỗi NPC lùi thêm spacing * indexInLine
        Vector2 offsetBehind = new Vector2(0f, -spacing * (indexInLine + 1));
        Vector2 desiredPos = (Vector2)target.position + offsetBehind;

        float dist = Vector2.Distance(transform.position, desiredPos);
        if (dist > followDistance)
        {
            // Tính hướng di chuyển
            Vector2 direction = (desiredPos - (Vector2)transform.position).normalized;
            SetAnimInput(direction, true);

            transform.position = Vector2.MoveTowards(
                transform.position,
                desiredPos,
                followSpeed * Time.deltaTime
            );
        }
        else
        {
            // Đứng yên khi đã đủ gần
            SetAnimInput(Vector2.zero, false);
        }
    }

    // =====================
    // ANIMATOR INPUT
    // =====================
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

    public void Pause()
    {
        isPaused = true;
        SetAnimInput(Vector2.zero, false);
    }

    public void Resume() => isPaused = false;
}
