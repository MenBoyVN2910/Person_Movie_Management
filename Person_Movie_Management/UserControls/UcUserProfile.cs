using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Person_Movie_Management.Services;

namespace Person_Movie_Management.UserControls
{
    public partial class UcUserProfile : UserControl
    {
        private readonly AuthService _authService;
        private readonly UserRepository _userRepo;
        private string? _selectedAvatarPath = null;
        private Label lblToggleWatcher = null!;
        private CheckBox tglWatcher = null!;

        public UcUserProfile()
        {
            InitializeComponent();
            _authService = new AuthService();
            _userRepo = new UserRepository();
            
            this.BackColor = UIHelper.BgDark;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.AutoScroll = true;
            
            // Style the panel
            pnlInfo.FillColor = UIHelper.BgCard;
            pnlInfo.FillColor2 = UIHelper.BgPanel;
            pnlInfo.BorderRadius = 16;

            // Avatar styling
            btnChooseAvatar.FillColor = UIHelper.AccentPrimary;

            // Style text boxes
            StyleTextBox(txtDisplayName, "👤  Tên hiển thị");
            StyleTextBox(txtEmail, "📧  Email");

            StyleTextBox(txtOldPassword, "🔒  Mật khẩu cũ");
            StyleTextBox(txtNewPassword, "🔑  Mật khẩu mới");
            StyleTextBox(txtConfirmPassword, "🔑  Xác nhận mật khẩu mới");

            // Style buttons
            btnSaveInfo.FillColor = UIHelper.GradEmerald1;
            btnSaveInfo.FillColor2 = UIHelper.GradEmerald2;
            btnSaveInfo.BorderRadius = 12;
            btnSaveInfo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnSaveInfo.Animated = true;

            btnSavePassword.FillColor = UIHelper.GradViolet1;
            btnSavePassword.FillColor2 = UIHelper.GradRose1;
            btnSavePassword.BorderRadius = 12;
            btnSavePassword.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnSavePassword.Animated = true;

            // Initialize toggle
            tglWidget.Checked = SessionManager.IsDropWidgetEnabled;

            // Initialize Folder Watcher toggle
            lblToggleWatcher = new Label
            {
                Text = "Tự động quét Videos",
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = Color.FromArgb(241, 245, 249),
                Location = new Point(350, 58),
                Size = new Size(200, 21),
                BackColor = Color.Transparent,
                AutoSize = true
            };
            tglWatcher = new CheckBox
            {
                Location = new Point(520, 61),
                Size = new Size(15, 14),
                Checked = SessionManager.IsFolderWatcherEnabled,
                AutoSize = true,
                UseVisualStyleBackColor = true
            };
            tglWatcher.CheckedChanged += TglWatcher_CheckedChanged;
            pnlInfo.Controls.Add(lblToggleWatcher);
            pnlInfo.Controls.Add(tglWatcher);

            LoadData();
        }

        private void StyleTextBox(Guna.UI2.WinForms.Guna2TextBox txt, string placeholder)
        {
            txt.FillColor = UIHelper.BgSurface;
            txt.ForeColor = UIHelper.TextPrimary;
            txt.BorderColor = UIHelper.Border;
            txt.BorderRadius = 10;
            txt.FocusedState.BorderColor = UIHelper.AccentPrimary;
            txt.HoverState.BorderColor = UIHelper.AccentSecondary;
            txt.Font = new Font("Segoe UI", 11F);
            txt.PlaceholderText = placeholder;
            txt.PlaceholderForeColor = UIHelper.TextMuted;
        }

        private void LoadData()
        {
            if (SessionManager.IsLoggedIn)
            {
                var user = SessionManager.CurrentUser!;
                lblUsername.Text = $"Tên đăng nhập: {user.Username}";
                lblCreatedAt.Text = $"Ngày tham gia: {user.CreatedAt:dd/MM/yyyy}";

                txtDisplayName.Text = user.DisplayName;
                txtEmail.Text = user.Email ?? "";

                // Load Avatar
                if (!string.IsNullOrEmpty(user.AvatarPath))
                {
                    string fullPath = FileHelper.GetFullPath(user.AvatarPath);
                    if (File.Exists(fullPath))
                    {
                        try 
                        {
                            var img = FileHelper.LoadImageSafe(fullPath);
                            if (img != null)
                            {
                                picAvatar.Image?.Dispose();
                                picAvatar.Image = new Bitmap(img); // Clone to prevent file lock
                                img.Dispose();
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        private void btnChooseAvatar_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            ofd.Title = "Chọn Ảnh Đại Diện";
            
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var img = FileHelper.LoadImageSafe(ofd.FileName);
                    if (img != null)
                    {
                        picAvatar.Image?.Dispose();
                        picAvatar.Image = new Bitmap(img);
                        img.Dispose();
                        _selectedAvatarPath = ofd.FileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi không thể đọc file ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSaveInfo_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsLoggedIn) return;
            if (string.IsNullOrWhiteSpace(txtDisplayName.Text))
            {
                MessageBox.Show("Tên hiển thị không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = SessionManager.CurrentUser!;
            user.DisplayName = txtDisplayName.Text.Trim();
            user.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();

            if (!string.IsNullOrEmpty(_selectedAvatarPath))
            {
                // Create Avatars folder if it doesn't exist
                string avatarDir = FileHelper.GetFullPath("Data/Avatars");
                if (!Directory.Exists(avatarDir)) Directory.CreateDirectory(avatarDir);

                string ext = Path.GetExtension(_selectedAvatarPath);
                string newFileName = $"avatar_{user.Id}_{DateTime.Now.Ticks}{ext}";
                string destPath = Path.Combine(avatarDir, newFileName);

                File.Copy(_selectedAvatarPath, destPath, true);
                user.AvatarPath = $"Data/Avatars/{newFileName}";
            }

            if (_userRepo.Update(user))
            {
                // Reload current user from DB to sync session
                var updatedUser = _userRepo.GetById(user.Id);
                if (updatedUser != null)
                {
                    SessionManager.UpdateCurrentUser(updatedUser);
                }
                
                MessageBox.Show("Cập nhật thông tin thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Fire an event up to the main form to refresh the sidebar
                var parentForm = this.FindForm() as Forms.FrmMain;
                parentForm?.RefreshSidebarUserInfo();
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSavePassword_Click(object sender, EventArgs e)
        {
            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (success, message) = _authService.ChangePassword(SessionManager.CurrentUser!.Id, txtOldPassword.Text, txtNewPassword.Text);
            if (success)
            {
                MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtOldPassword.Clear();
                txtNewPassword.Clear();
                txtConfirmPassword.Clear();
            }
            else
            {
                MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tglWidget_CheckedChanged(object sender, EventArgs e)
        {
            SessionManager.IsDropWidgetEnabled = tglWidget.Checked;
            var frmMain = this.FindForm() as Forms.FrmMain;
            if (frmMain != null)
            {
                frmMain.ToggleDropWidget(tglWidget.Checked);
            }
        }

        private void TglWatcher_CheckedChanged(object? sender, EventArgs e)
        {
            SessionManager.IsFolderWatcherEnabled = tglWatcher.Checked;
            var frmMain = this.FindForm() as Forms.FrmMain;
            if (frmMain != null)
            {
                frmMain.ToggleFolderWatcher(tglWatcher.Checked);
            }
        }
    }
}
