using UnityEngine;
using UnityEngine.UI;
using System;

public class HackLevel3UI : MonoBehaviour
{
    public Button[] blockButtons;    // 8 button max
    public Text[] blockLabels;       // text trên button

    public Text hintText;
    public Text statusText;
    public Text timeLeftText;
    public Text traceText;

    public float timeLimit = 8f;

    int activeBlockCount;
    bool[] isCorrupted;              // true nếu block này lỗi
    int corruptedCount;
    int fixedCount;
    int traceLevel;                  // 0–10

    float timeLeft;
    bool running;
    Action<bool> onComplete;

    public void Begin(Action<bool> callback)
    {
        onComplete = callback;
        hintText.text = "Fix corrupted blocks";
        statusText.text = "STATUS: Trace in progress...";
        traceLevel = 0;
        fixedCount = 0;

        // 🔹 random số block 4–8
        activeBlockCount = UnityEngine.Random.Range(4, blockButtons.Length + 1);

        // bật/tắt button
        for (int i = 0; i < blockButtons.Length; i++)
            blockButtons[i].gameObject.SetActive(i < activeBlockCount);

        // 🔹 random 1–3 block lỗi
        isCorrupted = new bool[activeBlockCount];
        corruptedCount = UnityEngine.Random.Range(1, Mathf.Min(4, activeBlockCount + 1));

        for (int i = 0; i < corruptedCount; i++)
        {
            int idx;
            do
            {
                idx = UnityEngine.Random.Range(0, activeBlockCount);
            } while (isCorrupted[idx]);

            isCorrupted[idx] = true;
        }

        // gán text & listener
        for (int i = 0; i < activeBlockCount; i++)
        {
            int idx = i;
            string okText = $"BLOCK {i + 1}";
            string badText = $"ERR_{i + 1}";

            blockLabels[i].text = isCorrupted[i] ? badText : okText;

            blockButtons[i].onClick.RemoveAllListeners();
            blockButtons[i].onClick.AddListener(() => OnBlockClicked(idx));
        }

        timeLeft = timeLimit;
        running = true;
        UpdateTraceBar();
    }

    void Update()
    {
        if (!running) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;
        timeLeftText.text = $"TIME LEFT: {timeLeft:0.0}s";

        if (timeLeft <= 0f)
        {
            End(false);
        }
    }

    void OnBlockClicked(int index)
    {
        if (!running) return;

        if (isCorrupted[index])
        {
            // sửa block
            isCorrupted[index] = false;
            blockLabels[index].text = $"FIXED {index + 1}";
            fixedCount++;

            if (fixedCount >= corruptedCount)
            {
                statusText.text = "STATUS: System stabilized.";
                End(true);
            }
        }
        else
        {
            // click nhầm -> tăng trace
            traceLevel++;
            UpdateTraceBar();

            if (traceLevel >= 10)
            {
                statusText.text = "STATUS: Trace complete. Terminal locked.";
                End(false);
            }
        }
    }

    void UpdateTraceBar()
    {
        int slots = 10;
        string bar = "";
        for (int i = 0; i < slots; i++)
            bar += (i < traceLevel ? '█' : '░');

        traceText.text = $"TRACE LEVEL: {bar}";
    }

    void End(bool success)
    {
        if (!running) return;
        running = false;
        onComplete?.Invoke(success);
    }
}
