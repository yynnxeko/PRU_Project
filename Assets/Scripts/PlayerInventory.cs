using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int carriedEvidence = 0;
    public int stashedEvidence = 0;

    public bool IsCarryingEvidence()
    {
        return carriedEvidence > 0;
    }

    public void AddEvidence(int amount = 1)
    {
        carriedEvidence += amount;
        Debug.Log("Evidence picked. Carrying: " + carriedEvidence);
    }

    public void StashEvidence()
    {
        stashedEvidence += carriedEvidence;
        carriedEvidence = 0;

        Debug.Log("Evidence stashed. Total saved: " + stashedEvidence);
    }   
}
