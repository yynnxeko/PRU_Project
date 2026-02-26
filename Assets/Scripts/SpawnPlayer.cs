using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position = transform.position;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Player!");
        }
    }
}