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
        Debug.Log("🚨 TRUY NÃ TOÀN BẢN ĐỒ! Vị trí: " + targetPos);

        // Tìm tất cả Enemy và ra lệnh
        EnemyAi[] enemies = FindObjectsOfType<EnemyAi>();
        foreach (EnemyAi enemy in enemies)
        {
            enemy.GoToLocation(targetPos);
        }
    }
}
