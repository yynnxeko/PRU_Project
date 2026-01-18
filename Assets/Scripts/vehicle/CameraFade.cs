using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class CameraFade : MonoBehaviour
{
    public Volume volume;
    private ColorAdjustments colorAdjustments;

    void Awake()
    {
        volume.profile.TryGet(out colorAdjustments);
    }

    public IEnumerator FadeToBlack(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            colorAdjustments.postExposure.value = Mathf.Lerp(0f, -15f, t / duration);
            yield return null;
        }
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            colorAdjustments.postExposure.value = Mathf.Lerp(-15f, 0f, t / duration);
            yield return null;
        }
    }
}
