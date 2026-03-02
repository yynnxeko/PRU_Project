using UnityEngine;
using System.Collections;

public class DailyDialogueTrigger : MonoBehaviour
{
    public DailyDialogueData dialogueData;
    public SpeechBubble bubblePrefab;
    public Vector3 bubbleOffset = new Vector3(0, 1.5f, 0);

    private void Start()
    {
        TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        if (DayManager.Instance == null || dialogueData == null || bubblePrefab == null)
        {
            Debug.LogWarning("DailyDialogueTrigger: Missing references or DayManager instance.");
            return;
        }

        int day = DayManager.Instance.currentDay;
        DayPhase phase = DayManager.Instance.currentPhase;

        DailyDialogueData.DayDialogue dialogue = dialogueData.GetDialogue(day, phase);
        if (dialogue != null && dialogue.lines.Length > 0)
        {
            StartCoroutine(ShowDialogueRoutine(dialogue));
        }
    }

    private IEnumerator ShowDialogueRoutine(DailyDialogueData.DayDialogue dialogue)
    {
        // Chờ 1 chút để Player kịp spawn hoàn tất
        yield return new WaitForSeconds(0.5f);

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("DailyDialogueTrigger: Cannot find Player with tag 'Player'");
            yield break;
        }

        foreach (string line in dialogue.lines)
        {
            SpeechBubble bubble = Instantiate(bubblePrefab, player.transform.position + bubbleOffset, Quaternion.identity);
            bubble.Init(player.transform, bubbleOffset);
            bubble.Show(line, dialogue.durationPerLine);
            
            // Chờ cho đến khi bong bóng biến mất trước khi hiện câu tiếp theo
            yield return new WaitForSeconds(dialogue.durationPerLine + 0.2f);
        }

        // --- MỚI: Sau khi thoại xong, có thể gọi NPC hành động ---
        NarrativeNpcController narrativeNpc = FindFirstObjectByType<NarrativeNpcController>();
        if (narrativeNpc != null)
        {
            narrativeNpc.StartNarrativeSequence();
        }
    }
}
