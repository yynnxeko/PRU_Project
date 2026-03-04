using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    void Start()
    {
        string id = DoorSceneChange.NextSpawnId;
        if (string.IsNullOrEmpty(id)) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        foreach (var sp in FindObjectsOfType<SpawnPoint>())
        {
            if (sp.spawnId == id)
            {
                player.transform.position = sp.transform.position;
                break;
            }
        }

        DoorSceneChange.NextSpawnId = null;
    }
}
