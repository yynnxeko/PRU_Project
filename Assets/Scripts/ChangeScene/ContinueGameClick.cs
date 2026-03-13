using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueGameClick : MonoBehaviour
{
    // Script này gắn vào GameObject của dòng chữ "Chơi Tiếp"
    void OnMouseDown()
    {
        if (GameSaveManager.HasSaveData())
        {
            // Thay vì nhảy cóc qua BedRoom (gây mất Camera và Player),
            // ta sẽ đi cửa chính yếu qua Scene sinh ra Player và Camera (ví dụ: Buss),
            // và cài một cái cờ "IsContinueFlow" để Scene Buss biết tự kéo ta vào BedRoom.
            PlayerPrefs.SetInt("IsContinueFlow", 1);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Map_Internal Area_Night");
            Debug.Log("[ContinueGameClick] Kích hoạt IsContinueFlow! -> Vào Buss lấy Camera trước...");
        }
        else
        {
            Debug.Log("[ContinueGameClick] Không có Save Data, nút Chơi Tiếp bị vô hiệu hóa.");
        }
    }
}
