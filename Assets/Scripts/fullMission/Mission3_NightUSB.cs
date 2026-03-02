using UnityEngine;

public class Mission3_NightUSB : MissionStep
{
    public override void StartStep()
    {
        base.StartStep();
        Debug.Log("Nhiệm vụ 3: Đợi đến tối, lẻn vào IT Room để lấy USB.");
    }

    public override void UpdateStep()
    {
        // Có thể thêm logic cảnh báo nếu người chơi vào phòng IT lúc ban ngày
        // Nhưng logic chính sẽ nằm ở hàm nhặt USB bên dưới
    }

    // Gọi hàm này khi tương tác với USB
    public void OnUSBCollected()
    {
        // Kiểm tra xem có đúng là ban đêm không
        if (DayManager.Instance != null && DayManager.Instance.currentPhase == DayPhase.Night)
        {
            if (IsCompleted) return;
            
            Debug.Log("Đã lấy được USB thành công trong đêm!");
            CompleteStep();
        }
        else
        {
            Debug.Log("Bây giờ là ban ngày, lấy USB lúc này sẽ bị lộ!");
        }
    }
}
