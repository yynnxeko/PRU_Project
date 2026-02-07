using System.Collections;
using UnityEngine;

public class AutoBattleController : MonoBehaviour
{
    [Header("Animators")]
    public Animator playerAnimator;
    public Animator nv1Animator;
    public Animator nv2Animator;

    [Header("Timing")]
    public float punchInterval = 1.0f;   // thời gian bật/tắt punch
    public float dameInterval = 0.5f;    // thời gian bật/tắt dame

    void Start()
    {
        // Set hướng cố định
        nv1Animator.SetFloat("LastInputX", 1);
        nv2Animator.SetFloat("LastInputX", -1);
        playerAnimator.SetFloat("LastInputY", -1);

        StartCoroutine(NV1PunchLoop());
        StartCoroutine(NV2PunchLoop());
        StartCoroutine(PlayerDameLoop());
    }

    IEnumerator NV1PunchLoop()
    {
        while (true)
        {
            nv1Animator.SetBool("IsPunch", true);
            yield return new WaitForSeconds(punchInterval);

            nv1Animator.SetBool("IsPunch", false);
            yield return new WaitForSeconds(punchInterval);
        }
    }

    IEnumerator NV2PunchLoop()
    {
        while (true)
        {
            nv2Animator.SetBool("IsPunch", true);
            yield return new WaitForSeconds(punchInterval);

            nv2Animator.SetBool("IsPunch", false);
            yield return new WaitForSeconds(punchInterval);
        }
    }

    IEnumerator PlayerDameLoop()
    {
        while (true)
        {
            bool nv1Punch = nv1Animator.GetBool("IsPunch");
            bool nv2Punch = nv2Animator.GetBool("IsPunch");

            if (nv1Punch || nv2Punch)
            {
                playerAnimator.SetBool("IsDame", true);
                yield return new WaitForSeconds(dameInterval);
                playerAnimator.SetBool("IsDame", false);
            }

            yield return null;
        }
    }
}
