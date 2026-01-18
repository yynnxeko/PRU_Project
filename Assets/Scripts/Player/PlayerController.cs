using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 3f;

    public Rigidbody2D rb;
    public Animator animator;

    [HideInInspector]
    public Vector2 faceDir = Vector2.down;

    Vector2 input;

    void Update()
    {
        // ===== INPUT (CHO PHÉP ĐI CHÉO) =====
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        input = new Vector2(x, y);

        // ===== XÁC ĐỊNH HƯỚNG NHÌN (CHỈ 4 HƯỚNG) =====
        if (input != Vector2.zero)
        {
            if (Mathf.Abs(input.y) >= Mathf.Abs(input.x))
                faceDir = input.y > 0 ? Vector2.up : Vector2.down;
            else
                faceDir = input.x > 0 ? Vector2.right : Vector2.left;
        }

        // ===== ANIMATOR =====
        animator.SetFloat("MoveX", faceDir.x);
        animator.SetFloat("MoveY", faceDir.y);
        animator.SetFloat("Speed", input.sqrMagnitude);
    }

    void FixedUpdate()
    {
        // 🚀 DI CHUYỂN THẬT (CHO PHÉP CHÉO)
        rb.velocity = input.normalized * speed;
    }
}
