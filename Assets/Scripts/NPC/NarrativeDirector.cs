using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NarrativeDirector : MonoBehaviour
{
    public enum StepType { Speak, Move, Hide, SetFlag, CameraFocus, Wait }

    [System.Serializable]
    public class NarrativeStep
    {
        public string stepName = "New Step";
        public StepType type;
        public List<GameObject> actors; // Hỗ trợ nhiều nhân vật cùng hành động
        [TextArea] public string textValue;
        public AudioClip voiceClip;
        public Transform targetPoint;
        public float duration = 2f;
        public float moveTimeout = 7f;
        public string flagName;
        public bool flagValue = true; // Giá trị cần set cho flag (mặc định true)
    }

    [Header("Sequence Settings")]
    public List<NarrativeStep> steps;
    public bool playOnStart = false;

    [Tooltip("Nếu bật, kịch bản này chỉ chạy 1 lần duy nhất (lưu bằng GameFlagManager)")]
    public bool playOnce = false;

    [Tooltip("Tên flag dùng để đánh dấu đã chạy (tự sinh nếu để trống)")]
    public string onceFlagName = "";

    [Header("References")]
    public CameraFollowPersist camFollow;
    public SpeechBubble bubblePrefab;
    public Vector3 bubbleOffset = new Vector3(0, 1.5f, 0);

    private bool isPlaying = false;

    void Awake()
    {
        if (camFollow == null)
        {
            camFollow = FindFirstObjectByType<CameraFollowPersist>();
        }
    }

    void Start()
    {
        if (playOnStart) StartNarrative();
    }

    public void StartNarrative()
    {
        Debug.Log("Director STARTED on: " + gameObject.name);
        if (isPlaying) return;

        // Kiểm tra nếu kịch bản chỉ chạy 1 lần và đã chạy rồi → bỏ qua (lưu PlayerPrefs)
        if (playOnce)
        {
            string flag = GetOnceFlag();
            if (PlayerPrefs.GetInt(flag, 0) == 1)
            {
                Debug.Log($"[NarrativeDirector] Skipped (already played, saved to disk): {flag}");
                return;
            }
        }

        // Cập nhật lại camFollow nếu vẫn null (đề phòng trường hợp Manager sinh sau đẻ muộn)
        if (camFollow == null) camFollow = FindFirstObjectByType<CameraFollowPersist>();

        InitializeActors(); // Khóa xoay cho mọi nhân vật ngay lập tức (Dựa trên NpcMoveNav)
        StartCoroutine(ExecuteSequence());
    }

    private void InitializeActors()
    {
        foreach (var step in steps)
        {
            if (step.actors == null) continue;
            foreach (var actor in step.actors)
            {
                if (actor == null) continue;
                NavMeshAgent a = actor.GetComponent<NavMeshAgent>();
                if (a != null)
                {
                    a.updateRotation = false; // "Bí kíp" từ NpcMoveNav
                    a.updateUpAxis = false;
                }
                actor.transform.rotation = Quaternion.identity; // Đảm bảo X = 0
            }
        }
    }

    private IEnumerator ExecuteSequence()
    {
        isPlaying = true;

        foreach (var step in steps)
        {
            Debug.Log($"[NarrativeDirector] Executing: {step.stepName} ({step.type})");

            switch (step.type)
            {
                case StepType.Speak:
                    yield return HandleSpeak(step);
                    break;
                case StepType.Move:
                    yield return HandleMove(step);
                    break;
                case StepType.Hide:
                    if (step.actors != null)
                    {
                        foreach (var actor in step.actors) if (actor != null) actor.SetActive(false);
                    }
                    break;
                // case StepType.SetFlag:
                //     if (GameFlagManager.Instance != null) GameFlagManager.Instance.SetFlag(step.flagName, true);
                //     break;
                case StepType.SetFlag:
                    if (GameFlagManager.Instance != null)
                    {
                        GameFlagManager.Instance.SetFlag(step.flagName, step.flagValue);
                        Debug.Log($"[NarrativeDirector] FLAG SET: {step.flagName} = {step.flagValue} (Step: {step.stepName})");
                    }
                    else
                    {
                        Debug.LogWarning($"[NarrativeDirector] GameFlagManager NULL when trying to set: {step.flagName}");
                    }
                    break;
                case StepType.CameraFocus:
                    yield return HandleCameraFocus(step);
                    break;
                case StepType.Wait:
                    yield return new WaitForSeconds(step.duration);
                    break;
            }
        }


        // Đánh dấu đã chạy xong nếu playOnce (lưu vào ổ cứng)
        if (playOnce)
        {
            PlayerPrefs.SetInt(GetOnceFlag(), 1);
            PlayerPrefs.Save();
        }

        isPlaying = false;
        Debug.Log("[NarrativeDirector] Sequence Finished.");
    }

    private IEnumerator HandleSpeak(NarrativeStep step)
    {
        if (step.actors == null || step.actors.Count == 0 || step.actors[0] == null || bubblePrefab == null) yield break;

        GameObject mainActor = step.actors[0];
        SpeechBubble bubble = Instantiate(bubblePrefab, mainActor.transform.position + bubbleOffset, Quaternion.identity);
        bubble.Init(mainActor.transform, bubbleOffset);
        bubble.Show(step.textValue, step.duration, step.voiceClip);
        yield return new WaitForSeconds(step.duration + 0.5f);
    }

    private IEnumerator HandleMove(NarrativeStep step)
    {
        if (step.actors == null || step.actors.Count == 0 || step.targetPoint == null) yield break;

        List<NavMeshAgent> agents = new List<NavMeshAgent>();
        List<Animator> anims = new List<Animator>();

        foreach (var actor in step.actors)
        {
            if (actor == null) continue;
            NavMeshAgent a = actor.GetComponent<NavMeshAgent>();
            if (a != null)
            {
                agents.Add(a);
                anims.Add(actor.GetComponent<Animator>());

                if (!a.isOnNavMesh)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(a.transform.position, out hit, 2f, NavMesh.AllAreas))
                        a.Warp(hit.position);
                }

                if (a.isOnNavMesh)
                {
                    a.isStopped = false;
                    a.SetDestination(step.targetPoint.position);
                }
            }
        }

        float timeout = step.moveTimeout;
        float timer = 0f;

        bool allArrived = false;
        while (!allArrived && timer < timeout)
        {
            timer += Time.deltaTime;
            allArrived = true;

            for (int i = 0; i < agents.Count; i++)
            {
                var agent = agents[i];
                var anim = anims[i];

                if (!agent.isOnNavMesh)
                {
                    allArrived = false;
                    continue;
                }

                if (agent.pathPending ||
                    agent.remainingDistance > agent.stoppingDistance + 0.3f)
                {
                    allArrived = false;

                    if (anim != null && agent.velocity.sqrMagnitude > 0.01f)
                    {
                        anim.SetBool("IsMoving", true);
                        anim.SetFloat("InputX", agent.velocity.x);
                        anim.SetFloat("InputY", agent.velocity.y);
                    }
                }
                else
                {
                    if (anim != null) anim.SetBool("IsMoving", false);
                    agent.isStopped = true;
                }
            }

            yield return null;
        }

        // Dù tới hay timeout cũng stop lại cho sạch
        foreach (var agent in agents)
            if (agent != null && agent.isOnNavMesh)
                agent.isStopped = true;
    }

    private IEnumerator HandleCameraFocus(NarrativeStep step)
    {
        if (step.actors == null || step.actors.Count == 0 || step.actors[0] == null || Camera.main == null)
            yield break;

        if (camFollow != null)
            camFollow.enabled = false;

        GameObject mainActor = step.actors[0];

        Vector3 originalPos = Camera.main.transform.position;
        Vector3 focusPos = new Vector3(
            mainActor.transform.position.x,
            mainActor.transform.position.y,
            originalPos.z);

        float zoomSpeed = 2f;
        float t = 0f;

        // 🔥 Zoom qua actor
        while (t < 1f)
        {
            Camera.main.transform.position = Vector3.Lerp(originalPos, focusPos, t);
            t += Time.deltaTime * zoomSpeed;
            yield return null;
        }

        Camera.main.transform.position = focusPos;

        // 🔥 Giữ trong duration
        yield return new WaitForSeconds(step.duration);

        // 🔥 Zoom ngược lại về player
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Vector3 playerPos = new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                originalPos.z);

            t = 0f;
            while (t < 1f)
            {
                Camera.main.transform.position = Vector3.Lerp(focusPos, playerPos, t);
                t += Time.deltaTime * zoomSpeed;
                yield return null;
            }
        }

        // 🔥 Trả quyền lại cho camFollow
        if (camFollow != null)
            camFollow.enabled = true;
    }

    private void ReturnCameraToPlayer()
    {
        if (camFollow == null) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            camFollow.enabled = true;
        }
    }

    /// <summary>
    /// Trả về tên flag dùng để đánh dấu đã chạy.
    /// Nếu onceFlagName để trống thì tự sinh từ tên GameObject.
    /// </summary>
    private string GetOnceFlag()
    {
        return string.IsNullOrEmpty(onceFlagName)
            ? $"narrative_done_{gameObject.name}"
            : onceFlagName;
    }

#if UNITY_EDITOR
    void Update()
    {
        // F10 = xóa tất cả narrative playOnce đã lưu, để test lại
        if (Input.GetKeyDown(KeyCode.F10))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("<color=yellow>[NarrativeDirector] PlayerPrefs cleared! Narrative sẽ chạy lại lần Play sau.</color>");
        }
    }
#endif
}
