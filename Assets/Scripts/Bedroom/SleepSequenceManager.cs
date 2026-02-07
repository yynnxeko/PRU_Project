using System.Collections;
using UnityEngine;

public class SleepSequenceManager : MonoBehaviour
{
    [Header("Player")]
    public Animator playerAnim;

    [Header("Enemy")]
    public GameObject enemy1;
    public Transform enemySp1;

    [Header("NPC")]
    public GameObject npc1;
    public GameObject npc2;
    public GameObject npc3;
    public GameObject npc4;

    public Transform npc1SleepPoint;
    public Transform npc2SleepPoint;
    public Transform npc3SleepPoint;
    public Transform npc4SleepPoint;

    public Transform npc2Sp1;   // sp1 riêng của npc2

    bool started = false;

    void Update()
    {
        if (playerAnim == null) return;

        if (!started && playerAnim.GetBool("IsSleep"))
        {
            started = true;
            StartCoroutine(SleepSequence());
        }
    }

    IEnumerator SleepSequence()
    {
        // ⏱ Player ngủ 5s
        yield return new WaitForSeconds(5f);

        // Enemy đi sp1
        Move(enemy1, enemySp1);

        // ⏱ chờ 5s
        yield return new WaitForSeconds(5f);

        // NPC 1 3 4 đi ngủ
        Move(npc1, npc1SleepPoint);
        Move(npc3, npc3SleepPoint);
        Move(npc4, npc4SleepPoint);

        // NPC2 đi sp1 riêng
        Move(npc2, npc2Sp1);

        // ⏱ đợi tới sp1
        yield return new WaitForSeconds(3f);

        // NPC2 về giường
        Move(npc2, npc2SleepPoint);

        // ⏱ chờ tới giường
        yield return new WaitForSeconds(2f);

        // 😴 tất cả ngủ
        SetSleep(npc1);
        SetSleep(npc2);
        SetSleep(npc3);
        SetSleep(npc4);
    }

    void Move(GameObject character, Transform target)
    {
        if (character == null || target == null)
        {
            Debug.LogError("Move missing object!");
            return;
        }

        NPCMoveToTarget mover = character.GetComponent<NPCMoveToTarget>();
        Animator anim = character.GetComponent<Animator>();

        if (mover == null)
        {
            Debug.LogError(character.name + " missing NPCMoveToTarget!");
            return;
        }

        if (anim == null)
        {
            Debug.LogError(character.name + " missing Animator!");
            return;
        }

        mover.MoveTo(target);
        anim.SetBool("IsMoving", true);
    }

    void SetSleep(GameObject character)
    {
        Animator anim = character.GetComponent<Animator>();
        if (anim != null)
            anim.SetBool("IsSleep", true);
    }
}
