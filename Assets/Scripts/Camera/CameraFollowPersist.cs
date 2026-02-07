using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollowPersist : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private float smooth = 10f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("Persist")]
    [SerializeField] private bool dontDestroyOnLoad = true;

    private Transform target;

    void Awake()
    {
        if (dontDestroyOnLoad)
        {
            var cams = FindObjectsOfType<Camera>();
            foreach (var c in cams)
            {
                if (c != GetComponent<Camera>()) Destroy(c.gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        FindPlayer();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer(); 
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, smooth * Time.deltaTime);
    }

    void FindPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        target = player != null ? player.transform : null;
    }
}
