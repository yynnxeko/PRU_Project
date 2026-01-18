using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int hiddenEvidence = 0;   // giấu trong người
    public int handEvidence = 0;     // cầm tay

    public int maxHidden = 2;
    public int maxHand = 1;

    // ===== KIỂM TRA =====
    public bool CanPickEvidence()
    {
        return hiddenEvidence < maxHidden || handEvidence < maxHand;
    }

    // ===== NHẶT =====
    public void AddEvidence()
    {
        if (hiddenEvidence < maxHidden)
        {
            hiddenEvidence++;
            Debug.Log("Evidence giấu trong người. Hidden: " + hiddenEvidence);
        }
        else if (handEvidence < maxHand)
        {
            handEvidence++;
            Debug.Log("Evidence cầm tay. Hand: " + handEvidence);
        }
        else
        {
            Debug.Log("Không còn slot chứa evidence!");
        }
    }

    public int TotalEvidence()
    {
        return hiddenEvidence + handEvidence;
    }

    // ===== GIẤU (STASH) =====
    public void StashAll()
    {
        int total = hiddenEvidence + handEvidence;

        if (total <= 0)
        {
            Debug.Log("Không có evidence để giấu");
            return;
        }

        hiddenEvidence = 0;
        handEvidence = 0;

        Debug.Log("Đã giấu " + total + " evidence. Slot được reset.");
    }

    public bool HasEvidence()
    {
        return hiddenEvidence + handEvidence > 0;
    }
}
