using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip walkClip;

    PlayerController2 player;

    void Start()
    {
        player = GetComponent<PlayerController2>();
        audioSource.clip = walkClip;
        audioSource.loop = true;
    }

    void Update()
    {
        if (player == null) return;

        bool isMoving =
            Input.GetAxisRaw("Horizontal") != 0 ||
            Input.GetAxisRaw("Vertical") != 0;

        if (isMoving && player.canMove)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}