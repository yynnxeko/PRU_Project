using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TruckMoveDown : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 10f;
    public AudioSource audioSource;
    public AudioClip truckSound;

    public CameraCutscene cameraCutscene;

    [Header("Fade & Scene Transition")]
    [Tooltip("Image phủ toàn màn hình (màu đen, alpha = 0)")]
    public Image fadeImage;
    [Tooltip("Khi xe đi được bao nhiêu % quãng đường thì bắt đầu fade (0.0 - 1.0)")]
    public float fadeStartPercent = 0.6f;
    public float fadeDuration = 2f;
    public string nextSceneName = "Map_Ending";

    Vector3 startPos;
    bool moving = true;
    bool fading = false;
    bool sceneLoading = false;

    void Start()
    {
        startPos = transform.position;

        if (audioSource && truckSound)
        {
            audioSource.clip = truckSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        // Đảm bảo fadeImage bắt đầu trong suốt
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    void Update()
    {
        if (!moving) return;

        transform.Translate(Vector2.down * speed * Time.deltaTime);

        float traveled = Vector3.Distance(startPos, transform.position);
        float progress = traveled / moveDistance; // 0.0 → 1.0

        // Bắt đầu fade khi xe đi được fadeStartPercent quãng đường
        if (!fading && progress >= fadeStartPercent && fadeImage != null)
        {
            fading = true;
            StartCoroutine(FadeToBlack());
        }

        // Xe tới đích
        if (traveled >= moveDistance)
        {
            moving = false;

            if (audioSource)
                audioSource.Stop();

            if (cameraCutscene)
                cameraCutscene.StartCameraMove();
        }
    }

    IEnumerator FadeToBlack()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
                fadeImage.color = c;
            }
            t += Time.deltaTime;
            yield return null;
        }

        // Đảm bảo alpha = 1
        if (fadeImage != null)
        {
            Color finalC = fadeImage.color;
            finalC.a = 1f;
            fadeImage.color = finalC;
        }

        // Chờ chút rồi chuyển scene
        yield return new WaitForSeconds(0.5f);

        if (!sceneLoading)
        {
            sceneLoading = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}