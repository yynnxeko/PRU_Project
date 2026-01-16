using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    public PlayerController player;

    void Update()
    {
        Vector2 dir = player.faceDir;
        if (dir == Vector2.zero) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
