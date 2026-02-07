using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Hàm này Camera sẽ gọi khi báo động
    public void AlertAllEnemies(Vector3 targetPos)
    {
        Debug.Log(" TRUY NÃ TOÀN BẢN ĐỒ! Vị trí: " + targetPos);

        // Tìm tất cả Enemy và ra lệnh
        EnemyAi[] enemies = FindObjectsOfType<EnemyAi>();
        foreach (EnemyAi enemy in enemies)
        {
            enemy.GoToLocation(targetPos);
        }
    }

    // Teleport tất cả enemy tới 1 điểm cố định
    public void TeleportAllEnemies(Vector3 centerPos, float radius = 2f)
    {
        EnemyAi[] enemies = FindObjectsOfType<EnemyAi>();
        int count = enemies.Length;

        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            enemies[i].transform.position = centerPos + offset;
            enemies[i].GoToLocation(centerPos);
        }
    }
}
