using UnityEngine;

public class EvidencePickup : MonoBehaviour
{
    [SerializeField] private EvidenceType evidenceType = EvidenceType.Photo; 
    [SerializeField] private string evidenceName = "Evidence";
    private bool playerInRange;
    private PlayerInventory inventory;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inventory = other.GetComponent<PlayerInventory>();
            playerInRange = true;
            Debug.Log("Press E to pick evidence");
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
                EvidenceItem newItem = new EvidenceItem { type = evidenceType, itemName = evidenceName };
                inventory.AddEvidence(newItem);
                Destroy(gameObject); // evidence biến mất
            }
            else
            {
                Debug.Log("Không còn chỗ chứa evidence");
            }
        }
    }
}
