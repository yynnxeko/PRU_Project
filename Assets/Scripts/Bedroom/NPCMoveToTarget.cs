using UnityEngine;

public class NPCMoveToTarget : MonoBehaviour
{
    public float speed = 2f;

    Transform target;
    Animator anim;

    public bool IsMoving { get; private set; }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void MoveTo(Transform t)
    {
        target = t;
        IsMoving = true;
    }

    void Update()
    {
        if (!IsMoving || target == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            IsMoving = false;
            anim.SetBool("IsMoving", false);
        }
    }
}
