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
        public Transform targetPoint;
        public float duration = 2f;
        public string flagName;
    }

    [Header("Sequence Settings")]
    public List<NarrativeStep> steps;
    public bool playOnStart = false;

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
        if (isPlaying) return;
        
        // Cập nhật lại camFollow nếu vẫn null (đề phòng trường hợp Manager sinh sau đẻ muộn)
        if (camFollow == null) camFollow = FindFirstObjectByType<CameraFollowPersist>();

        StartCoroutine(ExecuteSequence());
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
                        foreach(var actor in step.actors) if(actor != null) actor.SetActive(false);
                    }
                    break;
                case StepType.SetFlag:
                    if (GameFlagManager.Instance != null) GameFlagManager.Instance.SetFlag(step.flagName, true);
                    break;
                case StepType.CameraFocus:
                    yield return HandleCameraFocus(step);
                    break;
                case StepType.Wait:
                    yield return new WaitForSeconds(step.duration);
                    break;
            }
        }

        // Trả camera về cho Player nếu cần
        ReturnCameraToPlayer();
        
        isPlaying = false;
        Debug.Log("[NarrativeDirector] Sequence Finished.");
    }

    private IEnumerator HandleSpeak(NarrativeStep step)
    {
        if (step.actors == null || step.actors.Count == 0 || step.actors[0] == null || bubblePrefab == null) yield break;

        GameObject mainActor = step.actors[0];
        SpeechBubble bubble = Instantiate(bubblePrefab, mainActor.transform.position + bubbleOffset, Quaternion.identity);
        bubble.Init(mainActor.transform, bubbleOffset);
        bubble.Show(step.textValue, step.duration);
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
                a.isStopped = false;
                a.SetDestination(step.targetPoint.position);
            }
        }

        bool allArrived = false;
        while (!allArrived)
        {
            allArrived = true;
            for (int i = 0; i < agents.Count; i++)
            {
                var agent = agents[i];
                var anim = anims[i];

                if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.2f)
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
    }

    private IEnumerator HandleCameraFocus(NarrativeStep step)
    {
        if (step.actors == null || step.actors.Count == 0 || step.actors[0] == null || Camera.main == null) yield break;

        if (camFollow != null) camFollow.enabled = false;

        GameObject mainActor = step.actors[0];
        Vector3 startPos = Camera.main.transform.position;
        Vector3 endPos = new Vector3(mainActor.transform.position.x, mainActor.transform.position.y, startPos.z);
        
        float t = 0;
        while (t < 1f)
        {
            Camera.main.transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime * 2f;
            yield return null;
        }
        
        yield return new WaitForSeconds(step.duration);
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
}
