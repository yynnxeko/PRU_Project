using UnityEngine;

public class StashPoint : MonoBehaviour
{
    public float holdTime = 1.5f;

    private bool playerInRange;
    private float holdTimer;
    private bool hasStashedThisHold;

    private PlayerInventory inventory;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inventory = other.GetComponent<PlayerInventory>();
            playerInRange = true;
            Debug.Log("Hold E to stash evidence");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            holdTimer = 0f;
            hasStashedThisHold = false;
            inventory = null;
        }
    }

    void Update()
    {
        if (!playerInRange || inventory == null) return;

        // ⛔ Nếu đã stash trong lần giữ này → KHÔNG làm gì nữa
        if (hasStashedThisHold) return;

        if (Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;
            Debug.Log("Stashing... " + holdTimer.ToString("F2"));

            if (holdTimer >= holdTime)
            {
                if (inventory.HasEvidence())
                {
                    int amount = inventory.TotalEvidence();
                    inventory.StashAll();

                    Debug.Log($"Stashed {amount} evidence");
                }
                else
                {
                    Debug.Log("No evidence to stash");
                }

                hasStashedThisHold = true; // 🔒 khóa cho đến khi nhả E
            }
        }
        else
        {
            // 🔓 Khi nhả E → reset
            holdTimer = 0f;
            hasStashedThisHold = false;
        }
    }
}
