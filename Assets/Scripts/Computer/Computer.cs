using UnityEngine;

public class Computer : MonoBehaviour
{
    [SerializeField] private GameObject desktopCanvas;
    [SerializeField] private GameObject player;
    private bool isNear = false;
    private DesktopManager desktopMgr;

    void Start()
    {
        if (desktopCanvas != null) desktopMgr = desktopCanvas.GetComponent<DesktopManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isNear = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isNear = false;
    }

    void Update()
    {
        if (isNear && Input.GetKeyDown(KeyCode.E))
        {
            OpenDesktop();
        }
    }

    public void OpenDesktop()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        desktopCanvas.SetActive(true);
    }

    public void CloseDesktop()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        desktopCanvas.SetActive(false);
    }
}