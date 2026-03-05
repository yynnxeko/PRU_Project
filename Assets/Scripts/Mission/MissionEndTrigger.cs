using UnityEngine;

public class MissionEndTrigger : MonoBehaviour
{
    public MissionManager mission;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && mission != null && mission.IsActive)
        {
            // Mission sẽ tự complete khi step cuối hoàn thành
            Debug.Log("[MissionEndTrigger] Player reached end trigger");
        }
    }
}
