using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneChange : MonoBehaviour
{
    public static string NextSpawnId;

    [SerializeField] private string sceneToLoad;
    [SerializeField] private string spawnIdInNextScene;
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Unity không tự động chặn OnTriggerEnter2D khi component bị disable,
        // Nên ta phải tự check if (!enabled) để ngưng load scene!
        if (!enabled) return;

        if (!other.CompareTag(playerTag)) return;

        NextSpawnId = spawnIdInNextScene;
        SceneManager.LoadScene(sceneToLoad);
    }
}
