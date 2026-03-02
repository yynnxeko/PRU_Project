using UnityEngine;
using UnityEngine.Events;

public class MissionEndTrigger : MonoBehaviour
{
    [Header("Settings")]
    public bool triggerOnEnter = true;
    public bool triggerOnKeyPress = true;
    public KeyCode interactKey = KeyCode.E;

    [Header("Sự kiện khi kích hoạt")]
    public UnityEvent onTriggered;

    private bool isPlayerInside = false;

    void Update()
    {
        if (triggerOnKeyPress && isPlayerInside && Input.GetKeyDown(interactKey))
        {
            ActivateTrigger();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            if (triggerOnEnter)
            {
                ActivateTrigger();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    private void ActivateTrigger()
    {
        Debug.Log("Trigger activated on: " + gameObject.name);
        onTriggered.Invoke();
    }
}
