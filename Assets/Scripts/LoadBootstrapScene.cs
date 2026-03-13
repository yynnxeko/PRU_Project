using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBootstrapScene : MonoBehaviour
{
    // Script này gắn vào GameObject của dòng chữ "Chơi Mới"
    void OnMouseDown()
    {
        // Xóa sạch save để chơi ván mới
        GameSaveManager.DeleteAllSaveData(); 
        
        // Đoạn này xóa nốt cờ ContinueFlow thủ công nếu có (kể cả DeleteAll cũng tự xóa)
        PlayerPrefs.SetInt("IsContinueFlow", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Buss");
    }
}