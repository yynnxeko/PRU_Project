using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorWithKeyChangeScene : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string spawnIdInNextScene;
    [SerializeField] private string playerTag = "Player";

    [Header("Key Requirement")]
    [Tooltip("Đánh dấu True nếu người chơi đã nhặt được chìa khóa này.")]
    public bool hasKey = false; 

    [Header("Feedback when locked")]
    public SpeechBubble bubblePrefab;
    [TextArea]
    public string lockedText = "Chưa có chìa khóa!\nKhông thể vào.";
    public float bubbleOffsetY = 1.5f;
    public float bubbleDuration = 2f;

    private SpeechBubble currentBubble;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (hasKey)
        {
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
        currentBubble.Show(lockedText, bubbleDuration);
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
