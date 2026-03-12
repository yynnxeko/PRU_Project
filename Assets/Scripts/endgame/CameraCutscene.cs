using UnityEngine;
using TMPro;

public class CameraCutscene : MonoBehaviour
{
    public Transform targetPosition;
    public float speed = 2f;

    public GameObject textObject;
    bool moving = false;

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
        }
    }
}