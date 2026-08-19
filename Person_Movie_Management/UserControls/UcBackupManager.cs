using System;
using System.IO;
using System.Windows.Forms;
using Person_Movie_Management.Services;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.UserControls
{
    public partial class UcBackupManager : UserControl
    {
        public UcBackupManager()
        {
            InitializeComponent();
            this.Load += UcBackupManager_Load;

            // Lắng nghe tiến trình backup để cập nhật UI
            AppServices.BackupSvc.ProgressChanged += OnBackupProgress;
            this.Disposed += (s, e) => { AppServices.BackupSvc.ProgressChanged -= OnBackupProgress; };
        }

        private void UcBackupManager_Load(object? sender, EventArgs e)
        {
            if (!DesignMode)
            {
                RefreshList();
                flpPaths.Resize += FlpPaths_Resize;
                UpdateStatusLabel("Trạng thái: Đang rảnh", System.Drawing.Color.FromArgb(148, 163, 184));
            }
        }

        private void FlpPaths_Resize(object? sender, EventArgs e)
        {
            foreach (Control c in flpPaths.Controls)
            {
                c.Width = flpPaths.Width - 40;
                foreach (Control child in c.Controls)
                {
                    if (child is Guna.UI2.WinForms.Guna2Button btn && btn.Name == "btnDel")
                    {
                        btn.Location = new System.Drawing.Point(c.Width - 50, 22);
                    }
                    else if (child is Label lbl && (lbl.Name == "lblPath" || lbl.Name == "lblStatus"))
                    {
                        lbl.Width = c.Width - 130;
                    }
                }
            }
        }

        // ─── Progress callback ─────────────────────────────────────────────────

        private void OnBackupProgress(int current, int total, string message)
        {
            // Chạy trên UI thread vì có thể callback từ thread khác
            if (this.InvokeRequired)
            {
                this.BeginInvoke(() => OnBackupProgress(current, total, message));
                return;
            }

            if (message.StartsWith("✅") || message.StartsWith("❌"))
            {
                bool success = message.StartsWith("✅");
                UpdateStatusLabel(message,
                    success
                        ? System.Drawing.Color.FromArgb(74, 222, 128)
                        : System.Drawing.Color.FromArgb(252, 100, 100));

                // Khi hoàn thành tất cả → refresh list + unlock nút
                if (current >= total)
                {
                    RefreshList();
                    SetBackupBusy(false);
                    UpdateStatusLabel(
                        $"Trạng thái: Sao lưu xong lúc {DateTime.Now:HH:mm:ss}",
                        System.Drawing.Color.FromArgb(74, 222, 128));
                }
            }
            else
            {
                // Đang xử lý
                UpdateStatusLabel($"({current}/{total}) {message}",
                    System.Drawing.Color.FromArgb(251, 191, 36));
            }
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private void UpdateStatusLabel(string text, System.Drawing.Color color)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }

        private void SetBackupBusy(bool isBusy)
        {
            btnBackupNow.Enabled = !isBusy;
            btnBackupNow.Text = isBusy ? "⏳ Đang backup..." : "🚀 Backup Ngay";
        }

        // ─── Refresh danh sách đường dẫn ──────────────────────────────────────

        private void RefreshList()
        {
            flpPaths.Controls.Clear();

            if (AppServices.BackupSvc == null) return;

            var paths = AppServices.BackupSvc.GetBackupPaths();
            if (paths.Count == 0)
            {
                // Hiển thị placeholder khi chưa có đường dẫn nào
                var lblEmpty = new Label
                {
                    Text = "📂 Chưa có thư mục sao lưu nào.\nNhấn \"+ Thêm thư mục\" để bắt đầu.",
                    Font = new System.Drawing.Font("Segoe UI", 11F),
                    ForeColor = System.Drawing.Color.FromArgb(100, 116, 139),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    BackColor = System.Drawing.Color.Transparent
                };
                flpPaths.Controls.Add(lblEmpty);
                return;
            }

            foreach (var path in paths)
            {
                flpPaths.Controls.Add(CreatePathCard(path));
            }
        }

        private Control CreatePathCard(string path)
        {
            bool exists = Directory.Exists(path);

            var pnl = new Guna.UI2.WinForms.Guna2Panel();
            pnl.BorderRadius = 10;
            pnl.FillColor = System.Drawing.Color.FromArgb(30, 41, 59);
            pnl.Size = new System.Drawing.Size(flpPaths.Width - 40, 80);
            pnl.Margin = new Padding(0, 0, 0, 8);

            // Icon
            var lblIcon = new Label();
            lblIcon.Text = exists ? "📁" : "⚠️";
            lblIcon.Font = new System.Drawing.Font("Segoe UI", 18F);
            lblIcon.AutoSize = true;
            lblIcon.Location = new System.Drawing.Point(16, 20);
            lblIcon.BackColor = System.Drawing.Color.Transparent;

            // Path text
            var lblPath = new Label();
            lblPath.Name = "lblPath";
            lblPath.Text = path;
            lblPath.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            lblPath.ForeColor = System.Drawing.Color.White;
            lblPath.AutoSize = false;
            lblPath.Size = new System.Drawing.Size(pnl.Width - 130, 24);
            lblPath.Location = new System.Drawing.Point(75, 14);
            lblPath.BackColor = System.Drawing.Color.Transparent;
            lblPath.AutoEllipsis = true;

            // Status
            var lblStatus = new Label();
            lblStatus.Name = "lblStatus";
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblStatus.AutoSize = false;
            lblStatus.Size = new System.Drawing.Size(pnl.Width - 130, 22);
            lblStatus.Location = new System.Drawing.Point(75, 42);
            lblStatus.BackColor = System.Drawing.Color.Transparent;

            if (!exists)
            {
                lblStatus.Text = "Thư mục không tồn tại";
                lblStatus.ForeColor = System.Drawing.Color.Orange;
            }
            else
            {
                // Tìm file backup mới nhất để hiển thị thời gian
                string lastBackupInfo = GetLastBackupInfo(path);
                lblStatus.Text = lastBackupInfo;
                lblStatus.ForeColor = System.Drawing.Color.FromArgb(74, 222, 128);
            }

            // Nút xóa
            var btnDel = new Guna.UI2.WinForms.Guna2Button();
            btnDel.Name = "btnDel";
            btnDel.Text = "✕";
            btnDel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnDel.FillColor = System.Drawing.Color.FromArgb(60, 40, 40);
            btnDel.ForeColor = System.Drawing.Color.FromArgb(252, 100, 100);
            btnDel.BorderRadius = 8;
            btnDel.Size = new System.Drawing.Size(36, 36);
            btnDel.Location = new System.Drawing.Point(pnl.Width - 50, 22);
            btnDel.Cursor = Cursors.Hand;
            btnDel.Click += (s, e) =>
            {
                var res = MessageBox.Show(
                    $"Xóa thư mục backup này?\n\n{path}\n\nLưu ý: Thao tác này chỉ xóa khỏi danh sách, không xóa file trên máy.",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    AppServices.BackupSvc?.RemoveBackupPath(path);
                    RefreshList();
                    UpdateStatusLabel("Trạng thái: Đã xóa đường dẫn",
                        System.Drawing.Color.FromArgb(148, 163, 184));
                }
            };

            pnl.Controls.AddRange(new Control[] { lblIcon, lblPath, lblStatus, btnDel });
            return pnl;
        }

        private static string GetLastBackupInfo(string folderPath)
        {
            try
            {
                // Ưu tiên kiểm tra file .zip trước
                string targetZip = Path.Combine(folderPath, "MovieVault_Backup.zip");
                if (File.Exists(targetZip))
                {
                    var lastWrite = File.GetLastWriteTime(targetZip);
                    var fileInfo = new FileInfo(targetZip);
                    string sizeStr = fileInfo.Length > 1024 * 1024
                        ? $"{(fileInfo.Length / (1024.0 * 1024.0)):F1} MB"
                        : $"{(fileInfo.Length / 1024.0):F1} KB";
                    return $"✅ Đã sao lưu: {lastWrite:dd/MM/yyyy HH:mm:ss} ({sizeStr})";
                }

                // Fallback: kiểm tra file .db
                string targetDb = Path.Combine(folderPath, "MovieVault_Backup.db");
                if (File.Exists(targetDb))
                {
                    var lastWrite = File.GetLastWriteTime(targetDb);
                    var fileInfo = new FileInfo(targetDb);
                    string sizeStr = fileInfo.Length > 1024 * 1024 
                        ? $"{(fileInfo.Length / (1024.0 * 1024.0)):F1} MB" 
                        : $"{(fileInfo.Length / 1024.0):F1} KB";
                    return $"✅ Đã sao lưu: {lastWrite:dd/MM/yyyy HH:mm:ss} ({sizeStr})";
                }

                var zipFiles = Directory.GetFiles(folderPath, "*.zip");
                if (zipFiles.Length > 0)
                {
                    Array.Sort(zipFiles);
                    string latest = zipFiles[zipFiles.Length - 1];
                    var lastWrite = File.GetLastWriteTime(latest);
                    return $"✅ Đã sao lưu: {lastWrite:dd/MM/yyyy HH:mm:ss}";
                }

                var dbFiles = Directory.GetFiles(folderPath, "*.db");
                if (dbFiles.Length > 0)
                {
                    Array.Sort(dbFiles);
                    string latest = dbFiles[dbFiles.Length - 1];
                    var lastWrite = File.GetLastWriteTime(latest);
                    return $"✅ Đã sao lưu: {lastWrite:dd/MM/yyyy HH:mm:ss}";
                }

                return "✅ Thư mục sẵn sàng (chưa có file sao lưu)";
            }
            catch
            {
                return "✅ Đang hoạt động";
            }
        }

        // ─── Event Handlers ────────────────────────────────────────────────────

        private void btnAddPath_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            fbd.Description = "Chọn thư mục để lưu file backup";
            fbd.UseDescriptionForTitle = true;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                AppServices.BackupSvc?.AddBackupPath(fbd.SelectedPath);
                RefreshList();
                UpdateStatusLabel(
                    "Trạng thái: Đã thêm thư mục sao lưu thành công.",
                    System.Drawing.Color.FromArgb(74, 222, 128));
            }
        }

        private void btnRemovePath_Click(object sender, EventArgs e)
        {
            // Được xử lý bởi nút ✕ trên mỗi card
        }

        private async void btnBackupNow_Click(object sender, EventArgs e)
        {
            if (AppServices.BackupSvc == null) return;

            var paths = AppServices.BackupSvc.GetBackupPaths();
            if (paths.Count == 0)
            {
                MessageBox.Show(
                    "Chưa có thư mục backup nào được cấu hình.\nVui lòng nhấn \"+ Thêm thư mục\" trước.",
                    "Chưa có thư mục",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            SetBackupBusy(true);
            UpdateStatusLabel("Trạng thái: Đang chuẩn bị sao lưu...",
                System.Drawing.Color.FromArgb(251, 191, 36));

            await AppServices.BackupSvc.PerformBackupAsync();

            // Nếu không có progress events (không có paths hợp lệ) → reset
            SetBackupBusy(false);
        }

        private async void btnRestore_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Title = "Chọn File Backup Movie Vault để khôi phục";
            ofd.Filter = "File Backup Movie Vault (*.zip;*.db)|*.zip;*.db|File ZIP (*.zip)|*.zip|File Database (*.db;*.sqlite)|*.db;*.sqlite|Tất cả tệp (*.*)|*.*";
            ofd.CheckFileExists = true;

            if (ofd.ShowDialog() != DialogResult.OK) return;

            string selectedFile = ofd.FileName;

            // Đọc thông tin backup
            var info = BackupService.ReadBackupFileInfo(selectedFile);

            if (!info.IsValid)
            {
                MessageBox.Show(
                    (IWin32Window)(this.FindForm() ?? (IWin32Window)this),
                    $"File backup không hợp lệ:\n{info.ErrorMessage}\n\nVui lòng chọn file database (.db) hợp lệ của Movie Vault.",
                    "File không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Hiển thị thông tin backup trước khi confirm
            string fileTypeNote = info.IsZipFormat
                ? "📦 Định dạng: ZIP (Bao gồm Database + Hình ảnh + Cấu hình)"
                : "⚠️ Định dạng: Database (.db) - Chỉ khôi phục dữ liệu, không bao gồm hình ảnh";

            string confirmMsg =
                $"⚠️  XÁC NHẬN KHÔI PHỤC TOÀN BỘ HỆ THỐNG  ⚠️\n\n" +
                $"Hệ thống sẽ được khôi phục về trạng thái tại thời điểm của bản sao lưu đã chọn!\n" +
                $"Toàn bộ dữ liệu hiện tại (nếu có) sẽ được thay thế hoàn toàn.\n\n" +
                $"📄 File sao lưu:   {Path.GetFileName(info.OriginalFilePath)}\n" +
                $"📁 Đường dẫn:     {Path.GetDirectoryName(info.OriginalFilePath)}\n" +
                $"🕐 Thời gian tạo: {info.BackupTime}\n" +
                $"{fileTypeNote}\n\n" +
                $"Sau khi khôi phục, ứng dụng sẽ tự động khởi động lại.\n\n" +
                $"Bạn có chắc chắn muốn tiếp tục khôi phục?";

            var result = MessageBox.Show(
                (IWin32Window)(this.FindForm() ?? (IWin32Window)this),
                confirmMsg,
                "Xác nhận khôi phục",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes) return;

            btnRestore.Enabled = false;
            UpdateStatusLabel("Trạng thái: Đang khôi phục dữ liệu...",
                System.Drawing.Color.FromArgb(251, 191, 36));

            var (success, message) = await AppServices.BackupSvc!.RestoreFromFileAsync(selectedFile);

            if (success)
            {
                MessageBox.Show(
                    (IWin32Window)(this.FindForm() ?? (IWin32Window)this),
                    "✅ Khôi phục thành công!\nToàn bộ dữ liệu hệ thống đã được phục hồi hoàn chỉnh (bao gồm hình ảnh).\nỨng dụng sẽ khởi động lại ngay bây giờ.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                Application.Restart();
                Environment.Exit(0);
            }
            else
            {
                MessageBox.Show(
                    (IWin32Window)(this.FindForm() ?? (IWin32Window)this),
                    $"❌ Khôi phục thất bại!\n\n{message}",
                    "Lỗi khôi phục",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                btnRestore.Enabled = true;
                UpdateStatusLabel($"Trạng thái: Lỗi khôi phục - {message}",
                    System.Drawing.Color.FromArgb(252, 100, 100));
            }
        }
    }
}
