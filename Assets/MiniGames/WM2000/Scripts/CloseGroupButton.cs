using UnityEngine;

public class CloseGroupButton : MonoBehaviour
{
    private DesktopManager desktopManager;
    private AppWindow appWindow;

    public void Init(DesktopManager dm, AppWindow win)
    {
        desktopManager = dm;
        appWindow = win;
    }

    public void Close()
    {
        if (desktopManager != null && appWindow != null)
            desktopManager.CloseWindow(appWindow);
        else
            Debug.LogWarning("CloseGroupButton: chưa Init đúng (dm/appWindow null).");
    }
}
