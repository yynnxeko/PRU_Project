using UnityEngine;

public class SleepZone : MonoBehaviour
{
    bool playerInside;

    GameObject player;
    Animator anim;
    PlayerController2 controller;

    public Transform sleepPoint;

    bool isSleeping = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        anim = player.GetComponent<Animator>();
        controller = player.GetComponent<PlayerController2>(); // lấy script movement
    }

    void Update()
    {
        if (!playerInside) return;

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

        controller.canMove = false;   // 🔒 khóa di chuyển

        isSleeping = true;
    }

    void WakeUp()
    {
        anim.SetBool("IsSleep", false);

        controller.canMove = true;    // 🔓 mở di chuyển

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
