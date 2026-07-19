hình hệ thống như đường dẫn gốc của kho phim.
- `Id` (INT, Primary Key, Auto Increment)
- `SettingKey` (VARCHAR, Unique) - Ví dụ: `LocalRepositoryPath`
- `SettingValue` (VARCHAR) - Ví dụ: `D:\\KhoPhimNộiBộ`

---

## 4. ĐẶC TẢ GIAO DIỆN VÀ CÁC THÀNH PHẦN UI (UI/UX SPECIFICATION)

Giao diện chính sử dụng cấu trúc **Dashboard với Sidebar** điều hướng trái, vùng hiển thị nội dung chính sử dụng cơ chế chuyển đổi các `UserControl` để tránh giật lag và nạp lại Form.

### 4.1. Thiết kế Thẻ Phim (Movie Card Custom UserControl)
Thay vì dùng bảng lưới `DataGridView` thô kệch, AI cần tạo ra một `UserControl` đặt tên là `MovieCard`:
- Kích thước chuẩn: `180px` x `260px`.
- Gồm: `PictureBox` chiếm 80% chiều cao (hiển thị ảnh bìa dạng `Zoom`), phía dưới là `Label` hiển thị `MovieCode` chữ đậm, nền tối.
- Hiệu ứng: Khi di chuột qua (Hover), đổi màu viền sang xanh lam hoặc cam để tăng trải nghiệm người dùng.

### 4.2. Trang 1: Quản Lý Mã Phim Trực Tuyến (Online Movie Manager)
- **Khu vực bộ lọc (Top Panel):**
  - 1 `TextBox` Tìm kiếm nhanh theo Mã Phim (Bắt sự kiện `TextChanged` để lọc thời gian thực).
  - 1 Nút "Thêm Phim Mới" (Mở Form CRUD).
- **Khu vực hiển thị (Center Panel):**
  - Sử dụng một `FlowLayoutPanel` có thuộc tính `AutoScroll = True`. 
  - Nạp toàn bộ phim có `SourceType = 0` lên lưới dưới dạng các `MovieCard`.
- **Logic Tương tác:**
  - Khi click đúp vào một Thẻ Phim, ứng dụng sẽ gọi lệnh hệ thống để mở Trình duyệt Web mặc định truy cập thẳng vào `MediaUrl`.

### 4.3. Trang 2: Quản Lý Video Tại Máy Tính (Local Video Storage Manager)
- **Khu vực cấu hình đường dẫn (Top Panel):**
  - 1 `TextBox` hiển thị thư mục gốc của kho phim hiện tại.
  - 1 Nút "Định tuyến đường dẫn" (Mở `FolderBrowserDialog` để người dùng chọn folder tổng trên máy, lưu giá trị này vào bảng `AppSettings`).
  - 1 Nút "Quét thư mục (Auto-Scan)": Thuật toán tự động tìm các file video có đuôi `.mp4`, `.mkv`, `.avi` trong thư mục đã định tuyến, lấy tên file làm `MovieCode` và tự động tạo bản ghi mới nếu chưa tồn tại.
- **Khu vực hiển thị (Center Panel):**
  - Một `FlowLayoutPanel` hiển thị các phim có `SourceType = 1`.
- **Logic Tương tác:**
  - Khi click vào nút "Xem" trên thẻ phim, ứng dụng sẽ gọi tiến trình hệ thống mở trực tiếp file video đó bằng phần mềm xem video mặc định của Windows (VLC, KMPlayer, Windows Media Player...).

### 4.4. Form Chi Tiết & Thêm/Sửa Phim (Form CRUD Movie)
Một Form dùng chung cho cả việc thêm mới và cập nhật thông tin:
- Các trường nhập liệu: Mã phim (`TextBox`), Loại nguồn (`ComboBox`), Link/Đường dẫn file (`TextBox` + nút Browse file nếu là Local).
- Khu vực ảnh bìa: Click vào để chọn ảnh từ máy tính thông qua `OpenFileDialog`.
- Khu vực ảnh mô tả: Một `ListBox` hoặc danh sách nhỏ hiển thị các ảnh mô tả kèm nút "Thêm ảnh" (cho phép chọn nhiều file cùng lúc) và nút "Xóa ảnh".

---

## 5. ĐẶC TẢ LOGIC XỬ LÝ SÂU RỘNG (CORE UTILITIES)

AI cần thực hiện chính xác các khối xử lý logic nghiệp vụ sau để hệ thống vận hành chuyên nghiệp:

### 5.1. Quản lý Tệp tin và Đường dẫn Tương đối (Image Portability)
Để đảm bảo khi di chuyển thư mục phần mềm sang máy tính khác không bị mất ảnh, nghiêm cấm lưu đường dẫn tuyệt đối của ảnh vào DB.
- Quy trình khi người dùng chọn một ảnh bất kỳ:
  1. Kiểm tra thư mục gốc của ứng dụng, tạo thư mục con mang tên `App_Data\\CoverImages\\` và `App_Data\\DetailImages\\` nếu chưa có.
  2. Tạo một tên file ngẫu nhiên hoặc đặt tên theo định dạng: `[MovieCode]_[Guid].jpg` để tránh trùng lặp.
  3. Thực hiện lệnh `File.Copy(fileGoc, fileDich, true)`.
  4. Lưu đường dẫn tương đối (Ví dụ: `App_Data\\CoverImages\\JUR402_abc.jpg`) vào cơ sở dữ liệu.
  5. Khi hiển thị, kết hợp với `Application.StartupPath` để lấy đường dẫn đầy đủ.

### 5.2. Công cụ Khởi chạy Tiến trình Hệ thống (Media Launcher)
Để khởi chạy link web hoặc video mà không bị treo ứng dụng (UI Freeze), sử dụng bất đồng bộ (`async/await`) kết hợp với lớp `Process`:

```csharp
public static void LaunchMedia(string targetPath, int sourceType)
{
    try
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = targetPath,
            UseShellExecute = true // Bắt buộc để Windows tự điều hướng theo giao thức file/http
        };
        Process.Start(psi);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Không thể mở tệp tin hoặc liên kết. Lỗi: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}