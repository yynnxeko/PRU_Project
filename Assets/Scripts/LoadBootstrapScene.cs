using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBootstrapScene : MonoBehaviour
{
    public string sceneName = "Bootstrap";

    void OnMouseDown()
    {
        SceneManager.LoadScene(sceneName);
    }
}