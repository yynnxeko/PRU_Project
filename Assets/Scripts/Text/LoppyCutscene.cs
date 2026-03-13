using UnityEngine;
using System.Collections;

public class LoppyCutscene : MonoBehaviour
{
    [Header("Settings")]
    public float moveTime = 1.0f;
    public Sprite enemyAvatar;

    [Header("Voice")]
    public AudioSource voiceAudioSource;

    [Header("Voice Clips")]
    public AudioClip lineIntro;
    public AudioClip lineRelax;
    public AudioClip lineEat;
    public AudioClip lineMedical;
    public AudioClip lineWork;
    public AudioClip lineEnd;

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

    private const string PREFS_KEY = "LoppyCutscenePlayed";
    private bool hasPlayed => PlayerPrefs.GetInt(PREFS_KEY, 0) == 1;
    public static bool isPlaying = false;

    void Start()
    {
        if (!hasPlayed)
        {
            isPlaying = true;
        }

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

        if (npcPath != null) npcPath.Pause();

        if (followers != null)
        {
            foreach (var f in followers)
            {
                if (f != null) f.Pause();
            }
        }

        string boss = "<color=red>Bảo vệ</color>";

        yield return ShowLine("Có 4 khu vực chúng mày buộc phải thuộc lòng.", boss, lineIntro);

        yield return MoveAndTalk(posRelax, "Khu SINH HOẠT: Ngủ và im lặng.", boss, lineRelax);
        yield return MoveAndTalk(posEat, "NHÀ BẾP: Đớp nhanh rồi biến, đừng có lề mề.", boss, lineEat);
        yield return MoveAndTalk(posMedical, "Khu Y TẾ: Nơi dành cho mấy thằng sắp chết.", boss, lineMedical);
        yield return MoveAndTalk(posWork, "PHÒNG LÀM VIỆC: Đến giờ thì vác xác vào mà cày!", boss, lineWork);

        yield return MoveCamera(cam.transform.position, initialCamPos);
        yield return ShowLine("Nghe đủ chưa? Giờ thì biến vào phòng tập thể đi!", boss, lineEnd);

        if (camFollow != null) camFollow.enabled = true;

        if (npcPath != null)
        {
            npcPath.ResetPath();
            npcPath.Resume();
        }

        if (followers != null)
        {
            foreach (var f in followers)
            {
                if (f != null) f.Resume();
            }
        }

        if (GameFlagManager.Instance != null)
        {
            GameFlagManager.Instance.SetFlag("lobby_To_Bed", true);
            Debug.Log("[LoppyCutscene] Flag 'lobby_To_Bed' set to TRUE");
        }

        PlayerPrefs.SetInt(PREFS_KEY, 1);
        PlayerPrefs.Save();
        isPlaying = false;
    }

    IEnumerator MoveAndTalk(Transform target, string text, string name, AudioClip voiceClip)
    {
        if (target == null) yield break;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, initialCamPos.z);
        yield return MoveCamera(cam.transform.position, targetPos);
        yield return ShowLine(text, name, voiceClip);
    }

    IEnumerator MoveCamera(Vector3 from, Vector3 to)
    {
        float t = 0f;

        while (t < moveTime)
        {
            cam.transform.position = Vector3.Lerp(from, to, t / moveTime);
            t += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = to;
    }

    IEnumerator ShowLine(string text, string name, AudioClip voiceClip = null)
    {
        bool done = false;

        DialogueUI.Instance.ShowDialogue(text, name, enemyAvatar, () => { done = true; });

        if (voiceAudioSource != null && voiceClip != null)
        {
            voiceAudioSource.Stop();
            voiceAudioSource.clip = voiceClip;
            voiceAudioSource.Play();
        }

        while (!done)
        {
            yield return null;
        }

        if (voiceAudioSource != null && voiceAudioSource.isPlaying)
        {
            voiceAudioSource.Stop();
        }
    }
}