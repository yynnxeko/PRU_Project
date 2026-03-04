using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class HackLevel2UI : MonoBehaviour
{
    public Text sequenceLabelText;
    public Text sequenceValueText;
    public Text inputLabelText;
    public Text inputValueText;

    public Text statusText;
    public Text timeLeftText;
    public Text progressText;

    public float inputPhaseTime = 8f;  // thời gian cho pha nhập

    float timeLeft;
    string currentSequence;
    string typed;
    bool running;
    bool showingSequence;

    Action<bool> onComplete;

    public void Begin(Action<bool> callback)
    {
        onComplete = callback;

        sequenceLabelText.text = "TYPE SEQUENCE:";
        inputLabelText.text = "INPUT:";
        statusText.text = "STATUS: Memorize the sequence.";

        // 🔹 random độ dài 4–7
        int length = UnityEngine.Random.Range(4, 8);

        // 🔹 random thời gian hiển thị 2–4s
        float showTime = UnityEngine.Random.Range(2f, 4f);

        currentSequence = GenerateRandomSequence(length);
        sequenceValueText.text = $"> {currentSequence}";
        inputValueText.text = "> ";

        timeLeft = showTime;
        showingSequence = true;
        running = true;
    }

    void Update()
    {
        if (!running) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;

        if (showingSequence)
        {
            timeLeftText.text = $"MEMORIZE: {timeLeft:0.0}s";

            if (timeLeft <= 0f)
            {
                // chuyển sang pha nhập
                showingSequence = false;
                statusText.text = "STATUS: Type the sequence.";
                timeLeft = inputPhaseTime;
                sequenceValueText.text = "> ???";  // ẩn đi
            }
        }
        else
        {
            timeLeftText.text = $"TIME LEFT: {timeLeft:0.0}s";
            UpdateProgressBar();

            if (timeLeft <= 0f)
            {
                End(false);
                return;
            }

            HandleTyping();
        }
    }

    void HandleTyping()
    {
        string input = Input.inputString;
        if (string.IsNullOrEmpty(input)) return;

        foreach (char c in input)
        {
            if (c == '\b')
            {
                if (typed.Length > 0) typed = typed[..^1];
            }
            else if (c == '\n' || c == '\r')
            {
                CheckSequence();
            }
            else
            {
                if (char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '#')
                    typed += char.ToUpper(c);
            }
        }

        inputValueText.text = $"> {typed}";
    }

    void CheckSequence()
    {
        if (typed == currentSequence)
        {
            statusText.text = "STATUS: Override success.";
            End(true);
        }
        else
        {
            statusText.text = "STATUS: Incorrect. Sequence reset.";
            typed = "";
            inputValueText.text = "> ";
            // reset tiến trình
            timeLeft -= 2f;
        }
    }

    void UpdateProgressBar()
    {
        int slots = 5;
        float ratio = timeLeft / inputPhaseTime;
        int filled = Mathf.Clamp(Mathf.RoundToInt(ratio * slots), 0, slots);

        string bar = "";
        for (int i = 0; i < slots; i++)
            bar += (i < filled ? '▓' : '░');

        progressText.text = $"PROGRESS: {bar}";
    }

    string GenerateRandomSequence(int length)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string symbols = "_-#";

        string pool = letters + digits + symbols;

        var sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            char c = pool[UnityEngine.Random.Range(0, pool.Length)];
            sb.Append(c);
        }
        return sb.ToString();
    }

    void End(bool success)
    {
        if (!running) return;
        running = false;
        onComplete?.Invoke(success);
    }
}
