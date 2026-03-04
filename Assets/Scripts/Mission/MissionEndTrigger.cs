using UnityEngine;
using UnityEngine.Events;

public class MissionEndTrigger : MonoBehaviour
{

    public MissionManager mission;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && mission.IsActive)
        {
            // Mission sẽ tự complete khi step cuối hoàn thành
            Debug.Log("[MissionEndTrigger] Player reached end trigger");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    private void ActivateTrigger()
    {
        Debug.Log("Trigger activated on: " + gameObject.name);
        onTriggered.Invoke();
    }
}
