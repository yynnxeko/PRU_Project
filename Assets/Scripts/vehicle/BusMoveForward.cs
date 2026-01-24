using UnityEngine;
using System.Collections;

public class BusMoveForward : MonoBehaviour
{
    [Header("Move")]
    public float speed = 2f;
    public float stopDistancePx = 100f;     // quãng đường tới barrier (pixel)
    public Vector3 finalTarget;             // tọa độ xe chạy tiếp tới

    [Header("Barriers (open together)")]
    public Animator[] barrierAnimators;     // nhiều barrier
    public float waitAfterOpen = 0.3f;      // đứng lại sau khi barrier mở

    [Header("Player & Camera Switch")]
    public GameObject player;               // Player (disable lúc đầu)
    public BusCameraFollow cameraFollow;    // script camera follow

    // --- PHẦN THÊM MỚI CHO HỘI THOẠI ---
    [Header("Dialogue Trigger")]
    public float dialogueTriggerX;          // Tọa độ X để kích hoạt hội thoại
    private bool hasTriggeredDialogue = false;
    // -----------------------------------

    private Vector3 stopTarget;
    private bool reachedStop = false;
    private bool movingToFinal = false;
    private bool finished = false;

    void Start()
    {
        // pixel -> unit (PPU = 32)
        float moveUnit = stopDistancePx / 32f;
        stopTarget = transform.position + Vector3.right * moveUnit;

        // TỰ ĐỘNG TÍNH TOÁN ĐIỂM KÍCH HOẠT THOẠI
        // Mặc định: Kích hoạt khi chạy được 1/3 quãng đường tới rào chắn
        // Bạn có thể sửa thủ công trong Inspector nếu muốn
        if (dialogueTriggerX == 0)
        {
            dialogueTriggerX = transform.position.x + (moveUnit * 0.3f);
        }

        // Ẩn player lúc đầu
        if (player != null)
            player.SetActive(false);

        // Camera theo bus lúc đầu
        if (cameraFollow != null)
            cameraFollow.target = transform;
    }

    void Update()
    {
        if (finished) return;

        // 🚍 Chặng 1: chạy tới điểm dừng
        if (!reachedStop)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                stopTarget,
                speed * Time.deltaTime
            );

            // --- KIỂM TRA ĐỂ GỌI HỘI THOẠI ---
            // Nếu xe chạy qua điểm dialogueTriggerX thì gọi thoại
            if (transform.position.x >= dialogueTriggerX && !hasTriggeredDialogue)
            {
                hasTriggeredDialogue = true;
                DialogueManager dm = FindObjectOfType<DialogueManager>();
                if (dm != null) dm.StartDialogue();
            }
            // ---------------------------------

            if (Vector3.Distance(transform.position, stopTarget) < 0.01f)
            {
                reachedStop = true;
                StartCoroutine(StopAndOpenBarriers());
            }
            return;
        }

        // 🚍 Chặng 2: chạy tiếp tới đích
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
        // 🚧 mở tất cả barrier song song
        foreach (Animator anim in barrierAnimators)
        {
            if (anim != null)
                anim.Play("traffic_barrier_open", 0, 0f);
        }

        // ⏱ đứng lại một chút
        yield return new WaitForSeconds(waitAfterOpen);

        // ▶️ chạy tiếp
        movingToFinal = true;
    }

    void FinishBusFlow()
    {
        finished = true;
        movingToFinal = false;

        // 👤 hiện player
        if (player != null)
        {
            player.transform.position = transform.position;
            player.SetActive(true);
        }

        // 🎥 camera theo player
        if (cameraFollow != null && player != null)
            cameraFollow.target = player.transform;

        // ❌ (tuỳ chọn) ẩn bus
        // gameObject.SetActive(false);
    }
}
