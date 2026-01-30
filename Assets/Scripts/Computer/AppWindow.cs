using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class AppWindow : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    public string appName;
    private DesktopManager manager;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button closeBtn;
    [SerializeField] private CanvasGroup canvasGroup;
    private RectTransform rect;

    public void Init(string name, DesktopManager mgr)
    {
        appName = name; manager = mgr;
        rect = GetComponent<RectTransform>();
        titleText ??= transform.Find("Frame/TitleBar/Title").GetComponent<TextMeshProUGUI>();
        titleText.text = name;
        closeBtn ??= transform.Find("Frame/TitleBar/CloseBtn").GetComponent<Button>();
        closeBtn.onClick.AddListener(() => manager.CloseWindow(this));
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        // Nội dung app
        Transform content = transform.Find("Content");
        if (content) content.GetComponent<Text>().text = "App " + name + " đang chạy!";
    }

    public void OnDrag(PointerEventData data)
    {
        rect.anchoredPosition += data.delta;
    }

    public void OnPointerClick(PointerEventData data)
    {
        ToggleFocus();
    }

    public void ToggleFocus()
    {
        canvasGroup.alpha = 1f;
        transform.SetAsLastSibling();
    }
}