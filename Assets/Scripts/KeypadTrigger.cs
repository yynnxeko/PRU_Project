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
            if (keypadUI != null)
                keypadUI.SetActive(true);
        }
    }
}
