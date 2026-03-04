using UnityEngine;

public class PlayerPersist : MonoBehaviour
{
    public static PlayerPersist Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // tránh duplicate
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
