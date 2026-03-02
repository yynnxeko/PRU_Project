using UnityEngine;

public class Mission2_UnlockCabinet : MissionStep
{
    private bool hasPassword = false;

    public override void StartStep()
    {
        base.StartStep();
        hasPassword = false;
        Debug.Log("Nhiệm vụ 2: Tìm mật khẩu trên máy tính để mở tủ phòng ngủ.");
    }

    // Gọi hàm này khi tương tác với máy tính
    public void OnComputerInteracted()
    {
        if (hasPassword) return;
        
        hasPassword = true;
        Debug.Log("Đã tìm thấy mật khẩu! Giờ hãy đi mở tủ.");
    }

    // Gọi hàm này khi tương tác với tủ sau khi đã có pass
    public void OnCabinetOpened()
    {
        if (!hasPassword)
        {
            Debug.Log("Tủ đã khóa, bạn cần mật khẩu từ máy tính.");
            return;
        }

        if (IsCompleted) return;

        Debug.Log("Đã mở tủ thành công!");
        CompleteStep();
    }
}
