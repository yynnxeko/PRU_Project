using UnityEngine;

public class NPCAnimation : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Idle Animations")]
    public AnimationClip idleEast;
    public AnimationClip idleWest;
    public AnimationClip idleNorth;
    public AnimationClip idleSouth;

    [Header("Walk Animations")]
    public AnimationClip walkEast;
    public AnimationClip walkWest;
    public AnimationClip walkNorth;
    public AnimationClip walkSouth;

    private Vector2 lastDir = Vector2.down;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Gọi hàm này mỗi frame từ script di chuyển NPC
    /// </summary>
    public void UpdateAnimation(Vector2 moveDir)
    {
        if (moveDir.magnitude > 0.01f)
        {
            lastDir = moveDir;
            PlayWalk(moveDir);
        }
        else
        {
            PlayIdle(lastDir);
        }
    }

    void PlayWalk(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            animator.Play(dir.x > 0 ? walkEast.name : walkWest.name);
        }
        else
        {
            animator.Play(dir.y > 0 ? walkNorth.name : walkSouth.name);
        }
    }

    void PlayIdle(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            animator.Play(dir.x > 0 ? idleEast.name : idleWest.name);
        }
        else
        {
            animator.Play(dir.y > 0 ? idleNorth.name : idleSouth.name);
        }
    }
}
