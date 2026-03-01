using UnityEngine;
using System.Collections;

public class LoppyCutscene : MonoBehaviour
{
    [Header("Settings")]
    public float moveTime = 1.0f;
    public Sprite enemyAvatar;

    [Header("Room Points")]
    public Transform posRelax;
    public Transform posEat;
    public Transform posMedical;
    public Transform posWork;

    [Header("NPC (pause during cutscene)")]
    public NPCPathFollower npcPath;
    public Follower[] followers;

    private Camera cam;
    private CameraFollowPersist camFollow;
    private Vector3 initialCamPos;

    private static bool hasPlayed = false;
    public static bool isPlaying = false;

    void Start()
    {
        // 1. Khoá các tính năng khác lại ngay lập tức từ đầu frame
        if (!hasPlayed)
        {
            isPlaying = true;
        }

        // Nếu đã chạy rồi trong lần Play này thì không chạy lại
        if (hasPlayed)
        {
            if (npcPath != null) npcPath.gameObject.SetActive(false);
            if (followers != null)
            {
                foreach (var f in followers)
                {
                    if (f != null) f.gameObject.SetActive(false);
                }
            }
            gameObject.SetActive(false);
            return;
        }

        cam = Camera.main;
        initialCamPos = cam.transform.position;
        camFollow = cam.GetComponent<CameraFollowPersist>();

        StartCoroutine(FullIntroSequence());
    }

    IEnumerator FullIntroSequence()
    {
        if (camFollow != null) camFollow.enabled = false;

        // ===== PAUSE NPC trong lúc cutscene =====
        if (npcPath != null) npcPath.Pause();
        foreach (var f in followers)
            if (f != null) f.Pause();

        string boss = "<color=red>Giang hồ</color>";

        yield return ShowLine("Nhìn cho kỹ, nhớ cho rõ. Ở đây không có chỗ cho lũ ngu.", boss);
        yield return ShowLine("Có 4 khu vực chúng mày buộc phải thuộc lòng.", boss);

        yield return MoveAndTalk(posRelax, "Khu SINH HOẠT: Ngủ và im lặng.", boss);
        yield return MoveAndTalk(posEat, "NHÀ BẾP: Đớp nhanh rồi biến, đừng có lề mề.", boss);
        yield return MoveAndTalk(posMedical, "Khu Y TẾ: Nơi dành cho mấy thằng sắp chết.", boss);
        yield return MoveAndTalk(posWork, "PHÒNG LÀM VIỆC: Đến giờ thì vác xác vào mà cày!", boss);

        yield return MoveCamera(cam.transform.position, initialCamPos);
        yield return ShowLine("Nghe thủng chưa? Giờ thì biến vào phòng tập thể đi!", boss);

        if (camFollow != null) camFollow.enabled = true;

        // ===== RESUME NPC sau cutscene =====
        if (npcPath != null)
        {
            npcPath.ResetPath();
            npcPath.Resume();
        }
        foreach (var f in followers)
            if (f != null) f.Resume();

        // Đánh dấu đã chạy trong lần Play này
        hasPlayed = true;
        isPlaying = false;
    }

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

        DialogueUI.Instance.ShowDialogue(text, name, enemyAvatar, () => { done = true; });

        while (!done)
        {
            yield return null;
        }
    }
}