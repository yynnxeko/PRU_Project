using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NpcSceneEntry : MonoBehaviour
{
    [Header("Activation Setting")]
    public string requiredFlag = "npc_ready_in_lobby";
    public bool destroyIfNoFlag = false;

    [Header("Behavior to trigger on arrival")]
    public MonoBehaviour behaviorToEnable;
    public List<NarrativeDirector> directorsToStart = new List<NarrativeDirector>();

    private bool activated = false;
    private bool cachedFlag = false;
    private bool isHidden = false;

    void Awake()
    {
        // Cache flag sớm (trước khi bất kỳ Start nào clear)
        if (GameFlagManager.Instance != null)
        {
            cachedFlag = GameFlagManager.Instance.GetFlag(requiredFlag);
            Debug.Log($"[NpcSceneEntry] Awake – cached '{requiredFlag}' = {cachedFlag}");
        }
    }

    void Start()
    {
        CheckEntry();
    }

    void OnEnable()
    {
        GameFlagManager.OnFlagChanged += OnFlagChanged;
    }

    void OnDisable()
    {
        GameFlagManager.OnFlagChanged -= OnFlagChanged;
    }

    private void OnFlagChanged(string flagName, bool value)
    {
        // Chỉ phản ứng khi đúng flag mình cần và chưa kích hoạt
        if (!activated && flagName == requiredFlag && value)
        {
            CheckEntry();
        }
    }

    public void CheckEntry()
    {
        if (activated) return;
        if (GameFlagManager.Instance == null) return;

        // Dùng giá trị đã cache từ Awake (tránh bị NpcMoveNav.Start clear mất)
        // Nếu được gọi lại từ OnFlagChanged thì đọc trực tiếp
        bool hasArrived = cachedFlag || GameFlagManager.Instance.GetFlag(requiredFlag);

        if (hasArrived)
        {
            activated = true;
            Debug.Log($"[NpcSceneEntry] Flag '{requiredFlag}' is TRUE. Activating NPC '{name}'.");

            // Luôn gọi ShowNPC để đảm bảo NPC hiện
            ShowNPC();

            if (behaviorToEnable != null) behaviorToEnable.enabled = true;
            foreach (var director in directorsToStart)
            {
                if (director != null) director.StartNarrative();
            }
        }
        else
        {
            Debug.Log($"[NpcSceneEntry] Flag '{requiredFlag}' is FALSE. NPC '{name}' remains hidden.");
            if (destroyIfNoFlag)
            {
                Destroy(gameObject);
            }
            else
            {
                // Ẩn visual nhưng giữ GO active để event listener vẫn hoạt động
                HideNPC();
            }
        }
    }

    // ======================
    // ẨN / HIỆN NPC (giữ GO active)
    // ======================
    void HideNPC()
    {
        isHidden = true;

        // Tắt renderer
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // Tắt tất cả renderer con (nếu có)
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        // Tắt animator
        var animator = GetComponent<Animator>();
        if (animator != null) animator.enabled = false;

        // Tắt NavMeshAgent
        var nav = GetComponent<NavMeshAgent>();
        if (nav != null) nav.enabled = false;

        // Tắt collider
        foreach (var col in GetComponents<Collider2D>())
            col.enabled = false;
    }

    void ShowNPC()
    {
        isHidden = false;

        // Bật renderer
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;

        // Bật tất cả renderer con
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = true;

        // Bật animator
        var animator = GetComponent<Animator>();
        if (animator != null) animator.enabled = true;

        // Bật NavMeshAgent
        var nav = GetComponent<NavMeshAgent>();
        if (nav != null) nav.enabled = true;

        // Bật collider
        foreach (var col in GetComponents<Collider2D>())
            col.enabled = true;
    }
}
