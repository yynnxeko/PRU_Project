using UnityEngine;

[System.Serializable]
public class ChairInfo
{
    public Transform chairPoint;
    public Vector2 sitFaceDirection = new Vector2(0, -1);
    [Range(0.5f, 2f)] public float detectRange = 1f;
}

public class PlayerController2 : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Chair")]
    public ChairInfo chair; // chỉ 1 ghế, không cần array

    [Header("Game State")]
    public bool isInGame = false;

    Rigidbody2D rb;
    Animator animator;
    Vector2 movement;

    bool isSitting = false;
    public bool IsSitting => isSitting;
    bool isNearChair = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isSitting)
        {
            if (!isInGame && Input.GetKeyDown(KeyCode.C)) StandUp();
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        CheckChair();

        if (!isInGame && Input.GetKeyDown(KeyCode.C) && isNearChair)
            SitDown();

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (isSitting) return;
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void CheckChair()
    {
        float distance = Vector2.Distance(transform.position, chair.chairPoint.position);
        isNearChair = distance <= chair.detectRange;
    }


    void SitDown()
    {
        transform.position = chair.chairPoint.position;
        rb.Sleep();

        animator.SetBool("IsSitting", true);
        animator.SetFloat("LastInputX", chair.sitFaceDirection.x);
        animator.SetFloat("LastInputY", chair.sitFaceDirection.y);
        animator.SetFloat("InputX", 0);
        animator.SetFloat("InputY", 0);

        isSitting = true;
    }

    void StandUp()
    {
        isSitting = false;
        rb.WakeUp();

        animator.SetBool("IsSitting", false);
        animator.SetFloat("InputX", 0);
        animator.SetFloat("InputY", 0);
    }

    void UpdateAnimation()
    {
        bool isMoving = movement.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            animator.SetFloat("InputX", movement.x);
            animator.SetFloat("InputY", movement.y);
            animator.SetFloat("LastInputX", movement.x);
            animator.SetFloat("LastInputY", movement.y);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (chair == null || chair.chairPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(chair.chairPoint.position, chair.detectRange);

        Gizmos.color = Color.cyan;
        Vector3 sitDir = new Vector3(chair.sitFaceDirection.x, chair.sitFaceDirection.y, 0);
        Gizmos.DrawRay(chair.chairPoint.position, sitDir * 0.7f);
    }

    public void ForceStandUp()
    {
        if (isSitting) StandUp();
    }
}