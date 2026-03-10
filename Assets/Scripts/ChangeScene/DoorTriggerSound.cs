using UnityEngine;

public class DoorTriggerSound : MonoBehaviour
{
    public AudioClip doorSound;
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (doorSound != null)
        {
            GameObject obj = new GameObject("TempDoorSound");
            AudioSource source = obj.AddComponent<AudioSource>();

            source.clip = doorSound;
            source.Play();

            DontDestroyOnLoad(obj);
            Destroy(obj, doorSound.length);
        }
    }
}