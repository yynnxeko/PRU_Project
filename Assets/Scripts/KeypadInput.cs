
using System.Collections;
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
    private bool isActive = false;

    void Awake()
    {
        // Lấy AudioSource gắn trên chính object Keypad
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        isActive = true;
    }

    void OnDisable()
    {
        isActive = false;
    }

    void Update()
    {
        if (!isActive) return;

        // Nhập số 0-9 từ bàn phím
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                PressNumber(i.ToString());
            }
        }

        // Backspace = Xóa hết
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Clear();
        }

        // Enter = Xác nhận
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Enter();
        }
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

            // Dùng PlayClipAtPoint để âm thanh không bị cắt khi tắt UI
            if (correctClip)
                AudioSource.PlayClipAtPoint(correctClip, Camera.main.transform.position);

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

            // Delay tắt UI để âm thanh kịp phát
            StartCoroutine(DelayedCloseUI());
        }
        else
        {
            Debug.Log("Wrong Code!");

            if (incorrectClip)
                AudioSource.PlayClipAtPoint(incorrectClip, Camera.main.transform.position);
        }

        Clear();
    }

    IEnumerator DelayedCloseUI()
    {
        yield return new WaitForSeconds(0.15f);

        // Tắt canvas keypadUI
        if (keypadTrigger != null && keypadTrigger.keypadUI != null)
            keypadTrigger.keypadUI.SetActive(false);

        // Hiện popup
        if (keypadTrigger != null)
        {
            keypadTrigger.ShowPopup("Đã tìm thấy tài liệu, tìm cách đưa cho đầu bếp mà không bị bắt");
        }
    }
}