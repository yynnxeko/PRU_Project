using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    private static bool hasSpawned = false;

    void Start()
    {
        // Đã spawn rồi trong lần Play này → không spawn lại
        if (hasSpawned)
        {
            // Xóa Player duplicate nếu có (do scene tự tạo thêm)
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length > 1)
            {
                for (int i = 1; i < players.Length; i++)
                    Destroy(players[i]);
            }
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position = transform.position;
            hasSpawned = true;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Player!");
        }
    }
}
