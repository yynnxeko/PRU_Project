using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHearing : MonoBehaviour
{
    public float hearingRadius = 3f;

    void Update()
    {
        PlayerNoise player = FindObjectOfType<PlayerNoise>();
        if (player == null) return;

        float noiseRadius = player.GetNoiseRadius();
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (noiseRadius > 0 && distance <= noiseRadius && distance <= hearingRadius)
        {
            Debug.Log("Enemy heard noise!");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}
