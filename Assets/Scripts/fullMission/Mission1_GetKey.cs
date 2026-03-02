using UnityEngine;

public class Mission1_GetKey : MissionStep
{
    [Header("Cài đặt nhiệm vụ")]
    public string targetItemName = "Bedroom Key";
    
    public override void StartStep()
    {
        base.StartStep();
        Debug.Log("Nhiệm vụ 1: Hãy đến IT Room và lấy chìa khóa trong tủ đồ.");
    }

    // Hàm này sẽ được gọi từ Script của cái Tủ hoặc Item khi Player nhặt
    public void OnKeyCollected()
    {
        if (IsCompleted) return;
        
        Debug.Log("Đã lấy được chìa khóa!");
        CompleteStep();
    }
}
