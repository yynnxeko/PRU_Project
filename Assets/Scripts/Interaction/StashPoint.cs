using UnityEngine;

public class StashPoint : Interactable
{
    void Awake()
    {
        promptMessage = "Hold E to stash evidence";
    }

    public override void HoldInteract(PlayerInteraction player, float holdTime)
    {
        // 👉 Hook cho AI suspicion sau
        Debug.Log($"Stashing... {holdTime:F1}s");
    }

    public override void CancelHold(PlayerInteraction player)
    {
        Debug.Log("Stash cancelled");
    }

    public override void Interact(PlayerInteraction player)
    {
        PlayerInventory inv = player.GetComponent<PlayerInventory>();
        if (inv == null || !inv.HasEvidence()) return;

        inv.StashAll();
        Debug.Log("Evidence safely hidden");
    }
}
