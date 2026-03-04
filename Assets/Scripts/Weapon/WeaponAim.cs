using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    public PlayerController player;

    Animator animator;

    void Start()
    {
        animator = player.GetComponent<Animator>();
    }

    void Update()
    {
        // Lấy hướng hiện tại từ Animator
        float x = animator.GetFloat("InputX");
        float y = animator.GetFloat("InputY");

        Vector2 dir = new Vector2(x, y);

        // Nếu không di chuyển -> dùng hướng idle cuối
        if (dir == Vector2.zero)
        {
            x = animator.GetFloat("LastInputX");
            y = animator.GetFloat("LastInputY");
            dir = new Vector2(x, y);
        }

        if (dir == Vector2.zero) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
