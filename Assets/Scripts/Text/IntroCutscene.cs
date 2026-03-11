using UnityEngine;
using System.Collections;

[System.Serializable]
public class BossDialogueLine
{
    [TextArea(2, 4)]
    public string text;
    public AudioClip voiceClip;
}

public class IntroCutscene : MonoBehaviour
{
    [Header("Assets")]
    public Sprite playerAvatar;
    public Sprite bossAvatar;
    public Sprite guardAvatar;

    [Header("Voice")]
    public AudioSource voiceAudioSource;

    [Header("Boss Dialogues")]
    public BossDialogueLine[] bossLines;

    void Start()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        yield return ShowLine("Có những nơi, pháp luật không tồn tại.", "Tôi", playerAvatar);
        yield return ShowLine("Chỉ có tiền, bạo lực… và sự im lặng.", "Tôi", playerAvatar);

        Debug.Log("--- [SFX] Màn hình nhiễu sóng, âm thanh méo ---");
        yield return new WaitForSeconds(1.0f);

        string bossName = "<color=red>Nhớ Lại Lời Của Boss</color>";

        for (int i = 0; i < bossLines.Length; i++)
        {
            yield return ShowBossLine(bossLines[i], bossName);
            yield return new WaitForSeconds(0.15f);
        }
        yield return new WaitForSeconds(1.0f);
        yield return new WaitUntil(() => GameFlow.BusCutscene == false);

        string guardName = "<color=orange>Lính gác</color>";
        yield return ShowLine("Xuống xe! Xếp hàng!", guardName, guardAvatar);

        Debug.Log("--- [SFX] Màn hình nhiễu mạnh. Tắt cuộc gọi ---");
    }

    IEnumerator ShowBossLine(BossDialogueLine line, string bossName)
    {
        bool done = false;

        DialogueUI.Instance.ShowDialogue(line.text, bossName, bossAvatar, () => { done = true; });

        if (voiceAudioSource != null && line.voiceClip != null)
        {
            voiceAudioSource.Stop();
            voiceAudioSource.clip = line.voiceClip;
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