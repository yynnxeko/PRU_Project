using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public List<EvidenceItem> hiddenEvidences = new List<EvidenceItem>(); // Giấu trong người
    public List<EvidenceItem> handEvidences = new List<EvidenceItem>();  // Cầm tay
    public int maxHidden = 2;
    public int maxHand = 1;

    // ===== KIỂM TRA =====
    public bool CanPickEvidence()
    {
        return hiddenEvidences.Count < maxHidden || handEvidences.Count < maxHand;
    }

    // ===== NHẶT =====
    public void AddEvidence(EvidenceItem item)
    {
        if (hiddenEvidences.Count < maxHidden)
        {
            hiddenEvidences.Add(item);
            Debug.Log("Evidence giấu trong người: " + item.type + " (" + item.itemName + "). Hidden: " + hiddenEvidences.Count);
        }
        else if (handEvidences.Count < maxHand)
        {
            handEvidences.Add(item);
            Debug.Log("Evidence cầm tay: " + item.type + " (" + item.itemName + "). Hand: " + handEvidences.Count);
        }
        else
        {
            Debug.Log("Không còn slot chứa evidence!");
        }
    }

    public int TotalEvidence()
    {
        return hiddenEvidences.Count + handEvidences.Count;
    }

    // ===== GIẤU (STASH) =====
    public void StashAll()
    {
        int total = TotalEvidence();
        if (total <= 0)
        {
            Debug.Log("Không có evidence để giấu");
            return;
        }
        hiddenEvidences.Clear();
        handEvidences.Clear();
        Debug.Log("Đã giấu " + total + " evidence. Slot được reset.");
    }

    public bool HasEvidence()
    {
        return TotalEvidence() > 0;
    }

    // ===== MỚI: Check có evidence loại nào đó (dùng để unlock quest) =====
    public bool HasEvidenceOfType(EvidenceType type)
    {
        foreach (var item in hiddenEvidences)
        {
            if (item.type == type) return true;
        }
        foreach (var item in handEvidences)
        {
            if (item.type == type) return true;
        }
        return false;
    }

    // Optional: Lấy evidence đầu tiên của type (nếu cần xóa khi unlock)
    public EvidenceItem GetEvidenceOfType(EvidenceType type)
    {
        for (int i = 0; i < hiddenEvidences.Count; i++)
        {
            if (hiddenEvidences[i].type == type)
            {
                EvidenceItem item = hiddenEvidences[i];
                hiddenEvidences.RemoveAt(i);
                return item;
            }
        }
        for (int i = 0; i < handEvidences.Count; i++)
        {
            if (handEvidences[i].type == type)
            {
                EvidenceItem item = handEvidences[i];
                handEvidences.RemoveAt(i);
                return item;
            }
        }
        return null;
    }
}