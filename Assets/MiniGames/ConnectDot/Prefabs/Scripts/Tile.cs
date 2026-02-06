using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Để bắt sự kiện chuột/touch trên UI
using UnityEngine.Events;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    static public int UNPLAYABLE_INDEX = 0;
    static public Color COLOR_HIGHTLIGHT = new Color(1, 1, 0, 0.5f);
    static public string NAME_CONNECTION = "Connection";
    static public string NAME_BACK = "Back";
    static public string NAME_MAK = "Mark"; // Tên object trong Hierarchy là "Mark"

    public int cid = 0;
    public int gridX;
    public int gridY;

    [HideInInspector] public UnityEvent<Tile> onSelected;
    [HideInInspector] public UnityEvent<Tile> onHover;

    // --- AUTO-PROPERTIES (Thay thế cho các biến có dấu "_" cũ) ---
    public bool isSelected { get; private set; }
    public bool isHighlighted { get; private set; }
    public bool isSolved { get; set; }
    public bool isPlayble { get; private set; }

    // Helper lấy Component Image nhanh
    private Image BackComponentImage => transform.Find(NAME_BACK).GetComponent<Image>();
    private Image ConnectionComponentImage => transform.Find(NAME_CONNECTION).Find("Pipe").GetComponent<Image>();
    private Image MarkComponentImage => transform.Find(NAME_MAK).GetComponent<Image>();

    public Color ConnectionColor => ConnectionComponentImage.color;

    private Color _originalColor;

    void Start()
    {
        // [FIXED] Dùng tên biến mới (không có dấu _)
        isPlayble = cid > UNPLAYABLE_INDEX;

        _originalColor = BackComponentImage.color;

        if (isPlayble)
            SetConnectionColor(MarkComponentImage.color);
        else
        {
            var mark = transform.Find(NAME_MAK);
            if (mark) mark.gameObject.SetActive(false);
        }
    }

    // --- XỬ LÝ EVENT UI ---

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isPlayble && !isSolved) // [FIXED]
        {
            isSelected = true;
            InvokeOnSelected();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPlayble && !isSolved) // [FIXED]
        {
            isSelected = false;
            InvokeOnSelected();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Khi chuột lướt vào ô này -> Báo cho Field biết để vẽ đường
        if (onHover != null) onHover.Invoke(this);
    }

    // --- LOGIC HIỂN THỊ ---

    public void ResetConnection()
    {
        var connection = transform.Find(NAME_CONNECTION).gameObject;
        connection.SetActive(false);
        connection.transform.localEulerAngles = Vector3.zero;
        isSolved = false; // [FIXED]
    }

    public void HightlightReset()
    {
        isHighlighted = false; // [FIXED]
        BackComponentImage.color = _originalColor;
    }

    public void Highlight()
    {
        isHighlighted = true; // [FIXED]
        BackComponentImage.color = COLOR_HIGHTLIGHT;
    }

    public void SetConnectionColor(Color color)
    {
        if (ConnectionComponentImage != null)
            ConnectionComponentImage.color = color;
    }

    public void ConnectionToSide(bool top, bool right, bool bottom, bool left)
    {
        transform.Find(NAME_CONNECTION).gameObject.SetActive(true);
        // Xoay ống nối theo hướng
        float angle = right ? -90 : bottom ? -180 : left ? -270 : 0;
        transform.Find(NAME_CONNECTION).localEulerAngles = new Vector3(0, 0, angle);
    }

    void InvokeOnSelected()
    {
        if (onSelected != null) onSelected.Invoke(this);
    }
}