using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    private static bool hasSpawned = false;

    void Start()
    {
        if (hasSpawned) return;

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
