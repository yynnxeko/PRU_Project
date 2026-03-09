using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DesktopImageIcon : MonoBehaviour
{
    [Header("App Setup")]
    public string appTitle = "My Image";
    public Sprite imageToShow;

    private Button btn;
    private DesktopManager desktopManager;

    void Start()
    {
        btn = GetComponent<Button>();
        desktopManager = GetComponentInParent<DesktopManager>();
        
        // Cố gắng tìm DesktopManager trong scene nếu không thấy ở parent
        if (desktopManager == null)
        {
            desktopManager = FindObjectOfType<DesktopManager>();
        }

        btn.onClick.AddListener(OnClickIcon);
    }

    private void OnClickIcon()
    {
        if (desktopManager != null)
        {
            desktopManager.OpenImageApp(appTitle, imageToShow);
        }
        else
        {
            Debug.LogError("Chưa tìm thấy DesktopManager cho Icon này!");
        }
    }
}
