using UnityEngine;

public class SearchCabinet : Interactable
{
    PlayerInventory inventory;
    bool completedThisHold = false;
    public int evidenceFoundCount = 1; // số evidence tìm được khi mở tủ (tùy chỉnh)

    void Awake()
    {
        promptMessage = "Hold E to search cabinet";
        holdAction.requiredHoldTime = 2.0f; // thời gian giữ để search (tùy chỉnh)
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        inventory = other.GetComponent<PlayerInventory>();
        completedThisHold = false;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        inventory = null;
        completedThisHold = false;
        holdAction.CancelHold();
    }

    public override void OnHoldStart(PlayerInteraction player)
    {
        completedThisHold = false;
        // (nếu muốn) play sound, mở animation tủ
    }

    public override void OnHolding(PlayerInteraction player, float progress)
    {
        // có thể thêm sound/particle theo progress ở đây
    }

    public override void OnHoldCancel(PlayerInteraction player)
    {
        completedThisHold = false;
        // (nếu muốn) stop sound/animation
    }

    public override void OnHoldComplete(PlayerInteraction player)
    {
        if (completedThisHold) return;
        if (inventory == null) return;

        // kiểm tra có chỗ chứa evidence không
        if (!inventory.CanPickEvidence())
        {
            Debug.Log("Không có chỗ chứa để lấy evidence từ tủ.");
            return;
        }

        // add evidence (có thể lặp nếu evidenceFoundCount >1)
        for (int i = 0; i < evidenceFoundCount; i++)
        {
            if (inventory.CanPickEvidence())
                inventory.AddEvidence();
            else
                break;
        }

        Debug.Log("Tìm được manh mối trong tủ.");

        // đánh dấu đã hoàn tất (không cho lặp lại) — nếu muốn cho lặp lại, remove completedThisHold
        completedThisHold = true;

        // (tuỳ chọn) disable component hoặc bật object chỉ mục để hiển thị clue đã tìm
        // this.enabled = false;
    }
}