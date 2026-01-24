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
    public Vector3 playerSpawnPos;           // ✅ vị trí xuất hiện PLAYER
    public BusCameraFollow cameraFollow;

    [Header("Spawn Enemy")]
    public GameObject enemyPrefab;
    public Vector3 enemySpawnPos;

    [Header("Spawn NPCs (2 Prefabs)")]
    public GameObject npcPrefab1;
    public Vector3 npcSpawnPos1;

    public GameObject npcPrefab2;
    public Vector3 npcSpawnPos2;

    private Vector3 stopTarget;
    private bool reachedStop = false;
    private bool movingToFinal = false;
    private bool finished = false;

    void Start()
    {
        float moveUnit = stopDistancePx / 32f;
        stopTarget = transform.position + Vector3.right * moveUnit;

        // Ẩn player lúc đầu
        if (player != null)
            player.SetActive(false);

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

        // 👤 PLAYER xuất hiện tại vị trí chỉ định
        if (player != null)
        {
            player.transform.position = playerSpawnPos;
            player.SetActive(true);
        }

        // 🎥 Camera theo player
        if (cameraFollow != null && player != null)
            cameraFollow.target = player.transform;

        // 👾 Spawn Enemy
        if (enemyPrefab != null)
        {
            Instantiate(enemyPrefab, enemySpawnPos, Quaternion.identity);
        }

        // 👥 Spawn NPC 1
        if (npcPrefab1 != null)
        {
            Instantiate(npcPrefab1, npcSpawnPos1, Quaternion.identity);
        }

        // 👥 Spawn NPC 2
        if (npcPrefab2 != null)
        {
            Instantiate(npcPrefab2, npcSpawnPos2, Quaternion.identity);
        }

        // ❌ Tuỳ chọn: ẩn bus
        // gameObject.SetActive(false);
    }
}
