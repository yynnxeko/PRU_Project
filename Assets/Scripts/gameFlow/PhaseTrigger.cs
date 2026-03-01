using UnityEngine;

public class PhaseTrigger : MonoBehaviour
{
    [Header("Settings")]
    public DayPhase targetPhase;
    public bool triggerOnEnter = true;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerOnEnter) return;
        
        if (other.CompareTag("Player"))
        {
            TriggerPhaseChange();
        }
    }

    public void TriggerPhaseChange()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.SetPhase(targetPhase);
        }
    }
}
