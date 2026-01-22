using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpeechBubble : MonoBehaviour
{
    public Text text;
    public float defaultDuration = 2f;

    Transform target;
    Vector3 offset;
    Camera cam;
    Coroutine routine;

    public void Init(Transform followTarget, Vector3 worldOffset)
    {
        target = followTarget;
        offset = worldOffset;
        cam = Camera.main;
    }

    public void Show(string message, float duration = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ShowRoutine(message, duration));
    }

    IEnumerator ShowRoutine(string message, float duration)
    {
        Debug.Log("SpeechBubble show: " + message);
        text.text = message;
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;

        if (cam == null) cam = Camera.main;
        if (cam != null)
            transform.forward = cam.transform.forward;
    }
}
