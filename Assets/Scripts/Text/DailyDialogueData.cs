using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DailyDialogueData", menuName = "Dialogues/Daily Dialogue Data")]
public class DailyDialogueData : ScriptableObject
{
    [System.Serializable]
    public class DayDialogue
    {
        public int day;
        public DayPhase phase = DayPhase.Morning;
        [TextArea(3, 10)]
        public string[] lines;
        public float durationPerLine = 9f;
    }

    public List<DayDialogue> dailyDialogues = new List<DayDialogue>();

    public DayDialogue GetDialogue(int day, DayPhase phase)
    {
        return dailyDialogues.Find(d => d.day == day && d.phase == phase);
    }
}
