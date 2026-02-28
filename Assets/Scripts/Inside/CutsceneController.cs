using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [Header("Characters")]
    public GameObject nv1, nv2, nv3, nv4;

    [Header("Points")]
    public Transform spawnPoint;
    public Transform sp2;
    public Transform sp3;
    public Transform sp4;

    [Header("Move")]
    public float speed = 1f;

    [Header("Distances")]
    public float dist12 = 7f;
    public float dist23 = 2f;
    public float dist34 = 2f;

    [Header("Dialogue UI")]
    public GameObject dialogueCanvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contextText;


    Animator a1, a2, a3, a4;

    void Start()
    {
        dialogueCanvas.SetActive(false);

        nv1.transform.position = spawnPoint.position;
        nv2.transform.position = spawnPoint.position + Vector3.left * dist12;
        nv3.transform.position = nv2.transform.position + Vector3.left * dist23;
        nv4.transform.position = nv3.transform.position + Vector3.left * dist34;

        a1 = nv1.GetComponent<Animator>();
        a2 = nv2.GetComponent<Animator>();
        a3 = nv3.GetComponent<Animator>();
        a4 = nv4.GetComponent<Animator>();

        StartCoroutine(CutsceneSequence());
    }

    IEnumerator CutsceneSequence()
    {
        // ===== GROUP TO SP2 =====
        yield return WalkGroupToPoint(sp2.position);

        FaceDirection(a1, Vector2.left);

        // ===== DIALOG =====
        yield return ShowLine("Tên môi giới", "Chỗ này chính là nơi làm việc của chúng ta.");
        yield return ShowLine("Tên môi giới", "Ở đây không cần bằng cấp, chỉ cần dám nghĩ dám làm!");
        yield return ShowLine("Người đàn ông", "Tôi... tôi có thể về được không?");

        // ===== NV1 QUAY LẠI =====
        yield return WalkSingleToPoint(nv1, a1, nv2.transform.position + Vector3.right * 0.5f);

        yield return ShowLine("Tên môi giới", "Mày vừa nói gì!!");
        yield return ShowLine("Người đàn ông", "Tôi muốn về...");

        // ===== SHOCK =====
        a1.SetTrigger("shocking");

        yield return new WaitForSeconds(1f);

        yield return ShowLine("Người đàn ông", "Á Á Á....");

        // ===== FAINT =====
        a2.SetTrigger("shocked");
        a2.SetTrigger("faint");

        yield return new WaitForSeconds(1f);

        // ===== AFTER =====
        yield return ShowLine("Tên môi giới", "Cút vào phòng ngay!");

        // 👉 đi ngang tới sp3
        yield return WalkTwoToPoint(nv1, a1, nv2, a2, sp3.position);

        FaceDirection(a1, Vector2.left);

        yield return ShowLine("Tên môi giới", "Còn đứng đó nhìn à?");

        // 👉 đi lên trên tới sp4
        yield return WalkTwoToPoint(nv1, a1, nv2, a2, sp4.position);

        yield return new WaitForSeconds(1f);

        // Load scene mới
        SceneManager.LoadScene("Map_Internal Area_Night");
    }

    // ================= MOVE =================

    IEnumerator WalkGroupToPoint(Vector3 target)
    {
        Vector3 dir = (target - nv1.transform.position).normalized;
        SetInputAll(new Vector2(dir.x, dir.y), true);

        while (Vector3.Distance(nv1.transform.position, target) > 0.1f)
        {
            MoveAllTowards(target);
            yield return null;
        }

        SetInputAll(Vector2.zero, false);
    }

    IEnumerator WalkSingleToPoint(GameObject obj, Animator anim, Vector3 target)
    {
        Vector3 dir = (target - obj.transform.position).normalized;
        SetInput(anim, new Vector2(dir.x, dir.y), true);

        while (Vector3.Distance(obj.transform.position, target) > 0.1f)
        {
            obj.transform.position = Vector3.MoveTowards(
                obj.transform.position,
                target,
                speed * Time.deltaTime);

            yield return null;
        }

        SetInput(anim, Vector2.zero, false);
    }

    IEnumerator WalkTwoToPoint(GameObject o1, Animator a1,
                               GameObject o2, Animator a2,
                               Vector3 target)
    {
        Vector3 dir = (target - o1.transform.position).normalized;

        SetInput(a1, new Vector2(dir.x, dir.y), true);
        SetInput(a2, new Vector2(dir.x, dir.y), true);

        while (Vector3.Distance(o1.transform.position, target) > 0.1f)
        {
            o1.transform.position = Vector3.MoveTowards(
                o1.transform.position,
                target,
                speed * Time.deltaTime);

            o2.transform.position = Vector3.MoveTowards(
                o2.transform.position,
                target,
                speed * Time.deltaTime);

            yield return null;
        }

        SetInput(a1, Vector2.zero, false);
        SetInput(a2, Vector2.zero, false);
    }

    void MoveAllTowards(Vector3 target)
    {
        nv1.transform.position = Vector3.MoveTowards(nv1.transform.position, target, speed * Time.deltaTime);
        nv2.transform.position = Vector3.MoveTowards(nv2.transform.position, target, speed * Time.deltaTime);
        nv3.transform.position = Vector3.MoveTowards(nv3.transform.position, target, speed * Time.deltaTime);
        nv4.transform.position = Vector3.MoveTowards(nv4.transform.position, target, speed * Time.deltaTime);
    }

    // ================= ANIM INPUT =================

    void SetInputAll(Vector2 dir, bool moving)
    {
        SetInput(a1, dir, moving);
        SetInput(a2, dir, moving);
        SetInput(a3, dir, moving);
        SetInput(a4, dir, moving);
    }

    void SetInput(Animator anim, Vector2 dir, bool moving)
    {
        anim.SetFloat("InputX", dir.x);
        anim.SetFloat("InputY", dir.y);

        if (dir != Vector2.zero)
        {
            anim.SetFloat("LastInputX", dir.x);
            anim.SetFloat("LastInputY", dir.y);
        }

        anim.SetBool("IsMoving", moving);
    }

    void FaceDirection(Animator anim, Vector2 dir)
    {
        anim.SetFloat("InputX", 0);
        anim.SetFloat("InputY", 0);

        anim.SetFloat("LastInputX", dir.x);
        anim.SetFloat("LastInputY", dir.y);

        anim.SetBool("IsMoving", false);
    }

    // ================= DIALOG =================

    IEnumerator ShowLine(string name, string text)
    {
        dialogueCanvas.SetActive(true);

        nameText.text = name;
        contextText.text = text;

        yield return new WaitForSeconds(2f);

        dialogueCanvas.SetActive(false);
    }
}
