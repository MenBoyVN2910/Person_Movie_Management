# Lưu ý về Kích thước Cơ sở dữ liệu (SQLite DB Size Warning)

## Vấn đề
Khi tính năng **Quản lý Âm Thanh (.mp3)** được đưa vào sử dụng, file `.mp3` sẽ được lưu trực tiếp dưới dạng dữ liệu nhị phân (BLOB) vào bên trong file cơ sở dữ liệu `AppDatabase.db` thay vì chỉ lưu đường dẫn như các tính năng trước đây.

### Nguyên nhân gây tăng dung lượng
- Mỗi bài hát MP3 chất lượng thông thường dài khoảng 3-5 phút có kích thước từ **3MB đến 10MB**.
- Với giới hạn tối đa cho phép là **50MB** mỗi file, nếu người dùng thêm 20 bài hát lớn (50MB/bài), kích thước file CSDL `.db` có thể dễ dàng tăng thêm **1GB**.
- SQLite hoàn toàn có thể hỗ trợ các file cơ sở dữ liệu lên đến hàng trăm Gigabyte, tuy nhiên:
  1. Việc sao lưu (Backup) và phục hồi (Restore) toàn bộ file `.db` sẽ mất nhiều thời gian hơn và tốn nhiều dung lượng ổ cứng hơn.
  2. Khi thao tác nạp (Load) dữ liệu có chứa cột BLOB lớn vào bộ nhớ (RAM), nếu không xử lý tối ưu (như bỏ qua cột AudioData khi chỉ cần hiển thị danh sách), phần mềm có thể bị chậm hoặc tiêu tốn nhiều RAM.

## Khuyến nghị và Giải pháp
1. **Giới hạn kích thước:** Tính năng thêm file Âm thanh đã được thiết lập cứng (hardcode) giới hạn mỗi file MP3 chỉ được tối đa **50MB** và thời lượng không quá **20 phút**.
2. **Thiết kế Truy vấn:** Lớp `AudioRepository` chỉ lấy dữ liệu `AudioData` khi người dùng yêu cầu phát nhạc (truy vấn lấy chi tiết từng bài). Danh sách tổng quan (`UcAudioList`) không tải cột `AudioData` để đảm bảo tốc độ phản hồi tức thời của giao diện.
3. **Quản lý Sao lưu:** Backup của Âm Thanh được tách riêng biệt với Phim Online. Khi người dùng bấm sao lưu, hệ thống sẽ chỉ nén dữ liệu thuộc về Âm thanh (Audios) thành một file `.zip` độc lập, tránh làm nặng các quy trình sao lưu khác.
4. **Bảo trì:** Người dùng nên thường xuyên xuất (Export) các audio cũ sang file `.zip` lưu trữ và xóa bớt trong phần mềm nếu không còn dùng thường xuyên để giữ file `.db` gọn nhẹ.
