using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DesktopManager : MonoBehaviour
{
    [SerializeField] private GameObject windowPrefab;
    [SerializeField] private Transform windowsParent;
    [SerializeField] private Transform taskbarContent;
    [SerializeField] private Computer computer;
    [SerializeField] private Button[] iconButtons; // 4 icons

    private List<AppWindow> openWindows = new List<AppWindow>();

    void Start()
    {
        // Tạo parent cho windows
        GameObject wp = new GameObject("WindowsParent");
        wp.transform.SetParent(transform);
        windowsParent = wp.transform;

        // Tạo taskbar content
        GameObject tc = new GameObject("TaskbarContent");
        tc.transform.SetParent(GameObject.Find("Taskbar").transform);
        HorizontalLayoutGroup hlg = tc.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 5;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        taskbarContent = tc.transform;

        // Assign icons (kéo 4 buttons vào array sau)
        // Close all: ESC
    }

    public void OpenApp(string appName)
    {
        GameObject winObj = Instantiate(windowPrefab, windowsParent);
        AppWindow appWin = winObj.GetComponent<AppWindow>();
        if (appWin == null) appWin = winObj.AddComponent<AppWindow>();
        appWin.Init(appName, this);
        openWindows.Add(appWin);
        CreateTaskBtn(appWin);
        winObj.transform.SetAsLastSibling();
    }

    void CreateTaskBtn(AppWindow appWin)
    {
        GameObject btn = new GameObject("Task_" + appWin.appName);
        Image img = btn.AddComponent<Image>();
        img.color = Color.gray;
        Button taskBtn = btn.AddComponent<Button>();
        taskBtn.onClick.AddListener(() => appWin.ToggleFocus());
        btn.transform.SetParent(taskbarContent);
    }

    public void CloseWindow(AppWindow appWin)
    {
        openWindows.Remove(appWin);
        Destroy(appWin.gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) CloseAll();
    }

    public void CloseAll()
    {
        foreach (var w in openWindows) Destroy(w.gameObject);
        openWindows.Clear();
        foreach (Transform t in taskbarContent) Destroy(t.gameObject);
        computer.CloseDesktop();
    }
}