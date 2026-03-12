using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Caught Feedback")]
    public SpeechBubble bubblePrefab;
    public string caughtMessage = "Bị phát hiện! Chạy đi!";
    public AudioClip caughtVoice;
    public float bubbleDuration = 3f;
    public Vector3 bubbleOffset = new Vector3(0, 1.5f, 0);

    private bool isCatching = false;
    private float lastCaughtTime = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void HandlePlayerCaught(PlayerInventory inventory, EnemyAi caughtBy = null)
    {
        if (isCatching) return; // Prevent multiple triggers

        bool hasEvidence = inventory != null && inventory.TotalEvidence() > 0;
        bool isNightScene = SceneManager.GetActiveScene().name == "Map_Internal Area_Night";

        if (isNightScene)
        {
            // Debounce the bubble so it doesn't spam every frame
            if (Time.time - lastCaughtTime > 2f)
            {
                lastCaughtTime = Time.time;
                ShowCaughtBubble();
            }
            return;
        }

        if (hasEvidence)
        {
            StartCoroutine(CatchSequence(caughtBy));
        }
    }

    System.Collections.IEnumerator CatchSequence(EnemyAi caughtBy)
    {
        isCatching = true;
        
        GameObject playerObj = GameObject.FindWithTag("Player");

        // 1. Khóa di chuyển người chơi
        if (playerObj != null)
        {
            var controller = playerObj.GetComponent<PlayerController2>();
            if (controller != null) controller.canMove = false;
        }

        // 2. Tác động tới lính gác bắt được
        if (caughtBy != null)
        {
            // Dừng di chuyển lính gác
            var agent = caughtBy.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null) agent.isStopped = true;
            
            // Quay lính gác về phía player
            if (playerObj != null)
            {
                Vector3 dirToPlayer = (playerObj.transform.position - caughtBy.transform.position).normalized;
                
                // Lấy hướng trục chính (Gần đúng X hoặc Y)
                float xDir = Mathf.Abs(dirToPlayer.x) > Mathf.Abs(dirToPlayer.y) ? Mathf.Sign(dirToPlayer.x) : 0;
                float yDir = xDir == 0 ? Mathf.Sign(dirToPlayer.y) : 0;

                // Định hướng Animator của lính gác
                if (caughtBy.anim != null)
                {
                    caughtBy.anim.SetFloat("LastInputX", xDir);
                    caughtBy.anim.SetFloat("LastInputY", yDir);
                    caughtBy.anim.SetFloat("InputX", 0);
                    caughtBy.anim.SetFloat("InputY", 0);
                    caughtBy.anim.SetTrigger("shocking");
                }
                
                // Định hướng Animator của Player về phía lính gác
                Animator playerAnim = playerObj.GetComponent<Animator>();
                if (playerAnim != null)
                {
                    playerAnim.SetFloat("LastInputX", -xDir);
                    playerAnim.SetFloat("LastInputY", -yDir);
                    playerAnim.SetFloat("InputX", 0);
                    playerAnim.SetFloat("InputY", 0);
                    playerAnim.SetBool("IsShocked", true);
                }
            }

            // Hiện thoại phát hiện
            if (bubblePrefab != null)
            {
                SpeechBubble bubble = Instantiate(bubblePrefab, caughtBy.transform.position + bubbleOffset, Quaternion.identity);
                bubble.Init(caughtBy.transform, bubbleOffset);
                bubble.Show(caughtMessage, 3f, caughtVoice);
            }
        }
        else
        {
            // Fallback nếu không có tham chiếu Enemy
            if (playerObj != null)
            {
                Animator playerAnim = playerObj.GetComponent<Animator>();
                if (playerAnim != null) playerAnim.SetBool("IsShocked", true);
            }
        }

        // 3. Chờ diễn hoạt ảnh xong
        yield return new WaitForSeconds(4f);

        // Reset Player Animation
        if (playerObj != null)
        {
            Animator playerAnim = playerObj.GetComponent<Animator>();
            if (playerAnim != null) playerAnim.SetBool("IsShocked", false);
        }

        yield return new WaitForSeconds(0.5f);

        // 4. Kết thúc
        isCatching = false;
        GameOver();
    }

    void GameOver()
    {
        Debug.Log("GAME OVER - Resetting Day...");

        if (DayManager.Instance != null)
        {
            DayManager.Instance.FailDay("to_bedroomDay");
        }
    }

    void ShowCaughtBubble()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null || bubblePrefab == null) return;

        SpeechBubble bubble = Instantiate(bubblePrefab, player.transform.position + bubbleOffset, Quaternion.identity);
        bubble.Init(player.transform, bubbleOffset);
        bubble.Show(caughtMessage, bubbleDuration, caughtVoice);
    }
    // Hàm này Camera sẽ gọi khi báo động
    public void AlertAllEnemies(Vector3 targetPos)
    {
        Debug.Log(" TRUY NÃ TOÀN BẢN ĐỒ! Vị trí: " + targetPos);

        // Tìm tất cả Enemy và ra lệnh
        EnemyAi[] enemies = FindObjectsOfType<EnemyAi>();
        foreach (EnemyAi enemy in enemies)
        {
            enemy.GoToLocation(targetPos);
        }
    }

    // Teleport tất cả enemy tới 1 điểm cố định
    public void TeleportAllEnemies(Vector3 centerPos, float radius = 2f)
    {
        EnemyAi[] enemies = FindObjectsOfType<EnemyAi>();
        int count = enemies.Length;

        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            enemies[i].transform.position = centerPos + offset;
            enemies[i].GoToLocation(centerPos);
        }
    }
}
