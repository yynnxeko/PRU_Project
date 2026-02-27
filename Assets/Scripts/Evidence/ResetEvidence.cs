using UnityEngine;

public class ResetEvidence : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))   // Nhấn R để reset
        {
            if (EvidenceManager.Instance != null)
                EvidenceManager.Instance.DeleteAllSave();
        }
    }
}