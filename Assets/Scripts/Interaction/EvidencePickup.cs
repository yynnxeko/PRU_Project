using UnityEngine;

/// <summary>
/// Gắn lên vật phẩm có BoxCollider2D (isTrigger).
/// Player lại gần → hiện SpeechBubble, bấm E → +1 evidence + hiện DialogueUI thông báo.
/// </summary>
public class EvidencePickup : MonoBehaviour
{
    [Header("=== EVIDENCE INFO ===")]
    [SerializeField] private EvidenceType evidenceType = EvidenceType.Photo;
    [SerializeField] private string evidenceName = "Evidence";

    [Header("=== UNIQUE ID (BẮT BUỘC) ===")]
    [SerializeField] public string uniqueID = "Evidence_Photo_Room1_01";

    [Header("Speech Bubble")]
    public SpeechBubble bubblePrefab;
    public string hintMessage = "Bấm E để nhặt";
    public float bubbleOffsetY = 1.5f;
    public float bubbleDuration = 3f;

    [Header("Dialogue Popup (sau khi nhặt)")]
    [TextArea]
    public string popupMessage = "Đã tìm thấy bằng chứng!";
    public string speakerName = "Tôi";
    public Sprite speakerAvatar;

    private bool playerInRange;
    private PlayerInventory inventory;
    private SpeechBubble currentBubble;

    private void Awake()
    {
        // Tự động đảm bảo BoxCollider2D là Trigger
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void Start()
    {
        // Nếu đã nhặt rồi → biến mất luôn
        if (EvidenceManager.Instance != null && EvidenceManager.Instance.IsCollected(uniqueID))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[EvidencePickup] OnTriggerEnter2D: {other.name}, tag={other.tag}");
        if (other.CompareTag("Player"))
        {
            inventory = other.GetComponent<PlayerInventory>();
            playerInRange = true;
            Debug.Log($"[EvidencePickup] Player vào vùng! inventory={inventory != null}");
            ShowBubble(hintMessage);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            inventory = null;
            HideBubble();
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[EvidencePickup] Bấm E! inventory={inventory != null}, canPick={inventory?.CanPickEvidence()}");
            if (inventory != null && inventory.CanPickEvidence())
            {
                Collect();
            }
            else
            {
                Debug.Log("Không còn chỗ chứa evidence");
            }
        }
    }

    private void Collect()
    {
        HideBubble();
        playerInRange = false; // Ngừng nhận input

        // +1 Evidence
        EvidenceItem newItem = new EvidenceItem
        {
            type = evidenceType,
            itemName = evidenceName
        };
        inventory.AddEvidence(newItem);
        Debug.Log($"[EvidencePickup] Đã nhặt: {evidenceName}");

        // Đánh dấu đã nhặt (persistent)
        if (EvidenceManager.Instance != null)
            EvidenceManager.Instance.MarkAsCollected(uniqueID);

        // Bật cờ Pass_Internal
        if (GameFlagManager.Instance != null)
        {
            GameFlagManager.Instance.SetFlag("Pass_Internal", true);
            Debug.Log("[EvidencePickup] Đã bật cờ Pass_Internal!");
        }

        // Tắt dialogue cũ trước, rồi hiện popup mới
        StartCoroutine(ShowPopupThenDestroy());
    }

    private System.Collections.IEnumerator ShowPopupThenDestroy()
    {
        // Tắt dialogue đang hiện (nếu có)
        if (DialogueUI.Instance != null && DialogueUI.Instance.dialoguePanel.activeSelf)
        {
            DialogueUI.Instance.HideDialogue();
        }

        yield return null; // Đợi 1 frame

        // Hiện thông báo bằng DialogueUI
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.ShowDialogue(popupMessage, speakerName, speakerAvatar, null);
            Debug.Log($"[EvidencePickup] Hiện popup: {popupMessage}");
        }
        else
        {
            Debug.Log($"[EvidencePickup] DialogueUI.Instance = null!");
        }

        // Biến mất
        Destroy(gameObject);
    }

    private void ShowBubble(string message)
    {
        if (bubblePrefab == null) return;
        HideBubble();

        // Ép Z = 0 để bubble không bị đẩy sau camera
        Vector3 spawnPos = transform.position + Vector3.up * bubbleOffsetY;
        spawnPos.z = 0f;

        currentBubble = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);
        currentBubble.Init(transform, Vector3.up * bubbleOffsetY);
        currentBubble.Show(message, bubbleDuration);
    }

    private void HideBubble()
    {
        if (currentBubble != null)
        {
            Destroy(currentBubble.gameObject);
            currentBubble = null;
        }
    }
}