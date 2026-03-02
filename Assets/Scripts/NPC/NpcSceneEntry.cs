using UnityEngine;
using System.Collections;

public class NpcSceneEntry : MonoBehaviour
{
    [Header("Activation Setting")]
    public string requiredFlag = "npc_ready_in_lobby";
    public bool destroyIfNoFlag = false;

    [Header("Behavior to trigger on arrival")]
    public MonoBehaviour behaviorToEnable; 
    public NarrativeDirector directorToStart; 

    void Start()
    {
        CheckEntry();
    }

    public void CheckEntry()
    {
        if (GameFlagManager.Instance == null) return;

        bool hasArrived = GameFlagManager.Instance.GetFlag(requiredFlag);

        if (hasArrived)
        {
            Debug.Log($"[NpcSceneEntry] Flag '{requiredFlag}' is TRUE. Activating NPC '{name}'.");
            
            // Hiện ra nếu đang ẩn
            gameObject.SetActive(true);
            
            // Bật script di chuyển hoặc gọi Director để đi tiếp
            if (behaviorToEnable != null) behaviorToEnable.enabled = true;
            if (directorToStart != null) directorToStart.StartNarrative();
        }
        else
        {
            Debug.Log($"[NpcSceneEntry] Flag '{requiredFlag}' is FALSE. NPC '{name}' remains hidden.");
            if (destroyIfNoFlag) Destroy(gameObject);
            else gameObject.SetActive(false);
        }
    }
}
