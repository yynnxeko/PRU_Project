using UnityEngine;

public class EvidencePickup : MonoBehaviour
{
    [Header("=== EVIDENCE INFO ===")]
    [SerializeField] private EvidenceType evidenceType = EvidenceType.Photo;
    [SerializeField] private string evidenceName = "Evidence";

    [Header("=== UNIQUE ID (BẮT BUỘC) ===")]
    [SerializeField] public string uniqueID = "Evidence_Photo_Room1_01"; // ← Đặt tên KHÁC NHAU cho mỗi cái!

    private bool playerInRange;
    private PlayerInventory inventory;

    private void Start()
    {
        // Kiểm tra ngay khi scene load: nếu đã nhặt thì biến mất luôn
        if (EvidenceManager.Instance != null && EvidenceManager.Instance.IsCollected(uniqueID))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inventory = other.GetComponent<PlayerInventory>();
            playerInRange = true;
            Debug.Log("Nhấn E để nhặt evidence");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            inventory = null;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (inventory != null && inventory.CanPickEvidence())
            {
                // Tạo EvidenceItem
                EvidenceItem newItem = new EvidenceItem
                {
                    type = evidenceType,
                    itemName = evidenceName
                };

                inventory.AddEvidence(newItem);

                // Ghi nhớ đã nhặt (vĩnh viễn giữa các scene)
                if (EvidenceManager.Instance != null)
                    EvidenceManager.Instance.MarkAsCollected(uniqueID);

                Destroy(gameObject);        // Biến mất ngay
            }
            else
            {
                Debug.Log("Không còn chỗ chứa evidence");
            }
        }
    }
}