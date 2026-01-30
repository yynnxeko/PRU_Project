using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    public float moveSpeed = 5f;

    // Bỏ public để không hiện ngoài Inspector cho đỡ rối
    Rigidbody2D rb;
    Animator animator;

    Vector2 movement;
    Vector2 lastMoveDir;

    void Start()
    {
        // Tự động tìm Component trên chính GameObject này
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        lastMoveDir = Vector2.down;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        UpdateAnimation();
    }

    void FixedUpdate()
    {
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
