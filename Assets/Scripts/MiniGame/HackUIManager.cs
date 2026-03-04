using UnityEngine;
using System;

public enum HackLevel { Level1, Level2, Level3 }

public class HackUIManager : MonoBehaviour
{
    public static HackUIManager Instance { get; private set; }

    [Header("Root")]
    public GameObject hackCanvas;
    public GameObject headerPanel;
    public GameObject statusPanel;
    public UnityEngine.UI.Text headerText;

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
        hackCanvas.SetActive(false);
        Debug.Log($"[HackLevel1UI Awake] {gameObject.name}", this);
    }

    public void StartHack(HackLevel level, Action<bool> callback)
    {
        onComplete = callback;

        Debug.Log($"[StartHack] level = {level}");
        Debug.Log($"[StartHack] hackCanvas = {hackCanvas}");
        Debug.Log($"[StartHack] headerPanel = {headerPanel}");
        Debug.Log($"[StartHack] statusPanel = {statusPanel}");
        Debug.Log($"[StartHack] headerText = {headerText}");
        Debug.Log($"[StartHack] level1Panel = {level1Panel}, level2Panel = {level2Panel}, level3Panel = {level3Panel}");
        Debug.Log($"[StartHack] level1 = {level1}, level2 = {level2}, level3 = {level3}");

        // 1. Bật / tắt các panel gốc
        Debug.Log("[StartHack] A - enable root panels");
        if (hackCanvas == null) { Debug.LogError("[StartHack] hackCanvas is NULL"); return; }
        if (headerPanel == null) { Debug.LogError("[StartHack] headerPanel is NULL"); return; }
        if (statusPanel == null) { Debug.LogError("[StartHack] statusPanel is NULL"); return; }

        hackCanvas.SetActive(true);
        headerPanel.SetActive(true);
        statusPanel.SetActive(true);

        // 2. Tắt hết level panels
        Debug.Log("[StartHack] B - disable all level panels");
        if (level1Panel != null) level1Panel.SetActive(false); else Debug.LogError("[StartHack] level1Panel is NULL");
        if (level2Panel != null) level2Panel.SetActive(false); else Debug.LogError("[StartHack] level2Panel is NULL");
        if (level3Panel != null) level3Panel.SetActive(false); else Debug.LogError("[StartHack] level3Panel is NULL");

        // 3. Bật level tương ứng
        Debug.Log("[StartHack] C - switch level");
        switch (level)
        {
            case HackLevel.Level1:
                if (headerText == null) { Debug.LogError("[StartHack] headerText is NULL"); return; }
                if (level1 == null) { Debug.LogError("[StartHack] level1 script is NULL"); return; }        
                if (level1Panel == null) { Debug.LogError("[StartHack] level1Panel is NULL (in switch)"); return; }

                headerText.text = "SYSTEM ACCESS - LEVEL 1";
                level1Panel.SetActive(true);
                Debug.Log("[StartHack] D - calling level1.Begin");
                level1.Begin(OnLevelFinished);
                break;

            case HackLevel.Level2:
                if (headerText == null) { Debug.LogError("[StartHack] headerText is NULL"); return; }
                if (level2 == null) { Debug.LogError("[StartHack] level2 script is NULL"); return; }
                if (level2Panel == null) { Debug.LogError("[StartHack] level2Panel is NULL (in switch)"); return; }

                headerText.text = "SECURITY OVERRIDE - LEVEL 2";
                level2Panel.SetActive(true);
                Debug.Log("[StartHack] D - calling level2.Begin");
                level2.Begin(OnLevelFinished);
                break;

            case HackLevel.Level3:
                if (headerText == null) { Debug.LogError("[StartHack] headerText is NULL"); return; }
                if (level3 == null) { Debug.LogError("[StartHack] level3 script is NULL"); return; }
                if (level3Panel == null) { Debug.LogError("[StartHack] level3Panel is NULL (in switch)"); return; }

                headerText.text = "!!! SYSTEM TRACE DETECTED !!!";
                level3Panel.SetActive(true);
                Debug.Log("[StartHack] D - calling level3.Begin");
                level3.Begin(OnLevelFinished);
                break;
        }
    }


    void OnLevelFinished(bool success)
    {
        hackCanvas.SetActive(false);
        onComplete?.Invoke(success);
        // TODO: mở lại điều khiển Player
    }
}
