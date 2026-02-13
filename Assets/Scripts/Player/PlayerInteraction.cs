using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    Interactable current;
    PlayerController2 playerController;

    void Awake()
    {
        playerController = GetComponent<PlayerController2>();
    }

    void Update()
    {
        if (current == null) return;

        // Key down: bắt đầu hold
        if (Input.GetKeyDown(KeyCode.E))
        {
            current.holdAction.StartHold();
            current.OnHoldStart(this);

            // khóa di chuyển nếu có PlayerController2
            if (playerController != null)
                playerController.canMove = false;

            // hiện UI prompt + reset progress
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.Show(current.promptMessage);
                InteractionUI.Instance.UpdateProgress(0f);
            }
        }

        // Key giữ: update tiến trình
        if (Input.GetKey(KeyCode.E))
        {
            bool completed = current.holdAction.UpdateHold(Time.deltaTime);

            float progress = current.holdAction.GetProgress();
            current.OnHolding(this, progress);

            // update UI progress
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.UpdateProgress(progress);

            if (completed)
            {
                current.OnHoldComplete(this);
                current.holdAction.CancelHold();

                // unlock movement
                if (playerController != null)
                    playerController.canMove = true;

                if (InteractionUI.Instance != null)
                    InteractionUI.Instance.Hide();
            }
        }

        // Key up: cancel hold
        if (Input.GetKeyUp(KeyCode.E))
        {
            current.holdAction.CancelHold();
            current.OnHoldCancel(this);

            // unlock movement
            if (playerController != null)
                playerController.canMove = true;

            if (InteractionUI.Instance != null)
                InteractionUI.Instance.Hide();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Interactable i = other.GetComponent<Interactable>();
        if (i != null)
        {
            current = i;
            Debug.Log(i.promptMessage);

            // show prompt (non-intrusive) so player biết có thể tương tác
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.Show(i.promptMessage);
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
                InteractionUI.Instance.Hide();

            current = null;
        }
    }
}