using UnityEngine;

public class TruckMoveDown : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 10f;
    public AudioSource audioSource;
    public AudioClip truckSound;

    public CameraCutscene cameraCutscene;

    Vector3 startPos;
    bool moving = true;

    void Start()
    {
        startPos = transform.position;

        if (audioSource && truckSound)
        {
            audioSource.clip = truckSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (!moving) return;

        transform.Translate(Vector2.down * speed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) >= moveDistance)
        {
            moving = false;

            if (audioSource)
                audioSource.Stop();

            if (cameraCutscene)
                cameraCutscene.StartCameraMove();
        }
    }
}