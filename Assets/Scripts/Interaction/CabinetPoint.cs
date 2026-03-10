using UnityEngine;

public class CabinetPoint : Interactable
{
    bool isCompleted = false;

    [Header("Evidence Settings")]
    public int evidenceReward = 1;
    public EvidenceItem evidenceItem;

    void Awake()
    {
        promptMessage = "Hold E";
        holdAction.requiredHoldTime = 2.0f;
        holdAction.resetOnStart = false; // Không reset progress khi bấm lại
    }

    public void Update()
    {
        // Nếu đã hoàn thành thì disable chính mình để PlayerInteraction không tìm thấy Interactable nữa
        if (isCompleted)
        {
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.HideAll();
            this.enabled = false;
            gameObject.layer = 0; // Chuyển layer về default để Raycast/Trigger không dính (tùy setup)
        }
    }

    public override void OnHoldCancel(PlayerInteraction player)
    {
        if (InteractionUI.Instance != null)
            InteractionUI.Instance.HideAll();
    }

    public override void OnHoldComplete(PlayerInteraction player)
    {
        if (isCompleted) return;
        isCompleted = true;

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            for (int i = 0; i < evidenceReward; ++i)
            {
                if (evidenceItem != null)
                    inventory.AddEvidence(evidenceItem);
            }
            Debug.Log($"Bạn đã tìm được {evidenceReward} evidence trong tủ.");
        }

        if (InteractionUI.Instance != null)
            InteractionUI.Instance.HideAll();
            
        // Disable để không hiện UI Prompt nữa
        this.enabled = false;
    }
}