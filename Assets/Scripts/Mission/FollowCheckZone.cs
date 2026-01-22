using UnityEngine;

public class FollowCheckZone : MonoBehaviour
{
    public bool PlayerInside { get; private set; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInside = true;
            Debug.Log("Player inside follow zone");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInside = false;
            Debug.Log("Player left follow zone");
        }
    }
}
