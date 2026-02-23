using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance;

    [Header("UI Elements")]
    public CanvasGroup panel;
    public TMPro.TMP_Text promptText;
    public Slider progressBar;

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
        HideAll();
    }

    public void ShowPrompt(string prompt)
    {
        if (panel == null) return;
        panel.alpha = 1f;
        panel.blocksRaycasts = true;
        if (promptText != null)
        {
            promptText.text = prompt;
            promptText.gameObject.SetActive(true);
        }
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }

    public void ShowProgressBar(float progress)
    {
        if (panel == null) return;
        panel.alpha = 1f;
        panel.blocksRaycasts = true;
        if (promptText != null)
            promptText.gameObject.SetActive(false);
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = Mathf.Clamp01(progress);
        }
    }

    public void HideAll()
    {
        if (panel != null)
        {
            panel.alpha = 0f;
            panel.blocksRaycasts = false;
        }
        if (promptText != null)
            promptText.gameObject.SetActive(false);
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }
}