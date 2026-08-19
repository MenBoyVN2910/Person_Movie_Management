# Tài Liệu Tóm Tắt Chức Năng TMDB API (Movie Vault 3.0)

---

## 1. TMDB API là gì?
- **TMDB (The Movie Database)** là một trong những nền tảng cơ sở dữ liệu mở trực tuyến uy tín và lớn nhất thế giới về phim ảnh, diễn viên, đạo diễn, tóm tắt nội dung và bộ sưu tập poster/backdrop chất lượng cao.
- **TMDB API (v3)** là giao thức lập trình RESTful web service do TMDB cung cấp, cho phép ứng dụng gửi truy vấn tìm kiếm phim qua mạng Internet và nhận dữ liệu chuẩn JSON có hỗ trợ đa ngôn ngữ (bao gồm tiếng Việt - `vi-VN`).

---

## 2. Vị Trí Mã Nguồn & Triển Khai Trong Dự Án
Các tệp mã nguồn liên quan trực tiếp đến TMDB API trong hệ thống:
- **`Services/TMDBService.cs`**: Lớp dịch vụ chính xử lý kết nối HTTP (`HttpClient`), đóng gói dữ liệu JSON thành đối tượng C# (`TMDBMovie`), ánh xạ thể loại sang tiếng Việt và quy đổi điểm số.
- **`Forms/FrmMovieDetail.cs`**: Màn hình Thêm/Sửa Phim chứa nút bấm **`🎬 TMDB API`** (`btnFetchTMDB`) kích hoạt tra cứu và gán dữ liệu vào Form.
- **`Models/AppSettings.cs`**: Cấu hình lưu trữ `TMDBApiKey` tùy chỉnh cho từng môi trường/người dùng.

---

## 3. Các Chức Năng Của TMDB API Trong Ứng Dụng

Khi người dùng nhập tên phim vào ô **Mã phim / Tên phim** và nhấn nút **`🎬 TMDB API`**, hệ thống sẽ thực hiện chuỗi quy trình tự động sau:

1. **Tìm kiếm phim thông minh (`SearchMoviesAsync`)**:
   - Gửi yêu cầu tìm kiếm đến endpoint: `https://api.themoviedb.org/3/search/movie?api_key={key}&query={query}&language=vi-VN`.
   - Nhận về danh sách kết quả phù hợp nhất từ cơ sở dữ liệu TMDB toàn cầu.

2. **Tự động điền Mô tả nội dung & Ngày phát hành**:
   - Trích xuất tóm tắt nội dung phim (`Overview`) và ngày công chiếu (`ReleaseDate`).
   - Tự động điền vào ô **Ghi chú** (`txtNote`) trên giao diện.

3. **Tự động tải & áp dụng Ảnh bìa chất lượng cao (Poster)**:
   - Lấy đường dẫn ảnh áp phích có độ phân giải chuẩn (`https://image.tmdb.org/t/p/w500/...`).
   - Tải ảnh về thư mục tạm và nạp trực tiếp vào khung ảnh bìa chính (`picCover`). Khi lưu phim, hệ thống sẽ tự động sao chép và quản lý ảnh này trong kho dữ liệu nội bộ (`App_Data/CoverImages`).

4. **Tự động ánh xạ & phân loại Thể loại (Genre / Tag Mapping)**:
   - Tự động chuyển đổi các ID thể loại quốc tế của TMDB sang tên tiếng Việt (ví dụ: *Hành động, Phiêu lưu, Hoạt hình, Hài hước, Tội phạm, Viễn tưởng, Kinh dị, Tình cảm, Chiến tranh,...*).
   - Tự động kiểm tra danh sách Tag của người dùng hiện tại:
     - Nếu tag đã tồn tại: Tự động chọn và gán tag vào phim.
     - Nếu tag chưa có: Tự động khởi tạo Tag mới vào bảng `Tags` trong SQLite với màu sắc mặc định, sau đó liên kết với phim.

5. **Tự động quy đổi Điểm đánh giá (Rating Conversion)**:
   - Lấy điểm bình chọn trung bình trên TMDB (thang 10 điểm, `vote_average`).
   - Tự động quy đổi về thang điểm 5 sao của Movie Vault (`Rating = Math.Round(vote / 2.0, 1)`).

---

## 4. Phạm Vi Tác Động Lên Toàn Bộ Hệ Thống

| Tiêu chí | Phạm vi & Mức độ ảnh hưởng | Chi tiết kỹ thuật |
| :--- | :--- | :--- |
| **Giao diện người dùng (UI)** | **Cục bộ (1 điểm duy nhất)** | Chỉ xuất hiện tại nút bấm **`🎬 TMDB API`** trong form `FrmMovieDetail.cs`. Hoàn toàn không can thiệp hay hiển thị ở các màn hình khác (Diễn viên, Âm thanh, Danh sách phát, Thùng rác, Sao lưu,...). |
| **Cơ chế kích hoạt** | **Chủ động từ người dùng (On-Demand)** | Chỉ chạy khi người dùng chủ động nhấn nút. Hệ thống **không bao giờ** tự động quét ngầm hoặc tự ý gửi dữ liệu ra ngoài Internet. |
| **Độ độc lập & Khả năng chịu lỗi** | **Độc lập hoàn toàn (Loose Coupling)** | Nếu máy tính không có Internet, hoặc TMDB bị nghẽn mạng, hoặc API Key bị lỗi: **Ứng dụng vẫn hoạt động 100% bình thường**. Người dùng vẫn có thể nhập tay thông tin, chọn ảnh từ máy tính hoặc dùng chức năng cào Web URL (`🔍 Lấy thông tin`). |
| **Bảo mật & Quản lý Key** | **Linh hoạt** | Đi kèm sẵn 1 Public Key mặc định trong mã nguồn để sử dụng ngay mà không cần cấu hình phức tạp. Đồng thời hỗ trợ nhập API Key cá nhân qua `appsettings.json` / `AppSettings.cs`. |
| **Cơ sở dữ liệu (SQLite)** | **An toàn & Minh bạch** | Dữ liệu từ TMDB chỉ được lưu vào SQLite khi người dùng bấm nút **Lưu** trên form. Ngoại trừ việc tạo mới các Tag thể loại (nếu chưa có trong DB), không có bất kỳ bảng dữ liệu hệ thống nào bị thay đổi cấu trúc. |

---

## 5. Hướng Dẫn Sử Dụng Nhanh

1. Mở màn hình Thêm mới hoặc Chỉnh sửa một bộ phim.
2. Tại ô **Mã phim / Tên phim**, nhập tên phim (ví dụ: `Inception`, `Avengers`, `Avatar`, `One Piece`,...).
3. Nhấn nút **`🎬 TMDB API`**.
4. Chờ 1 - 2 giây để hệ thống tự động điền tóm tắt nội dung, ngày chiếu, thể loại và áp phích chất lượng cao.
5. Chỉnh sửa thêm các trường khác theo ý muốn và nhấn **Lưu**.
