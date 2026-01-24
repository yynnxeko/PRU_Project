using UnityEngine;

public class StashPoint : Interactable
{
    PlayerInventory inventory;
    bool completedThisHold;

    void Awake()
    {
        promptMessage = "Hold E to stash evidence";
        holdAction.requiredHoldTime = 1.5f;
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
    }

    public override void OnHolding(PlayerInteraction player, float progress)
    {
        // Optional: update UI progress bar
        // Debug.Log($"Stashing... {progress:P0}");
    }

    public override void OnHoldCancel(PlayerInteraction player)
    {
        completedThisHold = false;
    }

    public override void OnHoldComplete(PlayerInteraction player)
    {
        if (completedThisHold) return;
        if (inventory == null) return;

        if (!inventory.HasEvidence())
        {
            Debug.Log("No evidence to stash");
            return;
        }

        int amount = inventory.TotalEvidence();
        inventory.StashAll();

        Debug.Log($"Stashed {amount} evidence");

        completedThisHold = true;
    }
}
