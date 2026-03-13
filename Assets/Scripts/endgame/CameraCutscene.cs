using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class CameraCutscene : MonoBehaviour
{
    public Transform targetPosition;
    public float speed = 2f;

    public GameObject textObject;
    bool moving = false;

    [Header("Fade & Scene Transition")]
    [Tooltip("Image phủ toàn màn hình (màu đen, alpha = 0)")]
    public Image fadeImage;
    public float fadeOutDuration = 1.5f;
    [Tooltip("Thời gian chờ sau khi hiện text trước khi fade đen")]
    public float delayBeforeFade = 3f;
    public string nextSceneName = "Map_Ending";

    public void StartCameraMove()
    {
        moving = true;
    }

    void Update()
    {
        if (!moving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
        {
            moving = false;

            if (textObject)
                textObject.SetActive(true);

            // Bắt đầu fade đen rồi chuyển scene
            StartCoroutine(FadeAndLoadScene());
        }
    }

    IEnumerator FadeAndLoadScene()
    {
        // Chờ cho người chơi đọc text
        yield return new WaitForSeconds(delayBeforeFade);

        // Ẩn text trước khi fade
        if (textObject)
            textObject.SetActive(false);

        // Fade to black
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float t = 0f;
            while (t < fadeOutDuration)
            {
                Color c = fadeImage.color;
                c.a = Mathf.Lerp(0f, 1f, t / fadeOutDuration);
                fadeImage.color = c;
                t += Time.deltaTime;
                yield return null;
            }

            Color finalC = fadeImage.color;
            finalC.a = 1f;
            fadeImage.color = finalC;
        }

        yield return new WaitForSeconds(0.5f);

        // Chuyển sang Map_Ending
        SceneManager.LoadScene(nextSceneName);
    }
}