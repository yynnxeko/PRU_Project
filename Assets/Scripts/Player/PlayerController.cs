using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    [HideInInspector]
    public Vector2 faceDir = Vector2.down; // Mặc định nhìn xuống

    Vector2 input;
    bool isRunning;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. INPUT
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        input = new Vector2(x, y);

        isRunning = Input.GetKey(KeyCode.LeftShift) && input.sqrMagnitude > 0;

        // 2. XÁC ĐỊNH HƯỚNG NHÌN (CHỈ 4 HƯỚNG)
        // Logic này sẽ ép hướng chéo về hướng ngang hoặc dọc
        if (input.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            {
                // Ưu tiên đi dọc (Lên/Xuống)
                faceDir = input.y > 0 ? Vector2.up : Vector2.down;
            }
            else
            {
                // Ưu tiên đi ngang (Trái/Phải)
                faceDir = input.x > 0 ? Vector2.right : Vector2.left;
            }
        }

        // 3. GỬI VÀO ANIMATOR
        // Gửi faceDir (đã ép về 0,1 hoặc 1,0) để Animator biết hướng chính xác
        if (input.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("InputX", faceDir.x);
            animator.SetFloat("InputY", faceDir.y);

            // Lưu LastInput y chang Input
            animator.SetFloat("LastInputX", faceDir.x);
            animator.SetFloat("LastInputY", faceDir.y);

            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

        animator.SetBool("IsRunning", isRunning);
    }

    void FixedUpdate()
    {
        // Di chuyển thì vẫn cho phép đi chéo mượt mà
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        rb.velocity = input.normalized * currentSpeed;
    }
}
