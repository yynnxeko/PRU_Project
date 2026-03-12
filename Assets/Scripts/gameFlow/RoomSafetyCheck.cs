using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomSafetyCheck : MonoBehaviour
{
    [Header("Safety Settings")]
    public List<DayPhase> allowedPhases = new List<DayPhase>();
    public bool allowFlag = false; // Tích vào nếu muốn kiểm tra flag thay vì buổi
    public string requiredFlag; // Tên flag cần có (vd: "has_hospital_pass")
    public string roomName = "Unknown Room";

    [Header("Feedback Settings")]
    public SpeechBubble bubblePrefab;
    public string catchMessage = "AI ĐÓ? ĐỨNG LẠI MAU!";
    public AudioClip catchVoice;
    public float waitBeforeReset = 2f;
    public string resetSpawnId = "Bed";

    private bool isPlayerInside = false;
    private bool isResetting = false;

    private void Update()
    {
        if (isPlayerInside && !isResetting)
        {
            CheckSafety();
        }
    }

    private void CheckSafety()
    {
        if (DayManager.Instance == null || isResetting) return;

        // Bypass: nếu go_to_medical đang TRUE → luôn cho vào (Mission 2 gửi player đến Hospital)
        if (GameFlagManager.Instance != null && GameFlagManager.Instance.GetFlag("go_to_medical"))
        {
            return;
        }

        bool isSafe = false;

        if (allowFlag)
        {
            // Kiểm tra theo Flag
            if (GameFlagManager.Instance != null)
            {
                isSafe = GameFlagManager.Instance.GetFlag(requiredFlag);
            }
            else
            {
                Debug.LogWarning("RoomSafetyCheck: GameFlagManager instance not found!");
            }
        }
        else
        {
            // Kiểm tra theo Buổi (DayPhase) như cũ
            isSafe = allowedPhases.Contains(DayManager.Instance.currentPhase);
        }

        if (!isSafe)
        {
            StartCoroutine(CatchSequence());
        }
    }

    IEnumerator CatchSequence()
    {
        isResetting = true;
        Debug.LogWarning($"Bị bảo vệ bắt! Bạn không được phép ở {roomName} vào buổi {DayManager.Instance.currentPhase}");

        // 1. Khóa di chuyển người chơi
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            var controller = playerObj.GetComponent<PlayerController2>();
            if (controller != null) controller.canMove = false;
        }

        // 2. Tìm bảo vệ gần nhất
        EnemyAi nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null && playerObj != null)
        {
            // 3. Yêu cầu bảo vệ đi tới chỗ người chơi
            nearestEnemy.GoToLocation(playerObj.transform.position);
            
            // Chờ bảo vệ đi lại gần (cách khoảng 1.5m)
            float dist = Vector3.Distance(nearestEnemy.transform.position, playerObj.transform.position);
            while (dist > 1.5f)
            {
                dist = Vector3.Distance(nearestEnemy.transform.position, playerObj.transform.position);
                yield return null;
            }

            // 4. Diễn Shock + Talk
            if (nearestEnemy.anim != null) 
            {
                nearestEnemy.anim.SetTrigger("shocking"); 
            }

            if (playerObj != null)
            {
                Animator playerAnim = playerObj.GetComponent<Animator>();
                if (playerAnim != null) playerAnim.SetBool("IsShocked", true);
            }

            if (bubblePrefab != null)
            {
                SpeechBubble bubble = Instantiate(bubblePrefab, nearestEnemy.transform.position + Vector3.up * 1.5f, Quaternion.identity);
                bubble.Init(nearestEnemy.transform, Vector3.up * 1.5f);
                bubble.Show(catchMessage, 3f, catchVoice); // Hiện bóng thoại 3s
            }
        }

        // 5. Chờ cho guard nói xong và animation diễn ra
        yield return new WaitForSeconds(4f); 

        // Reset Player Animation trước khi chuyển cảnh
        if (playerObj != null)
        {
            Animator playerAnim = playerObj.GetComponent<Animator>();
            if (playerAnim != null) playerAnim.SetBool("IsShocked", false);
        }

        // Chờ thêm 1 chút để Player kịp thoát thế Shock trước khi màn hình đen/load
        yield return new WaitForSeconds(0.5f);

        // Reset Ngày
        DayManager.Instance.FailDay(resetSpawnId);
        isResetting = false;
    }

    private EnemyAi FindNearestEnemy()
    {
        EnemyAi[] enemies = FindObjectsOfType<EnemyAi>();
        EnemyAi nearest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (EnemyAi e in enemies)
        {
            float dist = Vector3.Distance(e.transform.position, currentPos);
            if (dist < minDist)
            {
                nearest = e;
                minDist = dist;
            }
        }
        return nearest;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            CheckSafety(); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}
