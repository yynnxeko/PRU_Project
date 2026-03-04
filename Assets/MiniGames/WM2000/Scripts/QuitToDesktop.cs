using UnityEngine;

public class QuitToDesktop : MonoBehaviour
{
    [SerializeField] private GameObject wm2000Root;   // object gốc của WM2000

    public void Quit()
    {
        // Tìm GameObject window (panel xám bên ngoài) rồi tắt nó
        var window = transform.parent.gameObject; // hoặc GetComponentInParent<Canvas>()/AppWindow
        window.SetActive(false);
    }
}
