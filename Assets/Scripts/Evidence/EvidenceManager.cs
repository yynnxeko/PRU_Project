using UnityEngine;
using System.Collections.Generic;
using System;

public class EvidenceManager : MonoBehaviour
{
    public static EvidenceManager Instance { get; private set; }

    // Danh sách đã nhặt (evidence không hiện lại)
    private HashSet<string> collectedEvidence = new HashSet<string>();

    // Inventory thực tế (để load/save)
    public List<EvidenceItem> savedHidden = new List<EvidenceItem>();
    public List<EvidenceItem> savedHand = new List<EvidenceItem>();

    private const string SAVE_KEY_COLLECTED = "CollectedEvidence";
    private const string SAVE_KEY_HIDDEN = "InventoryHidden";
    private const string SAVE_KEY_HAND = "InventoryHand";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllData();
        
        // Tạo snapshot ngay khi game bắt đầu (Lần đầu tiên vào game)
        BackupDayStart();
    }

    // ====================== SNAPSHOT LOGIC ======================
    
    private HashSet<string> dayStartCollected = new HashSet<string>();
    private List<EvidenceItem> dayStartHidden = new List<EvidenceItem>();
    private List<EvidenceItem> dayStartHand = new List<EvidenceItem>();

    public void BackupDayStart()
    {
        dayStartCollected = new HashSet<string>(collectedEvidence);
        dayStartHidden = new List<EvidenceItem>(savedHidden);
        dayStartHand = new List<EvidenceItem>(savedHand);
        Debug.Log("Evidence snapshot created for start of day.");
    }

    public void RestoreDayStart()
    {
        collectedEvidence = new HashSet<string>(dayStartCollected);
        savedHidden = new List<EvidenceItem>(dayStartHidden);
        savedHand = new List<EvidenceItem>(dayStartHand);
        
        // Cần lưu lại vào PlayerPrefs để đồng bộ
        SaveAllData();
        Debug.Log("Evidence restored to start of day snapshot.");
    }

    // ====================== PUBLIC METHODS ======================

    public void AddToInventory(EvidenceItem item, bool isHidden)
    {
        if (isHidden)
            savedHidden.Add(item);
        else
            savedHand.Add(item);

        SaveAllData();
    }

    public void MarkAsCollected(string uniqueID)
    {
        if (!string.IsNullOrEmpty(uniqueID))
        {
            collectedEvidence.Add(uniqueID);
            SaveAllData();
        }
    }

    public bool IsCollected(string uniqueID)
    {
        return collectedEvidence.Contains(uniqueID);
    }

    // Lưu công khai (để PlayerInventory gọi được)
    public void SaveAllData()
    {
        PlayerPrefs.SetString(SAVE_KEY_COLLECTED, string.Join("|", collectedEvidence));

        string hiddenJson = JsonUtility.ToJson(new Serialization<EvidenceItem>(savedHidden));
        PlayerPrefs.SetString(SAVE_KEY_HIDDEN, hiddenJson);

        string handJson = JsonUtility.ToJson(new Serialization<EvidenceItem>(savedHand));
        PlayerPrefs.SetString(SAVE_KEY_HAND, handJson);

        PlayerPrefs.Save();
    }

    // ====================== PRIVATE LOAD ======================
    private void LoadAllData()
    {
        collectedEvidence.Clear();
        savedHidden.Clear();
        savedHand.Clear();

        // Load collected
        string collectedData = PlayerPrefs.GetString(SAVE_KEY_COLLECTED, "");
        if (!string.IsNullOrEmpty(collectedData))
        {
            string[] ids = collectedData.Split('|');
            foreach (string id in ids)
                if (!string.IsNullOrEmpty(id)) collectedEvidence.Add(id);
        }

        // Load Hidden
        string hiddenJson = PlayerPrefs.GetString(SAVE_KEY_HIDDEN, "");
        if (!string.IsNullOrEmpty(hiddenJson))
        {
            Serialization<EvidenceItem> wrapper = JsonUtility.FromJson<Serialization<EvidenceItem>>(hiddenJson);
            savedHidden = wrapper?.target ?? new List<EvidenceItem>();
        }

        // Load Hand
        string handJson = PlayerPrefs.GetString(SAVE_KEY_HAND, "");
        if (!string.IsNullOrEmpty(handJson))
        {
            Serialization<EvidenceItem> wrapper = JsonUtility.FromJson<Serialization<EvidenceItem>>(handJson);
            savedHand = wrapper?.target ?? new List<EvidenceItem>();
        }
    }

    public void DeleteAllSave()
    {
        collectedEvidence.Clear();
        savedHidden.Clear();
        savedHand.Clear();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("ĐÃ XÓA HẾT SAVE EVIDENCE!");
    }
}

// Class hỗ trợ serialize List
[System.Serializable]
public class Serialization<T>
{
    public List<T> target;
    public Serialization(List<T> target) { this.target = target; }
    public List<T> ToList() { return target; }
}