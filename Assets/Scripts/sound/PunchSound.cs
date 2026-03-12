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

    bool HasParameter(string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    void Update()
    {
        if (animator == null) return;

        // nếu không có IsPunch thì bỏ qua
        if (!HasParameter("IsPunch")) return;

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