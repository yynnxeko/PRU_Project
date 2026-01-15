using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Animator animator;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    private Vector2 lastMoveDir = Vector2.down;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Không cho đi chéo (ưu tiên trục X)
        if (moveInput.x != 0)
            moveInput.y = 0;

        // Set Speed
        animator.SetFloat("Speed", moveInput.sqrMagnitude);

        // Nếu đang di chuyển
        if (moveInput != Vector2.zero)
        {
            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);

            lastMoveDir = moveInput;
        }
        else
        {
            // Khi đứng yên → giữ hướng cuối
            animator.SetFloat("MoveX", lastMoveDir.x);
            animator.SetFloat("MoveY", lastMoveDir.y);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}
