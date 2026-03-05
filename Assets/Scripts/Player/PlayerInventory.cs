using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("=== INVENTORY ===")]
    public List<EvidenceItem> hiddenEvidences = new List<EvidenceItem>(); // Giấu trong người

    public int maxHidden = 2;

    private void Start()
    {
        LoadFromManager();   // Tự động load khi scene bắt đầu
    }

    // ====================== LOAD TỪ SAVE ======================
    public void LoadFromManager()
    {
        if (EvidenceManager.Instance == null) return;

        hiddenEvidences = new List<EvidenceItem>(EvidenceManager.Instance.savedHidden);

        Debug.Log($"[Inventory] Load thành công → Hidden: {hiddenEvidences.Count}");
    }

    // ====================== KIỂM TRA ======================
    public bool CanPickEvidence()
    {
        return hiddenEvidences.Count < maxHidden;
    }

    // ====================== NHẶT ======================
    public void AddEvidence(EvidenceItem item)
    {
        if (hiddenEvidences.Count < maxHidden)
        {
            hiddenEvidences.Add(item);
            Debug.Log($"[Inventory] Giấu trong người: {item.itemName} ({item.type}) | Hidden: {hiddenEvidences.Count}");
        }
        else
        {
            Debug.Log("[Inventory] Không còn slot chứa evidence!");
            return;
        }

        // Gửi lên EvidenceManager để lưu vĩnh viễn
        if (EvidenceManager.Instance != null)
            EvidenceManager.Instance.AddToInventory(item);
    }

    // ====================== QUERY ======================
    public int TotalEvidence()
    {
        return hiddenEvidences.Count;
    }

    public bool HasEvidence()
    {
        return hiddenEvidences.Count > 0;
    }

    public bool HasEvidenceOfType(EvidenceType type)
    {
        foreach (var item in hiddenEvidences)
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
        return null;
    }

    // ====================== STASH ALL ======================
    public void StashAll()
    {
        int total = hiddenEvidences.Count;
        if (total <= 0)
        {
            Debug.Log("Không có evidence để giấu");
            return;
        }

        // Xóa trong Manager trước
        if (EvidenceManager.Instance != null)
        {
            EvidenceManager.Instance.savedHidden.Clear();
            EvidenceManager.Instance.SaveAllData();
        }

        hiddenEvidences.Clear();

        Debug.Log($"Đã stash tất cả {total} evidence. Slot được reset.");
    }
}