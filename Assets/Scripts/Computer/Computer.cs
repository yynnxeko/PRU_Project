using UnityEngine;

public class Computer : MonoBehaviour
{
    [SerializeField] private GameObject desktopCanvas;
    [SerializeField] private GameObject player;

    private bool isNear = false;
    private DesktopManager desktopMgr;
    private PlayerController2 playerController;

    void Start()
    {
        if (desktopCanvas != null) desktopMgr = desktopCanvas.GetComponent<DesktopManager>();

        // nếu chưa kéo trong inspector thì tự lấy từ player
        if (playerController == null && player != null)
            playerController = player.GetComponent<PlayerController2>();

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController2>();
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
        if (playerController == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerController = playerObj.GetComponent<PlayerController2>();
        }
        
        bool isInGame = playerController != null && playerController.isInGame;
        bool isSitting = playerController != null && playerController.IsSitting;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInGame) return;
            if (desktopCanvas.activeSelf)
            {
                CloseDesktop();
                return;
            }

            if (!isInGame && isSitting)
            {
                OpenDesktop();
            }
        }
    }
    public void OpenDesktop()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        desktopCanvas.SetActive(true);
        if (playerController != null)
            playerController.isInGame = false;
    }

    public void CloseDesktop()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        desktopCanvas.SetActive(false);
    }
}