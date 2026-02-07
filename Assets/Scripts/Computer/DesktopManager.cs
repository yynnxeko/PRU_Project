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
    }

    public void OpenApp(string appName)
    {
        if (appName == "Game1")
        {
            OpenOrFocusGame1();
            return;
        }
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

    //Game1: nếu đang mở thì focus, nếu chưa thì tạo mới (reset)
    private void OpenOrFocusGame1()
    {
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
