using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float holdDuration = 1.5f;

    Interactable current;
    float holdTimer;
    bool isHolding;

    void Update()
    {
        if (current == null) return;

        // TAP E
        if (Input.GetKeyDown(KeyCode.E))
        {
            isHolding = true;
            holdTimer = 0f;
        }

        // HOLD E
        if (isHolding && Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;
            current.HoldInteract(this, holdTimer);

            if (holdTimer >= holdDuration)
            {
                current.Interact(this);
                ResetHold();
            }
        }

        // RELEASE E
        if (Input.GetKeyUp(KeyCode.E))
        {
            current.CancelHold(this);
            ResetHold();
        }
    }

    void ResetHold()
    {
        isHolding = false;
        holdTimer = 0f;
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
        if (other.GetComponent<Interactable>() == current)
        {
            current = null;
            ResetHold();
        }
    }
}
