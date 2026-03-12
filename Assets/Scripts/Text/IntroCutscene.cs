using System.Collections;
using UnityEngine;

[System.Serializable]
public class DialogueLineData
{
    [TextArea(2, 4)]
    public string text;

    public string speakerName;
    public Sprite avatar;
    public AudioClip voiceClip;
}

public class IntroCutscene : MonoBehaviour
{
    [Header("Voice")]
    public AudioSource voiceAudioSource;

    [Header("Dialogue Sequence")]
    public DialogueLineData[] dialogueSequence;

    void Start()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        for (int i = 0; i < dialogueSequence.Length; i++)
        {
            DialogueLineData line = dialogueSequence[i];
            yield return ShowLine(line);
        }

        Debug.Log("--- Cutscene kết thúc ---");
    }

    IEnumerator ShowLine(DialogueLineData line)
    {
        bool done = false;

        DialogueUI.Instance.ShowDialogue(
            line.text,
            line.speakerName,
            line.avatar,
            () => { done = true; }
        );

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
}