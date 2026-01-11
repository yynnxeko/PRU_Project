using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float normalSpeed = 3f;
    public float slowSpeed = 1.5f;

    public Rigidbody2D rb;
    public PlayerNoise noise;

    Vector2 movement;
    float currentSpeed;

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveX, moveY).normalized;

        bool isHoldingShift =
            Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        currentSpeed = isHoldingShift ? slowSpeed : normalSpeed;

        noise.isMoving = movement != Vector2.zero;
        noise.isSlowWalking = isHoldingShift;
    }

    void FixedUpdate()
    {
        rb.velocity = movement * currentSpeed;
    }
}
