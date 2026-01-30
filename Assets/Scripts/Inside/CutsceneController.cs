using System.Collections;
using UnityEngine;
using TMPro;

public class CutsceneController : MonoBehaviour
{
    [Header("Characters")]
    public GameObject nv1, nv2, nv3, nv4;

    [Header("Points")]
    public Transform spawnPoint;
    public Transform sp2;

    [Header("Move")]
    public float speed = 1f;

    [Header("Distances")]
    public float dist12 = 7f;
    public float dist23 = 2f;
    public float dist34 = 2f;

    [Header("Dialogue UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contextText;

    Coroutine move1, move2, move3, move4;

    void Start()
    {
        nv1.transform.position = spawnPoint.position;
        nv2.transform.position = spawnPoint.position + Vector3.left * dist12;
        nv3.transform.position = nv2.transform.position + Vector3.left * dist23;
        nv4.transform.position = nv3.transform.position + Vector3.left * dist34;

        StartCoroutine(CutsceneSequence());
    }

    IEnumerator CutsceneSequence()
    {
        Animator a1 = nv1.GetComponent<Animator>();
        Animator a2 = nv2.GetComponent<Animator>();
        Animator a3 = nv3.GetComponent<Animator>();
        Animator a4 = nv4.GetComponent<Animator>();

        // ===== Move group =====
        move1 = StartCoroutine(MoveToPoint(nv1, sp2.position));
        move2 = StartCoroutine(Follow(nv2, nv1, dist12));
        move3 = StartCoroutine(Follow(nv3, nv2, dist23));
        move4 = StartCoroutine(Follow(nv4, nv3, dist34));

        yield return new WaitUntil(() =>
            Vector3.Distance(nv1.transform.position, sp2.position) < 0.1f
        );

        StopMove();

        // ===== Dialogue =====
        yield return ShowLine("Trưởng nhóm", "Chỗ này chính là nơi làm việc của chúng ta.");
        yield return ShowLine("Trưởng nhóm", "Ở đây không cần bằng cấp, chỉ cần dám nghĩ dám làm!");
        yield return ShowLine("Nhân viên", "Tôi... tôi có thể về được không?");

        // NV1 quay lại
        yield return MoveToPoint(nv1, nv2.transform.position + Vector3.right);

        yield return ShowLine("Trưởng nhóm", "Mày vừa nói gì!!");
        yield return ShowLine("Nhân viên", "Tôi muốn về...");

        // ===== Actions =====
        a1.SetTrigger("ActionA");
        yield return new WaitForSeconds(0.4f);

        yield return ShowLine("Nhân viên", "Á Á Á....");

        a2.SetTrigger("ActionB");
    }

    // ================= MOVE =================

    IEnumerator MoveToPoint(GameObject obj, Vector3 target)
    {
        while (Vector3.Distance(obj.transform.position, target) > 0.1f)
        {
            obj.transform.position = Vector3.MoveTowards(
                obj.transform.position,
                target,
                speed * Time.deltaTime
            );
            yield return null;
        }
    }

    IEnumerator Follow(GameObject follower, GameObject target, float distance)
    {
        while (true)
        {
            if (Vector3.Distance(follower.transform.position, target.transform.position) > distance)
            {
                follower.transform.position = Vector3.MoveTowards(
                    follower.transform.position,
                    target.transform.position - Vector3.right * distance,
                    speed * Time.deltaTime
                );
            }
            yield return null;
        }
    }

    void StopMove()
    {
        if (move1 != null) StopCoroutine(move1);
        if (move2 != null) StopCoroutine(move2);
        if (move3 != null) StopCoroutine(move3);
        if (move4 != null) StopCoroutine(move4);
    }

    // ================= DIALOG =================

    IEnumerator ShowLine(string name, string text)
    {
        nameText.text = name;
        contextText.text = text;

        yield return new WaitForSeconds(2f);
    }
}
