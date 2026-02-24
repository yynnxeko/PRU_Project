// using UnityEngine;

// public class GeneralMissionStep : MissionStep
// {
//     private int curIndex = 0;

//     [Header("Tham chiếu object nhiệm vụ")]
//     public CabinetPoint cabinet;


//     public override void StartStep()
//     {
//         base.StartStep();
//         curIndex = 0;
//         SetupCurrentMission();
//     }

//     public override void UpdateStep()
//     {
//         // Chủ động gọi NextMission() nơi khác khi NV xong
//     }

//     void SetupCurrentMission()
//     {
//         switch (curIndex)
//         {
//             case 0:
//                 Debug.Log("Nhiệm vụ 1: Lấy bằng chứng trong tủ.");
//                 // Đăng ký sự kiện lấy đồ xong ở cabinet:
//                 if (cabinet != null)
//                     cabinet.OnHoldCompleteEvent += OnCabinetLooted;
//                 break;
//             case 1:
//                 Debug.Log("Nhiệm vụ 2: Hoàn thành minigame 2.");
//                 // Đăng ký sự kiện thắng minigame:
//                 if (minigame2 != null)
//                     minigame2.OnMinigameWin += OnMinigame2Win;
//                 break;
//             case 2:
//                 Debug.Log("Nhiệm vụ 3: Nói chuyện với quản lý.");
//                 // Đăng ký khi npcManager được nói chuyện:
//                 if (npcManager != null)
//                     npcManager.OnTalked += OnManagerTalked;
//                 break;
//             default:
//                 Debug.Log("Tất cả nhiệm vụ đã hoàn thành!");
//                 break;
//         }
//     }

//     void NextMission()
//     {
//         // Huỷ đăng ký event cũ trước khi sang bước mới!
//         switch (curIndex)
//         {
//             case 0:
//                 if (cabinet != null)
//                     cabinet.OnHoldCompleteEvent -= OnCabinetLooted;
//                 break;
//             case 1:
//                 if (minigame2 != null)
//                     minigame2.OnMinigameWin -= OnMinigame2Win;
//                 break;
//             case 2:
//                 if (npcManager != null)
//                     npcManager.OnTalked -= OnManagerTalked;
//                 break;
//         }
//         curIndex++;
//         if (curIndex >= 3)
//         {
//             CompleteStep();
//             Debug.Log("Đã hoàn thành toàn bộ nhiệm vụ!");
//         }
//         else
//         {
//             SetupCurrentMission();
//         }
//     }

//     // Các hàm callback nhận event:
//     void OnCabinetLooted()
//     {
//         Debug.Log("Đã lấy bằng chứng trong tủ => Chuyển nhiệm vụ 2.");
//         NextMission();
//     }
//     void OnMinigame2Win()
//     {
//         Debug.Log("Đã hoàn thành minigame 2 => Chuyển nhiệm vụ 3.");
//         NextMission();
//     }
//     void OnManagerTalked()
//     {
//         Debug.Log("Đã nói chuyện xong với quản lý => Hoàn thành nhiệm vụ!");
//         NextMission();
//     }
// }