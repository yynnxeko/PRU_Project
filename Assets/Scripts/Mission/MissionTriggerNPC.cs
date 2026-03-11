using UnityEngine;

public class MissionTriggerNPC : MonoBehaviour
{
    public MissionManager mission;

    void Start()
    {
        mission.StartMission();
    }
}
