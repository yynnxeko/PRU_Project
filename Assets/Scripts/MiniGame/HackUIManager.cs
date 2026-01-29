using UnityEngine;
using System;

public enum HackLevel { Level1, Level2, Level3 }

public class HackUIManager : MonoBehaviour
{
    public static HackUIManager Instance { get; private set; }

    [Header("Root")]
    public GameObject hackCanvas;          // Canvas tổng
    public GameObject headerPanel;         // Panel title trên cùng
    public GameObject statusPanel;         // Panel status dưới cùng
    public UnityEngine.UI.Text headerText; // Text trong header

    [Header("Terminal")]
    public GameObject terminalPanel;       // Khung máy tính (chứa menu + level)
    public GameObject gameSelectPanel;     // Menu chọn mini game

    [Header("Level Panels")]
    public GameObject level1Panel;
    public GameObject level2Panel;
    public GameObject level3Panel;

    [Header("Level Scripts")]
    public HackLevel1UI level1;
    public HackLevel2UI level2;
    public HackLevel3UI level3;

    Action<bool> onComplete;

    void Awake()
    {
        Instance = this;
        if (hackCanvas != null)
            hackCanvas.SetActive(false);
    }

    // Gọi hàm này khi player nhấn E vào terminal
    public void OpenTerminal(Action<bool> callback)
    {
        onComplete = callback;

        // Bật canvas + terminal + menu
        hackCanvas.SetActive(true);
        headerPanel.SetActive(true);
        statusPanel.SetActive(true);

        terminalPanel.SetActive(true);
        gameSelectPanel.SetActive(true);

        // Tắt hết level panels
        level1Panel.SetActive(false);
        level2Panel.SetActive(false);
        level3Panel.SetActive(false);

        headerText.text = "SYSTEM TERMINAL";
    }

    // 3 hàm này gán cho OnClick của các nút trong GameSelectPanel
    public void OnSelectLevel1()
    {
        StartHack(HackLevel.Level1);
    }

    public void OnSelectLevel2()
    {
        StartHack(HackLevel.Level2);
    }

    public void OnSelectLevel3()
    {
        StartHack(HackLevel.Level3);
    }

    // Chỉ dùng nội bộ để bật level đúng và gọi Begin
    public void StartHack(HackLevel level)
    {
        // Ẩn menu chọn game
        if (gameSelectPanel != null)
            gameSelectPanel.SetActive(false);

        // Tắt hết panels level
        if (level1Panel != null) level1Panel.SetActive(false);
        if (level2Panel != null) level2Panel.SetActive(false);
        if (level3Panel != null) level3Panel.SetActive(false);

        switch (level)
        {
            case HackLevel.Level1:
                headerText.text = "SYSTEM ACCESS - LEVEL 1";
                level1Panel.SetActive(true);
                level1.Begin(OnLevelFinished);
                break;

            case HackLevel.Level2:
                headerText.text = "SECURITY OVERRIDE - LEVEL 2";
                level2Panel.SetActive(true);
                level2.Begin(OnLevelFinished);
                break;

            case HackLevel.Level3:
                headerText.text = "!!! SYSTEM TRACE DETECTED !!!";
                level3Panel.SetActive(true);
                level3Panel.SetActive(true);
                level3.Begin(OnLevelFinished);
                break;
        }
    }

    void OnLevelFinished(bool success)
    {
        // Tắt toàn bộ UI hack
        terminalPanel.SetActive(false);
        hackCanvas.SetActive(false);

        onComplete?.Invoke(success);
    }
}
