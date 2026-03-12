using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorWithKeyChangeScene : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string spawnIdInNextScene;
    [SerializeField] private string playerTag = "Player";

    [Header("Flag Requirement")]
    [Tooltip("Tên cờ cần bật TRUE để qua được cửa này")]
    public string requiredFlag;

    [Header("Phase Advance")]
    [Tooltip("Tích để chuyển buổi khi qua cửa (Morning → Noon → Night)")]
    public bool advancePhaseOnPass = false;

    [Header("Feedback when locked")]
    public SpeechBubble bubblePrefab;
    [TextArea]
    public string lockedText = "Chưa đủ điều kiện!\nKhông thể vào.";
    public AudioClip lockedVoice;
    public float bubbleOffsetY = 1.5f;
    public float bubbleDuration = 2f;

    private SpeechBubble currentBubble;

    private bool CanPass()
    {
        // Kiểm tra cờ (nếu có set)
        if (!string.IsNullOrEmpty(requiredFlag))
        {
            if (GameFlagManager.Instance == null || !GameFlagManager.Instance.GetFlag(requiredFlag))
                return false;
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (CanPass())
        {
            // Chuyển buổi nếu được bật
            if (advancePhaseOnPass && DayManager.Instance != null)
            {
                Debug.Log($"[DoorWithKeyChangeScene] Trước khi qua cửa: {DayManager.Instance.currentPhase}");
                DayManager.Instance.AdvancePhase();
                Debug.Log($"[DoorWithKeyChangeScene] Sau khi qua cửa: {DayManager.Instance.currentPhase}");
            }

            // Báo cho scene mới biết ID của điểm spawn
            DoorSceneChange.NextSpawnId = spawnIdInNextScene;
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            // Hiện chữ báo hiệu
            ShowBubble(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        HideBubble();
    }

    void ShowBubble(Transform playerTransform)
    {
        if (bubblePrefab == null || currentBubble != null) return;

        // Tạo bubble mọc trên đầu Player (hoặc có thể đổi thành transform của cửa)
        currentBubble = Instantiate(
            bubblePrefab,
            playerTransform.position + Vector3.up * bubbleOffsetY,
            Quaternion.identity
        );

        currentBubble.Init(playerTransform, Vector3.up * bubbleOffsetY);
        currentBubble.Show(lockedText, bubbleDuration, lockedVoice);
    }

    void HideBubble()
    {
        if (currentBubble != null)
        {
            Destroy(currentBubble.gameObject);
            currentBubble = null;
        }
    }
}
