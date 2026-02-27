using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("=== INVENTORY ===")]
    public List<EvidenceItem> hiddenEvidences = new List<EvidenceItem>(); // Giấu trong người
    public List<EvidenceItem> handEvidences = new List<EvidenceItem>();   // Cầm tay

    public int maxHidden = 2;
    public int maxHand = 1;

    private void Start()
    {
        LoadFromManager();   // Tự động load khi scene bắt đầu
    }

    // ====================== LOAD TỪ SAVE ======================
    public void LoadFromManager()
    {
        if (EvidenceManager.Instance == null) return;

        hiddenEvidences = new List<EvidenceItem>(EvidenceManager.Instance.savedHidden);
        handEvidences = new List<EvidenceItem>(EvidenceManager.Instance.savedHand);

        Debug.Log($"[Inventory] Load thành công → Hidden: {hiddenEvidences.Count} | Hand: {handEvidences.Count}");
    }

    // ====================== KIỂM TRA ======================
    public bool CanPickEvidence()
    {
        return hiddenEvidences.Count < maxHidden || handEvidences.Count < maxHand;
    }

    // ====================== NHẶT ======================
    public void AddEvidence(EvidenceItem item)
    {
        bool addedToHidden = false;

        if (hiddenEvidences.Count < maxHidden)
        {
            hiddenEvidences.Add(item);
            addedToHidden = true;
            Debug.Log($"[Inventory] Giấu trong người: {item.itemName} ({item.type}) | Hidden: {hiddenEvidences.Count}");
        }
        else if (handEvidences.Count < maxHand)
        {
            handEvidences.Add(item);
            Debug.Log($"[Inventory] Cầm trên tay: {item.itemName} ({item.type}) | Hand: {handEvidences.Count}");
        }
        else
        {
            Debug.Log("[Inventory] Không còn slot chứa evidence!");
            return;
        }

        // Gửi lên EvidenceManager để lưu vĩnh viễn
        if (EvidenceManager.Instance != null)
            EvidenceManager.Instance.AddToInventory(item, addedToHidden);
    }

    // ====================== CÁC HÀM CŨ GIỮ NGUYÊN ======================
    public int TotalEvidence()
    {
        return hiddenEvidences.Count + handEvidences.Count;
    }

    public bool HasEvidence()
    {
        return TotalEvidence() > 0;
    }

    public void StashAll()
    {
        int total = TotalEvidence();
        if (total <= 0)
        {
            Debug.Log("Không có evidence để giấu");
            return;
        }

        // Xóa trong Manager trước
        if (EvidenceManager.Instance != null)
        {
            EvidenceManager.Instance.savedHidden.Clear();
            EvidenceManager.Instance.savedHand.Clear();
            EvidenceManager.Instance.SaveAllData();
        }

        hiddenEvidences.Clear();
        handEvidences.Clear();

        Debug.Log($"Đã stash tất cả {total} evidence. Slot được reset.");
    }

    public bool HasEvidenceOfType(EvidenceType type)
    {
        foreach (var item in hiddenEvidences)
            if (item.type == type) return true;

        foreach (var item in handEvidences)
            if (item.type == type) return true;

        return false;
    }

    public EvidenceItem GetEvidenceOfType(EvidenceType type)
    {
        for (int i = 0; i < hiddenEvidences.Count; i++)
        {
            if (hiddenEvidences[i].type == type)
            {
                EvidenceItem item = hiddenEvidences[i];
                hiddenEvidences.RemoveAt(i);
                if (EvidenceManager.Instance != null) EvidenceManager.Instance.SaveAllData();
                return item;
            }
        }

        for (int i = 0; i < handEvidences.Count; i++)
        {
            if (handEvidences[i].type == type)
            {
                EvidenceItem item = handEvidences[i];
                handEvidences.RemoveAt(i);
                if (EvidenceManager.Instance != null) EvidenceManager.Instance.SaveAllData();
                return item;
            }
        }
        return null;
    }
}