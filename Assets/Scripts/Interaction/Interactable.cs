using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage = "Hold E";

    public HoldAction holdAction = new HoldAction();

    public virtual void OnHoldStart(PlayerInteraction player) { }

    public virtual void OnHolding(PlayerInteraction player, float progress) { }

    public virtual void OnHoldCancel(PlayerInteraction player) { }

    public abstract void OnHoldComplete(PlayerInteraction player);
}
