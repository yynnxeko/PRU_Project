using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    public float speed = 2f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 0.6f;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float a = Mathf.Lerp(minAlpha, maxAlpha,
            (Mathf.Sin(Time.time * speed) + 1f) / 2f);

        Color c = sr.color;
        c.a = a;
        sr.color = c;
    }
}
