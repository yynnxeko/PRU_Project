using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    public float moveSpeed = 5f;

    Rigidbody2D rb;
    Animator animator;

    Vector2 movement;
    Vector2 lastMoveDir;

    // 🔒 Khóa di chuyển khi ngủ
    public bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        lastMoveDir = Vector2.down;
    }

    void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void UpdateAnimation()
    {
        bool isMoving = movement.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            animator.SetFloat("InputX", movement.x);
            animator.SetFloat("InputY", movement.y);
            animator.SetBool("IsMoving", true);

            lastMoveDir = movement;
            animator.SetFloat("LastInputX", lastMoveDir.x);
            animator.SetFloat("LastInputY", lastMoveDir.y);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }
}
