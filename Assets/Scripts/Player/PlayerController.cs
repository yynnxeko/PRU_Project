using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;

    Rigidbody2D rb;
    Animator animator;

    Vector2 movement;
    Vector2 lastMovement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Nếu đang di chuyển thì lưu hướng cuối
        if (movement != Vector2.zero)
        {
            lastMovement = movement.normalized;

            animator.SetFloat("InputX", movement.x);
            animator.SetFloat("InputY", movement.y);

            animator.SetFloat("LastInputX", lastMovement.x);
            animator.SetFloat("LastInputY", lastMovement.y);

            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}
