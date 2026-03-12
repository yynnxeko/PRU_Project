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

    public void Show(string message, float duration = -1f, AudioClip voiceClip = null)
    {
        Debug.Log("GOI HAM SHOW ROI NE!");
        if (duration <= 0f) duration = defaultDuration;

        if (voiceClip != null)
        {
            AudioSource source = GetComponent<AudioSource>();
            if (source == null) source = gameObject.AddComponent<AudioSource>();
            source.PlayOneShot(voiceClip);
        }

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ShowRoutine(message, duration));
    }

    IEnumerator ShowRoutine(string message, float duration)
    {
        text.text = message;
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = target.position + offset;
        pos.z = 0f; // Ép Z = 0 để không bị đẩy sau camera
        transform.position = pos;

        if (cam == null) cam = Camera.main;
        if (cam != null)
            transform.forward = cam.transform.forward;
    }
}
