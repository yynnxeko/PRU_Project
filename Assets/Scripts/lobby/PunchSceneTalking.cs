using UnityEngine;
using System.Collections;
using System;

public class PunchSceneTalking : MonoBehaviour
{
    [Header("Assets")]
    public Sprite enemyAvatar; // Hình Đại ca
    public Sprite playerAvatar; // Hình Bạn (Lính mới)

    public void PlayWarningDialogue(Action onFinish)
    {
        StartCoroutine(PlayWarningSequence(onFinish));
    }

    public void PlayHospitalDialogue(Action onFinish)
    {
        StartCoroutine(PlayHospitalSequence(onFinish));
    }

    // Kịch bản 1: Đánh xong làm tiếp
    IEnumerator PlayWarningSequence(Action onFinish)
    {
        string boss = "<color=red>Đại ca</color>";
        string me = "Tôi";

        yield return ShowLine("Mày ăn nói kiểu gì đấy? Khách nó nghi ngờ rồi kìa! *Bốp*", boss, enemyAvatar);
        yield return ShowLine("Em xin đại ca! Em cuống quá nên lỡ lời...", me, playerAvatar);
        yield return ShowLine("Lỡ cái con khỉ! Lau máu mồm rồi làm tiếp cho tao! Nhanh!", boss, enemyAvatar);
        yield return ShowLine("Dạ dạ... em làm ngay... không dám sai nữa đâu...", me, playerAvatar);

        onFinish?.Invoke();
    }

    // Kịch bản 2: Đánh nhập viện
    IEnumerator PlayHospitalSequence(Action onFinish)
    {
        string boss = "<color=red>Đại ca</color>";
        string me = "Tôi";

        yield return ShowLine("Thằng ăn hại! Sắp dụ được nó đọc OTP mà mày để nó chửi vào mặt thế à?!", boss, enemyAvatar);
        yield return ShowLine("Anh ơi tha cho em... khách này họ tỉnh quá em không đỡ được!", me, playerAvatar);
        yield return ShowLine("Không đỡ được thì để bác sĩ đỡ mày! Đi cấp cứu luôn đi con!", boss, enemyAvatar);
        yield return ShowLine("Em lạy anh... áaaaa! Đừng đánh vào đầu em... cứu với!", me, playerAvatar);

        onFinish?.Invoke();
    }

    // Hàm phụ trợ gọn nhẹ, tin tưởng hoàn toàn vào DialogueUI
    IEnumerator ShowLine(string text, string name, Sprite avatar)
    {
        bool done = false;

        DialogueUI.Instance.ShowDialogue(text, name, avatar, () => { done = true; });

        while (!done)
        {
            yield return null;
        }
    }
}