using UnityEngine;
using TMPro;

public class KeypadInput : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public AudioSource beepSound;
    public string correctCode = "1234";   // mật mã đúng
    string currentCode = "";

    public void PressNumber(string number)
    {
        currentCode += number;
        displayText.text = currentCode;
        beepSound.Play();
    }

    public void Clear()
    {
        currentCode = "";
        displayText.text = "";
    }

    public void Enter()
    {
        if (currentCode == correctCode)
        {
            Debug.Log("Correct Code!");
            // TODO: mở cửa / trigger event
        }
        else
        {
            Debug.Log("Wrong Code!");
        }
        Clear();
    }
}