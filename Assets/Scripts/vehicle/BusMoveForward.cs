using UnityEngine;
using System.Collections;

public class BusMoveForward : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistancePx = 100f;

    public Vector3 teleportTarget = new Vector3(62f, 6.64f, 0f);

    [Header("Fade")]
    public CameraFade cameraFade;
    public float fadeTime = 0.5f;
    public float darkHoldTime = 2f;

    private Vector3 firstTarget;
    private bool done = false;

    void Start()
    {
        float moveUnit = moveDistancePx / 32f;
        firstTarget = transform.position + Vector3.right * moveUnit;
    }

    void Update()
    {
        if (done) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            firstTarget,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, firstTarget) < 0.01f)
        {
            done = true;
            StartCoroutine(TeleportSequence());
        }
    }

    IEnumerator TeleportSequence()
    {
        // fade tối
        yield return StartCoroutine(cameraFade.FadeToBlack(fadeTime));

        // teleport khi màn hình đã đen
        transform.position = teleportTarget;

        // giữ màn hình đen
        yield return new WaitForSeconds(darkHoldTime);

        // fade sáng lại
        yield return StartCoroutine(cameraFade.FadeFromBlack(fadeTime));
    }
}
