using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowNPCStep : MonoBehaviour
{
    public Transform[] pathPoints;
    int index = 0;
    public float speed = 2f;

    void Update()
    {
        if (index >= pathPoints.Length) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            pathPoints[index].position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, pathPoints[index].position) < 0.1f)
            index++;
    }

    public bool FinishedPath()
    {
        return index >= pathPoints.Length;
    }
}
