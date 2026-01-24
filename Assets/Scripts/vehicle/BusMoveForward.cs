using UnityEngine;
using System.Collections;

public class BusMoveForward : MonoBehaviour
{
    [Header("Move")]
    public float speed = 2f;
    public float stopDistancePx = 100f;     // quãng đường tới barrier (pixel)
    public Vector3 finalTarget;             // tọa độ xe chạy tiếp tới

    [Header("Barriers (open together)")]
    public Animator[] barrierAnimators;
    public float waitAfterOpen = 0.3f;

    [Header("Player & Camera Switch")]
    public GameObject player;
    public Vector3 playerSpawnPos;           // vị trí xuất hiện PLAYER
    public BusCameraFollow cameraFollow;

    [Header("Enemy (Scene Object)")]
    public GameObject enemy;
    public Vector3 enemySpawnPos;

    [Header("NPCs (Scene Objects)")]
    public GameObject npc1;
    public Vector3 npcSpawnPos1;

    public GameObject npc2;
    public Vector3 npcSpawnPos2;

    private Vector3 stopTarget;
    private bool reachedStop = false;
    private bool movingToFinal = false;
    private bool finished = false;

    void Start()
    {
        // Tính khoảng dừng (pixel -> unit)
        float moveUnit = stopDistancePx / 32f;
        stopTarget = transform.position + Vector3.right * moveUnit;

        // Ẩn toàn bộ nhân vật lúc đầu
        if (player != null) player.SetActive(false);
        if (enemy != null) enemy.SetActive(false);
        if (npc1 != null) npc1.SetActive(false);
        if (npc2 != null) npc2.SetActive(false);

        // Camera theo bus
        if (cameraFollow != null)
            cameraFollow.target = transform;
    }

    void Update()
    {
        if (finished) return;

        // 🚍 Chặng 1: chạy tới barrier
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

        // 🚍 Chặng 2: chạy tới đích
        if (movingToFinal)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                finalTarget,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, finalTarget) < 0.01f)
            {
                FinishBusFlow();
            }
        }
    }

    IEnumerator StopAndOpenBarriers()
    {
        // Mở tất cả barrier
        foreach (Animator anim in barrierAnimators)
        {
            if (anim != null)
                anim.Play("traffic_barrier_open", 0, 0f);
        }

        yield return new WaitForSeconds(waitAfterOpen);
        movingToFinal = true;
    }

    void FinishBusFlow()
    {
        finished = true;
        movingToFinal = false;

        // 👤 PLAYER
        if (player != null)
        {
            player.transform.position = playerSpawnPos;
            player.SetActive(true);
        }

        // 🎥 Camera theo player
        if (cameraFollow != null && player != null)
            cameraFollow.target = player.transform;

        // 👾 ENEMY
        if (enemy != null)
        {
            enemy.transform.position = enemySpawnPos;
            enemy.SetActive(true);
        }

        // 👥 NPC 1
        if (npc1 != null)
        {
            npc1.transform.position = npcSpawnPos1;
            npc1.SetActive(true);
        }

        // 👥 NPC 2
        if (npc2 != null)
        {
            npc2.transform.position = npcSpawnPos2;
            npc2.SetActive(true);
        }

        // ❌ Tuỳ chọn: ẩn bus sau khi xong
        // gameObject.SetActive(false);
    }
}
