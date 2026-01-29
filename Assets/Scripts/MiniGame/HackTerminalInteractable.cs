using UnityEngine;

public class HackTerminalInteractable : Interactable
{
    public HackLevel hackLevel = HackLevel.Level1;
    PlayerInteraction currentPlayer;

    public override void OnHoldComplete(PlayerInteraction player)
    {
        Debug.Log("HackTerminal OnHoldComplete on " + gameObject.name);

        if (HackUIManager.Instance == null)
        {
            Debug.LogError("HackUIManager.Instance is NULL! Check if HackUIManager exists and Awake() sets Instance.");
            return;
        }

        // lưu player lại nếu cần dùng sau
        currentPlayer = player;

        // OpenTerminal cần Action<bool>  => truyền hàm nhận đúng 1 bool
        HackUIManager.Instance.OpenTerminal(OnHackFinished);
    }

    void OnHackFinished(bool success)
    {
        if (!success)
        {
            Debug.Log("Hack failed at " + gameObject.name);
            return;
        }

        Debug.Log("Hack success at " + gameObject.name);

        switch (hackLevel)
        {
            case HackLevel.Level1:
                // TODO: xử lý khi hack L1 thành công, có thể dùng currentPlayer
                break;
            case HackLevel.Level2:
                // TODO: xử lý khi hack L2 thành công
                break;
            case HackLevel.Level3:
                // TODO: xử lý khi hack L3 thành công
                break;
        }
    }
}
