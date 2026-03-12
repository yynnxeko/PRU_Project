using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBootstrapScene : MonoBehaviour
{

    void OnMouseDown()
    {
         SceneManager.LoadScene("Buss");
        // SceneManager.LoadScene("Map_Internal Area_Night");

        // SceneManager.LoadScene("IT_Room");
    }
}