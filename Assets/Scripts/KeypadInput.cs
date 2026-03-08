
using TMPro;
using UnityEngine;

public class KeypadInput : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public AudioSource beepSound;
    public AudioClip beepClip;
    public string correctCode = "1234";

    string currentCode = "";

    public void PressNumber(string number)
    {
        currentCode += number;
        displayText.text = currentCode;

        if (beepSound && beepClip)
            beepSound.PlayOneShot(beepClip);
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
        }
        else
        {
            Debug.Log("Wrong Code!");
        }

        Clear();
    }
}