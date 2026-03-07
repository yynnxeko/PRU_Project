using UnityEngine;

public class CutsceneSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip shockingSound;

    public void PlayShocking()
    {
        if (audioSource == null || shockingSound == null) return;

        audioSource.PlayOneShot(shockingSound);
    }
}