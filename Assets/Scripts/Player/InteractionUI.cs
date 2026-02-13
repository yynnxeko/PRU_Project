using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance;

    [Header("UI Elements")]
    public CanvasGroup panel;      // toàn bộ panel chứa prompt + progress
    public Text promptText;        // văn bản prompt (ví dụ "Hold E to search cabinet")
    public Slider progressBar;     // progress bar (0..1)

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("InteractionUI: Multiple instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        // start hidden
        if (panel != null)
        {
            panel.alpha = 0f;
            panel.blocksRaycasts = false;
        }
        if (progressBar != null)
            progressBar.value = 0f;
    }

    public void Show(string prompt)
    {
        if (panel == null) return;
        panel.alpha = 1f;
        panel.blocksRaycasts = true;
        if (promptText != null)
            promptText.text = prompt;
        if (progressBar != null)
            progressBar.value = 0f;
    }

    public void UpdateProgress(float p)
    {
        if (progressBar == null) return;
        progressBar.value = Mathf.Clamp01(p);
    }

    public void Hide()
    {
        if (panel == null) return;
        panel.alpha = 0f;
        panel.blocksRaycasts = false;
    }
}