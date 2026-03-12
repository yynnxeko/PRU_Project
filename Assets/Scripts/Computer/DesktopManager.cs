using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DesktopManager : MonoBehaviour
{
    [Header("Prefabs / References")]
    [SerializeField] private GameObject windowPrefab;
    [SerializeField] private Transform windowsParent;
    [SerializeField] private Transform taskbarContent;
    [SerializeField] private Computer computer;
    [SerializeField] private GameObject wm2000Prefab;
    [SerializeField] private GameObject dialogueGamePrefab;

    private readonly List<AppWindow> openWindows = new List<AppWindow>();
    private readonly Dictionary<AppWindow, GameObject> windowToGroup = new Dictionary<AppWindow, GameObject>();
    private readonly Dictionary<AppWindow, GameObject> windowToTaskBtn = new Dictionary<AppWindow, GameObject>();

    // giữ 1 instance Game1
    private AppWindow game1Instance;

    // Flags to control app visibility/activation
    [Header("App Flags")]
    [SerializeField] private bool allowGame1 = true;
    [SerializeField] private bool allowDialogueGame = true;

    // PlayerPrefs key chỉ cho Game1
    private const string AllowGame1Key = "DesktopManager_AllowGame1";

    void Start()
    {
        if (windowsParent == null)
        {
            GameObject wp = new GameObject("WindowsParent");
            wp.transform.SetParent(transform, false);
            windowsParent = wp.transform;
        }

        if (taskbarContent == null)
        {
            GameObject taskbar = GameObject.Find("Taskbar");
            if (taskbar != null)
            {
                GameObject tc = new GameObject("TaskbarContent");
                tc.transform.SetParent(taskbar.transform, false);
                HorizontalLayoutGroup hlg = tc.AddComponent<HorizontalLayoutGroup>();
                hlg.spacing = 5;
                hlg.childAlignment = TextAnchor.MiddleCenter;
                taskbarContent = tc.transform;
            }
            else
            {
                Debug.LogWarning("Không tìm thấy GameObject 'Taskbar' trong scene.");
            }
        }

        // Đọc cờ từ PlayerPrefs nếu có (chỉ cho Game1)
        if (PlayerPrefs.HasKey(AllowGame1Key))
            allowGame1 = PlayerPrefs.GetInt(AllowGame1Key) == 1;
    }

    public void OpenApp(string appName)
    {
        // if (appName == "Game1")
        // {
        //     if (!allowGame1)
        //     {
        //         Debug.Log("[Desktop] Game1 is not active (flag is off)");
        //         return;
        //     }
        //     OpenOrFocusGame1();
        //     return;
        // }
        if (appName == "DialogueGame")
        {
            OpenDialogueGame();
            return;
        }
        // app thường
        var winObj = Instantiate(windowPrefab, windowsParent);
        var appWin = winObj.GetComponent<AppWindow>() ?? winObj.AddComponent<AppWindow>();

        appWin.Init(appName, this);
        openWindows.Add(appWin);
        CreateTaskBtn(appWin);
        winObj.transform.SetAsLastSibling();
    }

    /// <summary>
    /// Mở một cửa sổ mới và hiển thị một tấm hình bên trong
    /// </summary>
    // giữ instance cửa sổ ảnh duy nhất
    private AppWindow imageViewerInstance;

    public void OpenImageApp(string appName, Sprite imageToShow)
    {
        if (imageToShow == null)
        {
            Debug.LogError("Chưa gán hình ảnh cho icon này!");
            return;
        }

        // 1. Nếu đang mở ảnh rồi -> Đóng cái cũ để tránh hiện "2 cái trắng"
        if (imageViewerInstance != null)
        {
            CloseWindow(imageViewerInstance);
        }

        var winObj = Instantiate(windowPrefab, windowsParent);
        var appWin = winObj.GetComponent<AppWindow>() ?? winObj.AddComponent<AppWindow>();

        appWin.Init(appName, this);
        openWindows.Add(appWin);
        imageViewerInstance = appWin;

        // KHÔNG gọi CreateTaskBtn(appWin) để không hiện cái ô màu xám dưới thanh tác vụ

        // 2) Tạo GameObject con chứa Image và gán vào contentRoot của AppWindow
        if (appWin.contentRoot != null)
        {
            // --- LÀM TRONG SUỐT TUYỆT ĐỐI ---
            // Ẩn tất cả các hình ảnh có sẵn của Prefab cửa sổ (khung, nền, nút...)
            Image[] allImages = appWin.GetComponentsInChildren<Image>(true);
            foreach (var i in allImages) i.enabled = false;

            GameObject imgObj = new GameObject("ImageContent_" + appName);
            imgObj.transform.SetParent(appWin.contentRoot, false);

            Image img = imgObj.AddComponent<Image>();
            img.enabled = true; // Đảm bảo Image mới luôn hiện
            img.sprite = imageToShow;
            img.preserveAspect = true; // Giữ nguyên tỉ lệ ảnh

            // --- THÊM LOGIC CLICK ĐỂ ĐÓNG ---
            Button btn = imgObj.AddComponent<Button>();
            btn.onClick.AddListener(() => CloseWindow(appWin));

            // Căn giãn toàn bộ khu vực Content
            RectTransform imgRect = img.GetComponent<RectTransform>();
            imgRect.anchorMin = Vector2.zero;
            imgRect.anchorMax = Vector2.one;
            imgRect.offsetMin = Vector2.zero;
            imgRect.offsetMax = Vector2.zero;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy ContentRoot trên Prefab Window để gắn ảnh!");
        }

        winObj.transform.SetAsLastSibling();
    }

    // Public methods to set flags (optional, for external control)
    public void SetAllowGame1(bool allow)
    {
        allowGame1 = allow;
        PlayerPrefs.SetInt(AllowGame1Key, allow ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void SetAllowDialogueGame(bool allow)
    {
        allowDialogueGame = allow;
    }

    //Game1: nếu đang mở thì focus, nếu chưa thì tạo mới (reset)
    private void OpenOrFocusGame1()
    {
        // ✅ Kiểm tra USB trong người trước khi mở Game1
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerInventory inv = playerObj.GetComponent<PlayerInventory>();
            if (inv == null || !inv.HasEvidenceOfType(EvidenceType.USB))
            {
                Debug.Log("[Desktop] Cần có USB mới mở được Game1!");
                return;
            }
        }

        // nếu instance cũ vẫn tồn tại -> focus
        if (game1Instance != null)
        {
            FocusWindow(game1Instance);
            return;
        }

        // tạo mới => reset
        if (windowPrefab == null)
        {
            Debug.LogError("windowPrefab is NULL!");
            return;
        }

        // 1) group
        GameObject groupGO = new GameObject("Game1_Group");
        groupGO.transform.SetParent(windowsParent, false);

        // 2) window
        GameObject winObj = Instantiate(windowPrefab, groupGO.transform);
        AppWindow appWin = winObj.GetComponent<AppWindow>() ?? winObj.AddComponent<AppWindow>();
        appWin.Init("Game1", this);

        openWindows.Add(appWin);
        windowToGroup[appWin] = groupGO;
        CreateTaskBtn(appWin);

        // lưu instance
        game1Instance = appWin;

        // 3) wm2000
        if (wm2000Prefab != null)
        {
            GameObject wm = Instantiate(wm2000Prefab, groupGO.transform);
            wm.transform.SetAsLastSibling();

            // nút đỏ đóng chuẩn
            var closeBtn = wm.GetComponentInChildren<CloseGroupButton>(true);
            if (closeBtn != null) closeBtn.Init(this, appWin);
            else Debug.LogWarning("WM2000 prefab chưa có CloseGroupButton trên nút đỏ!");
        }

        // đưa lên trên cùng
        groupGO.transform.SetAsLastSibling();
    }

    //THÊM METHOD MỚI
    private void OpenDialogueGame()
    {
        if (windowPrefab == null)
        {
            Debug.LogError("windowPrefab is NULL!");
            return;
        }

        // 1) group
        GameObject groupGO = new GameObject("DialogueGame_Group");
        groupGO.transform.SetParent(windowsParent, false);

        // 2) window
        GameObject winObj = Instantiate(windowPrefab, groupGO.transform);
        AppWindow appWin = winObj.GetComponent<AppWindow>() ?? winObj.AddComponent<AppWindow>();
        appWin.Init("DialogueGame", this);

        openWindows.Add(appWin);
        windowToGroup[appWin] = groupGO;
        CreateTaskBtn(appWin);

        // 3) dialogueGamePrefab
        if (dialogueGamePrefab != null)
        {
            GameObject dialogueGame = Instantiate(dialogueGamePrefab, groupGO.transform);
            dialogueGame.transform.SetAsLastSibling();

            // nút đỏ đóng chuẩn
            var closeBtn = dialogueGame.GetComponentInChildren<CloseGroupButton>(true);
            if (closeBtn != null) closeBtn.Init(this, appWin);
            else Debug.LogWarning("DialogueGame prefab chưa có CloseGroupButton trên nút đỏ!");
        }
        else
        {
            Debug.LogError("dialogueGamePrefab chưa được gán trong DesktopManager!");
        }

        // đưa lên trên cùng
        groupGO.transform.SetAsLastSibling();
    }
    private void FocusWindow(AppWindow w)
    {
        if (w == null) return;

        // nếu có group thì bring group lên top (đúng với Game1)
        if (windowToGroup.TryGetValue(w, out var group) && group != null)
            group.transform.SetAsLastSibling();
        else
            w.transform.SetAsLastSibling();

        // nếu bạn có ToggleFocus thì gọi thêm
        // w.ToggleFocus();
    }

    private GameObject CreateTaskBtn(AppWindow appWin)
    {
        if (taskbarContent == null) return null;

        GameObject btn = new GameObject("Task_" + appWin.appName);
        Image img = btn.AddComponent<Image>();
        img.color = Color.gray;

        Button taskBtn = btn.AddComponent<Button>();
        taskBtn.onClick.AddListener(() => FocusWindow(appWin));

        btn.transform.SetParent(taskbarContent, false);
        windowToTaskBtn[appWin] = btn;
        return btn;
    }

    // ✅ nút đỏ gọi vào đây -> destroy group + taskbar + clear instance
    public void CloseWindow(AppWindow appWin)
    {
        if (appWin == null) return;

        if (appWin == imageViewerInstance) imageViewerInstance = null;

        openWindows.Remove(appWin);

        // xoá taskbar btn
        if (windowToTaskBtn.TryGetValue(appWin, out var taskBtn) && taskBtn != null)
            Destroy(taskBtn);
        windowToTaskBtn.Remove(appWin);

        // destroy group nếu có
        if (windowToGroup.TryGetValue(appWin, out var group) && group != null)
            Destroy(group);
        else
            Destroy(appWin.gameObject);
        windowToGroup.Remove(appWin);

        // ✅ nếu là Game1 thì clear instance để bấm icon lần sau tạo mới (reset)
        if (appWin == game1Instance)
            game1Instance = null;
    }

    public void CloseAll()
    {
        // destroy group
        foreach (var kv in windowToGroup)
            if (kv.Value != null) Destroy(kv.Value);
        windowToGroup.Clear();

        // destroy task buttons
        foreach (var kv in windowToTaskBtn)
            if (kv.Value != null) Destroy(kv.Value);
        windowToTaskBtn.Clear();

        openWindows.Clear();

        // clear game1
        game1Instance = null;

        if (computer != null) computer.CloseDesktop();
    }
}
