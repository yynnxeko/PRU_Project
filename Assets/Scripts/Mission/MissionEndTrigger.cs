using UnityEngine;

public class MissionEndTrigger : MonoBehaviour
{
    public MissionController mission;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && mission.missionActive)
        {
            mission.CompleteMission();
        }
    }
}
