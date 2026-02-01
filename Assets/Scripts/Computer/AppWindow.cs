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
    public Transform contentRoot;
    private RectTransform rect;

    public void Init(string name, DesktopManager mgr)
    {
        appName = name;
        manager = mgr;

        rect = GetComponent<RectTransform>();

        // TÌM LẠI CÁC THÀNH PHẦN NẾU CHƯA KÉO TRONG INSPECTOR
        if (titleText == null)
        {
            var t = transform.Find("Frame/TitleBar/Title");
            if (t != null) titleText = t.GetComponent<TextMeshProUGUI>();
        }
        if (closeBtn == null)
        {
            var t = transform.Find("Frame/TitleBar/CloseBtn");
            if (t != null) closeBtn = t.GetComponent<Button>();
        }
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // CHỈ GỌI KHI KHÔNG NULL
        if (titleText != null) titleText.text = name;
        if (closeBtn != null)
        {
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(() => manager.CloseWindow(this));
        }

        canvasGroup.alpha = 1f;

        // nếu chưa gán contentRoot thì tìm mặc định
        if (contentRoot == null)
        {
            var c = transform.Find("Content");
            if (c != null) contentRoot = c;
        }
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

    public void CloseSelf()
    {
        manager.CloseWindow(this);
    }
}