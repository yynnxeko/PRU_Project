using UnityEngine;

public class HackTerminalInteractable : Interactable
{
    public HackLevel hackLevel = HackLevel.Level1;

    public override void OnHoldComplete(PlayerInteraction player)
    {
        Debug.Log("HackTerminal OnHoldComplete on " + gameObject.name);

        if (HackUIManager.Instance == null)
        {
            Debug.LogError("HackUIManager.Instance is NULL! Check if HackUIManager exists and Awake() sets Instance.");
            return;
        }

        HackUIManager.Instance.StartHack(hackLevel, success =>
        {
            OnHackFinished(success, player);
        });
    }


    void OnHackFinished(bool success, PlayerInteraction player)
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
                // TODO: xử lý khi hack L1 thành công
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
