
using TMPro;
using UnityEngine;

public class KeypadInput : MonoBehaviour
{
    public TextMeshProUGUI displayText;

    public AudioClip beepClip;
    public AudioClip correctClip;
    public AudioClip incorrectClip;
    public string correctCode = "9466";
    public KeypadTrigger keypadTrigger; // Tham chiếu tới KeypadTrigger

    string currentCode = "";

    AudioSource audioSource;

    void Awake()
    {
        // Lấy AudioSource gắn trên chính object Keypad
        audioSource = GetComponent<AudioSource>();
    }

    public void PressNumber(string number)
    {
        if (currentCode.Length < 4)
        {
            currentCode += number;
            displayText.text = currentCode;

            if (audioSource && beepClip)
                audioSource.PlayOneShot(beepClip);
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

            if (audioSource && correctClip)
                audioSource.PlayOneShot(correctClip);

            // Tắt canvas keypadUI nếu có
            if (keypadTrigger != null && keypadTrigger.keypadUI != null)
                keypadTrigger.keypadUI.SetActive(false);

            // Hiện popup với nội dung mong muốn
            if (keypadTrigger != null)
            {
                keypadTrigger.ShowPopup("Đã tìm thấy tài liệu, tìm cách đưa cho đầu bếp mà không bị bắt");
            }

            // cộng Evidence
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

            // hoàn thành nhiệm vụ
            if (FullMissionManager.Instance != null)
            {
                FullMissionManager.Instance.ReportComplete();
            }
        }
        else
        {
            Debug.Log("Wrong Code!");

            if (audioSource && incorrectClip)
                audioSource.PlayOneShot(incorrectClip);
        }

        Clear();
    }
}