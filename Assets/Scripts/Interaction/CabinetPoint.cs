using UnityEngine;

public class CabinetPoint : Interactable
{
    bool isCompleted = false;
    private const string CABINET_ID = "Main_Cabinet";

    [Header("Evidence Settings")]
    public int evidenceReward = 1;
    public EvidenceItem evidenceItem;

    void Awake()
    {
        // Phải bật enabled lúc đầu để Update chạy được và kiểm tra mission
        this.enabled = true;

        promptMessage = "Hold E";
        holdAction.requiredHoldTime = 10f; // Chỉnh lại theo ảnh bạn gửi cho chắc
        holdAction.resetOnStart = false;    // Tùy bạn muốn reset hay không, mình để true theo ảnh Inspector

        // Kiểm tra trạng thái lưu vĩnh viễn
        if (EvidenceManager.Instance != null && EvidenceManager.Instance.IsCollected(CABINET_ID))
        {
            isCompleted = true;
            this.enabled = false;
            gameObject.layer = 0; // Chuyển layer về default
        }
    }

    public void Update()
    {
        if (isCompleted) return;

        // KIỂM TRA NHIỆM VỤ: Chỉ hiện khi đang ở nhiệm vụ index 2
        if (FullMissionManager.Instance != null)
        {
            int currentMission = FullMissionManager.Instance.currentMissionIndex;

            // Nếu không phải nhiệm vụ 2, tự tắt component để PlayerInteraction không tìm thấy
            if (currentMission != 2)
            {
                if (this.enabled)
                {
                    this.enabled = false;
                    // Nếu player đang đứng trong Trigger mà nv đổi, ẩn UI luôn
                    if (InteractionUI.Instance != null)
                        InteractionUI.Instance.HideAll();
                }
                return;
            }
            else
            {
                // Nếu đang ở nhiệm vụ 2 mà script vô tình bị tắt (không phải do hoàn thành)
                if (!this.enabled && !isCompleted)
                    this.enabled = true;
            }
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

        // Lưu trạng thái vĩnh viễn
        if (EvidenceManager.Instance != null)
        {
            EvidenceManager.Instance.MarkAsCollected(CABINET_ID);
        }

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

        // Bật cờ keyBedroom
        if (GameFlagManager.Instance != null)
        {
            GameFlagManager.Instance.SetFlag("keyBedroom", true);
            Debug.Log("[CabinetPoint] Đã bật cờ keyBedroom!");
        }

        // Disable để không hiện UI Prompt nữa
        this.enabled = false;
        gameObject.layer = 0;
    }
}