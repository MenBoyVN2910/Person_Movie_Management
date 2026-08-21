# 🎬 MOVIE VAULT (WinForm) — Person Movie Management System

<p align="center">
  <img src="./Person_Movie_Management/Person_Movie_Management/Icon_Movie_Hub.ico" alt="Movie Vault Logo" width="100"/>
</p>

<p align="center">
  <strong>Phần mềm máy bàn (Desktop App) quản lý phim ảnh, âm nhạc và bộ sưu tập đa phương tiện cá nhân đỉnh cao trên nền tảng Windows.</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0%20Windows-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10"/>
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#"/>
  <img src="https://img.shields.io/badge/SQLite-WAL%20Mode-003B57?style=for-the-badge&logo=sqlite&logoColor=white" alt="SQLite"/>
  <img src="https://img.shields.io/badge/UI-Guna.UI2%20WinForms-FF4154?style=for-the-badge" alt="Guna UI2"/>
  <img src="https://img.shields.io/badge/Platform-Windows%2010%20%2F%2011-0078D6?style=for-the-badge&logo=windows&logoColor=white" alt="Windows"/>
  <img src="https://img.shields.io/badge/Version-3.0.0-success?style=for-the-badge" alt="Version 3.0"/>
</p>

---

## 📖 MỤC LỤC
1. [Giới Thiệu Dự Án & Ý Tưởng](#-1-giới-thiệu-dự-án--ý-tưởng)
2. [Các Tính Năng Chính (Key Features)](#-2-các-tính-năng-chính-key-features)
3. [Công Nghệ Áp Dụng (Tech Stack)](#-3-công-nghệ-áp-dụng-tech-stack)
4. [Kiến Trúc & Tổ Chức Hệ Thống (Architecture & Workflow)](#-4-kiến-trúc--tổ-chức-hệ-thống)
5. [Cơ Sở Dữ Liệu Tóm Tắt (Database Design)](#-5-cơ-sở-dữ-liệu-tóm-tắt)
6. [Hướng Dẫn Cài Đặt & Triển Khai (Installation)](#-6-hướng-dẫn-cài-đặt--triển-khai)
7. [Hướng Dẫn Sử Dụng Nhanh (User Quick Guide)](#-7-hướng-dẫn-sử-dụng-nhanh)
8. [Phím Tắt & Tiện Ích Thông Minh](#-8-phím-tắt--tiện-ích-thông-minh)
9. [Định Hướng Phát Triển (Roadmap)](#-9-định-hướng-phát-triển)

---

## 💡 1. GIỚI THIỆU DỰ ÁN & Ý TƯỞNG

### 🎯 Ý Tưởng Khởi Nguồn
Trong kỷ nguyên số, chúng ta thường lưu trữ hàng trăm bộ phim, video, bài hát từ nhiều nguồn khác nhau: từ các file tải về ổ cứng (MP4, MKV, FLAC, MP3,...) cho đến các đường link video trực tuyến yêu thích trên YouTube, Vimeo, Bilibili, Dailymotion, Twitch. 

Tuy nhiên, việc quản lý chúng bằng các thư mục mặc định của Windows Explorer gặp rất nhiều bất cập:
- Dữ liệu bị phân tán, tên file lộn xộn, thiếu áp phích (poster), ảnh bìa, tóm tắt nội dung và phân loại thể loại.
- Không thể lưu thông tin diễn viên, danh sách phát kết hợp cả video lẫn âm thanh.
- Không nhớ được mình đã xem đến đâu (Watch Progress) hoặc đánh giá độ yêu thích (Rating/Favorite).
- Không có chế độ bảo mật riêng tư khi có nhiều người cùng dùng chung máy tính.

👉 **Movie Vault (Ver 3.0)** ra đời như một **"Két Sắt Đa Phương Tiện Cá Nhân"**, gom toàn bộ thế giới phim ảnh và âm nhạc của bạn về một nơi duy nhất với giao diện Dark Mode hiện đại, mượt mà và trực quan chuẩn trải nghiệm rạp chiếu phim tại gia.

### 🌟 Giá Trị Dự Án Giải Quyết
* **Quản lý đa nguồn hợp nhất**: Hỗ trợ đồng thời cả **Local File** (file trên ổ cứng) và **Online Stream URL** (YouTube, Vimeo, Twitch, Bilibili, Dailymotion,...).
* **Tự động hóa thông tin**: Tích hợp **TMDB API** và bộ trích xuất web tự động cào Poster sắc nét, tóm tắt phim, ngày chiếu, điểm đánh giá và thể loại chỉ với **1 cú click**.
* **Trình phát nhạc tích hợp**: Nghe nhạc MP3/FLAC chất lượng cao ngay trong ứng dụng mà không cần mở phần mềm thứ ba.
* **Bảo mật & Cá nhân hóa**: Hệ thống phân quyền nhiều người dùng (Multi-User) với mật khẩu mã hóa chuẩn công nghiệp (BCrypt), dữ liệu của mỗi tài khoản hoàn toàn tách biệt.
* **Tự động giám sát thư mục**: Tự động phát hiện và thêm video/nhạc mới khi bạn tải về thư mục máy tính (Folder Watcher).
* **An toàn dữ liệu tuyệt đối**: Cơ chế Thùng rác (Recycle Bin - Soft Delete) chống xóa nhầm và Hệ thống Sao lưu/Phục hồi toàn diện (Backup & 1-Click Restore).

---

## ✨ 2. CÁC TÍNH NĂNG CHÍNH (KEY FEATURES)

### 🎬 1. Quản Lý Phim & Video Thông Minh
- **Đa nguồn phát**: Khởi chạy trực tiếp file video bằng ứng dụng mặc định của Windows hoặc phát video trực tuyến qua trình duyệt.
- **Tích hợp TMDB API v3**: Tự động tra cứu kho phim toàn cầu, lấy Poster HD, nội dung tiếng Việt (`vi-VN`), thể loại và quy đổi điểm số 5 sao.
- **Trích xuất Thumbnail tự động**: Tự tạo ảnh bìa cho video nội bộ bằng công nghệ FFmpeg siêu nhanh.
- **Thư viện ảnh chi tiết (Gallery)**: Lưu trữ nhiều hình ảnh hậu trường, poster phụ cho từng bộ phim.
- **Gắn Tag & Đánh giá**: Hệ thống thẻ màu sắc (Color Tag), đánh dấu yêu thích và đánh giá sao (1 - 5 ⭐).
- **Theo dõi tiến trình xem (Watch Progress)**: Ghi nhận thời điểm xem gần nhất và thanh phần trăm tiến độ đã xem.

### 🎵 2. Quản Lý Âm Nhạc & Trình Phát Tích Hợp (Audio Player)
- **Hỗ trợ định dạng âm thanh phong phú**: MP3, WAV, FLAC, AAC, M4A, OGG,...
- **Đọc Metadata ID3**: Tự động đọc tên ca sĩ, album, năm phát hành, thời lượng và ảnh bìa từ metadata file gốc (TagLibSharp).
- **Trình phát nhạc Mini Bar**: Tích hợp sẵn thanh phát nhạc dưới đáy màn hình (Play/Pause, tua thời gian mượt mà, chỉnh âm lượng, lặp bài, hiển thị sóng âm).

### 🌟 3. Quản Lý Diễn Viên & Quốc Tịch (Cast & Celebrities)
- Lưu hồ sơ diễn viên: Ảnh đại diện, ngày sinh, quốc tịch, tiểu sử tóm tắt và thư viện ảnh cá nhân.
- Tự động liên kết và hiển thị danh sách các phim mà diễn viên đó đã tham gia cùng vai diễn cụ thể.

### 📋 4. Danh Sách Phát Linh Hoạt (Smart Playlists)
- Tạo playlist theo chủ đề với ảnh bìa tùy chọn.
- Cho phép kết hợp hỗn hợp cả **Phim** lẫn **Bài hát** trong cùng một danh sách.
- Hỗ trợ sắp xếp thứ tự các mục nhanh chóng.

### 🔍 5. Tìm Kiếm Toàn Cục Tức Thì (Omnibox / Spotlight)
- Nhấn tổ hợp phím tắt nhanh để mở thanh tìm kiếm thông minh: Tìm tức thì phim, audio, diễn viên hoặc tag theo từ khóa với kết quả hiển thị dạng pop-up thời gian thực.

### 📥 6. Nhập Dữ Liệu Siêu Tốc (Batch Import & Drop Widget)
- **Batch Import**: Quét toàn bộ thư mục trên máy và nạp hàng loạt video/audio chỉ trong vài giây.
- **Floating Drop Widget**: Cửa sổ widget nổi nhỏ gọn trên màn hình — chỉ cần kéo thả file từ Desktop vào widget là phim/nhạc được nạp ngay vào hệ thống.

### 👁️ 7. Giám Sát Thư Mục Tự Động (Folder Watcher)
- Thiết lập thư mục Download/Media cần theo dõi. Khi có file video/nhạc mới được tải về máy tính, hệ thống tự động nhận diện và thêm vào kho lưu trữ.

### 🗑️ 8. Thùng Rác An Toàn (Recycle Bin)
- Tính năng xóa mềm (Soft Delete) giúp bảo vệ dữ liệu. Người dùng có thể khôi phục lại (Restore) hoặc dọn dẹp vĩnh viễn (Permanent Delete) bất cứ lúc nào.

### 💾 9. Quản Lý Sao Lưu & Phục Hồi (Backup & Restore)
- Đóng gói toàn bộ Database, hình ảnh bìa và cài đặt thành một tệp `.zip` an toàn.
- Khôi phục dữ liệu chỉ với 1 cú click (1-Click Restore) kèm kiểm tra tính toàn vẹn.

---

## 🛠️ 3. CÔNG NGHỆ ÁP DỤNG (TECH STACK)

| Nhóm công nghệ | Thư viện / Nền tảng | Mục đích sử dụng |
| :--- | :--- | :--- |
| **Core Framework** | **.NET 10.0 (C# 13)** Windows Forms | Nền tảng ứng dụng desktop hiệu năng cao, tối ưu hóa cho Windows 10/11 x64. |
| **UI Components** | **Guna.UI2.WinForms (v2.0.4.8)** | Bộ giao diện hiện đại, hỗ trợ bo góc, hiệu ứng hover, đổ bóng, Dark Mode cao cấp. |
| **Database Engine** | **SQLite (Microsoft.Data.Sqlite 10.0)** | Cơ sở dữ liệu nhúng nhẹ, chạy chế độ **WAL (Write-Ahead Logging)** cho tốc độ đọc/ghi tức thì. |
| **Bảo mật** | **BCrypt.Net-Next (v4.2.0)** | Mã hóa băm mật khẩu người dùng với salt an toàn tuyệt đối. |
| **Âm thanh** | **NAudio (v2.3.0)** | Xử lý âm thanh đa luồng, phát nhạc nền và điều khiển phát trực tiếp trong ứng dụng. |
| **Trích xuất Video** | **NReco.VideoConverter (FFmpeg)** | Tạo ảnh thumbnail từ các đoạn phim nhanh chóng. |
| **Metadata Tag** | **TagLibSharp (v2.3.0)** | Trích xuất thông tin thẻ ID3 (Artist, Album, Cover Art, Bitrate) của file nhạc. |
| **Xử lý đồ họa** | **Magick.NET (ImageMagick Q8)** | Xử lý nén ảnh, tạo bộ nhớ đệm ảnh đại diện và poster không gây rò rỉ bộ nhớ (Memory Leak). |
| **Web & Scraping** | **HtmlAgilityPack (v1.12.4)** & **HttpClient** | Phân tích cú pháp HTML, trích xuất OpenGraph metadata và kết nối TMDB REST API. |

---

## 🏗️ 4. KIẾN TRÚC & TỔ CHỨC HỆ THỐNG

Dự án được cấu trúc theo mô hình **Phân lớp (Layered Architecture)** kết hợp **Repository Pattern** nhằm đảm bảo mã nguồn gọn gàng, dễ bảo trì và mở rộng:

```
📦 Person_Movie_Management
 ┣ 📂 Data               # Cấu hình SQLite, khởi tạo bảng, tạo chỉ mục Index & Migrations
 ┣ 📂 Models             # Các thực thể dữ liệu (User, Movie, Audio, Actor, Playlist, Tag,...)
 ┣ 📂 Repositories       # Tầng truy vấn dữ liệu CRUD riêng biệt cho từng đối tượng
 ┣ 📂 Services           # Tầng nghiệp vụ (Auth, TMDB, Backup, FolderWatcher, MediaLauncher)
 ┣ 📂 Helpers            # Các tiện ích bổ trợ:
 ┃   ┣ 📂 Adapters       # Trích xuất dữ liệu Online (YouTube, Vimeo, Twitch, Bilibili, Dailymotion)
 ┃   ┣ 📜 ImageCache.cs  # Bộ nhớ đệm xử lý ảnh mượt mà
 ┃   ┣ 📜 VirtualWrapPanel.cs # Tối ưu hóa render danh sách dạng lưới (Virtualization)
 ┃   ┗ 📜 UIHelper.cs    # Định dạng giao diện, hiệu ứng chuyển động
 ┣ 📂 Forms              # Các cửa sổ tương tác (Login, MovieDetail, ActorDetail, Omnibox,...)
 ┣ 📂 UserControls       # Các màn hình thành phần gắn vào giao diện chính (MovieList, AudioPlayer,...)
 ┗ 📜 Program.cs         # Điểm khởi chạy ứng dụng (Entry Point)
```

### 🔄 Luồng Hoạt Động Cơ Bản (Workflow)
1. **Khởi động**: `Program.cs` kiểm tra/nâng cấp SQLite (`DatabaseHelper.Initialize()`) ➔ Nạp cấu hình ➔ Hiển thị màn hình Đăng nhập (`FrmLogin`).
2. **Đăng nhập**: `AuthService` xác thực mật khẩu qua BCrypt ➔ Khởi tạo `SessionManager` cho người dùng hiện tại ➔ Mở `FrmMain`.
3. **Quản lý nội dung**: Người dùng tương tác trên các `UserControls` ➔ Tầng `Forms/Controls` gọi `Services` (xử lý logic, tải API, cào dữ liệu) ➔ `Repositories` tương tác với SQLite thông qua tham số hóa an toàn.
4. **Giám sát & Dọn dẹp**: `FolderWatcherService` hoạt động ngầm để auto-import; Khi đóng ứng dụng, các cache ảnh và luồng audio được giải phóng tự động.

---

## 🗄️ 5. CƠ SỞ DỮ LIỆU TÓM TẮT

Cơ sở dữ liệu SQLite (`AppDatabase.db`) được lưu trữ cục bộ trong thư mục `App_Data` với thiết kế quan hệ tinh gọn:

```
                      ┌────────────────────────────────────────┐
                      │                👤 USERS                │
                      │  (Id, Username, DisplayName, Password) │
                      └───────────────────┬────────────────────┘
             ┌────────────────┬───────────┴───────────┬────────────────┐
             │ (1:N)          │ (1:N)                 │ (1:N)          │ (1:N)
             ▼                ▼                       ▼                ▼
  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
  │    🎬 MOVIES     │  │    🎵 AUDIOS     │  │    🌟 ACTORS     │  │   📋 PLAYLISTS   │
  │ (Code, URL, Info)│  │ (Code, Blob, Info│  │ (Name, DOB, Bio) │  │  (Name, Cover)   │
  └──────────┬───────┘  └────────┬─────────┘  └────────┬─────────┘  └────────┬─────────┘
        1:N  │   N:M ┌───────────┘        N:M ┌────────┘                    │ 1:N
   ┌─────────┤       ▼                        ▼                             ▼
   ▼         ▼ ┌─────────────┐          ┌─────────────┐            ┌──────────────────┐
┌───────┐ ┌──┴─┤ AUDIO_TAGS  │          │ MOVIE_ACTORS│            │  PLAYLIST_ITEMS  │
│IMAGES │ │ M  │ (Audio+Tag) │          │ (Movie+Actor│            │ (Movie/Audio Id) │
└───────┘ │ O  └──────┬──────┘          └─────────────┘            └──────────────────┘
          │ V         │
          │ I         ▼
          │ E_TAGS ──► 🏷️ TAGS (TagName, ColorHex)
          └───────────┘
```

| Bảng (Table) | Mục đích & Quan hệ thực thể |
| :--- | :--- |
| **`Users`** | Quản lý tài khoản đăng nhập (mật khẩu băm BCrypt), phân tách dữ liệu từng người dùng. |
| **`Movies`** | Lưu trữ phim/video (Local file, URL Online, TMDB metadata, lịch sử & tiến độ xem). *(Thuộc về `Users`)* |
| **`Audios`** | Lưu trữ tệp âm thanh/nhạc (ID3 tags, ảnh bìa, dữ liệu nhúng). *(Thuộc về `Users`)* |
| **`Actors`** | Hồ sơ nghệ sĩ/diễn viên (quốc tịch, tiểu sử, ngày sinh, thư viện ảnh). *(Thuộc về `Users`)* |
| **`Playlists`** | Danh sách phát đa năng kết hợp cả Phim và Bài hát. *(Thuộc về `Users`)* |
| **`Tags`** | Hệ thống thẻ gắn màu sắc để phân loại phim/nhạc linh hoạt. *(Thuộc về `Users`)* |
| **`MovieTags` / `AudioTags`** | Bảng trung gian liên kết nhiều-nhiều (N:M) giữa Phim/Nhạc và Thẻ. |
| **`MovieActors`** | Bảng trung gian liên kết nhiều-nhiều (N:M) giữa Phim và Diễn viên kèm theo vai diễn (`Role`). |
| **`PlaylistItems`** | Chi tiết các mục (ItemType: Movie hoặc Audio) được xếp thứ tự trong Playlist. |

* **Khóa ngoại & Xóa theo tầng (Cascade Delete)**: Tự động dọn dẹp các liên kết trung gian khi phim/diễn viên bị xóa hoàn toàn khỏi DB.
* **Được tối ưu hóa bằng Index**: Tăng tốc độ lọc theo `UserId`, `IsDeleted`, `SourceType`, `IsFavorite`.


---

## 🚀 6. HƯỚNG DẪN CÀI ĐẶT & TRIỂN KHAI

### 💻 Yêu Cầu Hệ Thống
- **Hệ điều hành**: Windows 10 (Build 19041 trở lên) hoặc Windows 11 (64-bit).
- **Môi trường chạy**: [.NET 10.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) (x64).
- **Môi trường phát triển (Nếu tự build mã nguồn)**: 
  - [Visual Studio 2022 / 2026](https://visualstudio.microsoft.com/) (đã cài gói *.NET Desktop Development*).
  - .NET 10.0 SDK.

---

### 📥 Cách 1: Sử Dụng Bản Đóng Gói (Portable Release)
1. Tải file nén phát hành mới nhất: `Person_Movie_Management_Winform_Ver3.0.rar`.
2. Giải nén vào một thư mục bất kỳ trên máy tính (ví dụ: `D:\MovieVault`).
3. Nhấp đúp vào file `Person_Movie_Management.exe` để khởi chạy ngay mà không cần cài đặt rườm rà.

---

### 🛠️ Cách 2: Tải Mã Nguồn & Tự Biên Dịch (Build from Source)

1. **Clone mã nguồn từ kho lưu trữ:**
   ```bash
   git clone https://github.com/your-username/Person_Movie_Management.git
   cd Person_Movie_Management
   ```

2. **Khôi phục các gói NuGet dependencies:**
   ```bash
   dotnet restore
   ```

3. **Biên dịch và chạy ứng dụng bằng .NET CLI:**
   ```bash
   dotnet build --configuration Release
   dotnet run --project Person_Movie_Management/Person_Movie_Management.csproj
   ```

*(Hoặc bạn chỉ cần mở file `Person_Movie_Management.slnx` bằng Visual Studio và nhấn `F5` / `Ctrl + F5`)*.

---

## 🧭 7. HƯỚNG DẪN SỬ DỤNG CHI TIẾT THEO TỪNG CHỨC NĂNG

Hệ thống cung cấp cẩm nang tương tác trực quan ngay tại **Trang Chủ (Dashboard)**. Dưới đây là quy trình thao tác chuẩn xác cho từng phân hệ:

### 🌐 1. Quản Lý Phim Online (Web & Stream)
* **Thêm phim mới**: Nhấn nút `+ Thêm phim` ➔ Nhập tên/mã phim ➔ Dán đường link vào ô `Media URL` (hỗ trợ YouTube, Vimeo, Twitch, Bilibili, Dailymotion, web stream,...).
* **Cào dữ liệu tự động**: Nhấn nút **`🎬 TMDB API`** để hệ thống tự động điền poster chất lượng cao, tóm tắt nội dung, ngày công chiếu và thể loại tiếng Việt.
* **Gắn thẻ & Diễn viên**: Chọn các Tag thể loại, chọn diễn viên tham gia, chấm điểm sao (1 - 5 ⭐) và nhấn **Lưu**.
* **Thao tác nhanh trên thẻ phim**:
  - *Click đúp (2 lần chuột trái)*: Mở ngay trình duyệt web mặc định để phát video.
  - *Click chuột phải*: Mở menu ngữ cảnh để `Chỉnh sửa`, `Cập nhật tiến độ xem (%)`, `Thêm vào Playlist` hoặc `Xóa vào Thùng rác`.
  - *Nút "i"*: Mở cửa sổ xem tóm tắt chi tiết và album ảnh/poster phụ.
  - *Nút Trái Tim (❤️)*: Thêm hoặc bỏ khỏi danh sách Yêu Thích.

---

### 📁 2. Quản Lý Phim Trên Máy (Video Offline)
* **Thêm từng phim**: Nhấn `+ Thêm phim` (chọn nguồn *Phim Trên Máy*) ➔ Duyệt chọn file video (`.mp4`, `.mkv`, `.avi`, `.wmv`, `.mov`,...). Hệ thống sẽ tự trích xuất ảnh thumbnail sắc nét từ video bằng FFmpeg.
* **Quét thư mục hàng loạt (Batch Scan / Import)**: Nhấn `📂 Quét thư mục` ➔ Chọn thư mục phim trên máy tính ➔ Phần mềm sẽ tự động quét đệ quy, làm sạch tên file rác và tạo hàng loạt thẻ phim chỉ trong vài giây.
* **Giám sát tự động (Folder Watcher)**: Dịch vụ ngầm theo dõi thư mục Media đã thiết lập. Khi bạn tải hoặc sao chép phim mới về máy, ứng dụng sẽ tự động nhận diện và đưa vào danh sách.
* **Thao tác chuột**:
  - *Click đúp*: Khởi chạy video trực tiếp bằng trình xem video mặc định của Windows (VLC, PotPlayer, MPC-HC, Windows Media Player,...).
  - *Click chuột phải*: Chọn `Mở thư mục chứa file` để mở ngay Explorer trỏ đến vị trí file gốc trên ổ cứng.

---

### 🎵 3. Quản Lý Âm Nhạc & Trình Phát Toàn Cục (Audio Player)
* **Thêm Audio**: Nhấn `+ Thêm Audio` ➔ Chọn file nhạc (`.mp3`, `.wav`, `.flac`, `.m4a`, `.aac`, `.ogg`,...). Hệ thống tự động đọc thông tin thẻ ID3 (Ca sĩ, Album, Thời lượng, Bitrate, Ảnh bìa nhúng).
* **Trình phát nhạc nền toàn cục (Mini Audio Player)**: Bấm nút **Play** tại bất kỳ thẻ bài hát nào, thanh phát nhạc dưới đáy sẽ kích hoạt. Bạn có thể thoải mái chuyển sang Trang chủ, Phim, Diễn viên, Sao lưu... nhạc vẫn phát liên tục không bị ngắt quãng.
* **Xuất file nhạc (Export)**: Chuột phải vào thẻ bài hát ➔ Chọn `Export Audio` để trích xuất file âm thanh gốc ra thư mục mong muốn trên máy.

---

### 👥 4. Quản Lý Diễn Viên & Album Ảnh (Cast & Gallery)
* **Tạo hồ sơ**: Nhấn `+ Thêm diễn viên` ➔ Nhập họ tên, chọn ảnh đại diện (Avatar), ngày sinh, quốc tịch và tiểu sử tóm tắt.
* **Thư viện ảnh chi tiết**: Thêm nhiều ảnh photoshoot/poster/hậu trường vào hồ sơ diễn viên; click vào ảnh để phóng to xem chi tiết.
* **Liên kết tác phẩm tự động**: Khi bạn gán diễn viên vào một bộ phim, bộ phim đó sẽ **tự động xuất hiện** trong mục *"Các phim đã tham gia"* trong hồ sơ diễn viên. Click trực tiếp vào thẻ phim để mở xem ngay.

---

### 📑 5. Danh Sách Phát Đa Năng (Playlists)
* **Tạo Playlist**: Nhấn `+ Tạo Playlist` ➔ Đặt tên, mô tả và chọn ảnh bìa đại diện.
* **Kết hợp Phim + Nhạc**: Chuột phải vào bất kỳ thẻ phim hoặc bài hát nào ➔ Chọn `Thêm vào Playlist` ➔ Chọn danh sách phát mong muốn. *(Một Playlist có thể chứa đồng thời cả video và âm thanh)*.

---

### 🗑️ 6. Thùng Rác An Toàn (Recycle Bin - Soft Delete)
* Khi xóa bất kỳ phim hay bản nhạc nào, dữ liệu được chuyển vào Thùng rác nhằm bảo vệ an toàn, tránh xóa nhầm.
* **Khôi phục (Restore)**: Chọn mục cần lấy lại và nhấn `Khôi phục` ➔ Dữ liệu trở về đúng vị trí cũ với nguyên vẹn tags, diễn viên, ảnh bìa, ghi chú và điểm đánh giá.
* **Dọn sạch**: Nhấn `Dọn sạch thùng rác` nếu muốn xóa vĩnh viễn và giải phóng ổ cứng.

---

### 💾 7. Sao Lưu & Khôi Phục Hệ Thống (Backup & Restore)
* **Đa thư mục sao lưu**: Thêm nhiều đường dẫn đích (Ổ D:, E:, thư mục đồng bộ Google Drive, Dropbox, OneDrive,...). 
* **Tự động sao lưu an toàn**: Hệ thống tự động snapshot cơ sở dữ liệu SQLite mỗi khi thoát ứng dụng.
* **Khôi phục tức thì**: Nhấn `Khôi phục từ file` ➔ Chọn file database sao lưu `.db` ➔ Hệ thống kiểm tra tính toàn vẹn và phục hồi toàn bộ dữ liệu chỉ với 1 cú click.

---

## ⚡ 8. TIỆN ÍCH TOÀN CỤC & BẢNG PHÍM TẮT

### 🛠️ Tiện Ích Độc Đáo
- **Tìm kiếm toàn cục (Omnibox - `Ctrl + K`)**: Nhấn `Ctrl + K` ở bất kỳ đâu để mở thanh tra cứu nhanh tức thì theo tên phim, bài hát, playlist và mở xem trực tiếp.
- **Widget kéo thả nổi (Drop Widget)**: Widget tròn nhỏ luôn nổi ngoài Desktop — chỉ cần kéo link từ trình duyệt hoặc kéo file video từ máy thả vào widget để nạp nhanh vào kho.
- **Tiếp tục xem dở (Resume Progress)**: Hiển thị các phim đang xem dở (1% - 99%) ngay trên Trang chủ, hỗ trợ cập nhật tiến độ xem bằng chuột phải.

### ⌨️ Bảng Phím Tắt Thao Tác
| Phím tắt | Chức năng thực thi |
| :--- | :--- |
| **`Ctrl + K`** | Mở thanh tìm kiếm nhanh toàn cục (Omnibox). |
| **`Space` (Phím Cách)** | Tạm dừng (Pause) / Tiếp tục phát nhạc (Play). |
| **`Mũi tên Trái (◀)`** | Tua lùi 10 giây khi phát nhạc. |
| **`Mũi tên Phải (▶)`** | Tua tới 10 giây khi phát nhạc. |
| **`Mũi tên Lên (▲)`** | Tăng 5% âm lượng. |
| **`Mũi tên Xuống (▼)`** | Giảm 5% âm lượng. |
| **`F5`** | Làm mới lại danh sách dữ liệu trên màn hình hiện tại. [Lúc Được Lúc Không :)) ] |
| **`Esc`** | Đóng nhanh các hộp thoại popup hoặc cửa sổ tìm kiếm. [Lúc Được Lúc Không :)) ] |

---

## 🔮 9. ĐỊNH HƯỚNG PHÁT TRIỂN (ROADMAP)

- [ ] Tích hợp trình phát video nhúng trực tiếp bằng WebView2 / VLC Core.
- [ ] Tự động tải phụ đề tiếng Việt từ Subscene / OpenSubtitles API.
- [ ] Bổ sung tính năng đồng bộ đám mây (Cloud Sync qua Google Drive / OneDrive).
- [ ] Thống kê biểu đồ sở thích xem phim và thời gian nghe nhạc theo tháng.

---

## 📄 GIẤY PHÉP & BẢN QUYỀN (LICENSE)
Dự án được phát triển và phát hành cho mục đích học tập, quản lý cá nhân và phi thương mại.  
*Phát triển bởi **BMN2910** — 2026.*

