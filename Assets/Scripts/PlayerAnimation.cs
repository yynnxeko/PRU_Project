using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public float moveSpeed = 5f;

    // IDLE
    public AnimationClip idleEast;
    public AnimationClip idleWest;
    public AnimationClip idleNorth;
    public AnimationClip idleSouth;

    // WALK
    public AnimationClip walkEast;
    public AnimationClip walkWest;
    public AnimationClip walkNorth;
    public AnimationClip walkSouth;

    Rigidbody2D rb;
    Vector2 input;
    Vector2 lastMoveDir = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();
    }

    void Update()
    {
        input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (input != Vector2.zero)
        {
            lastMoveDir = input;
            PlayWalkByDirection();
        }
        else
        {
            PlayIdleByDirection();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = input * moveSpeed;
    }

    // ================= IDLE =================
    void PlayIdleByDirection()
    {
        if (Mathf.Abs(lastMoveDir.x) > Mathf.Abs(lastMoveDir.y))
        {
            if (lastMoveDir.x > 0)
                animator.Play(idleEast.name);
            else
                animator.Play(idleWest.name);
        }
        else
        {
            if (lastMoveDir.y > 0)
                animator.Play(idleNorth.name);
            else
                animator.Play(idleSouth.name);
        }
    }

    // ================= WALK =================
    void PlayWalkByDirection()
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            if (input.x > 0)
                animator.Play(walkEast.name);
            else
                animator.Play(walkWest.name);
        }
        else
        {
            if (input.y > 0)
                animator.Play(walkNorth.name);
            else
                animator.Play(walkSouth.name);
        }
    }
}
