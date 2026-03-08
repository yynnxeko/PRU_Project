using UnityEngine;

public class ShockingSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip shockingSound;

    Animator animator;
    bool played = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator == null) return;

        bool isShocking = animator.GetBool("IsShocking");

        if (isShocking && !played)
        {
            audioSource.PlayOneShot(shockingSound);
            played = true;
        }

        if (!isShocking)
        {
            played = false;
        }
    }
}