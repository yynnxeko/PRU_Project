
using TMPro;
using UnityEngine;

public class KeypadInput : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public AudioClip beepClip;
    public string correctCode = "9466";

    string currentCode = "";

    AudioSource mainAudioSource;

    void Awake()
    {
        // Tự động lấy AudioSource từ Main Camera
        GameObject mainCamera = Camera.main != null ? Camera.main.gameObject : null;
        if (mainCamera != null)
            mainAudioSource = mainCamera.GetComponent<AudioSource>();
    }

    public void PressNumber(string number)
    {
        if (currentCode.Length < 4)
        {
            currentCode += number;
            displayText.text = currentCode;

            if (mainAudioSource && beepClip)
                mainAudioSource.PlayOneShot(beepClip);
        }
    }

    public void Clear()
    {
        currentCode = "";
        displayText.text = "";
    }

    public EvidenceItem rewardEvidence;

    public void Enter()
    {
        if (currentCode == correctCode)
        {
            Debug.Log("Correct Code!");

            // Xử lý cộng Evidence
            if (rewardEvidence != null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    PlayerInventory inventory = playerObj.GetComponent<PlayerInventory>();
                    if (inventory != null)
                    {
                        inventory.AddEvidence(rewardEvidence);
                        Debug.Log("Đã nhận được 1 Evidence từ Keypad!");
                    }
                }
            }

            // Hoàn thành nhiệm vụ
            if (FullMissionManager.Instance != null)
            {
                FullMissionManager.Instance.ReportComplete();
            }
        }
        else
        {
            Debug.Log("Wrong Code!");
        }

        Clear();
    }
}