using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Interactable current;

    void Update()
    {
        if (current == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            current.holdAction.StartHold();
            current.OnHoldStart(this);
        }

        if (Input.GetKey(KeyCode.E))
        {
            bool completed = current.holdAction.UpdateHold(Time.deltaTime);

            current.OnHolding(this, current.holdAction.GetProgress());

            if (completed)
            {
                current.OnHoldComplete(this);
                current.holdAction.CancelHold();
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            current.holdAction.CancelHold();
            current.OnHoldCancel(this);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Interactable i = other.GetComponent<Interactable>();
        if (i != null)
        {
            current = i;
            Debug.Log(i.promptMessage);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (current == null) return;

        Interactable i = other.GetComponent<Interactable>();
        if (i != null && i == current)
        {
            if (current.holdAction != null)
                current.holdAction.CancelHold();

            current = null;
        }
    }

}
