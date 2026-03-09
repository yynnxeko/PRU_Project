using UnityEngine;

public class KeypadTrigger : MonoBehaviour
{
    public GameObject keypadUI;
    public SpeechBubble bubblePrefab;
    public string foundText = "Tìm thấy gì đó, bấm E để mở";
    public float bubbleDuration = 2f;
    public Vector3 bubbleOffset = new Vector3(0, 1.5f, 0);
    private SpeechBubble currentBubble;
    private bool playerNearby = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (bubblePrefab != null && currentBubble == null)
            {
                Vector3 spawnPos = transform.position + bubbleOffset;
                currentBubble = Instantiate(bubblePrefab, spawnPos, Quaternion.identity);
                currentBubble.Init(transform, bubbleOffset);
                currentBubble.Show(foundText, bubbleDuration);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (currentBubble != null)
            {
                Destroy(currentBubble.gameObject);
                currentBubble = null;
            }
            if (keypadUI != null)
                keypadUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Giả định có FullMissionManager.Instance.CurrentMissionState
            int missionIndex = 0;
            if (FullMissionManager.Instance != null)
                missionIndex = FullMissionManager.Instance.currentMissionIndex;

            if (missionIndex == 0)
            {
                if (keypadUI != null)
                    keypadUI.SetActive(true);
            }
            else if (missionIndex >= 1)
            {
                // Hiện khung thông báo giống IntroCutscene
                ShowPopup("Không tìm thấy gì.");
            }
        }

        // --- THÊM LOGIC: BẤM CHUỘT RA NGOÀI LÀ TẮT ---
        if (keypadUI != null && keypadUI.activeSelf && Input.GetMouseButtonDown(0))
        {
            // Kiểm tra xem chuột có đang đè lên bất kỳ UI nào không (nút bấm, khung hình...)
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                keypadUI.SetActive(false);
            }
        }
    }

    // Hàm mẫu để hiện popup giống IntroCutscene
    void ShowPopup(string message)
    {
        // TODO: Thay bằng code hiện popup thực tế của bạn
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.ShowDialogue(message, "Tôi", null, null);
        }
        else
        {
            Debug.Log($"Popup: {message}");
        }
    }
}
