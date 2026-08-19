using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.Forms
{
    public enum GuideTopic
    {
        OnlineMovies = 0,
        LocalMovies = 1,
        Audio = 2,
        Actors = 3,
        Playlists = 4,
        RecycleBin = 5,
        Backup = 6,
        Shortcuts = 7
    }

    public partial class FrmGuideDetail : Form
    {
        private class TopicData
        {
            public string Badge { get; set; } = "";
            public string Title { get; set; } = "";
            public string ShortDesc { get; set; } = "";
            public Color ThemeColor { get; set; } = Color.FromArgb(99, 102, 241);
            public string Content { get; set; } = "";
        }

        private readonly List<TopicData> _topics = new List<TopicData>();
        private readonly List<Guna2Button> _navButtons = new List<Guna2Button>();
        private int _currentIndex = 0;

        public FrmGuideDetail(GuideTopic initialTopic = GuideTopic.OnlineMovies)
        {
            InitializeComponent();
            this.BackColor = UIHelper.BgDark;

            InitializeTopicData();
            BuildNavButtons();

            SelectTopic((int)initialTopic);
        }

        private void InitializeTopicData()
        {
            // 1. Phim Online
            _topics.Add(new TopicData
            {
                Badge = "🌐",
                Title = "Trang Phim Online - Web & Stream",
                ShortDesc = "Lưu trữ, xem trực tuyến và phân loại phim từ link web, YouTube, trailer mà không tốn dung lượng ổ cứng.",
                ThemeColor = Color.FromArgb(99, 102, 241), // Indigo
                Content = @"🌟 1. TỔNG QUAN CHỨC NĂNG
Trang Phim Online giúp bạn quản lý toàn bộ liên kết phim trực tuyến, phim YouTube, web phim cá nhân yêu thích. Bạn có thể mở xem trực tiếp trên trình duyệt, gắn thẻ (Tags), diễn viên và chấm điểm đánh giá.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🚀 2. CÁC TÍNH NĂNG CHÍNH & HƯỚNG DẪN THAO TÁC

  ✦ Thêm Phim Online Mới:
     • Nhấn nút ""+ Thêm phim"" ở góc trên bên phải.
     • Nhập Mã phim / Tên phim (bắt buộc).
     • Dán đường link phim vào ô ""Media URL"" (hỗ trợ URL web, YouTube, embed link).
     • Nhập URL ảnh bìa hoặc tải poster tự động qua tra cứu TMDB.
     • Gắn Thẻ (Tags), chọn Diễn viên tham gia và ghi chú cá nhân rồi nhấn Lưu.

  ✦ Kéo Thả Thông Minh (Drag & Drop):
     • Bạn có thể bôi đen và kéo thả trực tiếp link từ trình duyệt web vào khung thêm phim để tự động điền URL.

  ✦ Tìm Kiếm & Lọc Nâng Cao:
     • Thanh tìm kiếm tức thời theo mã phim, tên phim hoặc nội dung ghi chú.
     • Lọc theo Thẻ (Tags) thể loại, theo Diễn viên hoặc theo Số sao đánh giá (1 - 5 sao).

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🖱️ 3. THAO TÁC CHUỘT TRÊN THẺ PHIM

  • Click đúp (2 lần chuột trái): Mở ngay trình duyệt web mặc định và truy cập vào đường link phim.
  • Click chuột phải: Mở menu ngữ cảnh:
       - ✏️ Chỉnh sửa thông tin phim.
       - 🗑️ Xóa vào Thùng rác (an toàn, khôi phục được).
       - ⏱️ Cập nhật tiến độ xem (% xem dở).
       - ➕ Thêm vào Danh sách phát (Playlist).
  • Click nút ""i"" (Góc trên thẻ): Mở cửa sổ xem chi tiết phim, tiểu sử và album ảnh.
  • Chấm điểm sao: Rê chuột qua các ngôi sao để chấm điểm 1 - 5 sao.
  • Nút Trái tim ❤️: Nhấn vào biểu tượng tim để thêm/bỏ khỏi danh sách Yêu Thích.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
💡 4. MẸO SỬ DỤNG (PRO TIPS)
  • Khi dán link YouTube, Movie Vault sẽ tự động mở xem bằng trình duyệt tiện lợi.
  • Đặt các Tags như #HanhDong, #Anime, #GiaiTri để lọc danh sách nhanh chóng chỉ với 1 click!"
            });

            // 2. Phim Trên Máy
            _topics.Add(new TopicData
            {
                Badge = "📁",
                Title = "Trang Phim Trên Máy - Video Offline",
                ShortDesc = "Quản lý kho video chất lượng cao lưu trên máy tính, quét thư mục tự động hàng loạt và phát video ngoài.",
                ThemeColor = Color.FromArgb(59, 130, 246), // Blue
                Content = @"🌟 1. TỔNG QUAN CHỨC NĂNG
Quản lý toàn bộ các file phim, video clip lưu trữ cục bộ trên ổ cứng (HDD, SSD, USB, ổ mạng). Hỗ trợ quét hàng loạt và tự động liên kết trình phát video mặc định của Windows.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🚀 2. CÁC TÍNH NĂNG CHÍNH & HƯỚNG DẪN THAO TÁC

  ✦ Thêm Từng Phim Thủ Công:
     • Nhấn nút ""+ Thêm phim"" và chọn nguồn ""Phim Trên Máy"".
     • Bấm nút duyệt file để chọn file video (.mp4, .mkv, .avi, .wmv, .mov, .flv...).

  ✦ Quét Thư Mục Hàng Loạt (Batch Scan / Import):
     • Nhấn nút ""📂 Quét thư mục"" trên thanh công cụ.
     • Chọn thư mục chứa phim trên máy tính của bạn.
     • Movie Vault sẽ tự động quét đệ quy, trích xuất tên phim sạch và tạo hàng loạt thẻ phim chỉ trong vài giây!

  ✦ Tự Động Theo Dõi Thư Mục (Folder Watcher):
     • Hệ thống chạy dịch vụ ngầm theo dõi thư mục Videos của bạn. Khi bạn tải hoặc sao chép phim mới vào, phần mềm sẽ hiển thị thông báo và tự động thêm vào danh sách.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🖱️ 3. THAO TÁC CHUỘT TRÊN THẺ PHIM

  • Click đúp (2 lần chuột trái): Khởi chạy video trực tiếp bằng trình xem video mặc định của Windows (VLC, MPC-HC, Windows Media Player, PotPlayer...).
  • Click chuột phải:
       - ✏️ Đổi tên, thay đổi poster, cập nhật tags.
       - 📂 Mở thư mục chứa file gốc trên máy.
       - ⏱️ Cập nhật tiến độ xem.
       - 🗑️ Xóa vào Thùng rác.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
💡 4. MẸO SỬ DỤNG (PRO TIPS)
  • Khi dùng tính năng Quét Thư Mục, hệ thống sẽ tự loại bỏ các ký tự rác trong tên file để tạo tên phim chuẩn và đẹp mắt nhất.
  • Nếu bạn di chuyển file phim sang ổ đĩa khác, chỉ cần chuột phải chọn Chỉnh sửa để cập nhật lại đường dẫn mới."
            });

            // 3. Âm thanh
            _topics.Add(new TopicData
            {
                Badge = "🎵",
                Title = "Trang Âm Thanh - Nhạc Phim & Audio Player",
                ShortDesc = "Kho nhạc phim (OST), bài hát, podcast với trình phát nhạc toàn cục phát nền liên tục khi chuyển trang.",
                ThemeColor = Color.FromArgb(236, 72, 153), // Pink
                Content = @"🌟 1. TỔNG QUAN CHỨC NĂNG
Lưu trữ và phát các bản nhạc phim (OST), hiệu ứng âm thanh, bài hát yêu thích. Đi kèm trình phát nhạc mini hiện đại luôn nổi ở góc dưới màn hình, phát nhạc xuyên suốt khi bạn thao tác các trang khác.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🚀 2. CÁC TÍNH NĂNG CHÍNH & HƯỚNG DẪN THAO TÁC

  ✦ Thêm Audio Mới:
     • Nhấn nút ""+ Thêm Audio"".
     • Chọn file âm thanh (.mp3, .wav, .flac, .m4a, .aac, .ogg...).
     • Đặt tên bài hát, chọn ảnh bìa album, gắn Tags thể loại.

  ✦ Trình Phát Nhạc Nền Toàn Cục (UcAudioPlayer):
     • Khi bấm Play bất kỳ bài hát nào, thanh phát nhạc sẽ hiện ra ở góc dưới.
     • Bạn có thể chuyển sang Trang chủ, Phim, Diễn viên, Sao lưu... nhạc vẫn phát liên tục không bị gián đoạn.
     • Hỗ trợ thanh kéo tua nhạc (Seekbar) và điều chỉnh âm lượng mượt mà.

  ✦ Xuất File Âm Thanh (Export):
     • Chuột phải vào thẻ bài hát và chọn ""Export Audio"" để lưu file nhạc gốc ra máy tính bất cứ lúc nào.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
⌨️ 3. PHÍM TẮT ĐIỀU KHIỂN NHẠC CỰC NHANH
  • Space (Phím Cách): Tạm dừng (Pause) / Tiếp tục phát (Play).
  • Mũi tên Trái (◀) / Phải (▶): Tua lùi / Tua tới 10 giây.
  • Mũi tên Lên (▲) / Xuống (▼): Tăng / Giảm 5% âm lượng.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
💡 4. MẸO SỬ DỤNG (PRO TIPS)
  • Bạn có thể vừa nghe nhạc nền vừa quản lý, phân loại và nhập liệu phim mà không hề bị đứt quãng âm thanh!"
            });

            // 4. Diễn viên
            _topics.Add(new TopicData
            {
                Badge = "👥",
                Title = "Trang Diễn Viên - Nghệ Sĩ & Album Ảnh",
                ShortDesc = "Quản lý hồ sơ diễn viên, đạo diễn, lưu trữ album ảnh chất lượng cao và tra cứu các phim đã tham gia.",
                ThemeColor = Color.FromArgb(245, 158, 11), // Amber
                Content = @"🌟 1. TỔNG QUAN CHỨC NĂNG
Trang Diễn viên cho phép bạn lưu trữ danh sách các nghệ sĩ, thần tượng yêu thích cùng tiểu sử, quốc tịch, ngày sinh, album ảnh riêng và danh sách các tác phẩm họ đã góp mặt.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🚀 2. CÁC TÍNH NĂNG CHÍNH & HƯỚNG DẪN THAO TÁC

  ✦ Tạo Hồ Sơ Diễn Viên:
     • Nhấn nút ""+ Thêm diễn viên"".
     • Nhập Họ tên, Ảnh đại diện (Avatar), Ngày sinh, Quốc tịch và Tiểu sử.

  ✦ Album Ảnh Chi Tiết (Gallery):
     • Mở hồ sơ diễn viên để thêm nhiều hình ảnh chất lượng cao (photoshoot, poster, hậu trường).
     • Click vào từng ảnh để phóng to xem chi tiết.

  ✦ Liên Kết Phim Tự Động:
     • Khi bạn thêm hoặc sửa phim và chọn tên diễn viên, bộ phim đó sẽ tự động hiển thị trong mục ""Các phim đã tham gia"" trong hồ sơ của diễn viên đó!
     • Click trực tiếp vào thẻ phim liên quan để mở xem phim ngay.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
💡 3. MẸO SỬ DỤNG (PRO TIPS)
  • Kết hợp với chức năng lọc theo Diễn viên ở trang Phim để xem nhanh toàn bộ bộ sưu tập phim của nghệ sĩ bạn yêu thích nhất."
            });

            // 5. Playlists
            _topics.Add(new TopicData
            {
                Badge = "📑",
                Title = "Trang Danh Sách Phát - Tuyển Tập Đa Phương Tiện",
                ShortDesc = "Tạo playlist theo chủ đề kết hợp cả Phim lẫn Bài hát, tùy chỉnh thứ tự phát và ảnh đại diện.",
                ThemeColor = Color.FromArgb(168, 85, 247), // Purple
                Content = @"🌟 1. TỔNG QUAN CHỨC NĂNG
Danh sách phát giúp bạn gom nhóm các nội dung theo chủ đề cụ thể (Ví dụ: ""Phim Bom Tấn Cuối Tuần"", ""Tuyển Tập Nhạc Chill"", ""Top Anime Hay Nhất"").

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🚀 2. CÁC TÍNH NĂNG CHÍNH & HƯỚNG DẪN THAO TÁC

  ✦ Tạo Playlist Mới:
     • Nhấn nút ""+ Tạo Playlist"".
     • Đặt tên Playlist, nhập mô tả, chọn ảnh bìa đại diện.

  ✦ Thêm Phim & Âm Thanh Kết Hợp:
     • Điểm độc đáo của Movie Vault là 1 Playlist có thể chứa CẢ PHIM LẪN BÀI HÁT!
     • Thêm nhanh bằng cách: Click chuột phải vào thẻ phim hoặc bài hát bất kỳ ➔ Chọn ""Thêm vào Playlist"" ➔ Chọn playlist muốn đưa vào.

  ✦ Quản Lý & Sắp Xếp:
     • Mở Playlist để xem danh sách các mục bên trong.
     • Dễ dàng xóa mục khỏi playlist mà không ảnh hưởng tới phim gốc.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
💡 3. MẸO SỬ DỤNG (PRO TIPS)
  • Hãy tạo các Playlist chuyên đề như ""Phim xem cùng gia đình"" hoặc ""Nhạc làm việc tập trung"" để truy cập nhanh nội dung phù hợp từng thời điểm."
            });

            // 6. Thùng rác
            _topics.Add(new TopicData
            {
                Badge = "🗑️",
                Title = "Trang Thùng Rác - Xóa An Toàn & Khôi Phục",
                ShortDesc = "Cơ chế Soft-Delete bảo vệ dữ liệu, tránh mất mát khi xóa nhầm và hỗ trợ khôi phục 1-click.",
                ThemeColor = Color.FromArgb(239, 68, 68), // Red
                Content = @"🌟 1. TỔNG QUAN CHỨC NĂNG
Movie Vault áp dụng cơ chế Soft-Delete hiện đại. Khi bạn xóa bất kỳ bộ phim hay bản nhạc nào, dữ liệu sẽ được chuyển vào Thùng rác chứ không bị mất vĩnh viễn.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🚀 2. CÁC TÍNH NĂNG CHÍNH & HƯỚNG DẪN THAO TÁC

  ✦ Khôi Phục Dữ Liệu (Restore):
     • Vào mục ""Thùng rác"" trên thanh menu bên trái.
     • Tìm mục muốn khôi phục và nhấn nút ""Khôi phục"" (hoặc click chuột phải chọn Khôi phục).
     • Mục đó sẽ được đưa về đúng trang ban đầu với đầy đủ thông tin, tags, diễn viên, ảnh bìa và điểm đánh giá.

  ✦ Xem Chi Tiết Trước Khi Quyết Định:
     • Nhấn vào nút xem chi tiết để xem lại tên, ngày xóa, đường dẫn file trước khi quyết định khôi phục hay xóa vĩnh viễn.

  ✦ Dọn Sạch Thùng Rác (Empty Bin):
     • Nhấn nút ""Dọn sạch thùng rác"" ở góc trên để xóa vĩnh viễn toàn bộ các mục đã xóa và giải phóng dung lượng bộ nhớ.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
💡 3. MẸO SỬ DỤNG (PRO TIPS)
  • Bạn hoàn toàn yên tâm khi dọn dẹp danh mục vì mọi thao tác xóa đều có thể khôi phục lại dễ dàng từ Thùng rác."
            });

            // 7. Sao lưu & Khôi phục
            _topics.Add(new TopicData
            {
                Badge = "💾",
                Title = "Trang Sao Lưu & Khôi Phục - Backup & Restore",
                ShortDesc = "Bảo vệ toàn diện cơ sở dữ liệu với sao lưu tự động đa thư mục và khôi phục hệ thống tức thì.",
                ThemeColor = Color.FromArgb(16, 185, 129), // Emerald
                Content = @"🌟 1. TỔNG QUAN CHỨC NĂNG
Hệ thống sao lưu chuyên nghiệp giúp đóng gói toàn bộ database của bạn thành file MovieVault_Backup.db độc lập, đảm bảo an toàn 100% dữ liệu trước mọi sự cố ổ cứng hay cài lại máy.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🚀 2. CÁC TÍNH NĂNG CHÍNH & HƯỚNG DẪN THAO TÁC

  ✦ Thiết Lập Đa Thư Mục Sao Lưu:
     • Nhấn nút ""Thêm thư mục sao lưu"".
     • Bạn có thể chọn nhiều ổ đĩa khác nhau (ổ D:, ổ E:, thư mục đồng bộ Google Drive, Dropbox, OneDrive...).
     • Khi sao lưu, hệ thống sẽ tự động đồng bộ bản backup mới nhất đến TẤT CẢ các thư mục này.

  ✦ Tự Động Sao Lưu An Toàn (Auto-Backup):
     • Movie Vault tự động tạo bản sao lưu snapshot SQLite mới nhất mỗi khi bạn đóng/thoát ứng dụng.
     • Kèm theo file README.md hướng dẫn mở database trên mọi thiết bị.

  ✦ Khôi Phục Hệ Thống (Restore):
     • Nhấn nút ""Khôi phục từ file"" và chọn file .db backup bất kỳ.
     • Hệ thống tự động kiểm tra tính hợp lệ, thống kê số lượng phim/nhạc có trong bản backup và cho phép khôi phục toàn vẹn dữ liệu chỉ với 1 click.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
💡 3. MẸO SỬ DỤNG (PRO TIPS)
  • Nên cấu hình ít nhất 1 thư mục sao lưu nằm trong thư mục đám mây (như Google Drive / OneDrive) để dữ liệu của bạn luôn được đồng bộ an toàn lên Cloud!"
            });

            // 8. Tiện ích & Phím tắt
            _topics.Add(new TopicData
            {
                Badge = "⚡",
                Title = "Tiện Ích Toàn Cục & Bảng Phím Tắt",
                ShortDesc = "Tìm kiếm nhanh Omnibox Ctrl+K, Drop Widget nổi ngoài màn hình Desktop và phím tắt thao tác nhanh.",
                ThemeColor = Color.FromArgb(6, 182, 212), // Cyan
                Content = @"🌟 1. CÁC TIỆN ÍCH NỔI BẬT

  ✦ Thanh Tìm Kiếm Toàn Cục (Omnibox - Ctrl + K):
     • Nhấn phím tổ hợp ""Ctrl + K"" ở bất kỳ đâu trong ứng dụng.
     • Gõ tên phim, tên bài hát hoặc tên playlist để tìm kiếm tức thì.
     • Nhấn chọn kết quả để mở xem ngay lập tức mà không cần chuyển trang thủ công!

  ✦ Widget Kéo Thả Nổi Ngoài Desktop (Drop Widget):
     • Một widget nhỏ gọn luôn nổi trên góc màn hình Desktop của bạn.
     • Bạn chỉ cần kéo thả link phim từ trình duyệt Chrome/Edge hoặc kéo file video từ File Explorer và thả vào Widget để thêm nhanh vào Movie Vault!
     • Bật/Tắt widget dễ dàng trong mục Hồ sơ cá nhân.

  ✦ Lịch Sử Xem & Tiếp Tục Xem Dở (Resume Progress):
     • Mục ""▶️ Tiếp tục xem"" trên Trang chủ sẽ hiển thị các bộ phim bạn đang xem dở (1% - 99%).
     • Chuột phải vào bất kỳ phim nào để cập nhật thanh tiến độ % xem.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
⌨️ 2. BẢNG TỔNG HỢP PHÍM TẮT TIỆN DỤNG

  • Ctrl + K           : Mở thanh tìm kiếm nhanh toàn cục (Omnibox).
  • Space (Phím Cách)  : Tạm dừng (Pause) / Tiếp tục phát nhạc (Play).
  • Mũi tên Trái (◀)   : Tua lùi 10 giây khi phát nhạc.
  • Mũi tên Phải (▶)  : Tua tới 10 giây khi phát nhạc.
  • Mũi tên Lên (▲)    : Tăng 5% âm lượng.
  • Mũi tên Xuống (▼)  : Giảm 5% âm lượng.
  • Esc                : Đóng các hộp thoại popup."
            });
        }

        private void BuildNavButtons()
        {
            flpNav.Controls.Clear();
            _navButtons.Clear();

            for (int i = 0; i < _topics.Count; i++)
            {
                int index = i;
                var topic = _topics[i];

                var btn = new Guna2Button
                {
                    Text = $"  {topic.Badge}  {topic.Title.Split('-')[0].Trim()}",
                    Font = new Font("Segoe UI Semibold", 9.75F),
                    ForeColor = Color.FromArgb(203, 213, 225),
                    FillColor = Color.Transparent,
                    BorderRadius = 10,
                    Size = new Size(245, 48),
                    Margin = new Padding(0, 0, 0, 6),
                    TextAlign = HorizontalAlignment.Left,
                    TextOffset = new Point(8, 0),
                    Animated = true,
                    Cursor = Cursors.Hand
                };

                btn.HoverState.FillColor = Color.FromArgb(30, 41, 59);
                btn.HoverState.ForeColor = Color.White;

                btn.Click += (s, e) => SelectTopic(index);

                flpNav.Controls.Add(btn);
                _navButtons.Add(btn);
            }
        }

        private void SelectTopic(int index)
        {
            if (index < 0 || index >= _topics.Count) return;
            _currentIndex = index;

            var topic = _topics[index];

            // Update Header Styling with Dynamic Gradient
            pnlGuideHeader.FillColor = Color.FromArgb(45, topic.ThemeColor);
            pnlGuideHeader.FillColor2 = Color.FromArgb(16, 22, 38);
            pnlGuideHeader.BorderColor = Color.FromArgb(90, topic.ThemeColor);

            lblTopicBadge.Text = topic.Badge;
            lblTopicTitle.Text = topic.Title;
            lblTopicDesc.Text = topic.ShortDesc;

            // Render Rich Formatted Text
            RenderRichContent(topic.Content);

            // Update Navigation Button States
            for (int i = 0; i < _navButtons.Count; i++)
            {
                if (i == index)
                {
                    _navButtons[i].FillColor = Color.FromArgb(40, topic.ThemeColor);
                    _navButtons[i].ForeColor = Color.White;
                    _navButtons[i].CustomBorderColor = topic.ThemeColor;
                    _navButtons[i].CustomBorderThickness = new Padding(4, 0, 0, 0);
                }
                else
                {
                    _navButtons[i].FillColor = Color.Transparent;
                    _navButtons[i].ForeColor = Color.FromArgb(203, 213, 225);
                    _navButtons[i].CustomBorderThickness = new Padding(0);
                }
            }

            // Update Prev/Next buttons
            btnPrev.Enabled = (_currentIndex > 0);
            btnNext.Enabled = (_currentIndex < _topics.Count - 1);
        }

        private void RenderRichContent(string rawText)
        {
            txtContent.SuspendLayout();
            txtContent.Clear();

            string[] lines = rawText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrWhiteSpace(line))
                {
                    AppendText("\n", FontStyle.Regular, Color.White, 6);
                    continue;
                }

                // Section Headers (🌟, 🚀, 🖱️, ⌨️, 💡)
                if (line.StartsWith("🌟") || line.StartsWith("🚀") || line.StartsWith("🖱️") || line.StartsWith("⌨️") || line.StartsWith("💡"))
                {
                    Color hColor = Color.FromArgb(251, 191, 36); // Gold default
                    if (line.StartsWith("🚀")) hColor = Color.FromArgb(56, 189, 248); // Sky
                    else if (line.StartsWith("🖱️")) hColor = Color.FromArgb(244, 114, 182); // Pink
                    else if (line.StartsWith("⌨️")) hColor = Color.FromArgb(52, 211, 153); // Emerald
                    else if (line.StartsWith("💡")) hColor = Color.FromArgb(192, 132, 252); // Purple

                    AppendText(line + "\n", FontStyle.Bold, hColor, 12F);
                }
                // Separator Lines
                else if (line.StartsWith("━") || line.StartsWith("-"))
                {
                    AppendText(line + "\n", FontStyle.Regular, Color.FromArgb(51, 65, 85), 8F);
                }
                // Subheadings (✦ ...)
                else if (line.TrimStart().StartsWith("✦"))
                {
                    AppendText(line + "\n", FontStyle.Bold, Color.FromArgb(125, 211, 252), 10.5F);
                }
                // Bullet points (• ...)
                else if (line.TrimStart().StartsWith("•"))
                {
                    RenderBulletLine(line);
                }
                // Indented details (- ...)
                else if (line.TrimStart().StartsWith("-"))
                {
                    AppendText(line + "\n", FontStyle.Regular, Color.FromArgb(203, 213, 225), 9.75F);
                }
                // Normal text paragraph
                else
                {
                    AppendText(line + "\n", FontStyle.Regular, Color.FromArgb(226, 232, 240), 10F);
                }
            }

            txtContent.SelectionStart = 0;
            txtContent.ScrollToCaret();
            txtContent.ResumeLayout();
        }

        private void RenderBulletLine(string line)
        {
            // Keywords that will be highlighted in bright yellow/mint
            string[] highlightKeywords = new[]
            {
                "\"+ Thêm phim\"", "\"+ Thêm Audio\"", "\"📂 Quét thư mục\"", "\"+ Tạo Playlist\"",
                "\"Media URL\"", "\"Export Audio\"", "\"Khôi phục\"", "\"Dọn sạch thùng rác\"",
                "\"Ctrl + K\"", "\"Space\"", "\"Mũi tên\"", "\"Esc\"",
                "Click đúp", "Click chuột phải", "Yêu Thích", "Soft-Delete", "Restore", "Auto-Backup"
            };

            int bulletIdx = line.IndexOf('•');
            if (bulletIdx >= 0)
            {
                // Leading spaces
                if (bulletIdx > 0)
                {
                    AppendText(line.Substring(0, bulletIdx), FontStyle.Regular, Color.White, 9.75F);
                }

                // Bullet icon
                AppendText("• ", FontStyle.Bold, Color.FromArgb(129, 140, 248), 10.5F);

                string rest = line.Substring(bulletIdx + 1).TrimStart();

                // Check and highlight keywords
                HighlightAndAppend(rest);
                AppendText("\n", FontStyle.Regular, Color.White, 9.75F);
            }
            else
            {
                AppendText(line + "\n", FontStyle.Regular, Color.FromArgb(226, 232, 240), 9.75F);
            }
        }

        private void HighlightAndAppend(string text)
        {
            // Split by quotes to easily highlight quoted keywords, shortcuts, and actions
            var parts = text.Split('"');
            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 1)
                {
                    // Inside quotes -> Highlighted Amber/Gold
                    AppendText("\"" + parts[i] + "\"", FontStyle.Bold, Color.FromArgb(253, 224, 71), 9.75F);
                }
                else
                {
                    // Check for key action words outside quotes
                    string segment = parts[i];
                    AppendText(segment, FontStyle.Regular, Color.FromArgb(226, 232, 240), 9.75F);
                }
            }
        }

        private void AppendText(string text, FontStyle style, Color color, float size = 9.75F)
        {
            txtContent.SelectionStart = txtContent.TextLength;
            txtContent.SelectionLength = 0;
            txtContent.SelectionFont = new Font("Segoe UI", size, style);
            txtContent.SelectionColor = color;
            txtContent.AppendText(text);
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentIndex > 0)
            {
                SelectTopic(_currentIndex - 1);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentIndex < _topics.Count - 1)
            {
                SelectTopic(_currentIndex + 1);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
