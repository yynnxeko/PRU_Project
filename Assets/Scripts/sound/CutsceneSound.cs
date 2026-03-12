using UnityEngine;

public class ShockingSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip shockingSound;

    Animator animator;
    bool played;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!animator) return;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Base Layer.shocking"))
        {
            if (!played)
            {
                audioSource.PlayOneShot(shockingSound);
                played = true;
            }
        }
        else
        {
            played = false;
        }
    }
}