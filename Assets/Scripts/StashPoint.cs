using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StashPoint : MonoBehaviour
{
    private bool playerInRange;
    private PlayerInventory inventory;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            inventory = other.GetComponent<PlayerInventory>();
            Debug.Log("Press E to stash evidence");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (inventory != null && inventory.IsCarryingEvidence())
            {
                inventory.StashEvidence();
            }
        }
    }
}

