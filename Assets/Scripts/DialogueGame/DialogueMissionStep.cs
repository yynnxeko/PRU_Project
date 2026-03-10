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
        correctIndex = 0
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

		// Giả Danh Điện Lực 
    new DialogueLine {
        question = "1. Alo, ai đang gọi cho tôi vậy?",
        choice1 = "Chào anh/chị, em gọi từ bên điện lực vì hợp đồng điện nhà mình đang có cảnh báo khẩn.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "2. Cảnh báo gì? Nhà tôi vẫn có điện bình thường mà.",
        choice1 = "Hệ thống báo công tơ nhà anh/chị chưa hoàn tất xác thực nên có thể bị ngắt điện trong hôm nay.",
        choice2 = "Nhà mình dùng điện rất đều nên bên em gọi hỏi thăm cho yên tâm thôi ạ.",
        choice3 = "Có thể công tơ đang mệt chút thôi, anh/chị cứ để nó nghỉ rồi kiểm tra lại sau.",
        choice4 = "Thật ra em gọi nhầm số, nhưng tiện thì mình trò chuyện vài phút cũng được ạ.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "3. Tôi không hề nhận được hóa đơn hay tin nhắn nào cả.",
        choice1 = "Đây là cảnh báo nội bộ phát sinh gấp nên bên em gọi trực tiếp để hỗ trợ xử lý trước cho mình.",
        choice2 = "Chắc hệ thống bận quá nên quên gửi, nhưng mình cứ làm trước rồi tính tiếp cũng được ạ.",
        choice3 = "Tin nhắn có khi đang đi lạc đâu đó, miễn giờ em gọi được cho anh/chị là ổn rồi.",
        choice4 = "Nếu chưa nhận thì thôi cũng không sao, nhưng em nghĩ mình cứ xử lý cho có cảm giác an toàn.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "4. Vậy bây giờ tôi cần làm gì?",
        choice1 = "Anh/chị cần xác nhận lại thông tin chủ hợp đồng để em giữ điện tạm thời trước khi hệ thống khóa.",
        choice2 = "Anh/chị cứ tắt cầu dao ba phút rồi mở lại, đôi khi may mắn sẽ tự quay về ngay đó ạ.",
        choice3 = "Mình đợi thêm chút nữa xem điện có tự hiểu chuyện rồi sáng đẹp trở lại hay không nhé.",
        choice4 = "Anh/chị cứ bình tĩnh trước đã, vì đôi khi hoảng quá lại làm mình quên mất chuyện khác.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "5. Xác nhận bằng cách nào? Tôi thấy hơi lạ.",
        choice1 = "Em sẽ gửi một đường link nội bộ, anh/chị mở ra và điền thông tin để xác thực công tơ ngay.",
        choice2 = "Anh/chị không cần lo đâu, link lạ một chút nhưng nhìn quen dần thì sẽ thấy bình thường thôi.",
        choice3 = "Nếu ngại bấm link thì mình cứ nhìn thử trước, biết đâu giao diện đẹp lại muốn làm tiếp ạ.",
        choice4 = "Thời nay thủ tục chỗ nào cũng có link cả, nên mình đừng để chuyện đó làm căng thẳng thêm nhé.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "6. Tôi muốn tự gọi tổng đài điện lực để hỏi trước.",
        choice1 = "Anh/chị đừng gọi lúc này vì hồ sơ đang treo xử lý, gọi ra ngoài dễ làm chậm quy trình xác nhận.",
        choice2 = "Gọi cũng được thôi, nhưng biết đâu chờ máy lâu hơn thì lại mất thời gian hơn nhiều đấy ạ.",
        choice3 = "Nếu gọi thì lát nữa mình gọi cũng chưa muộn, còn bây giờ em đang hỗ trợ trực tiếp cho nhanh.",
        choice4 = "Em nghĩ gọi hay không cũng vậy thôi, miễn cuối cùng anh/chị vẫn quay lại xử lý bước này nhé.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "7. Tôi vừa mở link, giờ nó yêu cầu nhập tài khoản ngân hàng.",
        choice1 = "Đó là bước đồng bộ thanh toán chủ hợp đồng, anh/chị nhập giúp em để hệ thống giữ điện cho nhà mình.",
        choice2 = "Có thể hệ thống hỏi hơi sâu thật, nhưng thủ tục bây giờ thường vòng vèo hơn mình nghĩ nhiều lắm.",
        choice3 = "Nếu thấy lạ thì cứ nhập thử xem sao, vì nhiều lúc làm tiếp rồi mới hiểu được mục đích của nó.",
        choice4 = "Em cũng không rõ giao diện mới này lắm, nhưng thường cứ điền đủ thì màn hình sẽ cho qua thôi ạ.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "8. Ứng dụng ngân hàng gửi cho tôi một mã OTP.",
        choice1 = "Anh/chị đọc mã đó giúp em để hoàn tất xác nhận giữ điện, nếu chậm là lệnh khóa sẽ chạy ngay.",
        choice2 = "Anh/chị cứ giữ mã đó trước cũng được, đôi khi nhìn những con số làm mình thấy bớt hồi hộp hơn.",
        choice3 = "Hay anh/chị chụp lại màn hình rồi gửi bạn bè xem thử họ đoán mã này dùng làm gì nhé ạ.",
        choice4 = "Nếu thích thì anh/chị đọc từng số thật chậm cũng được, miễn sao cuối cùng em nghe đủ là ổn.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "9. Tôi thấy lạ rồi, điện lực sao lại cần OTP ngân hàng?",
        choice1 = "Đây là mã xác thực giữ hợp đồng chứ không phải mã chuyển tiền, anh/chị yên tâm đọc giúp em nhé.",
        choice2 = "OTP giờ dùng cho nhiều việc lắm ạ, nên chuyện điện nước liên quan tài khoản cũng không hiếm đâu.",
        choice3 = "Quy trình mới đôi khi khó hiểu thật, nhưng mình cứ làm theo rồi sau đó thắc mắc cũng chưa muộn.",
        choice4 = "Nếu anh/chị suy nghĩ quá lâu thì dễ mất thời gian, mà những việc gấp thường nên xử lý trước đã.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "10. Không, tôi sẽ tự gọi tổng đài điện lực chính thức để xác minh.",
        choice1 = "Anh/chị gọi lúc này hồ sơ có thể bị chuyển lỗi và rất khó khôi phục ngay trong hôm nay đó nhé.",
        choice2 = "Anh/chị cứ gọi cũng được, nhưng có khi chờ lâu xong rồi vẫn phải quay lại làm đúng bước này thôi.",
        choice3 = "Nếu anh/chị thích kiểm tra lại thì em cũng không cản, chỉ là mình đang làm gần xong rồi thôi ạ.",
        choice4 = "Thôi được ạ, em dừng ở đây, nhưng đúng là anh/chị cảnh giác hơi nhiều hơn mức cần thiết rồi.",
        correctIndex = 0
    },
	//	Cọc Tiền Để Ứng Tuyển Việc Làm

    new DialogueLine {
        question = "1. Alo, ai đang gọi cho tôi vậy?",
        choice1 = "Chào anh/chị, em gọi từ bộ phận tuyển dụng vì hồ sơ ứng tuyển của mình đang được duyệt gấp.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "2. Hồ sơ nào? Tôi đâu nhớ vừa nộp chỗ nào.",
        choice1 = "Hệ thống bên em ghi nhận CV của anh/chị nằm trong nhóm ứng viên phù hợp cho vị trí làm việc từ xa.",
        choice2 = "Có thể anh/chị nộp lúc khuya nên quên thôi, chuyện này nhiều người cũng gặp lắm ạ.",
        choice3 = "Không sao đâu, đôi khi chưa nộp vẫn được chọn nếu hồ sơ nhìn có duyên với công ty.",
        choice4 = "Em cũng chưa rõ lắm, nhưng bên em đang cần người nên cứ liên hệ trước cho nhanh ạ.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "3. Công ty nào vậy? Tôi chưa thấy email hay tin nhắn gì cả.",
        choice1 = "Bên em tuyển gấp nên gọi trực tiếp trước, email mời phỏng vấn sẽ gửi sau khi xác nhận hồ sơ.",
        choice2 = "Do hệ thống thư bận quá nên bên em ưu tiên gọi miệng trước để đỡ chậm tiến độ ạ.",
        choice3 = "Tin nhắn có thể đến sau thôi, miễn hiện tại anh/chị vẫn nghe máy là quy trình vẫn chạy.",
        choice4 = "Email bây giờ nhiều lúc vào spam, nên nghe điện thoại trực tiếp vẫn tiện và nhanh hơn nhiều.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "4. Vị trí này cụ thể là làm gì?",
        choice1 = "Đây là vị trí nhập liệu và hỗ trợ khách hàng online, thời gian linh hoạt và thu nhập khá ổn định.",
        choice2 = "Công việc cũng nhẹ nhàng thôi, chủ yếu là thao tác cơ bản, không cần kinh nghiệm quá sâu đâu ạ.",
        choice3 = "Anh/chị cứ hiểu là việc văn phòng online, làm ở đâu cũng được miễn mình phản hồi đều là được.",
        choice4 = "Nội dung chi tiết sẽ gửi sau, hiện tại bên em chỉ cần giữ suất phỏng vấn cho mình trước đã.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "5. Nghe cũng được, vậy bước tiếp theo là gì?",
        choice1 = "Anh/chị cần xác nhận hồ sơ hôm nay và đóng khoản phí xử lý hồ sơ để hệ thống giữ lịch phỏng vấn.",
        choice2 = "Mình chỉ cần làm thêm một bước nhỏ trước, sau đó bên em sẽ sắp xếp lịch nhanh cho anh/chị.",
        choice3 = "Quy trình cũng đơn giản thôi, bên em ưu tiên người phản hồi sớm nên làm trước sẽ có lợi hơn ạ.",
        choice4 = "Nếu anh/chị thấy phù hợp thì mình chốt ngay hôm nay, vì đợt này số lượng hồ sơ vào khá nhiều.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "6. Phí hồ sơ là sao? Bình thường ứng tuyển có mất tiền đâu.",
        choice1 = "Đây là phí xác thực hồ sơ và tạo mã nhân sự tạm thời, sau khi nhận việc công ty sẽ hoàn lại.",
        choice2 = "Khoản này không lớn đâu, chủ yếu để lọc ứng viên nghiêm túc và giữ lịch cho đỡ bị hủy ngang.",
        choice3 = "Nhiều bên bây giờ đều thu trước một khoản nhỏ, nên anh/chị cứ xem như bước xác minh ban đầu.",
        choice4 = "Nếu không có khoản xác nhận thì hệ thống khó ưu tiên hồ sơ, nên bên em mới áp dụng vậy ạ.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "7. Bao nhiêu tiền? Và tôi chuyển cho ai?",
        choice1 = "Anh/chị chuyển 390 nghìn vào tài khoản hỗ trợ tuyển dụng, nội dung ghi mã hồ sơ để đối soát.",
        choice2 = "Số tiền khá nhẹ thôi, quan trọng là mình xác nhận sớm để bên em khóa lịch cho chắc chắn nhé.",
        choice3 = "Anh/chị chuyển ngay hôm nay là đẹp nhất, vì để sang ngày mai hồ sơ dễ bị đẩy xuống sau hơn.",
        choice4 = "Bên em chỉ cần thấy giao dịch vào hệ thống là sẽ gửi lịch phỏng vấn và tài liệu ngay sau đó.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "8. Tôi muốn xem website công ty và thư mời chính thức trước.",
        choice1 = "Anh/chị cứ yên tâm, thông tin đó sẽ gửi đủ sau khi hệ thống xác nhận hồ sơ đã được kích hoạt.",
        choice2 = "Website bên em đang cập nhật nên xem lúc này cũng chưa đầy đủ, mình làm bước trước sẽ tiện hơn.",
        choice3 = "Thư mời có sẵn cả, nhưng em chỉ gửi được sau khi hồ sơ chuyển sang trạng thái xác nhận thôi ạ.",
        choice4 = "Nếu anh/chị đợi đủ giấy tờ rồi mới làm thì có thể suất này sẽ chuyển sang ứng viên khác mất.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "9. Tôi vẫn thấy hơi đáng ngờ.",
        choice1 = "Em hiểu, nhưng đợt tuyển này chỉ giữ hồ sơ trong hôm nay, quá hạn là hệ thống tự động loại ra.",
        choice2 = "Nhiều ứng viên ban đầu cũng phân vân như mình, nhưng làm xong thì mọi thứ đều diễn ra khá nhanh.",
        choice3 = "Nếu anh/chị chần chừ quá lâu thì cơ hội sẽ qua mất, mà suất làm việc từ xa đang khá hiếm đấy.",
        choice4 = "Bên em không ép đâu, chỉ là quy trình tuyển đang chạy liên tục nên phản hồi chậm sẽ thiệt hơn.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "10. Không, tôi sẽ chỉ làm việc qua email công ty chính thức và không đóng phí ứng tuyển.",
        choice1 = "Anh/chị cân nhắc lại nhé, vì khi hồ sơ bị đóng rồi thì rất khó mở lại trong đợt tuyển này.",
        choice2 = "Nếu anh/chị muốn an toàn quá mức thì em cũng chịu, nhưng cơ hội tốt thường không chờ lâu đâu.",
        choice3 = "Thôi được ạ, em dừng hỗ trợ tại đây, nhưng đúng là mình đang bỏ lỡ một vị trí khá ổn đó.",
        choice4 = "Anh/chị cứ kiểm tra thêm cũng được, còn bên em sẽ chuyển suất này cho người xác nhận sớm hơn.",
        correctIndex = 0
    },

//	Giả Nhà Trường Báo Học Phí
    new DialogueLine {
        question = "1. Alo, ai đang gọi cho tôi vậy?",
        choice1 = "Chào phụ huynh, em gọi từ phòng tài vụ nhà trường vì học phí của em nhà mình đang có vấn đề.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "2. Vấn đề gì? Tôi tưởng đã đóng đủ từ đầu kỳ rồi mà.",
        choice1 = "Hệ thống kế toán báo khoản học phí kỳ này chưa được xác nhận hoàn tất nên cần kiểm tra gấp.",
        choice2 = "Có thể phần mềm nhớ nhầm một chút thôi, nhưng mình cứ xử lý trước cho yên tâm cũng được ạ.",
        choice3 = "Đôi khi dữ liệu đi hơi chậm nên trường gọi hỏi lại trước để tránh phụ huynh phải đi lại nhiều.",
        choice4 = "Em cũng chưa rõ chi tiết lắm, chỉ thấy màn hình báo đỏ nên gọi mình trước cho chắc thôi ạ.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "3. Tôi không hề nhận được thông báo nào từ giáo viên chủ nhiệm.",
        choice1 = "Do đây là lỗi phát sinh ở hệ thống thu phí nên bên em gọi trực tiếp để xử lý trước cho nhanh.",
        choice2 = "Có thể giáo viên bận nên chưa kịp báo, nhưng mình làm sớm thì thường sẽ đỡ rắc rối hơn ạ.",
        choice3 = "Thông báo giấy nhiều khi đến chậm lắm, nên trường mới ưu tiên gọi điện để phụ huynh nắm trước.",
        choice4 = "Nếu đợi đủ các đầu mối cùng báo thì hơi lâu, trong khi hồ sơ học vụ vẫn đang chạy liên tục ạ.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "4. Vậy giờ tôi cần làm gì?",
        choice1 = "Phụ huynh cần xác nhận lại mã học sinh và hoàn tất khoản bổ sung để tránh khóa hồ sơ học vụ.",
        choice2 = "Mình chỉ cần làm thêm một bước nhỏ thôi, sau đó hệ thống sẽ tự cập nhật lại trạng thái ngay ạ.",
        choice3 = "Quy trình cũng khá ngắn, bên em hướng dẫn qua điện thoại nên phụ huynh không phải đến trường đâu.",
        choice4 = "Nếu xử lý ngay trong hôm nay thì hồ sơ sẽ ổn định, còn để sang ngày mai dễ phát sinh chậm hơn ạ.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "5. Khoản bổ sung là khoản gì? Nhà trường chưa từng nói với tôi.",
        choice1 = "Đó là phần chênh lệch cập nhật theo danh sách mới, bao gồm phí hồ sơ điện tử và xác nhận học vụ.",
        choice2 = "Khoản này không lớn đâu ạ, chủ yếu để hệ thống đủ điều kiện khóa dữ liệu cho đúng từng học sinh.",
        choice3 = "Nhiều phụ huynh cũng hỏi giống mình, vì đợt điều chỉnh này phát sinh sau khi đã thu học phí đầu kỳ.",
        choice4 = "Bên em đang hỗ trợ chung nên gọi lần lượt từng người, phụ huynh xử lý sớm sẽ đỡ phải theo dõi lại.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "6. Tôi muốn tự gọi cho giáo viên chủ nhiệm để hỏi trước.",
        choice1 = "Phụ huynh cứ hỏi sau cũng được, nhưng hồ sơ hiện đang treo nên cần xử lý trước thời hạn hôm nay.",
        choice2 = "Giáo viên chủ nhiệm không trực tiếp thao tác phần tài vụ, nên gọi lúc này cũng chưa giải quyết được ạ.",
        choice3 = "Nếu liên hệ nhiều đầu mối cùng lúc thì thông tin dễ chồng chéo, bên em sợ phụ huynh càng rối hơn.",
        choice4 = "Mình xác nhận xong bước này trước rồi hỏi lại sau vẫn được, vì dữ liệu hiện đang cần cập nhật gấp.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "7. Số tiền cần đóng là bao nhiêu?",
        choice1 = "Phụ huynh chuyển 1 triệu 280 nghìn vào tài khoản hỗ trợ thu phí để bên em cập nhật ngay hôm nay.",
        choice2 = "Khoản này không quá lớn đâu ạ, chủ yếu là hoàn tất đúng hạn để hồ sơ học sinh không bị treo thêm.",
        choice3 = "Mình xử lý sớm thì hệ thống sẽ ghi nhận ngay, chứ để trễ là sang đợt đối soát tiếp theo khá phiền ạ.",
        choice4 = "Bên em chỉ cần thấy giao dịch vào hệ thống là có thể mở lại trạng thái bình thường cho học sinh ngay.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "8. Sao lại chuyển vào tài khoản cá nhân? Trường không có cổng thanh toán à?",
        choice1 = "Đây là tài khoản hỗ trợ đối soát tạm thời vì cổng thu phí đang bảo trì nên bên em xử lý thủ công.",
        choice2 = "Phụ huynh yên tâm, nhiều trường hợp hôm nay cũng đang làm theo cách này để kịp thời hạn cập nhật ạ.",
        choice3 = "Cổng thanh toán không phải lúc nào cũng mở ổn định, nên bên em mới linh hoạt hỗ trợ qua tài khoản này.",
        choice4 = "Nếu đợi cổng chính thức hoạt động lại thì có thể quá hạn, nên trường mới ưu tiên xử lý trước cho mình.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "9. Tôi vẫn thấy rất lạ và muốn xem thông báo chính thức.",
        choice1 = "Em hiểu, nhưng danh sách này chỉ giữ đến hết hôm nay, quá hạn là hệ thống tự chuyển sang nhắc nợ.",
        choice2 = "Nhiều phụ huynh ban đầu cũng phân vân vậy ạ, nhưng xử lý xong thì hồ sơ đều trở lại bình thường ngay.",
        choice3 = "Nếu mình chờ thêm giấy tờ đầy đủ thì có thể mất lượt xử lý ưu tiên và phải làm việc lại từ đầu ạ.",
        choice4 = "Bên em không ép đâu, chỉ là thời hạn của hệ thống khá chặt nên phản hồi chậm sẽ bất tiện hơn thôi.",
        correctIndex = 0
    },
    new DialogueLine {
        question = "10. Không, tôi sẽ liên hệ giáo viên chủ nhiệm và kiểm tra trên kênh chính thức của nhà trường.",
        choice1 = "Phụ huynh cân nhắc lại nhé, vì khi hồ sơ bị chuyển trạng thái rồi thì việc mở lại sẽ mất thêm thời gian.",
        choice2 = "Nếu phụ huynh muốn kiểm tra thêm thì em cũng không cản, nhưng hạn xử lý hôm nay sẽ không chờ lâu đâu ạ.",
        choice3 = "Thôi được ạ, em dừng hỗ trợ tại đây, còn danh sách này bên em sẽ chuyển sang phụ huynh khác sau nhé.",
        choice4 = "Phụ huynh cứ xác minh thêm cũng được, nhưng thật sự lúc này mình đang ở rất gần bước hoàn tất rồi ạ.",
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
        // Mỗi ngày bắt đầu từ câu hỏi thứ (ngày-1)*10
        int day = 1;
        if (DayManager.Instance != null)
            day = DayManager.Instance.currentDay;
        savedIndex = (day - 1) * 10;
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

        Debug.Log($"<color=yellow>[DMStep] Loading index={currentIndex} | correctIndex={line.correctIndex} | Q: {line.question}</color>");

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

        Debug.Log($"<color=yellow>[DMStep] Sent to controller: newCorrectIndex={newCorrectIndex}</color>");

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
        // Hết toàn bộ câu hỏi → loop lại từ đầu
        if (currentIndex >= lines.Length)
            currentIndex = 0;
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
            {
                GameFlagManager.Instance.SetFlag("it_toLobby", true);
            }

            return; // Không StartCurrentLine nữa, chờ NarrativeDirector
        }

        if (currentIndex < lines.Length)
            StartCurrentLine();
    }

    public void OnDialogueFailed()
    {
        // Nếu câu bẫy (correctIndex == -1) → lên 1 câu (skip qua câu bẫy)
        if (currentIndex < lines.Length && lines[currentIndex].correctIndex == -1)
        {
            savedIndex = currentIndex + 1;
            Debug.Log($"[DialogueMissionStep] Trap question at index {currentIndex} → savedIndex = {savedIndex}");
        }
        else
        {
            savedIndex = currentIndex;
        }

        StartCoroutine(FailedRoutine());
    }

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

    public override string GetMissionDescription()
    {
        return "Hãy trả lời các câu hỏi để chứng minh sự vô tội và tìm ra kẻ lừa đảo.";
    }
}