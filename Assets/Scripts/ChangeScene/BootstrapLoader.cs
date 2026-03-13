using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootstrapLoader : MonoBehaviour
{
    // Script dành cho "Chơi Mới"
    void OnMouseDown()
    {
        GameSaveManager.DeleteAllSaveData(); // Chơi mới -> Xóa sạch save cũ
        StartCoroutine(LoadGameScene("Buss"));
    }

    // Bỏ hàm Continue vì nút Continue đã có script riêng ContinueGameClick.cs

    IEnumerator LoadGameScene(string sceneName)
    {
        yield return null; // Hoặc yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(sceneName);
    }
}