using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Interactable currentInteractable;

    void Update()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            Debug.Log(interactable.promptMessage);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Interactable>() == currentInteractable)
        {
            currentInteractable = null;
        }
    }
}
