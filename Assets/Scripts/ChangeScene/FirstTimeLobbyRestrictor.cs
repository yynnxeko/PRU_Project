using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstTimeLobbyRestrictor : MonoBehaviour
{
    [Header("Allowed Door")]
    [Tooltip("Cửa đi vào phòng ngủ (được phép vào).")]
    public DoorSceneChange bedroomDoor;

    [Header("Restricted Doors")]
    [Tooltip("Các cửa bị cấm trong lần đầu vào Lobby.")]
    public DoorSceneChange[] restrictedDoors;

    [Header("Feedback when restricted")]
    public SpeechBubble bubblePrefab;
    [TextArea]
    public string restrictText = "Tôi phải về phòng tập thể\nđể cất đồ đã.";
    public float bubbleOffsetY = 1.5f;
    public float bubbleDuration = 2f;

    private SpeechBubble currentBubble;
    
    // Biến static để theo dõi xem người chơi đã vào bedroom lần nào chưa
    public static bool hasVisitedBedroom = false;

    void Start()
    {
        // Nếu đã từng vào phòng ngủ rồi thì không cần hạn chế nữa
        if (hasVisitedBedroom)
        {
            return;
        }

        // Vô hiệu hóa tính năng chuyển scene của các cửa bị cấm
        foreach (var door in restrictedDoors)
        {
            if (door != null)
            {
                // Thay vì chỉ disable, ta phá hủy luôn component chuyển scene để chắc chắn 100% không load qua scene khác
                Destroy(door);
                
                // Thêm một component bắt va chạm để hiện thông báo
                var restrictTrigger = door.gameObject.AddComponent<RestrictedDoorTrigger>();
                restrictTrigger.Init(this);
            }
        }
    }

    public void ShowRestrictMessage(Transform playerTransform)
    {
        if (bubblePrefab == null || currentBubble != null) return;

        currentBubble = Instantiate(
            bubblePrefab,
            playerTransform.position + Vector3.up * bubbleOffsetY,
            Quaternion.identity
        );

        currentBubble.Init(playerTransform, Vector3.up * bubbleOffsetY);
        currentBubble.Show(restrictText, bubbleDuration);
    }
}

public class RestrictedDoorTrigger : MonoBehaviour
{
    private FirstTimeLobbyRestrictor restrictor;

    public void Init(FirstTimeLobbyRestrictor restrictor)
    {
        this.restrictor = restrictor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (restrictor != null)
            {
                restrictor.ShowRestrictMessage(other.transform);
            }
        }
    }
}
