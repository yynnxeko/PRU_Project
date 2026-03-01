using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BedroomInitialCutscene : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public float camMoveTime = 1.0f;
    public string enemyName = "Giám thị";
    public Sprite enemyAvatar;
    [TextArea]
    public string dialogueText = "Ngủ đi mai còn làm việc!";
    
    [Header("Bed Instruction (Speech Bubble)")]
    public SpeechBubble bubblePrefab;
    [TextArea]
    public string bedInstructionText = "Bấm E để ngủ";
    public float bubbleOffsetY = 1.5f;
    public float bubbleDuration = 3f;

    [Header("Characters")]
    public GameObject enemy;
    [Tooltip("Vị trí Enemy sẽ đi tới sau khi nói xong để biến mất.")]
    public Transform enemyLeavePoint;
    public float enemyMoveSpeed = 2f;
    private GameObject player; // Tìm tự động theo tag
    
    [Header("NPC Moving & Sleeping")]
    [Tooltip("Danh sách các NPC sẽ tự động đi ngủ.")]
    public GameObject[] npcs;
    [Tooltip("Danh sách giường tương ứng của từng NPC.")]
    public Transform[] npcSleepPoints;
    public float npcMoveSpeed = 2f;

    [Header("Player Bed Interaction")]
    [Tooltip("Vị trí giường của Player để camera lia tới.")]
    public Transform playerBedPoint;
    public float interactDistance = 1.5f;

    [Header("Scene Transition")]
    public string nextSceneName = "Scene A";
    public string nextSceneSpawnId = "SpawnFromBedroom";
    public UnityEngine.UI.Image fadeImage;

    private static bool hasPlayed = false;
    private Camera cam;
    private CameraFollowPersist camFollow;
    private Vector3 initialCamPos;
    private bool isCutscenePlaying = false;
    private bool canSleep = false;

    private void Start()
    {
        // 1. Tự động tìm Player bằng Tag
        player = GameObject.FindGameObjectWithTag("Player");

        cam = Camera.main;
        if (cam != null)
        {
            camFollow = cam.GetComponent<CameraFollowPersist>();
            initialCamPos = cam.transform.position;
        }

        // Tắt tính năng tự đi dạo của NPC trong phòng ngủ (nếu có gắn NPCPathFollower từ trước)
        if (npcs != null)
        {
            foreach (var n in npcs)
            {
                if (n != null)
                {
                    NPCPathFollower pf = n.GetComponent<NPCPathFollower>();
                    if (pf != null) pf.enabled = false;
                }
            }
        }

        // 2. Chạy logic khởi tạo hoặc Cutscene
        if (hasPlayed)
        {
            // Nếu đã chạy cutscene rồi trong lượt play này -> Xóa Enemy, Cho ngủ luôn và NPC nằm sẵn
            if (enemy != null) enemy.SetActive(false);
            canSleep = true;
            InstantSleepNPCs();
            return;
        }

        // Đánh dấu là đã vào phòng ngủ để gỡ lệnh cấm ở Lobby
        FirstTimeLobbyRestrictor.hasVisitedBedroom = true;

        // Bắt đầu diễn
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        isCutscenePlaying = true;
        if (camFollow != null) camFollow.enabled = false;

        // 1. Lia camera tới Enemy
        if (enemy != null && cam != null)
        {
            Vector3 targetPos = new Vector3(enemy.transform.position.x, enemy.transform.position.y, initialCamPos.z);
            yield return MoveCamera(cam.transform.position, targetPos);
        }

        // 2. Hiện thoại của Enemy bằng Speech Bubble
        if (bubblePrefab != null && enemy != null)
        {
            SpeechBubble enemyBubble = Instantiate(
                bubblePrefab,
                enemy.transform.position + Vector3.up * bubbleOffsetY,
                Quaternion.identity
            );
            enemyBubble.Init(enemy.transform, Vector3.up * bubbleOffsetY);
            enemyBubble.Show(dialogueText, bubbleDuration);
            
            // Hiện trong thời gian cấu hình
            yield return new WaitForSeconds(bubbleDuration);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        // Cho các NPC đi về giường và Enemy đi mất (chạy ngầm song song, không cần đợi)
        StartCoroutine(MoveAllNPCsToBed());
        StartCoroutine(MoveEnemyAway());

        // 3. Lia camera tới chiếc Giường của Player
        if (playerBedPoint != null && cam != null)
        {
            Vector3 bedCamPos = new Vector3(playerBedPoint.position.x, playerBedPoint.position.y, initialCamPos.z);
            yield return MoveCamera(cam.transform.position, bedCamPos);
            
            // Hiện thoại hướng dẫn bấm E bằng SpeechBubble (nếu có ném Bubble Prefab vào)
            if (bubblePrefab != null)
            {
                SpeechBubble bubble = Instantiate(
                    bubblePrefab,
                    playerBedPoint.position + Vector3.up * bubbleOffsetY,
                    Quaternion.identity
                );
                bubble.Init(playerBedPoint, Vector3.up * bubbleOffsetY);
                bubble.Show(bedInstructionText, bubbleDuration);
                
                // Đợi thời gian hiển thị bubble
                yield return new WaitForSeconds(bubbleDuration);
            }
            else
            {
                // Nếu không set Bubble, có thể dùng tạm DialogueUI cũ hoặc chờ 2s
                yield return new WaitForSeconds(2f);
            }
        }

        // 4. Trả camera về cho Player
        if (cam != null && player != null)
        {
            Vector3 playerCamPos = new Vector3(player.transform.position.x, player.transform.position.y, initialCamPos.z);
            yield return MoveCamera(cam.transform.position, playerCamPos);
        }

        if (camFollow != null) camFollow.enabled = true;
        
        isCutscenePlaying = false;
        canSleep = true; // Cho phép ngủ
        hasPlayed = true;
    }

    // Logic di chuyển NPC thẳng trong script này
    private IEnumerator MoveAllNPCsToBed()
    {
        if (npcs == null || npcSleepPoints == null) yield break;

        int count = Mathf.Min(npcs.Length, npcSleepPoints.Length);
        
        for (int i = 0; i < count; i++)
        {
            if (npcs[i] != null)
            {
                Animator anim = npcs[i].GetComponent<Animator>();
                if (anim != null) anim.SetBool("IsMoving", true);
            }
        }

        bool allArrived = false;
        while (!allArrived)
        {
            allArrived = true;
            for (int i = 0; i < count; i++)
            {
                if (npcs[i] != null && npcSleepPoints[i] != null)
                {
                    float dist = Vector2.Distance(npcs[i].transform.position, npcSleepPoints[i].position);
                    if (dist > 0.05f)
                    {
                        // Thiết lập hướng đi cho Animator để không bị đi ngang như cua
                        Vector2 dir = ((Vector2)npcSleepPoints[i].position - (Vector2)npcs[i].transform.position).normalized;
                        Animator anim = npcs[i].GetComponent<Animator>();
                        if (anim != null)
                        {
                            anim.SetFloat("InputX", dir.x);
                            anim.SetFloat("InputY", dir.y);
                            anim.SetFloat("LastInputX", dir.x);
                            anim.SetFloat("LastInputY", dir.y);
                        }

                        // Di chuyển
                        npcs[i].transform.position = Vector2.MoveTowards(
                            npcs[i].transform.position,
                            npcSleepPoints[i].position,
                            npcMoveSpeed * Time.deltaTime
                        );
                        allArrived = false;
                    }
                    else
                    {
                        // Tới nơi -> Nằm ngủ
                        Animator anim = npcs[i].GetComponent<Animator>();
                        if (anim != null)
                        {
                            anim.SetBool("IsMoving", false);
                            anim.SetBool("IsSleep", true);
                        }
                    }
                }
            }
            yield return null;
        }
    }

    private IEnumerator MoveEnemyAway()
    {
        if (enemy == null || enemyLeavePoint == null) yield break;

        Animator anim = enemy.GetComponent<Animator>();
        if (anim != null) anim.SetBool("IsMoving", true);

        while (Vector2.Distance(enemy.transform.position, enemyLeavePoint.position) > 0.05f)
        {
            // Thiết lập hướng đi cho Animator của Enemy
            Vector2 dir = ((Vector2)enemyLeavePoint.position - (Vector2)enemy.transform.position).normalized;
            if (anim != null)
            {
                anim.SetFloat("InputX", dir.x);
                anim.SetFloat("InputY", dir.y);
                anim.SetFloat("LastInputX", dir.x);
                anim.SetFloat("LastInputY", dir.y);
            }

            enemy.transform.position = Vector2.MoveTowards(
                enemy.transform.position,
                enemyLeavePoint.position,
                enemyMoveSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Đã tới điểm đến -> Biến mất
        enemy.SetActive(false);
    }

    private void InstantSleepNPCs()
    {
        if (npcs == null || npcSleepPoints == null) return;
        int count = Mathf.Min(npcs.Length, npcSleepPoints.Length);
        for (int i = 0; i < count; i++)
        {
            if (npcs[i] != null && npcSleepPoints[i] != null)
            {
                npcs[i].transform.position = npcSleepPoints[i].position;
                Animator anim = npcs[i].GetComponent<Animator>();
                if (anim != null)
                {
                    anim.SetBool("IsMoving", false);
                    anim.SetBool("IsSleep", true);
                }
            }
        }
    }

    private IEnumerator MoveCamera(Vector3 from, Vector3 to)
    {
        float t = 0;
        while (t < camMoveTime && cam != null)
        {
            cam.transform.position = Vector3.Lerp(from, to, t / camMoveTime);
            t += Time.deltaTime;
            yield return null;
        }
        if (cam != null) cam.transform.position = to;
    }

    private void Update()
    {
        if (isCutscenePlaying || !canSleep) return;

        // Bấm phím E để ngủ
        if (Input.GetKeyDown(KeyCode.E) && player != null && playerBedPoint != null)
        {
            // Kiểm tra khoảng cách đứng gần giường
            float dist = Vector2.Distance(player.transform.position, playerBedPoint.position);
            if (dist <= interactDistance)
            {
                canSleep = false;
                StartCoroutine(SleepAndChangeSceneSequence());
            }
        }
    }

    private IEnumerator SleepAndChangeSceneSequence()
    {
        // Player nằm ngủ
        Animator pAnim = player.GetComponent<Animator>();
        if (pAnim != null)
        {
            pAnim.SetBool("IsMoving", false);
            pAnim.SetBool("IsSleep", true);
        }

        // Chờ 2 giây thưởng thức cảnh ngủ
        yield return new WaitForSeconds(2f);

        // Hiệu ứng Fade out đen màn hình (nếu có Image)
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float fadeTime = 1.5f;
            float t = 0;
            while (t < fadeTime)
            {
                Color c = fadeImage.color;
                c.a = Mathf.Lerp(0, 1, t / fadeTime);
                fadeImage.color = c;
                t += Time.deltaTime;
                yield return null;
            }
            Color finalC = fadeImage.color;
            finalC.a = 1;
            fadeImage.color = finalC;
        }

        yield return new WaitForSeconds(1f);

        // Đặt Spawn ID cho scene mới để nhân vật xuất hiện đúng vị trí
        DoorSceneChange.NextSpawnId = nextSceneSpawnId;

        // Trả lại trạng thái Animation bình thường cho Player trước khi sang scene khác
        if (pAnim != null)
        {
            pAnim.SetBool("IsSleep", false);
        }

        // Chuyển Scene
        SceneManager.LoadScene(nextSceneName);
    }
}
