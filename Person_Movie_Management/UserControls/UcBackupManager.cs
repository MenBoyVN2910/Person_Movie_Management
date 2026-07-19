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
        }

        private void UcBackupManager_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                RefreshList();
                flpPaths.Resize += FlpPaths_Resize;
            }
        }

        private void FlpPaths_Resize(object? sender, EventArgs e)
        {
            foreach (Control c in flpPaths.Controls)
            {
                c.Width = flpPaths.Width - 40;
                // Move delete button to right edge
                foreach (Control child in c.Controls)
                {
                    if (child is Guna.UI2.WinForms.Guna2Button btn && btn.Text == "❌")
                    {
                        btn.Location = new System.Drawing.Point(c.Width - 50, 15);
                    }
                }
            }
        }

        private void RefreshList()
        {
            flpPaths.Controls.Clear();
            if (AppServices.BackupSvc != null)
            {
                var paths = AppServices.BackupSvc.GetBackupPaths();
                foreach (var path in paths)
                {
                    flpPaths.Controls.Add(CreatePathCard(path));
                }
            }
        }

        private Control CreatePathCard(string path)
        {
            var pnl = new Guna.UI2.WinForms.Guna2Panel();
            pnl.BorderRadius = 8;
            pnl.FillColor = System.Drawing.Color.FromArgb(30, 41, 59);
            pnl.Size = new System.Drawing.Size(flpPaths.Width - 40, 70);
            pnl.Margin = new Padding(0, 0, 0, 10);
            
            var lblIcon = new Label();
            lblIcon.Text = "📁";
            lblIcon.Font = new System.Drawing.Font("Segoe UI", 20F);
            lblIcon.AutoSize = true;
            lblIcon.Location = new System.Drawing.Point(10, 15);
            lblIcon.BackColor = System.Drawing.Color.Transparent;
            
            var lblPath = new Label();
            lblPath.Text = path;
            lblPath.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            lblPath.ForeColor = System.Drawing.Color.White;
            lblPath.AutoSize = true;
            lblPath.Location = new System.Drawing.Point(50, 10);
            lblPath.BackColor = System.Drawing.Color.Transparent;
            
            bool exists = Directory.Exists(path);
            var lblStatusInfo = new Label();
            lblStatusInfo.Text = exists ? "✅ Đang hoạt động" : "⚠️ Thư mục không tồn tại";
            lblStatusInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblStatusInfo.ForeColor = exists ? System.Drawing.Color.LightGreen : System.Drawing.Color.Orange;
            lblStatusInfo.AutoSize = true;
            lblStatusInfo.Location = new System.Drawing.Point(50, 35);
            lblStatusInfo.BackColor = System.Drawing.Color.Transparent;

            // Try to read meta json
            if (exists)
            {
                string metaFile = Path.Combine(path, "AppDatabase_Backup.meta.json");
                if (File.Exists(metaFile))
                {
                    try
                    {
                        string metaContent = File.ReadAllText(metaFile);
                        var meta = System.Text.Json.JsonDocument.Parse(metaContent);
                        if (meta.RootElement.TryGetProperty("BackupTime", out var backupTime))
                        {
                            lblStatusInfo.Text += $" | Lần cuối: {backupTime.GetString()}";
                        }
                    }
                    catch { }
                }
            }
            
            var btnDel = new Guna.UI2.WinForms.Guna2Button();
            btnDel.Text = "❌";
            btnDel.FillColor = System.Drawing.Color.Transparent;
            btnDel.ForeColor = System.Drawing.Color.LightCoral;
            btnDel.Size = new System.Drawing.Size(40, 40);
            btnDel.Location = new System.Drawing.Point(pnl.Width - 50, 15);
            btnDel.Cursor = Cursors.Hand;
            btnDel.Click += (s, e) => {
                var res = MessageBox.Show($"Bạn có chắc muốn xóa thư mục backup này?\n{path}", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                {
                    AppServices.BackupSvc?.RemoveBackupPath(path);
                    RefreshList();
                    lblStatus.Text = "Trạng thái: Đã xóa đường dẫn";
                }
            };
            
            pnl.Controls.Add(lblIcon);
            pnl.Controls.Add(lblPath);
            pnl.Controls.Add(lblStatusInfo);
            pnl.Controls.Add(btnDel);
            
            return pnl;
        }

        private void btnAddPath_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                AppServices.BackupSvc?.AddBackupPath(fbd.SelectedPath);
                RefreshList();
                lblStatus.Text = "Trạng thái: Đã thêm đường dẫn";
            }
        }

        private void btnRemovePath_Click(object sender, EventArgs e)
        {
            // Now handled by individual delete buttons on each path card
        }

        private async void btnBackupNow_Click(object sender, EventArgs e)
        {
            if (AppServices.BackupSvc != null)
            {
                lblStatus.Text = "Trạng thái: Đang sao lưu...";
                btnBackupNow.Enabled = false;
                
                await AppServices.BackupSvc.PerformBackupAsync();
                
                btnBackupNow.Enabled = true;
                lblStatus.Text = $"Trạng thái: Sao lưu xong lúc {DateTime.Now:HH:mm:ss}";
            }
        }

        private async void btnRestore_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "SQLite Database (*.db)|*.db";
            ofd.Title = "Chọn file backup để khôi phục";
            
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var result = MessageBox.Show(
                    "CẢNH BÁO: Dữ liệu hiện tại sẽ bị ghi đè hoàn toàn bằng dữ liệu từ file backup!\n\nSau khi khôi phục, ứng dụng sẽ khởi động lại. Bạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận khôi phục", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    lblStatus.Text = "Trạng thái: Đang khôi phục...";
                    btnRestore.Enabled = false;

                    bool success = await AppServices.BackupSvc!.RestoreAsync(ofd.FileName);

                    if (success)
                    {
                        MessageBox.Show("Khôi phục thành công! Ứng dụng sẽ khởi động lại.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Restart();
                    }
                    else
                    {
                        MessageBox.Show("Khôi phục thất bại. Vui lòng kiểm tra lại file backup.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnRestore.Enabled = true;
                        lblStatus.Text = "Trạng thái: Lỗi khôi phục";
                    }
                }
            }
        }
    }
}
