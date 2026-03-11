using UnityEngine;
using System.Collections;
using System;

public class CanteenSceneTalking : MonoBehaviour
{
    private bool playerInZone = false;
    private const string PREFS_KEY = "CanteenDialoguePlayed";

    [Header("Assets")]
    public Sprite npcAvatar;    // Hình NPC trong canteen
    public Sprite playerAvatar; // Hình người chơi

    [Header("Flag Setting")]
    [Tooltip("Cờ sẽ bật TRUE khi thoại xong")]
    public string flagOnComplete;

    private bool hasTriggered = false; // Tránh trigger nhiều lần trong 1 session
    private bool dialoguePlayed = false; // Đã từng chạy thoại này trong mọi session

    // Player đi vào vùng trigger → tự động chạy thoại
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

        // Luôn cho phép hiện mô tả nhiệm vụ khi player trong vùng và bấm E
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

    /// <summary>
    /// Gọi hội thoại canteen. Chỉ chạy 1 lần duy nhất (lưu PlayerPrefs).
    /// Nếu đã chạy rồi thì gọi onFinish ngay.
    /// </summary>
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

        // 1. Gặp mặt: Nhờ vả việc vặt để che mắt
        yield return ShowLine("Này chú em, đứng ngây ra đó làm gì? Lại đây bê hộ tôi mấy cái khay inox này vào trong đi!", npc, npcAvatar);
        yield return ShowLine("Dạ... dạ tôi tới ngay.", me, playerAvatar);

        // 2. Thăm dò bằng ám hiệu (Chiếc mũ cao bồi)
        yield return ShowLine("Lính mới hả? Nhìn mặt lạ hoắc. Mà này... ở ngoài kia chú có quen ai hay đội mũ cao bồi không?", npc, npcAvatar);
        yield return ShowLine("Hả? Mũ cao bồi? Tôi... tôi không biết ông đang nói gì cả. Tôi chỉ tới đây làm việc thôi.", me, playerAvatar);

        // 3. NPC xác nhận danh tính
        yield return ShowLine("Khéo lo! Tôi nhận được thông tin về chú rồi. Đều là người một nhà cả nên cứ yên tâm.", npc, npcAvatar);
        yield return ShowLine("Anh... anh cũng là người của chúng ta sao?", me, playerAvatar);

        // 4. Thiết lập điểm liên lạc
        yield return ShowLine("Suỵt! Khẽ thôi. Sau này cứ canh giờ ăn chiều, chạy ra đây phụ tôi, tôi sẽ bàn giao nhiệm vụ tiếp theo cho.", npc, npcAvatar);
        yield return ShowLine("Rõ! Vậy bây giờ tôi cần phải làm gì trước tiên?", me, playerAvatar);

        // 5. Giao nhiệm vụ đầu tiên: Chìa khóa phòng IT
        yield return ShowLine("Trong này quản chặt lắm, chỉ có ban đêm mới dễ hành động. Tôi có tin là chìa khóa phòng ngủ được giấu trong tủ của phòng IT.", npc, npcAvatar);
        yield return ShowLine("Ráng mà canh lúc làm việc hoặc lúc sơ hở mà lấy cho bằng được tài liệu trong tủ khóa, nhớ tìm chìa khóa trong phòng IT.", npc, npcAvatar);


        // 6. Kết thúc hội thoại
        yield return ShowLine("Thôi, lấy phần cơm rồi về phòng ngủ đi, đừng ở đây lâu bọn nó nghi. Cẩn thận đấy!", npc, npcAvatar);

        // Đánh dấu đã chạy xong, lưu vào đĩa
        PlayerPrefs.SetInt(PREFS_KEY, 1);
        PlayerPrefs.Save();
        dialoguePlayed = true; // Cho phép bấm E ngay sau khi thoại xong

        // Bật cờ khi hoàn thành
        if (!string.IsNullOrEmpty(flagOnComplete) && GameFlagManager.Instance != null)
        {
            GameFlagManager.Instance.SetFlag(flagOnComplete, true);
            Debug.Log($"[CanteenSceneTalking] Flag '{flagOnComplete}' set to TRUE");
        }

        onFinish?.Invoke();
    }

    // Hàm phụ trợ giống PunchSceneTalking
    IEnumerator ShowLine(string text, string name, Sprite avatar)
    {
        bool done = false;
        DialogueUI.Instance.ShowDialogue(text, name, avatar, () => { done = true; });

        while (!done)
        {
            yield return null;
        }
    }

    // === Dùng trong Editor để reset (test lại) ===
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
