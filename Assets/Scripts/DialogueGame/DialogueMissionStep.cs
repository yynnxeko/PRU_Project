using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 5)]
    public string question;
    public string choice1;
    public string choice2;
    public string choice3;
    public string choice4;

    [Range(-1, 3)]
    public int correctIndex;
}

public class DialogueMissionStep : MissionStep
{
    [Header("Scene Management")]
    [SerializeField] private string failSceneName = "Map_Lobby_punch";


    [Header("Dialogue Data - 20 câu")]
    public DialogueLine[] lines = new DialogueLine[20]
{
    // --- NGƯỜI THỨ NHẤT: CẢNH GIÁC (Câu 1-9) ---
    new DialogueLine {
        question = "1. Alo, ai gọi cho tôi vào giờ này thế?",
        choice1 = "Chào anh, tôi gọi từ bộ phận hỗ trợ từ ngân hàng.",
        choice2 = "Tôi là người yêu cũ của anh đây, mình quay lại đi!",
        choice3 = "Em bên giao hàng, anh có đơn hàng áo thun cần thanh toán.",
        choice4 = "Tôi gọi để hỏi xem anh đã ăn cơm với cá chưa?",
        correctIndex = 0
    },
    new DialogueLine {
        question = "2. Thẻ của tôi bị sao à? Tôi vẫn dùng bình thường mà?",
        choice1 = "Hệ thống ghi nhận một giao dịch 20 triệu tại nước ngoài.",
        choice2 = "Tài khoản của anh đăng nhập sai nhiều lần nên cần xác minh.",
        choice3 = "Thẻ của anh bị đưa vào danh sách rủi ro vì liên quan đường dây rửa tiền",
        choice4 = "Anh dùng thẻ để nạo dừa đúng không? Nên nó hỏng rồi.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "3. Cái gì? Tôi đang ở nhà mà, có đi đâu đâu!",
        choice1 = "Dạ đúng rồi anh. Hệ thống ghi nhận giao dịch lạ nên bên em cần anh xác minh ngay để mở khóa thẻ.",
        choice2 = "Chắc thẻ của anh tự đi shopping, mình cứ kệ nó nhé.",
        choice3 = "Hay thú cưng nhà anh/chị cầm thẻ đi mua đồ ăn rồi ạ?",
        choice4 = "Anh cứ đọc mã PIN cho em là xong, khỏi cần kiểm tra gì thêm.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "4. Vậy giờ tôi phải làm sao? Anh giúp tôi với!",
        choice1 = "Anh cần cung cấp số CCCD để tôi đối soát hồ sơ gốc.",
        choice2 = "Anh thử nhảy dây 3 cái rồi kiểm tra lại xem.",
        choice3 = "Anh ra đầu đường hét lớn 'Tôi bị lừa rồi' là xong.",
        choice4 = "Uống một ngụm nước cho đỡ sợ rồi đi ngủ đi anh.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "5. Số của tôi là 030... nhưng sao giọng anh nghe lạ thế?",
        choice1 = "Tôi là nhân viên trực ca đêm, đường truyền đang hơi nhiễu.",
        choice2 = "Em mới đi phẫu thuật thanh quản về, anh thấy quyến rũ không?",
        choice3 = "Dạ tại em đang vừa ăn bún mắm vừa nói chuyện với anh đó.",
        choice4 = "Do em mới bị vợ mắng nên giọng nó trầm mặc vậy thôi.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "6. Ờ, đúng rồi. Mà sao ngân hàng không nhắn tin qua App?",
        choice1 = "Do đây là trường hợp khẩn cấp, chúng tôi phải gọi trực tiếp.",
        choice2 = "App bên em đang bị quá tải, không nhắn tin được.",
        choice3 = "Dạ tổng đài bên em hết tiền điện thoại nên chỉ gọi được thôi.",
        choice4 = "Nhắn tin lỗi thời rồi, giờ gọi điện nghe giọng nhau mới thân mật.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "7. Sợ quá, trong đó có toàn bộ tiền tiết kiệm của tôi!",
        choice1 = "Anh bình tĩnh, nếu phối hợp ngay thì tiền sẽ được bảo vệ.",
        choice2 = "Tiền chỉ là phù du thôi, người còn là còn tất cả anh ơi.",
        choice3 = "Thôi mất thì thôi, anh còn em mà?",
        choice4 = "Anh đưa tiền đây em giữ hộ cho, hứa không tiêu một xu!",
        correctIndex = 0
    },
    new DialogueLine {
        question = "8. Tôi nghe đây, anh cần thông tin gì nữa?",
        choice1 = "Anh đọc giúp tôi 16 số in trên mặt thẻ ngân hàng.",
        choice2 = "Anh cho em biết chiều cao, cân nặng để em làm hồ sơ.",
        choice3 = "Anh đọc mật khẩu Facebook cho em vào xem ảnh tí.",
        choice4 = "Nhà anh có nuôi chó không? Chó có hay cắn người lạ không?",
        correctIndex = 0
    },
    new DialogueLine {
        question = "9. Đợi tí... 4506... 1234... Xong rồi, được chưa?",
        choice1 = "Tốt lắm, bây giờ tôi sẽ gửi một mã OTP về máy anh.",
        choice2 = "Vâng, để em đi mua tờ vé số thử vận may bằng số thẻ anh.",
        choice3 = "Đẹp rồi, giờ anh hát cho em nghe một bài rồi mình làm tiếp.",
        choice4 = "Ok rồi, anh cứ yên tâm, em xử lý xong sẽ gọi lại… hoặc không.",
        correctIndex = 0
    },

    // CÂU 10: ÉP THUA
    new DialogueLine {
        question = "10. Chờ đã! Ngân hàng dặn không đưa OTP cho ai cả. Mày là quân lừa đảo!",
        choice1 = "Anh nhầm rồi, đây là mã xác thực hủy giao dịch mà!",
        choice2 = "Em là lừa đảo đây, nhưng em là lừa đảo dễ thương nhất quả đất!",
        choice3 = "Anh nói vậy em buồn em khóc cho anh xem giờ!",
        choice4 = "Nhanh lên anh, em còn phải đi ăn đám cưới nữa!",
        correctIndex = -1
    },

    // --- NGƯỜI THỨ HAI: DỄ TIN (Câu 11-20) ---
    new DialogueLine {
        question = "11. Alo, ai gọi đấy?",
        choice1 = "Dạ em gọi từ bộ phận chăm sóc khách hàng ngân hàng, liên hệ để hỗ trợ xác minh và mở khóa thẻ cho anh ạ.",
        choice2 = "Em là thiên sứ bay ngang qua, thấy ví anh kêu cứu nên gọi luôn ạ.",
        choice3 = "Dạ em là người sẽ giữ hộ tiền anh cho an toàn trong dịp Tết ạ.",
        choice4 = "Em gọi nhầm số, nhưng lỡ rồi mình tâm sự tí đi.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "12. Thẻ bị khóa hả? May quá, làm hộ tôi với.",
        choice1 = "Dạ anh đọc mã OTP vừa gửi về máy để em xử lý.",
        choice2 = "Anh cho em mượn thẻ để em đi quẹt thử bát phở xem được không.",
        choice3 = "Anh nạp cho em cái thẻ điện thoại 500k để lấy động lực.",
        choice4 = "Anh ra giữa sân đứng một chân rồi đọc OTP cho nó thiêng.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "13. Mã là 112233. Được chưa em?",
        choice1 = "Dạ được rồi, giờ em cần liên kết với ví điện tử của anh.",
        choice2 = "Dạ chưa, số này xấu quá anh xin mã khác đẹp hơn đi.",
        choice3 = "Ok rồi, giờ anh ngồi im như tượng để em làm phép.",
        choice4 = "Mã này em thấy quen quen, hình như là ngày sinh bồ cũ em.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "14. Cứ làm đi, miễn lấy lại được tiền cho tôi.",
        choice1 = "Anh tải ứng dụng 'Hỗ trợ từ xa' này về máy giúp em nhé.",
        choice2 = "Anh gửi cho em thêm cái địa chỉ nhà để em tới chơi.",
        choice3 = "Anh chụp ảnh chân dung đang cười thật tươi gửi em nhé.",
        choice4 = "Anh đứng yên đó, hít vào thở ra đều đặn cho em.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "15. Rồi, tải xong rồi, giờ làm gì nữa?",
        choice1 = "Anh nhấn 'Cho phép' để em quét virus trên điện thoại.",
        choice2 = "Anh đập cái điện thoại xuống đất 3 cái cho nó tỉnh.",
        choice3 = "Anh tắt nguồn rồi ném máy vào chậu nước cho mát.",
        choice4 = "Anh đi mua cho em ly trà sữa rồi mình tính tiếp.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "16. Sao màn hình của tôi cứ nhảy liên tục vậy?",
        choice1 = "Dạ bên em đang giải mã các lớp bảo mật, anh đừng chạm vào.",
        choice2 = "Dạ máy anh nó đang nhảy EDM vì sắp có tiền đó anh.",
        choice3 = "Chắc máy anh nó đang tập thể dục buổi sáng thôi.",
        choice4 = "Máy anh nó đang đòi được nghỉ ngơi, anh mặc kệ nó đi.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "17. Tôi thấy thông báo trừ tiền? 5 triệu, rồi 10 triệu?",
        choice1 = "Đó là tiền đang được chuyển vào 'Két sắt an toàn' của ngân hàng.",
        choice2 = "Dạ tiền nó đi du lịch vòng quanh thế giới rồi sẽ về.",
        choice3 = "Anh hoa mắt rồi, ngân hàng đang tặng thêm tiền cho anh đó.",
        choice4 = "Tiền nó thấy anh giàu quá nên nó rủ bạn nó đi chơi bớt.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "18. Két sắt an toàn à? Vậy khi nào tôi nhận lại được?",
        choice1 = "Sau 24h bảo trì hệ thống sẽ tự động hoàn trả ạ.",
        choice2 = "Khi nào em cưới vợ em sẽ trả lại anh gấp đôi.",
        choice3 = "Dạ sau khi ngân hàng xác minh thành công sẽ hoàn về ngay anh nhé.",
        choice4 = "Năm sau anh nhé, năm nay em bận tiêu mất rồi.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "19. Cảm ơn em nhé, suýt nữa thì tôi mất tiền.",
        choice1 = "Dạ không có gì, đó là trách nhiệm của tụi em.",
        choice2 = "Vâng, anh hiền quá nên em thấy thương cho cái ví của anh.",
        choice3 = "Dạ em là thiên thần hộ mệnh của những người dễ tin mà.",
        choice4 = "Dạ em chuẩn bị đổi số điện thoại rồi, tạm biệt anh nhé.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "20. Xong chưa em ơi?",
        choice1 = "Dạ xong rồi, cảm ơn anh đã đóng góp tiền tiêu tết cho em! *Cúp máy*",
        choice2 = "Dạ xong rồi, anh ra ngân hàng vay thêm tiền rồi gọi lại em.",
        choice3 = "Dạ xong, giờ anh có thể khóc được rồi đó ạ.",
        choice4 = "Xong rồi, em đi mua iPhone 15 đây, bye anh nhé!",
        correctIndex = 0
    }
};
    [Header("References")]
    [SerializeField] private Computer computer;
    [SerializeField] private PlayerController2 playerController;

    [Header("Teleport Points")]
    [SerializeField] private Transform doorPoint;
    [SerializeField] private Transform medicalRoomPoint;
    [SerializeField] private Transform enemyTeleportPoint;

    private DialogueGameController gameController;
    private bool hasStartedGame = false;
    private int currentIndex = 0;
    private static int savedIndex = 0;

    public override void StartStep()
    {
        base.StartStep();
        hasStartedGame = false;
        currentIndex = savedIndex;

        // if (currentIndex < 10)
        //     currentIndex = 0;
    }

    public override void UpdateStep()
    {
        base.UpdateStep();

        if (playerController == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerController = playerObj.GetComponent<PlayerController2>();
        }

        if (!hasStartedGame)
        {
            gameController = FindObjectOfType<DialogueGameController>();
            if (gameController != null)
            {
                hasStartedGame = true;
                StartCurrentLine();
            }
        }
    }

    void StartCurrentLine()
    {
        if (lines == null || currentIndex >= lines.Length) return;
        DialogueLine line = lines[currentIndex];

        var choices = new List<(string text, bool isCorrect)> {
            (line.choice1, line.correctIndex == 0),
            (line.choice2, line.correctIndex == 1),
            (line.choice3, line.correctIndex == 2),
            (line.choice4, line.correctIndex == 3),
        };

        for (int i = choices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (choices[i], choices[j]) = (choices[j], choices[i]);
        }

        int newCorrectIndex = (line.correctIndex < 0) ? -1 : choices.FindIndex(c => c.isCorrect);

        gameController.StartGame(
            line.question,
            choices[0].text,
            choices[1].text,
            choices[2].text,
            choices[3].text,
            newCorrectIndex,
            this
        );
    }

    public void OnDialogueCorrect()
    {
        currentIndex++;
        savedIndex = currentIndex;
        if (currentIndex >= lines.Length) CompleteStep();
        else StartCurrentLine();
    }

    public void OnDialogueFailed()
    {
        savedIndex = currentIndex;
        if (currentIndex == 9) StartCoroutine(SpecialFailedRoutine()); // câu 10
        else StartCoroutine(NormalFailedRoutine()); // câu 1-9, 11-20
    }

    private IEnumerator SpecialFailedRoutine()
    {
        if (gameController != null)
        {
            gameController.StartGame(
                "<color=red>Mày chưa bị chích điện nữa hả =))</color>",
                "...", "...", "...", "...",
                -1,
                this
            );
            BattleState.failIndex = 10;
            DoorSceneChange.NextSpawnId = "lobby_punch";
            // SceneManager.LoadScene(failSceneName);
        }

        yield return new WaitForSecondsRealtime(3f);

        if (computer != null) computer.CloseDesktop();
        if (playerController != null) playerController.ForceStandUp();
        if (GameManager.Instance != null)
            GameManager.Instance.TeleportAllEnemies(enemyTeleportPoint.position, 2f);


        // Load scene đánh nhau
        DoorSceneChange.NextSpawnId = "lobby_punch";
        SceneManager.LoadScene(failSceneName);
    }

    private IEnumerator NormalFailedRoutine()
    {
        if (computer != null) computer.CloseDesktop();
        if (playerController != null) playerController.ForceStandUp();
        yield return new WaitForSecondsRealtime(1f);

        if (GameManager.Instance != null)
            GameManager.Instance.TeleportAllEnemies(enemyTeleportPoint.position, 2f);


        // Load scene đánh nhau
        BattleState.failIndex = currentIndex;
        DoorSceneChange.NextSpawnId = "lobby_punch";
        SceneManager.LoadScene(failSceneName);
    }

}