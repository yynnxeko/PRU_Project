using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootstrapLoader : MonoBehaviour
{
    void Start()
    {
        // Optionally đợi 0.1s cho chắc chắn player khởi tạo
        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        yield return null; // Hoặc yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene("Buss");
    }
}