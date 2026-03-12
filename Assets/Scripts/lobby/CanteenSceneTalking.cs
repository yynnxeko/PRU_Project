using UnityEngine;
using System.Collections;
using System;

public class CanteenSceneTalking : MonoBehaviour
{
    private bool playerInZone = false;
    private const string PREFS_KEY = "CanteenDialoguePlayed";

    [Header("Assets")]
    public Sprite npcAvatar;
    public Sprite playerAvatar;

    [Header("Voice")]
    public AudioSource voiceAudioSource;

    [Header("Dialogue Voice Clips")]
    public AudioClip npcLine01;
    public AudioClip playerLine01;
    public AudioClip npcLine02;
    public AudioClip playerLine02;
    public AudioClip npcLine03;
    public AudioClip playerLine03;
    public AudioClip npcLine04;
    public AudioClip playerLine04;
    public AudioClip npcLine05;
    public AudioClip npcLine06;
    public AudioClip npcLine07;

    [Header("Flag Setting")]
    [Tooltip("Cờ sẽ bật TRUE khi thoại xong")]
    public string flagOnComplete;

    private bool hasTriggered = false;
    private bool dialoguePlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = true;

        if (hasTriggered) return;

        if (PlayerPrefs.GetInt(PREFS_KEY, 0) == 1)
        {
            dialoguePlayed = true;
            return;
        }

        hasTriggered = true;
        PlayCanteenDialogue(null);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInZone = false;
    }

    void Update()
    {
        Debug.Log($"Update: dialoguePlayed={dialoguePlayed}, playerInZone={playerInZone}");

        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Bấm E: Hiện nhiệm vụ");

            string msg = "Không có nhiệm vụ hiện tại.";
            if (FullMissionManager.Instance != null)
            {
                msg = FullMissionManager.Instance.GetActiveMissionDescription();
            }

            DialogueUI.Instance.ShowDialogue(msg, "Nhiệm vụ", null, null);
        }
    }

    public void PlayCanteenDialogue(Action onFinish)
    {
        if (PlayerPrefs.GetInt(PREFS_KEY, 0) == 1)
        {
            dialoguePlayed = true;
            Debug.Log("[CanteenSceneTalking] Đã chạy rồi, bỏ qua.");
            onFinish?.Invoke();
            return;
        }

        StartCoroutine(PlayCanteenSequence(onFinish));
    }

    IEnumerator PlayCanteenSequence(Action onFinish)
    {
        string npc = "<color=#FFA500>Đầu bếp</color>";
        string me = "Tôi";

        yield return ShowLine(
            "Này chú em, đứng ngây ra đó làm gì? Lại đây bê hộ tôi mấy cái khay inox này vào trong đi!",
            npc,
            npcAvatar,
            npcLine01
        );

        yield return ShowLine(
            "Dạ... dạ tôi tới ngay.",
            me,
            playerAvatar,
            playerLine01
        );

        yield return ShowLine(
            "Lính mới hả? Nhìn mặt lạ hoắc. Mà này... ở ngoài kia chú có quen ai hay đội mũ cao bồi không?",
            npc,
            npcAvatar,
            npcLine02
        );

        yield return ShowLine(
            "Hả? Mũ cao bồi? Tôi... tôi không biết ông đang nói gì cả. Tôi chỉ tới đây làm việc thôi.",
            me,
            playerAvatar,
            playerLine02
        );

        yield return ShowLine(
            "Khéo lo! Tôi nhận được thông tin về chú rồi. Đều là người một nhà cả nên cứ yên tâm.",
            npc,
            npcAvatar,
            npcLine03
        );

        yield return ShowLine(
            "Anh... anh cũng là người của chúng ta sao?",
            me,
            playerAvatar,
            playerLine03
        );

        yield return ShowLine(
            "Suỵt! Khẽ thôi. Sau này cứ canh giờ ăn chiều, chạy ra đây phụ tôi, tôi sẽ bàn giao nhiệm vụ tiếp theo cho.",
            npc,
            npcAvatar,
            npcLine04
        );

        yield return ShowLine(
            "Rõ! Vậy bây giờ tôi cần phải làm gì trước tiên?",
            me,
            playerAvatar,
            playerLine04
        );

        yield return ShowLine(
            "Trong này quản chặt lắm, chỉ có ban đêm mới dễ hành động. Tôi có tin là chìa khóa phòng ngủ được giấu trong tủ của phòng IT.",
            npc,
            npcAvatar,
            npcLine05
        );

        yield return ShowLine(
            "Ráng mà canh lúc làm việc hoặc lúc sơ hở mà lấy cho bằng được tài liệu trong tủ khóa, nhớ tìm chìa khóa trong phòng IT.",
            npc,
            npcAvatar,
            npcLine06
        );

        yield return ShowLine(
            "Thôi, lấy phần cơm rồi về phòng ngủ đi, đừng ở đây lâu bọn nó nghi. Cẩn thận đấy!",
            npc,
            npcAvatar,
            npcLine07
        );

        PlayerPrefs.SetInt(PREFS_KEY, 1);
        PlayerPrefs.Save();
        dialoguePlayed = true;

        if (!string.IsNullOrEmpty(flagOnComplete) && GameFlagManager.Instance != null)
        {
            GameFlagManager.Instance.SetFlag(flagOnComplete, true);
            Debug.Log($"[CanteenSceneTalking] Flag '{flagOnComplete}' set to TRUE");
        }

        onFinish?.Invoke();
    }

    IEnumerator ShowLine(string text, string name, Sprite avatar, AudioClip voiceClip = null)
    {
        bool done = false;

        DialogueUI.Instance.ShowDialogue(text, name, avatar, () => { done = true; });

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

#if UNITY_EDITOR
    [ContextMenu("Reset Canteen Dialogue")]
    private void ResetDialogue()
    {
        PlayerPrefs.DeleteKey(PREFS_KEY);
        PlayerPrefs.Save();
        Debug.Log("[CanteenSceneTalking] Đã reset, lần Play sau sẽ chạy lại.");
    }
#endif
}