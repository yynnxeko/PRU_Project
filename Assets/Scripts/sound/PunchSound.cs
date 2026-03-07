using UnityEngine;

public class PunchSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip punchSound;

    public void PlayPunchSound()
    {
        if (audioSource == null || punchSound == null) return;

        audioSource.PlayOneShot(punchSound);
    }
}