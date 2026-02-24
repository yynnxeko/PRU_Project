using UnityEngine;
using System.Collections;

public class BusMoveForward : MonoBehaviour
{
    [Header("Move")]
    public float speed = 2f;
    public float stopDistancePx = 100f;
    public Vector3 finalTarget;

    [Header("Barriers (open together)")]
    public Animator[] barrierAnimators;
    public float waitAfterOpen = 0.3f;

    [Header("Camera")]
    public BusCameraFollow cameraFollow;

    [Header("Enemy")]
    public GameObject enemy;
    public Vector3 enemySpawnPos;
    public SpeechBubble enemyBubble;
    public float enemyBubbleOffsetY = 1.5f;

    [Header("Player")]
    public GameObject player;
    public Vector3 playerSpawnPos;

    [Header("NPCs")]
    public GameObject npc1;
    public Vector3 npcSpawnPos1;

    public GameObject npc2;
    public Vector3 npcSpawnPos2;

    [Header("Spawn Timing")]
    public float bubbleTime = 2f;
    public float spawnDelay = 1f;

    Vector3 stopTarget;
    bool reachedStop;
    bool movingToFinal;
    bool finished;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
            {
                player = p;
            }
            else
            {
                Debug.LogWarning("Không tìm thấy Player với tag 'Player'");
            }
        }
        //  Khóa game khi bus chạy
        GameFlow.BusCutscene = true;

        float moveUnit = stopDistancePx / 32f;
        stopTarget = transform.position + Vector3.right * moveUnit;

        if (player) player.SetActive(false);
        if (enemy) enemy.SetActive(false);
        if (npc1) npc1.SetActive(false);
        if (npc2) npc2.SetActive(false);

        if (cameraFollow)
            cameraFollow.target = transform;
    }

    void Update()
    {
        if (finished) return;

        // 🚍 Chặng 1: tới barrier
        if (!reachedStop)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                stopTarget,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, stopTarget) < 0.01f)
            {
                reachedStop = true;
                StartCoroutine(StopAndOpenBarriers());
            }
            return;
        }

        // 🚍 Chặng 2: chạy tiếp
        if (movingToFinal)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                finalTarget,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, finalTarget) < 0.01f)
            {
                finished = true;
                movingToFinal = false;
                StartCoroutine(SpawnSequence());
            }
        }
    }

    IEnumerator StopAndOpenBarriers()
    {
        foreach (Animator anim in barrierAnimators)
        {
            if (anim)
                anim.Play("traffic_barrier_open", 0, 0f);
        }

        yield return new WaitForSeconds(waitAfterOpen);
        movingToFinal = true;
    }

    // =========================
    // SEQUENCE SAU KHI BUS XONG
    // =========================
    IEnumerator SpawnSequence()
    {
        // 🔓 Mở game logic
        GameFlow.BusCutscene = false;

        // 👾 ENEMY xuất hiện
        if (enemy)
        {
            enemy.transform.position = enemySpawnPos;
            enemy.SetActive(true);
        }

        // 💬 Bubble trên enemy
        if (enemyBubble && enemy)
        {
            var bubble = Instantiate(
                enemyBubble,
                enemy.transform.position + Vector3.up * enemyBubbleOffsetY,
                Quaternion.identity
            );

            bubble.Init(enemy.transform, Vector3.up * enemyBubbleOffsetY);
            bubble.Show("Lê lê cái chân lên", bubbleTime);

            yield return new WaitForSeconds(bubbleTime);
            Destroy(bubble.gameObject);
        }
        else
        {
            yield return new WaitForSeconds(bubbleTime);
        }

        // 👤 PLAYER
        if (player)
        {
            player.transform.position = playerSpawnPos;
            player.SetActive(true);

            if (cameraFollow)
                cameraFollow.target = player.transform;
        }

        yield return new WaitForSeconds(spawnDelay);

        // 👥 NPC 1
        if (npc1)
        {
            npc1.transform.position = npcSpawnPos1;
            npc1.SetActive(true);
        }

        yield return new WaitForSeconds(spawnDelay);

        // 👥 NPC 2
        if (npc2)
        {
            npc2.transform.position = npcSpawnPos2;
            npc2.SetActive(true);
        }
    }
}
