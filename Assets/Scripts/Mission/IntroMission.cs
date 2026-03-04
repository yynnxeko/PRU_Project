using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroMission : MonoBehaviour
{
    public MissionStep[] steps;
    int currentIndex = 0;

    void Start()
    {
        steps[0].StartStep();
    }

    void Update()
    {
        MissionStep step = steps[currentIndex];
        step.UpdateStep();

        if (step.IsCompleted)
        {
            step.EndStep();
            currentIndex++;

            if (currentIndex < steps.Length)
                steps[currentIndex].StartStep();
            else
                Debug.Log("INTRO FINISHED");
        }
    }
}
