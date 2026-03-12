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

        // === DEBUG: Kiểm tra trạng thái flag ngay khi vào scene ===
        bool goMedicalInit = GameFlagManager.Instance != null
            && GameFlagManager.Instance.GetFlag("go_to_medical");
        int missionIdx = FullMissionManager.Instance != null 
            ? FullMissionManager.Instance.GetCurrentMissionIndex() : -1;
        Debug.Log($"<color=yellow>[AutoBattle] === VÀO SCENE Map_Lobby_punch ===</color>");
        Debug.Log($"<color=yellow>[AutoBattle] go_to_medical = {goMedicalInit}</color>");
        Debug.Log($"<color=yellow>[AutoBattle] currentMissionIndex = {missionIdx}</color>");
        Debug.Log($"<color=yellow>[AutoBattle] dialogueController = {(dialogueController != null ? "CÓ" : "NULL")}</color>");

        if (dialogueController != null)
        {
            // Nếu có flag nhận nhiệm vụ → chạy kịch bản đánh nhập viện
            bool goMedical = GameFlagManager.Instance != null
                && GameFlagManager.Instance.GetFlag("go_to_medical");

            Debug.Log($"<color=cyan>[AutoBattle] goMedical cho dialogue = {goMedical}</color>");

            if (goMedical)
                dialogueController.PlayHospitalDialogue(() => { isDialogueFinished = true; });
            else
                dialogueController.PlayWarningDialogue(() => { isDialogueFinished = true; });

            yield return new WaitUntil(() => isDialogueFinished);
        }

        ResetPlayerAnim();

        bool goMedical2 = GameFlagManager.Instance != null
            && GameFlagManager.Instance.GetFlag("go_to_medical");

        Debug.Log($"<color=cyan>[AutoBattle] goMedical2 (quyết định scene) = {goMedical2}</color>");

        if (goMedical2)
        {
            // KHÔNG reset go_to_medical ở đây!
            // Để RoomSafetyCheck ở Hospital dùng bypassFlag kiểm tra.
            // Flag sẽ được tắt bởi Mission2_HospitalComplete sau khi tìm USB.
            DoorSceneChange.NextSpawnId = "medical";
            Debug.Log("<color=green>[AutoBattle] → LOADING HOSPITAL!</color>");
            SceneManager.LoadScene("Hospital");
        }
        else
        {
            DoorSceneChange.NextSpawnId = "IT";
            Debug.Log("<color=red>[AutoBattle] → LOADING IT_Room (KHÔNG đi Hospital)</color>");
            SceneManager.LoadScene("IT_Room");
        }
    }
}
