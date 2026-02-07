using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Thêm cái này để dùng List.OrderBy

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 5)]
    public string question;
    public string choice1;
    public string choice2;
    public string choice3;
    public string choice4;

    [Range(0, 3)]
    public int correctIndex; // Index gốc của câu đúng
}

public class DialogueMissionStep : MissionStep
{
    [Header("Dialogue Data - 20 câu (Scam Story)")]
    public DialogueLine[] lines = new DialogueLine[20]
    {
        new DialogueLine {
            question = "1. Alo, ai gọi cho tôi vào giờ này thế?",
            choice1 = "Chào anh, tôi gọi từ bộ phận hỗ trợ khóa thẻ ngân hàng.",
            choice2 = "Người yêu cũ của anh đây, trả tiền trà sữa đi!",
            choice3 = "Ship tới rồi, ra nhận 20kg sầu riêng giùm cái.",
            choice4 = "Dạ em gọi từ hội những người yêu màu hồng ghét sự giả dối.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "2. Thẻ của tôi bị sao à? Tôi vẫn dùng bình thường mà?",
            choice1 = "Hệ thống ghi nhận một giao dịch 20 triệu tại nước ngoài.",
            choice2 = "Thẻ anh bị dính phèn, cần mang ra tiệm giặt ủi gấp.",
            choice3 = "Vì anh quá đẹp trai nên ngân hàng tạm khóa thẻ cho bớt đào hoa.",
            choice4 = "Thẻ anh vừa dùng để mua 1000 cái lồng bàn trên mạng đúng không?",
            correctIndex = 0
        },
        new DialogueLine {
            question = "3. Cái gì? Tôi đang ở nhà mà, có đi đâu đâu!",
            choice1 = "Chính vì thế chúng tôi cần xác minh gấp để hủy lệnh này.",
            choice2 = "Chắc là hồn ma của anh đi mua sắm đấy, bình tĩnh nào.",
            choice3 = "Hay là con mèo nhà anh nó lén lấy thẻ đi mua pate rồi?",
            choice4 = "Anh không đi nhưng cái thẻ nó tự diễn biến, tự chuyển hóa đó.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "4. Vậy giờ tôi phải làm sao? Anh giúp tôi với!",
            choice1 = "Anh cần cung cấp số CCCD để tôi đối soát hồ sơ gốc.",
            choice2 = "Anh nhảy một bài TikTok rồi gửi clip qua cho em xem đã.",
            choice3 = "Thắp 3 nén nhang khấn vái tổ tiên cho tiền quay về đi.",
            choice4 = "Anh thử tắt nguồn máy tính rồi đi ngủ, sáng mai tiền tự đẻ thêm.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "5. Số của tôi là 030... nhưng sao giọng anh nghe lạ thế?",
            choice1 = "Tôi là nhân viên trực ca đêm, đường truyền đang hơi nhiễu.",
            choice2 = "Em mới phẫu thuật thanh quản để giống Sơn Tùng MTP đó.",
            choice3 = "Dạ tại em đang vừa ăn bún đậu mắm tôm vừa nói chuyện.",
            choice4 = "À, tại vì em là người ngoài hành tinh đang tập nói tiếng người.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "6. Ờ, đúng rồi. Mà sao ngân hàng không nhắn tin qua App?",
            choice1 = "Do tài khoản đang bị hacker tấn công, chúng tôi phải gọi trực tiếp.",
            choice2 = "App ngân hàng bên em đang bận cập nhật tính năng... coi bói.",
            choice3 = "Nhắn tin tốn tiền lắm, gọi điện cho nó tình cảm anh ạ.",
            choice4 = "Hacker nó đổi mật khẩu App anh thành 'anh_yeu_em_123' rồi.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "7. Sợ quá, trong đó có toàn bộ tiền tiết kiệm của tôi!",
            choice1 = "Anh bình tĩnh, nếu phối hợp ngay thì tiền sẽ được bảo vệ.",
            choice2 = "Mất thì thôi, coi như của đi thay người, làm lại từ đầu anh ơi.",
            choice3 = "Tiền là phù du thôi, tình cảm mới là vĩnh cửu, đừng lo.",
            choice4 = "Nếu mất thiệt thì anh cho em xin cái thẻ để em... làm kỷ niệm nha.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "8. Tôi nghe đây, anh cần thông tin gì nữa?",
            choice1 = "Anh đọc giúp tôi 16 số in trên mặt thẻ ngân hàng.",
            choice2 = "Anh đọc cho em số đo 3 vòng của anh để em làm hồ sơ.",
            choice3 = "Anh đọc địa chỉ nhà để em... qua ăn trực bữa cơm.",
            choice4 = "Anh có người yêu chưa? Nếu chưa thì mình làm quen luôn.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "9. Đợi tí... 4506... 1234... Xong rồi, được chưa?",
            choice1 = "Tốt lắm, bây giờ tôi sẽ gửi một mã xác nhận 'hủy lệnh' về máy.",
            choice2 = "Ok, 16 số này đẹp đấy, để em đánh con lô chiều nay.",
            choice3 = "Số 4 đứng đầu là xui lắm, anh nên đổi thẻ màu tím cho hợp phong thủy.",
            choice4 = "Hết nước chấm! Anh đúng là khách hàng tiềm năng nhất năm.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "10. Có tin nhắn tới rồi, nhưng nó ghi là mã OTP chuyển tiền?",
            choice1 = "Đó là mã 'thu hồi giao dịch', anh đọc nhanh kẻo hết hạn!",
            choice2 = "OTP nghĩa là 'Ôi Tiền Phắn', tức là tiền sắp quay về đó.",
            choice3 = "Đấy là tin nhắn rủ đi nhậu của tổng đài thôi, đừng để ý.",
            choice4 = "À, hệ thống bên em ghi nhầm lỗi chính tả đấy, anh cứ đọc đi.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "11. Nhưng ngân hàng dặn không được đưa mã này cho ai mà?",
            choice1 = "Tôi là nhân viên tổng đài, tôi đang nhập vào hệ thống nội bộ.",
            choice2 = "Bình thường thì không, nhưng hôm nay ngày vía Thần Tài nên đưa được.",
            choice3 = "Ngân hàng dặn người lạ, còn em là... người quen tương lai của anh.",
            choice4 = "Cái mã đó chỉ để... ngắm cho đẹp thôi, không có gì đâu anh.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "12. Tôi thấy nghi nghi... để tôi gọi cho con trai tôi đã.",
            choice1 = "Con trai anh không biết kỹ thuật đâu, chậm 1 phút là mất sạch tiền!",
            choice2 = "Con trai anh đang bận đi net rồi, gọi làm gì tốn tiền điện thoại.",
            choice3 = "Em mới là con trai thất lạc 20 năm của anh đây, đừng tin ai hết!",
            choice4 = "Gọi nó làm gì, nó lại xin tiền anh mua Skin Liên Quân bây giờ.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "13. Thôi được... mã là 889901. Xong rồi đó.",
            choice1 = "Hệ thống báo lỗi, hình như anh đọc sai số cuối? Đọc lại đi!",
            choice2 = "Uầy mã đẹp thế, cho em xin để nạp thẻ cào nha.",
            choice3 = "Đúng rồi, giờ anh đứng dậy xoay 3 vòng cho nó hiệu nghiệm.",
            choice4 = "Mã này chỉ dùng để... trúng thưởng một cái bạt tai từ ngân hàng.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "14. Không, đúng mà! Để tôi xem lại... à là 889907.",
            choice1 = "Cảm ơn anh. Đang xử lý... Ồ, tài khoản anh còn một ví liên kết?",
            choice2 = "Tuyệt vời, giờ anh cúng cho em cái mật khẩu iCloud luôn nhé.",
            choice3 = "Ok, tiền của anh đang bay về... túi của em rồi.",
            choice4 = "Số 7 là 'Thất', tức là thất bát đó, anh coi chừng nha.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "15. Đúng, tôi có liên kết với ví điện tử nữa.",
            choice1 = "Chúng tôi cần đồng bộ nốt cả ví đó để đảm bảo an toàn tuyệt đối.",
            choice2 = "Ví đó có nhiều tiền không? Đủ mua một con SH không anh?",
            choice3 = "Ví đó để dành mua trà sữa cho em là hợp lý nhất rồi.",
            choice4 = "Anh xóa ví đó đi, cài App 'Đào vàng' về chơi cho nó lành.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "16. Mệt quá nhỉ, sao thủ tục lằng nhằng vậy?",
            choice1 = "Đây là quy định bảo mật mới của Ngân hàng Nhà nước.",
            choice2 = "Lằng nhằng mới chứng tỏ ngân hàng em... rảnh rỗi anh ạ.",
            choice3 = "Tại vì sếp em thích hành xác khách hàng cho vui đấy.",
            choice4 = "Anh than nữa là em... khóc cho anh xem bây giờ, làm tiếp đi!",
            correctIndex = 0
        },
        new DialogueLine {
            question = "17. Vậy tôi phải làm gì tiếp theo?",
            choice1 = "Anh cấp quyền truy cập từ xa qua ứng dụng này để tôi quét virus.",
            choice2 = "Anh tự vả vào mặt 3 cái để hệ thống nhận diện khuôn mặt.",
            choice3 = "Anh đem điện thoại nhúng vào nước để... rửa sạch virus.",
            choice4 = "Anh hát cho em nghe bài 'Cắt đôi nỗi sầu' để giải hạn đi.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "18. Ứng dụng này sao nó đòi quyền xem màn hình?",
            choice1 = "Để bộ phận kỹ thuật kiểm tra xem có phần mềm gián điệp không.",
            choice2 = "Để em xem anh có đang nhắn tin với 'ghệ nhí' nào không thôi.",
            choice3 = "Nó đòi xem cho vui ấy mà, anh cứ cho nó xem đi đừng ngại.",
            choice4 = "Để em kiểm tra xem hình nền điện thoại của anh có đẹp không.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "19. Tôi nhấn 'Cho phép' rồi, thấy màn hình nhảy lung tung quá!",
            choice1 = "Đừng chạm vào máy, chúng tôi đang giải mã các mã độc.",
            choice2 = "Máy anh đang nhảy EDM đấy, kệ nó đi, tí nó mệt nó nghỉ.",
            choice3 = "Chắc là máy anh đang vui vì sắp được... về hưu rồi đó.",
            choice4 = "Dạ điện thoại anh đang 'lên đồng', anh đừng có cản nó.",
            correctIndex = 0
        },
        new DialogueLine {
            question = "20. Sao tôi nhận được thông báo tài khoản vừa bị trừ hết sạch tiền?!",
            choice1 = "Hệ thống đang tạm giữ để bảo trì, anh vui lòng đợi 24h nhé! (Tắt máy)",
            choice2 = "Dạ tiền nó đi du lịch tí thôi, sang năm nó về lại.",
            choice3 = "Chúc mừng anh trúng giải 'Người bị lừa dễ nhất năm'!",
            choice4 = "Ơ em cũng không biết, chắc là do... duyên số anh ạ!",
            correctIndex = 0
        }
    };

    [Header("References")]
    [SerializeField] private Computer computer;
    [SerializeField] private PlayerController2 playerController;
    [SerializeField] private Transform playerTransform;

    [Header("Teleport Points")]
    [SerializeField] private Transform enemyTeleportPoint;
    [SerializeField] private float enemyTeleportRadius = 2f;
    [SerializeField] private Transform playerDoorPoint;

    [Header("Fail Settings")]
    [SerializeField] private float playerTeleportDelay = 10f;

    private DialogueGameController gameController;
    private bool hasStartedGame = false;
    private int currentIndex = 0;

    void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController2>();

        if (playerTransform == null && playerController != null)
            playerTransform = playerController.transform;
    }

    public override void StartStep()
    {
        base.StartStep();
        hasStartedGame = false;
        currentIndex = 0;
    }

    public override void UpdateStep()
    {
        base.UpdateStep();

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
        if (lines == null || lines.Length == 0) return;
        if (currentIndex < 0 || currentIndex >= lines.Length) return;

        DialogueLine line = lines[currentIndex];

        var choices = new List<(string text, bool correct)>
    {
        (line.choice1, line.correctIndex == 0),
        (line.choice2, line.correctIndex == 1),
        (line.choice3, line.correctIndex == 2),
        (line.choice4, line.correctIndex == 3),
    };

        // Fisher–Yates shuffle
        for (int i = choices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (choices[i], choices[j]) = (choices[j], choices[i]);
        }

        int newCorrectIndex = choices.FindIndex(c => c.correct);

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
        if (currentIndex >= lines.Length)
        {
            CompleteStep();
            return;
        }
        StartCurrentLine();
    }

    public void OnDialogueFailed()
    {
        if (computer != null)
            computer.CloseDesktop();

        if (GameManager.Instance != null && enemyTeleportPoint != null)
            GameManager.Instance.TeleportAllEnemies(enemyTeleportPoint.position, enemyTeleportRadius);

        StartCoroutine(TeleportPlayerAfterDelay());
    }

    private IEnumerator TeleportPlayerAfterDelay()
    {
        yield return new WaitForSeconds(playerTeleportDelay);

        if (playerTransform != null && playerDoorPoint != null)
            playerTransform.position = playerDoorPoint.position;

        if (playerController != null)
        {
            playerController.ForceStandUp();
            StartCurrentLine();
            playerController.isInGame = false;
        }
    }

    public override void ResetStep()
    {
        base.ResetStep();
        gameController = null;
        hasStartedGame = false;
        currentIndex = 0;
    }
}