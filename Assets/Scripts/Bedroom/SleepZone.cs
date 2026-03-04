using UnityEngine;

public class SleepZone : MonoBehaviour
{
    bool playerInside;

    GameObject player;
    Animator anim;
    PlayerController2 controller;

    public Transform sleepPoint;

    // 👉 KÉO MAINBED VÀO ĐÂY
    public BoxCollider2D mainBedCollider;

    bool isSleeping = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        anim = player.GetComponent<Animator>();
        controller = player.GetComponent<PlayerController2>();
    }

    void Update()
    {
        if (!playerInside && !isSleeping) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isSleeping)
                Sleep();
            else
                WakeUp();
        }
    }

    void Sleep()
    {
        player.transform.position = sleepPoint.position;

        anim.SetBool("IsSleep", true);

        controller.canMove = false;

        // 🔁 Bật trigger cho giường (hoặc tắt va chạm)
        mainBedCollider.isTrigger = true;

        isSleeping = true;
        Debug.Log("[SleepZone] ngủ rồiaaaaaaaaaaaaaaaaaaaaaaaaaa");

        // 👉 KHI NGỦ THÌ SẼ SANG NGÀY MỚI (CHỈ KHI LÀ BAN ĐÊM)
        if (DayManager.Instance != null && DayManager.Instance.currentPhase == DayPhase.Night)
        {
            Debug.Log("[SleepZone] Đang là Night → Gọi AdvanceDayWithDelay(2f)!");
            StartCoroutine(DayManager.Instance.AdvanceDayWithDelay(2f));
        }
        else
        {
            Debug.Log($"[SleepZone] Chưa đến lúc đi ngủ! currentPhase={DayManager.Instance?.currentPhase}");
            WakeUp(); // Tự động thức dậy nếu chưa đến tối
        }
    }

    void WakeUp()
    {
        anim.SetBool("IsSleep", false);

        controller.canMove = true;

        // 🔁 Trả lại va chạm bình thường
        mainBedCollider.isTrigger = false;

        isSleeping = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}
