using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage = "Press E";

    public virtual void Interact(PlayerInteraction player) { }

    public virtual void HoldInteract(PlayerInteraction player, float holdTime) { }

    public virtual void CancelHold(PlayerInteraction player) { }
}