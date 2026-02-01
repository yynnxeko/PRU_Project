using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NpcMoveNav : MonoBehaviour
{
    [System.Serializable]
    public class WaypointInfo
    {
        public Transform point;
        public bool isChair = false;
        public Vector2 sitFaceDirection = new Vector2(0, -1);
    }

    [Header("Path Settings")]
    public List<WaypointInfo> waypoints;
    public float arriveDistance = 0.15f;

    [Header("Status")]
    public bool canMove = true;
    public bool IsSitting = false;

    int currentIndex = 0;
    NavMeshAgent agent;
    Animator anim;
    bool isFinished = false;
    bool isSittingProcessStarted = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Start()
    {
        if (waypoints.Count > 0)
            agent.SetDestination(waypoints[currentIndex].point.position);
    }

    void Update()
    {
        if (IsSitting || isSittingProcessStarted) return;

        if (!canMove || isFinished)
        {
            agent.isStopped = true;
            UpdateAnimation(false);
            return;
        }

        agent.isStopped = false;

        Move();
        CheckDestination();
        UpdateAnimation(true);
    }

    void Move()
    {
        if (currentIndex < waypoints.Count)
            agent.SetDestination(waypoints[currentIndex].point.position);
    }

    void CheckDestination()
    {
        if (agent.pathPending) return;
        if (agent.remainingDistance > agent.stoppingDistance + arriveDistance) return;

        WaypointInfo wp = waypoints[currentIndex];

        if (wp.isChair)
        {
            if (!isSittingProcessStarted)
                StartCoroutine(SitDownSmoothly(wp));
        }
        else
        {
            currentIndex++;
            if (currentIndex >= waypoints.Count)
                isFinished = true;
        }
    }

    IEnumerator SitDownSmoothly(WaypointInfo wp)
    {
        isSittingProcessStarted = true;

        // Giảm tốc cho mượt
        float originalSpeed = agent.speed;
        agent.speed = 1.2f;

        agent.SetDestination(wp.point.position);

        // ← CHỈ ĐỂ NPC ĐI TỚI CUỐI CÙNG (KHÔNG TẮT NAVMESH SỚM)
        while (agent.pathPending || agent.remainingDistance > 0.02f)
        {
            UpdateAnimation(true);
            yield return null;
        }

        // ← CHỈ TẮT NAVMESH KHI ĐÃ NGỒI THẬT
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        IsSitting = true;
        isFinished = true;
        agent.speed = originalSpeed;

        if (anim)
        {
            UpdateAnimation(false);
            anim.SetBool("IsSitting", true);
            anim.SetFloat("LastInputX", wp.sitFaceDirection.x);
            anim.SetFloat("LastInputY", wp.sitFaceDirection.y);
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputY", 0);
        }

        isSittingProcessStarted = false;
    }

    void UpdateAnimation(bool isMoving)
    {
        if (!anim) return;

        anim.SetBool("IsMoving", isMoving);

        if (isMoving && agent.hasPath)
        {
            Vector3 v = agent.velocity;
            if (v.sqrMagnitude > 0.01f)
            {
                anim.SetFloat("InputX", v.x);
                anim.SetFloat("InputY", v.y);
                anim.SetFloat("LastInputX", v.x);
                anim.SetFloat("LastInputY", v.y);
            }
        }
    }
}
