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


    [Header("Dialogue Data - 40 câu")]
    public DialogueLine[] lines = new DialogueLine[]
{
    // KỊCH BẢN 1 : Giả Danh Nhân Viên Ngân Hàng
    new DialogueLine {
        question = "1. Alo, ai gọi vậy?",
        choice1 = "Chào anh, tôi gọi từ bộ phận an ninh ngân hàng vì tài khoản anh có dấu hiệu bất thường.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "2. Bất thường gì? Tôi vẫn dùng bình thường mà.",
        choice1 = "Hệ thống ghi nhận giao dịch 35 triệu tại Singapore cách đây 10 phút.",
        choice2 = "Tài khoản anh vừa mua 3kg xoài online.",
        choice3 = "Anh đăng nhập Facebook sai 2 lần.",
        choice4 = "Anh bị chọn ngẫu nhiên để kiểm tra thôi.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "3. Tôi đang ở nhà, không hề giao dịch gì cả!",
        choice1 = "Vậy khả năng cao tài khoản anh đã bị chiếm quyền. Nếu không xử lý ngay, tiền sẽ bị trừ hết.",
        choice2 = "Chắc hệ thống nhầm, thôi kệ đi anh.",
        choice3 = "Hay anh thử tắt máy mở lại.",
        choice4 = "Không sao đâu, mất thì mất.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "4. Trời ơi! Giờ tôi phải làm sao?",
        choice1 = "Anh cần xác minh gấp để chúng tôi khóa giao dịch trong vòng 5 phút.",
        choice2 = "Anh bình tĩnh uống nước đi đã.",
        choice3 = "Anh chuyển tiền qua tôi giữ hộ cho an toàn.",
        choice4 = "Anh ra ngân hàng ngày mai cũng được.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "5. Xác minh như thế nào?",
        choice1 = "Anh đọc giúp tôi 16 số trên mặt thẻ để đối chiếu hệ thống.",
        choice2 = "Anh đọc mật khẩu Facebook cho tôi.",
        choice3 = "Anh gửi ảnh gia đình cho tôi kiểm tra.",
        choice4 = "Anh đọc biển số xe nhà anh.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "6. Ờ… 4506… nhưng sao anh không gọi từ tổng đài chính thức?",
        choice1 = "Đây là đường dây nội bộ khẩn cấp, tổng đài sẽ không xử lý kịp.",
        choice2 = "Tổng đài hết tiền điện thoại rồi.",
        choice3 = "Tôi dùng số riêng cho tiện.",
        choice4 = "Anh hỏi nhiều quá rồi đó.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "7. Tôi vừa nhận được mã OTP từ ngân hàng.",
        choice1 = "Anh đọc ngay mã OTP đó cho tôi để hủy giao dịch 35 triệu.",
        choice2 = "Anh giữ lại làm kỷ niệm.",
        choice3 = "Anh đăng lên Facebook hỏi thử.",
        choice4 = "Anh gửi cho bạn thân anh.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "8. Ngân hàng có dặn không cung cấp OTP cho ai cả mà?",
        choice1 = "Đây là mã hủy giao dịch chứ không phải OTP thanh toán, anh yên tâm.",
        choice2 = "Anh tin tôi đi, tôi làm ở đây 10 năm rồi.",
        choice3 = "Nếu anh không đọc, tiền sẽ mất ngay lập tức.",
        choice4 = "Anh không đọc thì thôi.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "9. Tôi vẫn thấy không yên tâm...",
        choice1 = "Anh còn 2 phút trước khi hệ thống tự động trừ tiền. Anh quyết định nhanh.",
        choice2 = "Thôi mất thì mất vậy.",
        choice3 = "Anh cứ chuyển 5 triệu trước đã.",
        choice4 = "Anh hát cho tôi nghe rồi tính tiếp.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "10. Khoan đã! Tôi sẽ tự gọi tổng đài chính thức của ngân hàng để kiểm tra!",
        choice1 = "Anh không cần làm vậy, tôi đang xử lý trực tiếp cho anh!",
        choice2 = "Ừ… thôi vậy tôi cúp máy đây.",
        choice3 = "Nhanh lên anh, tôi còn phải gọi người khác.",
        choice4 = "Anh hiểu lầm rồi!",
        correctIndex = -1
    },

    // KỊCH BẢN 2: GIẢ DANH NGƯỜI THÂN GẶP TAI NẠN
    new DialogueLine {
        question = "1. Alo? Ai đang gọi cho tôi vậy?",
        choice1 = "Chị ơi, em là bác sĩ ở bệnh viện. Người nhà chị vừa bị tai nạn nghiêm trọng.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "2. Cái gì? Ai bị tai nạn?",
        choice1 = "Anh Nam – chồng chị – đang cấp cứu. Tình trạng rất nguy kịch.",
        choice2 = "Một người giống người nhà chị thôi, chắc không sao đâu.",
        choice3 = "Em cũng không rõ ai, chị đoán thử đi.",
        choice4 = "Chị chuyển tiền trước rồi em nói tiếp.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "3. Trời ơi! Anh ấy sáng nay vẫn đi làm bình thường mà!",
        choice1 = "Tai nạn xảy ra 20 phút trước. Hiện anh ấy mất nhiều máu và cần phẫu thuật gấp.",
        choice2 = "Chắc do chạy nhanh quá thôi ạ.",
        choice3 = "Em cũng không biết nữa, tại số anh ấy xui.",
        choice4 = "Thôi chắc nhầm rồi, chị đừng lo.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "4. Tôi phải làm gì bây giờ?",
        choice1 = "Chị cần chuyển gấp 50 triệu tiền tạm ứng viện phí để chúng tôi tiến hành mổ ngay.",
        choice2 = "Chị cứ cầu nguyện đi ạ.",
        choice3 = "Chị đăng Facebook hỏi mượn tiền thử.",
        choice4 = "Chị ngủ một giấc rồi tính.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "5. Tôi muốn nói chuyện với anh ấy!",
        choice1 = "Hiện anh ấy đang hôn mê, không thể nghe máy. Thời gian rất gấp chị ạ.",
        choice2 = "Để em đánh thức anh ấy dậy nghe máy nhé.",
        choice3 = "Anh ấy đang bận chơi game.",
        choice4 = "Chị gọi thử số anh ấy xem sao.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "6. Tôi sẽ đến bệnh viện ngay!",
        choice1 = "Chị đừng đến vội, quy trình phòng dịch đang siết chặt. Chị chuyển khoản trước để không mất thời gian.",
        choice2 = "Chị đến cũng được, nhưng em không biết bệnh viện ở đâu.",
        choice3 = "Chị cứ đến đại bệnh viện nào gần nhà.",
        choice4 = "Chị đến nhớ mang bánh cho em nhé.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "7. Sao tôi chưa thấy bệnh viện gọi cho tôi bao giờ?",
        choice1 = "Đây là trường hợp khẩn cấp nên chúng tôi gọi trực tiếp từ số cá nhân.",
        choice2 = "Bệnh viện hết tiền điện thoại rồi chị.",
        choice3 = "Do hôm nay tổng đài nghỉ lễ.",
        choice4 = "Em gọi nhầm nhưng thôi chị chuyển tiền luôn đi.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "8. Tôi cần xác minh thông tin trước khi chuyển tiền.",
        choice1 = "Không còn thời gian đâu chị! Nếu chậm trễ, tính mạng anh ấy sẽ nguy hiểm.",
        choice2 = "Chị cứ bình tĩnh rồi tính.",
        choice3 = "Thôi để mai tính cũng được.",
        choice4 = "Chị thích thì xác minh, em đi ăn cơm đây.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "9. Tài khoản chuyển tiền là gì?",
        choice1 = "Chị chuyển vào tài khoản cá nhân của tôi để xử lý nhanh, nội dung ghi 'viện phí khẩn cấp'.",
        choice2 = "Chị đưa tiền mặt cho bảo vệ bệnh viện.",
        choice3 = "Chị gửi qua ví game cho tiện.",
        choice4 = "Chị đọc mật khẩu ATM cho em là được.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "10. Khoan đã! Tôi sẽ gọi trực tiếp cho chồng tôi và bệnh viện kiểm tra trước!",
        choice1 = "Chị không được gọi cho ai hết! Vi phạm quy định điều tra!",
        choice2 = "Thôi bị phát hiện rồi, em cúp máy đây!",
        choice3 = "Chị gọi đi rồi nhớ chuyển tiền nhé.",
        choice4 = "Nhanh lên chị, em còn phải gọi người khác nữa!",
        correctIndex = 0
    },
    // KỊCH BẢN 3: LỪA ĐẢO RỬA TIỀN
    new DialogueLine {
        question = "1. Alo? Ai đang gọi cho tôi vậy?",
        choice1 = "Chào anh, tôi là cán bộ điều tra từ Cục phòng chống tội phạm công nghệ cao.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "2. Công an gọi cho tôi làm gì?",
        choice1 = "Chúng tôi phát hiện số CMND của anh liên quan đến một tài khoản nhận 3 tỷ đồng bất hợp pháp.",
        choice2 = "Anh trúng thưởng xe SH nhưng quên nhận đó ạ.",
        choice3 = "Chúng tôi gọi hỏi anh ăn cơm chưa thôi.",
        choice4 = "Tại vì hôm nay rảnh quá nên gọi đại.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "3. Cái gì? Tôi không liên quan gì hết!",
        choice1 = "Nếu anh không hợp tác, chúng tôi sẽ chuyển hồ sơ sang Viện kiểm sát để phát lệnh bắt khẩn cấp.",
        choice2 = "Thôi không sao đâu anh, nhầm người rồi, bye nhé.",
        choice3 = "Hay anh chuyển tiền cho tôi để tôi bỏ qua vụ này?",
        choice4 = "Anh thử đổi tên Facebook xem có hết không.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "4. Trời ơi! Vậy giờ tôi phải làm sao?",
        choice1 = "Anh cần giữ bí mật tuyệt đối và làm theo hướng dẫn để chứng minh mình vô tội.",
        choice2 = "Anh đăng status Facebook cầu cứu thử xem.",
        choice3 = "Anh ra công an phường hỏi thử đi.",
        choice4 = "Anh cứ ngủ một giấc rồi tính tiếp.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "5. Sao lại phải giữ bí mật?",
        choice1 = "Vì vụ án đang điều tra đặc biệt, nếu lộ thông tin anh sẽ bị xem là cản trở điều tra.",
        choice2 = "Tại tôi thích bí mật cho hồi hộp.",
        choice3 = "Vì tôi sợ người khác biết tôi gọi anh.",
        choice4 = "Vì nói ra sẽ mất thiêng.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "6. Tôi run quá... tôi cần làm gì để chứng minh?",
        choice1 = "Anh cần chuyển toàn bộ tiền trong tài khoản sang tài khoản tạm giữ của cơ quan điều tra để xác minh nguồn tiền.",
        choice2 = "Anh gửi tôi 100k làm phí tư vấn trước đã.",
        choice3 = "Anh đọc mật khẩu Wifi nhà anh cho tôi kiểm tra.",
        choice4 = "Anh chụp hình sổ hộ khẩu gửi tôi cho vui.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "7. Nhưng đó là toàn bộ tiền tiết kiệm của tôi!",
        choice1 = "Nếu anh không chuyển ngay trong 30 phút, hệ thống sẽ tự động phong tỏa và anh có thể bị bắt giữ.",
        choice2 = "Thôi mất thì thôi, tiền là vật ngoài thân.",
        choice3 = "Anh cho tôi mượn ít tiêu trước cũng được.",
        choice4 = "Anh đưa tôi giữ hộ, tôi tiêu giùm cho.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "8. Tôi vừa nhận được mã OTP từ ngân hàng.",
        choice1 = "Anh đọc ngay mã OTP đó cho tôi để hoàn tất xác minh.",
        choice2 = "Anh giữ lại để chơi xổ số.",
        choice3 = "Anh đăng lên Facebook hỏi mọi người thử.",
        choice4 = "Anh gửi cho bạn gái anh xem thử.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "9. Tôi đọc đây... 563921... Xong rồi nhé?",
        choice1 = "Tốt lắm, hệ thống đang xử lý. Anh đừng tắt máy và không được nói với ai.",
        choice2 = "Được rồi, giờ anh nhảy 3 cái cho may mắn.",
        choice3 = "Hay anh kể tôi nghe chuyện tình của anh đi.",
        choice4 = "Thôi tôi bận rồi, tự lo đi nhé.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "10. Khoan đã! Công an không bao giờ yêu cầu chuyển tiền hay đọc OTP! Anh là lừa đảo!",
        choice1 = "Anh hiểu nhầm rồi, đây là quy trình điều tra đặc biệt!",
        choice2 = "Ừ tôi lừa đó, nhưng lừa có tâm mà!",
        choice3 = "Thôi bị phát hiện rồi thì tôi cúp máy đây!",
        choice4 = "Nhanh lên anh, tôi còn phải gọi người khác nữa!",
        correctIndex = 0
    },

    // KỊCH BẢN4 : GIẢ DANH SHIPPER THU PHÍ COD
    new DialogueLine {
        question = "1. Alo, ai gọi vậy?",
        choice1 = "Chào anh/chị, em là shipper. Anh/chị có đơn hàng cần thanh toán ngay để em giao.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "2. Tôi không nhớ đặt gì cả, đơn gì vậy?",
        choice1 = "Dạ đơn 'hàng tiêu dùng' COD 1.850.000đ, bên em chỉ có mã đơn chứ không xem được chi tiết.",
        choice2 = "Dạ đơn là… một giấc mơ đẹp, anh/chị nhận giúp em.",
        choice3 = "Đơn gì không quan trọng, quan trọng là anh/chị chuyển tiền trước.",
        choice4 = "Em gọi nhầm số, nhưng thôi nhận luôn cho vui ạ.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "3. Sao lại không xem được chi tiết? Vậy ai gửi?",
        choice1 = "Dạ hệ thống ghi 'quà tặng', em không có tên shop. Anh/chị thanh toán trước em mới giao được ạ.",
        choice2 = "Em cũng không biết ai gửi, nhưng em biết anh/chị cần nó.",
        choice3 = "Ai gửi không quan trọng, anh/chị cứ trả tiền là em vui.",
        choice4 = "Chắc người ngoài hành tinh gửi đó ạ.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "4. Tôi muốn nhận hàng rồi trả tiền mặt khi xem được hàng.",
        choice1 = "Dạ không được ạ, quy định mới: phải chuyển khoản trước để 'xác nhận nhận hàng' rồi em mới giao.",
        choice2 = "Được ạ, nhưng anh/chị phải đoán đúng màu áo em đang mặc.",
        choice3 = "Anh/chị cứ trả bằng kẹo cũng được.",
        choice4 = "Trả tiền mặt là lỗi thời rồi ạ.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "5. Nghe lạ quá. Nếu tôi không nhận thì sao?",
        choice1 = "Nếu anh/chị không nhận sẽ bị tính 'phí hoàn đơn + phí lưu kho' và bên em báo hệ thống khóa nhận hàng.",
        choice2 = "Không nhận thì em buồn thôi ạ.",
        choice3 = "Không nhận thì em… đi ăn bún bò.",
        choice4 = "Không nhận thì đơn tự biến mất như phép thuật.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "6. Tôi cần kiểm tra đơn trên app/website hãng vận chuyển.",
        choice1 = "Dạ anh/chị không cần kiểm tra đâu, em gửi link 'xác nhận COD' của bên em, anh/chị bấm vào là ra ngay.",
        choice2 = "Anh/chị kiểm tra làm gì cho mệt, tin em đi ạ.",
        choice3 = "Em gửi link xem tử vi cho chuẩn hơn.",
        choice4 = "Bấm link xong nhớ quay video reaction giúp em nhé.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "7. Link lạ tôi không bấm. Bạn đọc mã vận đơn/tên hãng cho tôi.",
        choice1 = "Dạ em chỉ có mã nội bộ. Anh/chị chuyển trước 50k 'phí giữ hàng' để em mở thông tin đơn trên hệ thống.",
        choice2 = "Mã vận đơn là: 0000-đừng-hỏi-nhiều-ạ.",
        choice3 = "Tên hãng là: Hãng Gió Bay.",
        choice4 = "Anh/chị chuyển 5k cũng được, cho có động lực.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "8. Tôi chỉ trả tiền khi nhận hàng. Bạn giao đến địa chỉ tôi xem.",
        choice1 = "Dạ em đang đứng gần nhà anh/chị rồi, nếu không chuyển trong 5 phút em hoàn đơn ngay và anh/chị chịu phí.",
        choice2 = "Em đứng gần nhà anh/chị… trong tưởng tượng ạ.",
        choice3 = "5 phút nữa em teleport tới nhé.",
        choice4 = "Em hoàn đơn nhưng vẫn ship tiếp cho vui.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "9. Tôi nghi ngờ lừa đảo. Bạn cần gì để tôi xác minh?",
        choice1 = "Dạ em sẽ gửi mã OTP về máy anh/chị để 'xác nhận hoàn phí'. Anh/chị đọc OTP cho em nhé.",
        choice2 = "Anh/chị đọc giúp em mật khẩu Facebook để em tin anh/chị.",
        choice3 = "Anh/chị chụp thẻ ngân hàng gửi em cho nhanh.",
        choice4 = "Anh/chị đọc tên thú cưng để em xác minh tình cảm.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "10. Khoan! Shipper không bao giờ yêu cầu chuyển khoản/OTP. Tôi sẽ gọi hãng vận chuyển và báo cáo số này!",
        choice1 = "Anh/chị hiểu nhầm rồi, đây là quy trình mới mà!",
        choice2 = "Thôi bị phát hiện rồi, em xin lỗi… (cúp máy).",
        choice3 = "Anh/chị đừng căng, em đùa thôi mà.",
        choice4 = "Nhanh lên anh/chị, em còn phải gọi người khác nữa!",
        correctIndex = 0
    },

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

    /// <summary>
    /// Reset tiến độ câu hỏi về đầu ngày. Gọi khi FailDay hoặc AdvanceDay.
    /// </summary>
    public static void ResetSavedIndex()
    {
        savedIndex = 0;
    }

    public override void StartStep()
    {
        base.StartStep();
        hasStartedGame = false;
        currentIndex = savedIndex;
    }

    /// <summary>
    /// Được DialogueGameController gọi khi nó vừa Start() xong.
    /// </summary>
    public static void NotifyControllerReady(DialogueGameController controller)
    {
        // Tìm instance DialogueMissionStep đang active trong scene
        var step = FindObjectOfType<DialogueMissionStep>();
        if (step != null)
        {
            // Luôn gắn lại controller mới (fix lỗi vào lần 2 không hoạt động)
            step.gameController = controller;
            step.hasStartedGame = true;

            if (step.playerController == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                    step.playerController = playerObj.GetComponent<PlayerController2>();
            }

            step.StartCurrentLine();
            Debug.Log($"[DialogueMissionStep] Controller ready → StartCurrentLine() (index={step.currentIndex})");
        }
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

        // Fallback: nếu NotifyControllerReady chưa kịp gọi
        if (!hasStartedGame)
        {
            gameController = DialogueGameController.Instance;
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

        // Mỗi khi xong đủ 10 câu (10, 20, 30, 40...) → tắt máy, đứng dậy, bật cờ, chuyển buổi
        if (currentIndex > 0 && currentIndex % 10 == 0)
        {
            // Tắt máy tính
            if (computer != null) computer.CloseDesktop();

            // Đứng dậy khỏi ghế
            if (playerController != null)
            {
                playerController.isInGame = false;
                playerController.ForceStandUp();
            }

            // Bật cờ để NpcSceneEntry kích hoạt enemy dắt đi ăn
            if (GameFlagManager.Instance != null)
                GameFlagManager.Instance.SetFlag("it_toLobby", true);

            // Chuyển buổi (Morning → Noon → Night)
            if (DayManager.Instance != null)
                DayManager.Instance.AdvancePhase();

            return; // Không StartCurrentLine nữa, chờ NarrativeDirector
        }

        if (currentIndex < lines.Length)
            StartCurrentLine();
    }

    public void OnDialogueFailed()
    {
        savedIndex = currentIndex;
        StartCoroutine(FailedRoutine());
    }

    // === Logic cũ câu 10 (tạm comment, chờ gắn flag nhận nhiệm vụ) ===
    // private IEnumerator SpecialFailedRoutine()
    // {
    //     if (gameController != null)
    //     {
    //         gameController.StartGame(
    //             "<color=red>Mày chưa bị chích điện nữa hả =))</color>",
    //             "...", "...", "...", "...",
    //             -1,
    //             this
    //         );
    //     }
    //     yield return new WaitForSecondsRealtime(3f);
    //     if (computer != null) computer.CloseDesktop();
    //     if (playerController != null) playerController.ForceStandUp();
    //     if (GameManager.Instance != null)
    //         GameManager.Instance.TeleportAllEnemies(enemyTeleportPoint.position, 2f);
    //     DoorSceneChange.NextSpawnId = "lobby_punch";
    //     SceneManager.LoadScene(failSceneName);
    // }

    private IEnumerator FailedRoutine()
    {
        if (computer != null) computer.CloseDesktop();
        if (playerController != null)
        {
            playerController.isInGame = false;
            playerController.ForceStandUp();
        }
        yield return new WaitForSecondsRealtime(1f);

        if (GameManager.Instance != null)
            GameManager.Instance.TeleportAllEnemies(enemyTeleportPoint.position, 2f);

        // Nếu đã nhận nhiệm vụ (flag bật) → sai sẽ đi medical
        if (GameFlagManager.Instance != null && GameFlagManager.Instance.GetFlag("mission_accepted"))
            GameFlagManager.Instance.SetFlag("go_to_medical", true);

        DoorSceneChange.NextSpawnId = "lobby_punch";
        SceneManager.LoadScene(failSceneName);
    }

}