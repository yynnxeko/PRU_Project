using UnityEngine;

public class PunchSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip punchSound;

    Animator animator;
    bool played = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator == null) return;

        bool isPunch = animator.GetBool("IsPunch");

        if (isPunch && !played)
        {
            audioSource.PlayOneShot(punchSound);
            played = true;
        }

        if (!isPunch)
        {
            played = false;
        }
    }
}