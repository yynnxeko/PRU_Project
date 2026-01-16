using UnityEngine;
using System.Collections.Generic;

public enum EvidenceType
{
    USB,
    Document,
    Phone
}

public class PlayerInventory : MonoBehaviour
{
    public int maxHidden = 2;
    public int maxHand = 1;

    public List<EvidenceType> hiddenEvidence = new List<EvidenceType>();
    public List<EvidenceType> handEvidence = new List<EvidenceType>();

    public int TotalCount => hiddenEvidence.Count + handEvidence.Count;

    public bool CanPickup()
    {
        return TotalCount < (maxHidden + maxHand);
    }

    public void AddEvidence(EvidenceType type)
    {
        if (hiddenEvidence.Count < maxHidden)
        {
            hiddenEvidence.Add(type);
        }
        else if (handEvidence.Count < maxHand)
        {
            handEvidence.Add(type);
        }

        Debug.Log($"Picked {type} | Hidden:{hiddenEvidence.Count} Hand:{handEvidence.Count}");
    }

    public bool HasEvidence()
    {
        return TotalCount > 0;
    }

    public void StashAll()
    {
        hiddenEvidence.Clear();
        handEvidence.Clear();
        Debug.Log("All evidence stashed safely");
    }

    public bool IsOverloaded()
    {
        return handEvidence.Count > 0;
    }
}
