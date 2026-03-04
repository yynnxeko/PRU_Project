using UnityEngine;

public class CabinetPoint : Interactable
{
    bool completedThisHold = false;

    [Header("Evidence Settings")]
    public int evidenceReward = 1;
    public EvidenceItem evidenceItem; // <-- ĐÂY

    void Awake()
    {
        promptMessage = "Hold E";
        holdAction.requiredHoldTime = 2.0f;
    }

    public override void OnHoldCancel(PlayerInteraction player)
    {
        completedThisHold = false;
        if (InteractionUI.Instance != null)
            InteractionUI.Instance.HideAll();
    }

    public override void OnHoldComplete(PlayerInteraction player)
    {
        if (completedThisHold) return;
        completedThisHold = true;

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            for (int i = 0; i < evidenceReward; ++i)
            {
                if (evidenceItem != null)
                    inventory.AddEvidence(evidenceItem);
                else
                    Debug.LogWarning("CabinetPoint: evidenceItem chưa được gán!");
            }
            Debug.Log($"Bạn đã tìm được {evidenceReward} evidence trong tủ.");
        }
        else
        {
            Debug.Log("Không tìm thấy PlayerInventory để nhận evidence!");
        }
        if (InteractionUI.Instance != null)
            InteractionUI.Instance.HideAll();
    }
}