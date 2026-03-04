using UnityEngine;

public class MissionController : MonoBehaviour
{
    public bool missionActive;
    public bool missionCompleted;

    public void StartMission()
    {
        missionActive = true;
        missionCompleted = false;
        Debug.Log("Mission started: Follow NPC");
    }

    public void CompleteMission()
    {
        missionCompleted = true;
        missionActive = false;
        Debug.Log("Mission completed!");
    }
}
