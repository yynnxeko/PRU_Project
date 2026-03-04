using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Interactable current;
    PlayerController2 playerController;
    bool isHolding = false;

    void Awake()
    {
        playerController = GetComponent<PlayerController2>();
    }

    void Update()
    {
        if (current == null) return;

        // Bắt đầu hold
        if (Input.GetKeyDown(KeyCode.E) && !isHolding)
        {
            current.holdAction.StartHold();
            current.OnHoldStart(this);
            isHolding = true;

            if (playerController != null)
                playerController.canMove = false;

            if (InteractionUI.Instance != null)
                InteractionUI.Instance.ShowProgressBar(0f);
        }

        // Đang giữ E và đang ở state holding
        if (Input.GetKey(KeyCode.E) && isHolding)
        {
            bool completed = current.holdAction.UpdateHold(Time.deltaTime);
            float progress = current.holdAction.GetProgress();
            current.OnHolding(this, progress);

            if (!completed && InteractionUI.Instance != null)
                InteractionUI.Instance.ShowProgressBar(progress);

            if (completed)
            {
                current.OnHoldComplete(this);
                current.holdAction.CancelHold();
                isHolding = false; // Chặn không update nữa!

                if (playerController != null)
                    playerController.canMove = true;

                if (InteractionUI.Instance != null)
                    InteractionUI.Instance.HideAll();
            }
        }

        // Nhả E
        if (Input.GetKeyUp(KeyCode.E) && isHolding)
        {
            current.holdAction.CancelHold();
            current.OnHoldCancel(this);
            isHolding = false;

            if (playerController != null)
                playerController.canMove = true;

            if (InteractionUI.Instance != null)
                InteractionUI.Instance.HideAll();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Interactable i = other.GetComponent<Interactable>();
        if (i != null)
        {
            current = i;
            Debug.Log(i.promptMessage);

            // Hiện PROMPT chỉ khi player đứng gần, KHÔNG GỌI Ở INTERACTABLE!
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.ShowPrompt(i.promptMessage);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (current == null) return;

        Interactable i = other.GetComponent<Interactable>();
        if (i != null && i == current)
        {
            if (current.holdAction != null)
                current.holdAction.CancelHold();

            // gọi event cancel để logic interactable biết
            current.OnHoldCancel(this);

            // unlock movement (phòng trường hợp k bị unlock trước)
            if (playerController != null)
                playerController.canMove = true;

            // hide UI
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.HideAll();

            current = null;
        }
    }
}