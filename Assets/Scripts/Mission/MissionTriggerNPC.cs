using UnityEngine;

public class MissionTriggerNPC : MonoBehaviour
{
    public MissionController mission;

    void Start()
    {
        mission.StartMission();
    }
}
