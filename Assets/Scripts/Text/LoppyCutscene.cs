using UnityEngine;
using System.Collections;

public class LoppyCutscene : MonoBehaviour
{
    [Header("Settings")]
    public float moveTime = 1.0f;
    public Sprite enemyAvatar;

    [Header("Room Points")]
    public Transform posWork;
    public Transform posEat;
    public Transform posMedical;
    public Transform posRelax;

    private Camera cam;
    private CameraFollowPersist camFollow;
    private Vector3 initialCamPos;

    void Start()
    {
        cam = Camera.main;
        initialCamPos = cam.transform.position;
        camFollow = cam.GetComponent<CameraFollowPersist>();

        StartCoroutine(FullIntroSequence());
    }

    IEnumerator FullIntroSequence()
    {
        // 1. Tắt follow để cam không bị giật về player
        if (camFollow != null) camFollow.enabled = false;

        string boss = "<color=red>Giang hồ</color>";

        // --- MỞ ĐẦU ---
        yield return ShowLine("Nhìn cho kỹ, nhớ cho rõ. Ở đây không có chỗ cho lũ ngu.", boss);
        yield return ShowLine("Có 4 khu vực chúng mày buộc phải thuộc lòng.", boss);

        // --- PHÒNG LÀM VIỆC ---
        yield return MoveAndTalk(posWork, "PHÒNG LÀM VIỆC: Đến giờ thì vác xác vào mà cày!", boss);

        // --- PHÒNG ĂN ---
        yield return MoveAndTalk(posEat, "NHÀ BẾP: Đớp nhanh rồi biến, đừng có lề mề.", boss);

        // --- PHÒNG Y TẾ ---
        yield return MoveAndTalk(posMedical, "Khu Y TẾ: Nơi dành cho mấy thằng sắp chết.", boss);

        // --- PHÒNG SINH HOẠT ---
        yield return MoveAndTalk(posRelax, "Khu SINH HOẠT: Ngủ và im lặng.", boss);

        // --- KẾT THÚC ---
        yield return MoveCamera(cam.transform.position, initialCamPos); // Về lại player
        yield return ShowLine("Nghe thủng chưa? Giờ thì biến đi làm việc!", boss);

        // Bật lại follow
        if (camFollow != null) camFollow.enabled = true;
    }

    // Hàm tích hợp: Di chuyển Cam xong mới hiện thoại
    IEnumerator MoveAndTalk(Transform target, string text, string name)
    {
        if (target == null) yield break;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, initialCamPos.z);
        yield return MoveCamera(cam.transform.position, targetPos);
        yield return ShowLine(text, name);
    }

    IEnumerator MoveCamera(Vector3 from, Vector3 to)
    {
        float t = 0;
        while (t < moveTime)
        {
            cam.transform.position = Vector3.Lerp(from, to, t / moveTime);
            t += Time.deltaTime;
            yield return null;
        }
        cam.transform.position = to;
    }

    IEnumerator ShowLine(string text, string name)
    {
        bool done = false;
        // Gọi DialogueUI và đợi callback onComplete
        DialogueUI.Instance.ShowDialogue(text, name, enemyAvatar, () => { done = true; });

        while (!done)
        {
            yield return null;
        }
    }
}