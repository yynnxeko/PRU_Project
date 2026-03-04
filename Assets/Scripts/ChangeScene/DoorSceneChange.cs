using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneChange : MonoBehaviour
{
    public static string NextSpawnId;

    [SerializeField] private string sceneToLoad;
    [SerializeField] private string sceneToLoadNight; // Ô mới để chỉnh tay scene buổi tối
    [SerializeField] private string spawnIdInNextScene;
    [SerializeField] private string playerTag = "Player";
    
    [Header("Day Cycle Settings")]
    [SerializeField] private bool requireNight = false; // Nếu bật, chỉ cho phép đi qua vào ban đêm

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled) return;

        if (!other.CompareTag(playerTag)) return;

        // Kiểm tra nếu cửa này yêu cầu ban đêm
        if (requireNight && DayManager.Instance != null && !DayManager.Instance.isNight)
        {
            Debug.Log("Cửa này chỉ có thể vào ban đêm!");
            return;
        }

        // Lấy tên scene thực tế
        string targetScene = sceneToLoad;
        
        // Nếu là ban đêm và CÓ set scene buổi tối riêng thì mới load bản tối
        if (DayManager.Instance != null && DayManager.Instance.isNight && !string.IsNullOrEmpty(sceneToLoadNight))
        {
            targetScene = sceneToLoadNight;
        }

        NextSpawnId = spawnIdInNextScene;
        SceneManager.LoadScene(targetScene);
    }
}
