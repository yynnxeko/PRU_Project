using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class SquareSceneTrigger : MonoBehaviour
{
    public string sceneName = "Map_Living Area Record";
    private bool playerInside = false;
    private GameObject playerObject;

    [Header("Speech Bubble")]
    public SpeechBubble bubblePrefab;
    public string hintMessage = "Bấm E để vào phòng";
    public float bubbleOffsetY = 1.5f;
    public float bubbleDuration = 3f;

    private SpeechBubble currentBubble;

    private void Awake()
    {
        // Tự động đảm bảo BoxCollider2D là Trigger
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[SquareSceneTrigger] Đã chạm vào vật: {other.name}, Tag: {other.tag}");
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerObject = other.gameObject;
            Debug.Log("[SquareSceneTrigger] Player đã vào vùng, hiện Bubble!");
            ShowBubble(hintMessage);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            playerObject = null;
            Debug.Log("[SquareSceneTrigger] Player đã ra khỏi vùng, ẩn Bubble!");
            HideBubble();
        }
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[SquareSceneTrigger] Đã bấm phím E! Đang tắt Player và chuyển cảnh...");
            HideBubble();
            
            if (playerObject != null)
            {
                playerObject.SetActive(false);
            }

            SceneManager.LoadScene(sceneName);
        }
    }

    private void ShowBubble(string message)
    {
        if (bubblePrefab == null)
        {
            Debug.LogWarning("[SquareSceneTrigger] Thiếu bubblePrefab, chưa được gán trong Inspector!");
            return;
        }
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