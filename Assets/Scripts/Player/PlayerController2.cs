using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ChairInfo
{
    public Transform chairPoint;
    public Vector2 sitFaceDirection = new Vector2(0, -1);
    [Range(0.5f, 2f)] public float detectRange = 1.5f;
}

public class PlayerController2 : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Chair")]
    public ChairInfo chair; // chỉ 1 ghế, không cần array

    [Header("Game State")]
    public bool isInGame = false;

    // 🔒 Khóa di chuyển khi ngủ
    public bool canMove = true;

    Rigidbody2D rb;
    Animator animator;
    Vector2 movement;
    Vector2 lastInputStored = Vector2.down;

    bool isSitting = false;
    public bool IsSitting => isSitting;
    bool isNearChair = false;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "IT_Room")
        {
            GameObject wpObj = GameObject.Find("WP_PLAYER");
            if (wpObj != null)
            {
                chair.chairPoint = wpObj.transform;
                Debug.Log("ChairPoint assigned on scene load: " + chair.chairPoint.position);
            }
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

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
        if (isSitting || !canMove) return;
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void CheckChair()
    {
        if (chair == null || chair.chairPoint == null)
        {
            isNearChair = false;
            return;
        }

        float distance = Vector2.Distance(transform.position, chair.chairPoint.position);
        isNearChair = distance <= chair.detectRange;
    }

    void SitDown()
    {
        if (chair == null || chair.chairPoint == null) return;

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

        Vector2 dir = lastInputStored;
        if (dir.sqrMagnitude < 0.01f)
            dir = (chair != null ? chair.sitFaceDirection.normalized : Vector2.down);
        dir = dir.normalized;

        // khoảng cách nhảy ra (chỉnh nếu cần)
        float pushDistance = 0.22f;

        // di chuyển trước khi wake physics
        transform.position = new Vector3(
            transform.position.x + dir.x * pushDistance,
            transform.position.y + dir.y * pushDistance,
            transform.position.z
        );
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