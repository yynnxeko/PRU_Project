using UnityEngine;
using System.Collections;

public class IntroCutscene : MonoBehaviour
{
    [Header("Assets")]
    public Sprite playerAvatar; // Kéo hình người chơi vào
    public Sprite bossAvatar;   // Kéo hình "Cấp trên" (hoặc để trống nếu là giọng nói bí ẩn)
    public Sprite guardAvatar;


    void Start()
    {
        // Bắt đầu cutscene ngay khi game chạy (hoặc gọi hàm này khi cần)
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // --- PHẦN 1: TỰ THOẠI NGƯỜI CHƠI ---
        yield return ShowLine("Có những nơi, pháp luật không tồn tại.", "Tôi", playerAvatar);
        yield return ShowLine("Chỉ có tiền, bạo lực… và sự im lặng.", "Tôi", playerAvatar);

        // --- HIỆU ỨNG NHIỄU SÓNG (Giả lập) ---
        // Ở đây sau này bạn có thể chèn code GlitchEffect.Enable()
        Debug.Log("--- [SFX] Màn hình nhiễu sóng, âm thanh méo ---");
        yield return new WaitForSeconds(1.0f);

        // --- PHẦN 2: CẤP TRÊN GỌI ---
        // Có thể đổi màu chữ hoặc font cho Cấp trên để tạo cảm giác "mã hóa"
        string bossName = "<color=red>UNKNOWN_ID</color>";

        yield return ShowLine("Từ thời điểm này, cậu không còn là người của chúng tôi.", bossName, bossAvatar);
        yield return ShowLine("Nếu bị lộ… chúng tôi sẽ phủ nhận toàn bộ.", bossName, bossAvatar);

        yield return new WaitForSeconds(1.0f); // (Ngắt một nhịp - như kịch bản)

        yield return ShowLine("Mục tiêu: thâm nhập.", bossName, bossAvatar);
        yield return ShowLine("Sống sót.", bossName, bossAvatar);
        yield return ShowLine("Và ghi nhớ mọi thứ.", bossName, bossAvatar);


        // chưa có điều kiện là khi xe buýt dừng thì mới nói
        string guardName = "<color=orange>Lính gác</color>";
        yield return ShowLine("Xuống xe! Xếp hàng!", guardName, guardAvatar);
        // --- KẾT THÚC ---
        Debug.Log("--- [SFX] Màn hình nhiễu mạnh. Tắt cuộc gọi ---");
    }

    // Hàm phụ trợ để code gọn hơn: Chờ hội thoại đóng mới chạy tiếp
    IEnumerator ShowLine(string text, string name, Sprite avatar)
    {
        bool done = false;

        // Gọi DialogueUI và truyền vào một hành động: khi xong thì đổi biến done = true
        DialogueUI.Instance.ShowDialogue(text, name, avatar, () => { done = true; });

        // Chờ cho đến khi DialogueUI báo là xong (người chơi bấm skip hoặc tự hết giờ)
        while (!done)
        {
            yield return null;
        }
    }
}
