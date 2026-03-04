using UnityEngine;
using UnityEngine.UI;

public class Display : MonoBehaviour
{
    [SerializeField] Terminal connectedToTerminal;
    [SerializeField] int charactersWide = 40;
    [SerializeField] int charactersHigh = 14;

    Text screenText;

    private void Awake()
    {
        screenText = GetComponentInChildren<Text>(true);
        if (!screenText)
        {
            Debug.LogWarning("[Display] Missing Text (Legacy) in children.");
        }

        if (!connectedToTerminal)
        {
            Debug.LogWarning("[Display] Missing Terminal reference.");
        }
    }

    private void Update()
    {
        if (!connectedToTerminal || !screenText) return;

        screenText.text = connectedToTerminal.GetDisplayBuffer(charactersWide, charactersHigh);
    }
}