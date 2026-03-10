using UnityEngine;

public class DoorSoundPlayer : MonoBehaviour
{
    public AudioClip doorSound;     // âm thanh cửa
    public AudioSource audioSource; // kéo thả AudioSource vào đây

    void Start()
    {
        // Nếu scene được load từ DoorSceneChange
        if (!string.IsNullOrEmpty(DoorSceneChange.NextSpawnId))
        {
            if (audioSource != null && doorSound != null)
            {
                audioSource.PlayOneShot(doorSound);
            }
        }
    }
}