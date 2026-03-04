using UnityEngine;
using TMPro;
using System.Collections;

public class ChairIntroCamera : MonoBehaviour
{
    public Transform chairPoint;
    public TextMeshProUGUI hintText;
    public float moveTime = 1f;

    Camera cam;
    Vector3 camStartPos;

    CameraFollowPersist camFollow; // đúng script của bạn

    void Start()
    {
        cam = Camera.main;
        camStartPos = cam.transform.position;

        camFollow = cam.GetComponent<CameraFollowPersist>();

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        // Tắt follow
        if (camFollow != null)
            camFollow.enabled = false;

        Vector3 chairCamPos = new Vector3(
            chairPoint.position.x,
            chairPoint.position.y,
            camStartPos.z);

        // Pan tới ghế
        yield return MoveCamera(camStartPos, chairCamPos);

        // Hiện text
        hintText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        hintText.gameObject.SetActive(false);

        // Pan về lại player
        yield return MoveCamera(chairCamPos, camStartPos);

        // Bật lại follow
        if (camFollow != null)
            camFollow.enabled = true;
    }

    IEnumerator MoveCamera(Vector3 from, Vector3 to)
    {
        float t = 0;
        while (t < moveTime)
        {
            cam.transform.position = Vector3.Lerp(from, to, t / moveTime);
            t += Time.deltaTime;
            yield return null;
        }
        cam.transform.position = to;
    }
}
