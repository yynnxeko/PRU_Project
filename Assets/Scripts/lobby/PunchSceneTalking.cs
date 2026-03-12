using UnityEngine;
using System.Collections;
using System;

public class PunchSceneTalking : MonoBehaviour
{
    [Header("Assets")]
    public Sprite enemyAvatar;   // Hình Đại ca
    public Sprite playerAvatar;  // Hình Bạn (Lính mới)

    [Header("Voice")]
    public AudioSource voiceAudioSource;

    [Header("Warning Dialogue Clips")]
    public AudioClip warningBossLine01;
    public AudioClip warningPlayerLine01;
    public AudioClip warningBossLine02;
    public AudioClip warningPlayerLine02;

    [Header("Hospital Dialogue Clips")]
    public AudioClip hospitalBossLine01;
    public AudioClip hospitalPlayerLine01;
    public AudioClip hospitalBossLine02;
    public AudioClip hospitalPlayerLine02;

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

        yield return ShowLine(
            "Mày ăn nói kiểu gì đấy? Khách nó nghi ngờ rồi kìa! *Bốp*",
            boss,
            enemyAvatar,
            warningBossLine01
        );

        yield return ShowLine(
            "Em xin đại ca! Em cuống quá nên lỡ lời...",
            me,
            playerAvatar,
            warningPlayerLine01
        );

        yield return ShowLine(
            "Lỡ cái con khỉ! Lau máu mồm rồi làm tiếp cho tao! Nhanh!",
            boss,
            enemyAvatar,
            warningBossLine02
        );

        yield return ShowLine(
            "Dạ dạ... em làm ngay... không dám sai nữa đâu...",
            me,
            playerAvatar,
            warningPlayerLine02
        );

        onFinish?.Invoke();
    }

    // Kịch bản 2: Đánh nhập viện
    IEnumerator PlayHospitalSequence(Action onFinish)
    {
        string boss = "<color=red>Đại ca</color>";
        string me = "Tôi";

        yield return ShowLine(
            "Thằng ăn hại! Sắp dụ được nó đọc OTP mà mày để nó chửi vào mặt thế à?!",
            boss,
            enemyAvatar,
            hospitalBossLine01
        );

        yield return ShowLine(
            "Anh ơi tha cho em... khách này họ tỉnh quá em không đỡ được!",
            me,
            playerAvatar,
            hospitalPlayerLine01
        );

        yield return ShowLine(
            "Không đỡ được thì để bác sĩ đỡ mày! Đi cấp cứu luôn đi con!",
            boss,
            enemyAvatar,
            hospitalBossLine02
        );

        yield return ShowLine(
            "Em lạy anh... áaaaa! Đừng đánh vào đầu em... cứu với!",
            me,
            playerAvatar,
            hospitalPlayerLine02
        );

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
}