using UnityEngine;

public class EvidencePickup : Interactable
{
    public EvidenceType type;

    void Awake()
    {
        promptMessage = "Press E to pick up evidence";
    }

    public override void Interact(PlayerInteraction player)
    {
        PlayerInventory inv = player.GetComponent<PlayerInventory>();
        if (inv == null) return;

        if (!inv.CanPickup())
        {
            Debug.Log("Inventory full!");
            return;
        }

        inv.AddEvidence(type);
        Destroy(gameObject);
    }
}
