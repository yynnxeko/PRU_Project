using UnityEngine;
using UnityEngine.SceneManagement;

public class SquareSceneTrigger : MonoBehaviour
{
    public string sceneName = "Map_Living Area Record";
    bool playerInside = false;

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}