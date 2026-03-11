using UnityEngine;
using System.Collections;
using UnityEngine.Networking; // Để tải file mp3 từ Google TTS

public class IntroCutscene : MonoBehaviour
{
    [Header("Assets")]
    public Sprite playerAvatar; // Kéo hình người chơi vào
    public Sprite bossAvatar;   // Kéo hình "Cấp trên" (hoặc để trống nếu là giọng nói bí ẩn)
    public Sprite guardAvatar;

    public AudioSource ttsAudioSource; // Kéo AudioSource vào Inspector


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
        string bossName = "<color=red>Nhớ lại lời của Boss</color>";

        yield return ShowLine("Từ thời điểm này, cậu không còn là người của chúng tôi.", bossName, bossAvatar);

        yield return new WaitForSeconds(1.0f); // (Ngắt một nhịp - như kịch bản)

        // Coroutine tải và phát thoại từ Google TTS
        IEnumerator PlayGoogleTTS(string text)
        {
            // Tạo link Google TTS
            string url = "https://translate.google.com/translate_tts?ie=UTF-8&q=" + UnityWebRequest.EscapeURL(text) + "&tl=vi&client=tw-ob";
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    if (ttsAudioSource != null && clip != null)
                    {
                        ttsAudioSource.clip = clip;
                        ttsAudioSource.Play();
                        // Chờ phát xong
                        yield return new WaitForSeconds(clip.length);
                    }
                }
                else
                {
                    Debug.LogWarning("TTS download failed: " + www.error);
                }
            }
        }

        yield return ShowLine("Mục tiêu: thâm nhập và ghi nhớ mọi thứ.", bossName, bossAvatar);


        // Chờ xe buýt dừng hẳn
        yield return new WaitUntil(() => GameFlow.BusCutscene == false);

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
