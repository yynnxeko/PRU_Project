using UnityEngine;
using UnityEngine.UI;
using System;

public class HackLevel1UI : MonoBehaviour
{
    public Text hintText;
    public Text statusText;           
    public Button[] nodeButtons;      
    public Text[] nodeLabels;        

    [TextArea] public string[] hintPool;

    string correctNodeId;
    Action<bool> onComplete;

    // dữ liệu node có thể là ký tự hay từ khoá
    readonly string[] possibleIds = { "A", "B", "C", "D", "E", "F" };

    public void Begin(Action<bool> callback)
    {
        onComplete = callback;

        statusText.text = "STATUS: Waiting input...";
        hintText.text = hintPool != null && hintPool.Length > 0
            ? hintPool[UnityEngine.Random.Range(0, hintPool.Length)]
            : "Select the correct node.";

        // random số node 3–6
        int nodeCount = UnityEngine.Random.Range(3, 7);

        // bật đúng số button, tắt phần còn lại
        for (int i = 0; i < nodeButtons.Length; i++)
        {
            bool active = i < nodeCount;
            nodeButtons[i].gameObject.SetActive(active);
        }

        // random node ID cho từng nút
        string[] chosenIds = new string[nodeCount];
        for (int i = 0; i < nodeCount; i++)
        {
            chosenIds[i] = possibleIds[UnityEngine.Random.Range(0, possibleIds.Length)];
            nodeLabels[i].text = $"[  {chosenIds[i]}  ]";
        }

        // random 1 node đúng trong số đó
        int correctIndex = UnityEngine.Random.Range(0, nodeCount);
        correctNodeId = chosenIds[correctIndex];

        // attach listener
        for (int i = 0; i < nodeCount; i++)
        {
            int idx = i;
            nodeButtons[i].onClick.RemoveAllListeners();
            nodeButtons[i].onClick.AddListener(() => OnNodeClicked(chosenIds[idx]));
        }
    }

    void OnNodeClicked(string nodeId)
    {
        if (nodeId == correctNodeId)
        {
            statusText.text = "STATUS: Node validated. Access bypassed.";
            Finish(true);
        }
        else
        {
            statusText.text = "STATUS: Invalid node. Try again.";
            // không fail, cho chọn lại
        }
    }

    void Finish(bool success) => onComplete?.Invoke(success);
}
