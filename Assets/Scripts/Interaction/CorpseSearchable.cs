using UnityEngine;
using System;

/// <summary>
/// Gắn lên mỗi xác trong scene Hospital.
/// Player đến gần → giữ E để lục soát → tìm thấy USB hoặc không.
/// Chỉ 1 xác được tick hasUSB = true.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class CorpseSearchable : MonoBehaviour
{
    [Header("Cài đặt")]
    [Tooltip("Tick TRUE cho đúng 1 xác duy nhất chứa USB")]
    public bool hasUSB = false;

    [Header("Hold E để lục soát")]
    public float holdTime = 2f;

    [Header("Feedback")]
    public SpeechBubble bubblePrefab;
    public string hintMessage = "Giữ E để lục soát";
    public string emptyMessage = "Không tìm thấy gì...";
    public string foundMessage = "Tìm thấy USB!";
    public float bubbleOffsetY = 1.5f;
    public float bubbleDuration = 2f;

    [Header("Evidence Info")]
    public string evidenceName = "USB Evidence";
    public string uniqueID = "Evidence_USB_Hospital";

    /// <summary>
    /// Event tĩnh – Mission2_HospitalComplete lắng nghe event này.
    /// </summary>
    public static event Action OnUSBFound;

    private bool isSearched = false;
    private bool playerInRange = false;
    private bool isHolding = false;
    private float holdTimer = 0f;
    private Transform playerTransform;
    private SpeechBubble currentBubble;

    private void Awake()
    {
        // Tự động đảm bảo BoxCollider2D là Trigger
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Start()
    {
        // Nếu USB đã từng nhặt → disable xác này (hoặc ẩn highlight)
        if (hasUSB && EvidenceManager.Instance != null && EvidenceManager.Instance.IsCollected(uniqueID))
        {
            isSearched = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerTransform = other.transform;

            if (!isSearched)
            {
                ShowBubble(hintMessage);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerTransform = null;
            CancelHold();
            HideBubble();
        }
    }

    private void Update()
    {
        if (!playerInRange || isSearched) return;

        // Bắt đầu giữ E
        if (Input.GetKeyDown(KeyCode.E))
        {
            isHolding = true;
            holdTimer = 0f;
        }

        // Đang giữ E
        if (isHolding && Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdTime)
            {
                isHolding = false;
                OnSearchComplete();
            }
        }

        // Nhả E giữa chừng
        if (isHolding && Input.GetKeyUp(KeyCode.E))
        {
            CancelHold();
        }
    }

    private void CancelHold()
    {
        isHolding = false;
        holdTimer = 0f;
    }

    private void OnSearchComplete()
    {
        isSearched = true;

        if (hasUSB)
        {
            // Tìm thấy USB!
            Debug.Log("[CorpseSearchable] Tìm thấy USB trong xác!");
            ShowBubble(foundMessage);

            // Thêm evidence vào inventory
            PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
            if (inventory != null)
            {
                EvidenceItem usbItem = new EvidenceItem
                {
                    type = EvidenceType.USB,
                    itemName = evidenceName
                };
                inventory.AddEvidence(usbItem);
            }

            // Đánh dấu đã nhặt (không hiện lại khi load scene)
            if (EvidenceManager.Instance != null)
                EvidenceManager.Instance.MarkAsCollected(uniqueID);

            // Tắt cờ mission_accepted (không tắt go_to_medical ở đây
            // vì RoomSafetyCheck cần cờ này để bypass, tắt sẽ bị bắt ngay)
            if (GameFlagManager.Instance != null)
            {
                GameFlagManager.Instance.SetFlag("mission_accepted", false);
                // go_to_medical sẽ được tắt khi rời Hospital (LobbyFlagCleaner)
                Debug.Log("[CorpseSearchable] Đã tắt cờ mission_accepted");
            }

            // Tăng index nhiệm vụ trực tiếp
            if (FullMissionManager.Instance != null)
            {
                FullMissionManager.Instance.ReportComplete();
                Debug.Log("[CorpseSearchable] Đã gọi ReportComplete() → tăng mission index!");
            }

            // Phát event (giữ lại cho các script khác nếu cần)
            OnUSBFound?.Invoke();
        }
        else
        {
            // Xác rỗng
            Debug.Log("[CorpseSearchable] Không tìm thấy gì trong xác này.");
            ShowBubble(emptyMessage);
        }
    }

    private void ShowBubble(string message)
    {
        if (bubblePrefab == null) return;

        HideBubble();

        currentBubble = Instantiate(
            bubblePrefab,
            transform.position + Vector3.up * bubbleOffsetY,
            Quaternion.identity
        );
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
