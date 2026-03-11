using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootstrapLoader : MonoBehaviour
{
    public void LoadGameSceneButton()
    {
        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        yield return null; // Hoặc yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene("Buss");
        // SceneManager.LoadScene("Map_Internal Area_Night");
        // SceneManager.LoadScene("IT_Room");
    }
}