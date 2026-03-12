using System.Collections;
using UnityEngine;

public class BusMoveForward : MonoBehaviour
{
    [Header("Move")]
    public float speed = 2f;
    public float stopDistancePx = 100f;
    public Vector3 finalTarget;

    [Header("Bus Audio")]
    public AudioSource audioSource;
    public AudioClip busMoveSound;
    public AudioClip busStopSound;

    [Header("Barriers (open together)")]
    public Animator[] barrierAnimators;
    public float waitAfterOpen = 0.3f;

    [Header("Camera")]
    public BusCameraFollow cameraFollow;

    [Header("Enemy")]
    public GameObject enemy;
    public Vector3 enemySpawnPos;
    public SpeechBubble enemyBubble;
    public AudioClip enemyVoice;
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
        // tìm player nếu chưa gán
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p) player = p;
        }

        GameFlow.BusCutscene = true;

        float moveUnit = stopDistancePx / 32f;
        stopTarget = transform.position + Vector3.right * moveUnit;

        if (player) player.SetActive(false);
        if (enemy) enemy.SetActive(false);
        if (npc1) npc1.SetActive(false);
        if (npc2) npc2.SetActive(false);

        if (cameraFollow)
            cameraFollow.target = transform;

        // nếu chưa gán AudioSource thì tự lấy
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        StartBusEngine();
    }

    void Update()
    {
        if (finished) return;

        // 🚍 đoạn 1: chạy tới barrier
        if (!reachedStop)
        {
            MoveTo(stopTarget);

            if (Vector3.Distance(transform.position, stopTarget) < 0.01f)
            {
                reachedStop = true;
                StopBus();
                StartCoroutine(StopAndOpenBarriers());
            }
            return;
        }

        // 🚍 đoạn 2: chạy tới điểm cuối
        if (movingToFinal)
        {
            MoveTo(finalTarget);

            if (Vector3.Distance(transform.position, finalTarget) < 0.01f)
            {
                finished = true;
                movingToFinal = false;

                StopBus();
                StartCoroutine(SpawnSequence());
            }
        }
    }

    void MoveTo(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }

    // ======================
    // BUS AUDIO
    // ======================

    void StartBusEngine()
    {
        if (audioSource && busMoveSound)
        {
            audioSource.clip = busMoveSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void StopBus()
    {
        if (audioSource)
        {
            audioSource.Stop();

            if (busStopSound)
                audioSource.PlayOneShot(busStopSound);
        }
    }

    // ======================
    // BARRIER
    // ======================

    IEnumerator StopAndOpenBarriers()
    {
        foreach (Animator anim in barrierAnimators)
        {
            if (anim)
                anim.Play("traffic_barrier_open", 0, 0f);
        }

        yield return new WaitForSeconds(waitAfterOpen);

        StartBusEngine();
        movingToFinal = true;
    }

    // ======================
    // SPAWN SEQUENCE
    // ======================

    IEnumerator SpawnSequence()
    {
        GameFlow.BusCutscene = false;

        if (enemy)
        {
            enemy.transform.position = enemySpawnPos;
            enemy.SetActive(true);
        }

        if (enemyBubble && enemy)
        {
            var bubble = Instantiate(
                enemyBubble,
                enemy.transform.position + Vector3.up * enemyBubbleOffsetY,
                Quaternion.identity
            );

            bubble.Init(enemy.transform, Vector3.up * enemyBubbleOffsetY);
            bubble.Show("Lê lê cái chân lên", bubbleTime, enemyVoice);

            yield return new WaitForSeconds(bubbleTime);
            Destroy(bubble.gameObject);
        }
        else
        {
            yield return new WaitForSeconds(bubbleTime);
        }

        if (player)
        {
            player.transform.position = playerSpawnPos;
            player.SetActive(true);

            if (cameraFollow)
                cameraFollow.target = player.transform;
        }

        yield return new WaitForSeconds(spawnDelay);

        if (npc1)
        {
            npc1.transform.position = npcSpawnPos1;
            npc1.SetActive(true);
        }

        yield return new WaitForSeconds(spawnDelay);

        if (npc2)
        {
            npc2.transform.position = npcSpawnPos2;
            npc2.SetActive(true);
        }
    }
}