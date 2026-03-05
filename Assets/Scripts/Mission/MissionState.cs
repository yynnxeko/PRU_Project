public enum MissionState
{
    Locked,      // Chưa mở khóa (chưa đủ điều kiện)
    Available,   // Đã mở khóa, chờ player nhận
    Active,      // Đang thực hiện
    Completed,   // Hoàn thành
    Failed       // Thất bại (có thể retry)
}
