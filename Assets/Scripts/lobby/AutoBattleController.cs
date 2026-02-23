using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoBattleController : MonoBehaviour
{
    [Header("Animators")]
    public Animator playerAnimator;
    public Animator nv1Animator;
    public Animator nv2Animator;

    [Header("Timing")]
    public float punchInterval = 1.0f;
    public float dameInterval = 0.5f;

    [Header("Scene Management")]
    public float autoReturnDelay = 10f;

    public PunchSceneTalking dialogueController;

    void Start()
    {
        // Tự động bắt đúng Player teamwork nếu chưa gán
        if (playerAnimator == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerAnimator = playerObj.GetComponent<Animator>();
            }
        }

        nv1Animator.SetFloat("LastInputX", 1);
        nv2Animator.SetFloat("LastInputX", -1);
        if (playerAnimator != null)
            playerAnimator.SetFloat("LastInputY", -1);

        StartCoroutine(NV1PunchLoop());
        StartCoroutine(NV2PunchLoop());
        StartCoroutine(PlayerDameLoop());

        StartCoroutine(AutoReturnToMainScene());
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

            if (playerAnimator != null)
            {
                if (nv1Punch || nv2Punch)
                {
                    playerAnimator.SetBool("IsDame", true);
                    yield return new WaitForSeconds(dameInterval);
                    playerAnimator.SetBool("IsDame", false);
                }
                else
                {
                    playerAnimator.SetBool("IsDame", false); // Đứng dậy khi không bị punch
                }
            }
            yield return null;
        }
    }

    void ResetPlayerAnim()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsDame", false);
            playerAnimator.SetBool("IsSitting", false);
            playerAnimator.SetBool("IsMoving", false);
            // Thêm các trạng thái khác nếu cần
        }
    }

    IEnumerator AutoReturnToMainScene()
    {
        bool isDialogueFinished = false;

        if (dialogueController != null)
        {
            // Truyền callback để báo hiệu khi toàn bộ chuỗi thoại kết thúc
            if (BattleState.failIndex == 10)
                dialogueController.PlayHospitalDialogue(() => { isDialogueFinished = true; });
            else
                dialogueController.PlayWarningDialogue(() => { isDialogueFinished = true; });

            // Đợi cho đến khi thoại chạy đến câu cuối và tự đóng sau 2s (autoCloseDelay)
            yield return new WaitUntil(() => isDialogueFinished);
        }

        ResetPlayerAnim();

        if (BattleState.failIndex == 10)
        {
            DoorSceneChange.NextSpawnId = "medical";
            SceneManager.LoadScene("BossRoom");
        }
        else
        {
            DoorSceneChange.NextSpawnId = "IT";
            SceneManager.LoadScene("IT_Room");
        }
    }
}
