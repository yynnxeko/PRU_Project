using UnityEngine;
using System.Collections;

public class DelayedGlobalAlert : MonoBehaviour
{
    public float delay = 10f;         
    public float huntInterval = 1f;   

    private const string PREF_KEY = "GlobalHuntTriggered";
    Coroutine huntLoop;

    void Awake()
    {
#if UNITY_EDITOR
        PlayerPrefs.DeleteKey(PREF_KEY); 
#endif
    }

    void Start()
    {
        if (PlayerPrefs.GetInt(PREF_KEY, 0) == 1)
            return;

        StartCoroutine(StartHuntAfterDelay());
    }

    IEnumerator StartHuntAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            huntLoop = StartCoroutine(HuntLoop(playerObj.transform));

            PlayerPrefs.SetInt(PREF_KEY, 1);
            PlayerPrefs.Save();
        }
    }

    IEnumerator HuntLoop(Transform player)
    {
        while (true)
        {
            GameManager.Instance.AlertAllEnemies(player.position);
            yield return new WaitForSeconds(huntInterval); 
        }
    }
}