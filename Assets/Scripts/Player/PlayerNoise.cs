using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNoise : MonoBehaviour
{
    public float walkNoiseRadius = 3f;
    public float slowNoiseRadius = 1.5f;

    public bool isMoving;
    public bool isSlowWalking;

    void Update()
    {
        isSlowWalking = Input.GetKey(KeyCode.LeftShift);
    }

    public float GetNoiseRadius()
    {
        if (!isMoving) return 0f;
        return isSlowWalking ? slowNoiseRadius : walkNoiseRadius;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        float radius = GetNoiseRadius();
        if (radius > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
