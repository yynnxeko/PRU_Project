using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablePlayerStep : MissionStep
{
    public PlayerController player;

    public override void StartStep()
    {
        base.StartStep();
        player.enabled = true;
        CompleteStep();
    }
}
