using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.UserControls
{
    public partial class UcSidebar : UserControl
    {
        public event EventHandler<string>? MenuItemClicked;

        public UcSidebar()
        {
            InitializeComponent();
            this.BackColor = UIHelper.BgPanel;
            this.DoubleBuffered = true;

            // Style all menu buttons
            UIHelper.StyleMenuButton(btnHome);
            UIHelper.StyleMenuButton(btnOnlineMovies);
            UIHelper.StyleMenuButton(btnLocalMovies);
            UIHelper.StyleMenuButton(btnAudio);
            UIHelper.StyleMenuButton(btnFavorites);
            UIHelper.StyleMenuButton(btnPlaylist);
            UIHelper.StyleMenuButton(btnRecycleBin);
            UIHelper.StyleMenuButton(btnProfile);
            UIHelper.StyleMenuButton(btnBackup);
            UIHelper.StyleMenuButton(btnActor);

            // Logout gets special danger styling
            btnLogout.FillColor = Color.Transparent;
            btnLogout.ForeColor = UIHelper.TextMuted;
            btnLogout.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
            btnLogout.HoverState.FillColor = Color.FromArgb(40, 248, 113, 113);
            btnLogout.HoverState.ForeColor = UIHelper.Danger;

            // Style avatar
            picAvatar.ShadowDecoration.Enabled = true;
            picAvatar.ShadowDecoration.Color = UIHelper.AccentPrimary;
            picAvatar.ShadowDecoration.Depth = 8;
            
            LoadUserInfo();

            // Separator line hidden, we will draw a gradient one in OnPaint
            pnlSeparator.Visible = false;

            // Labels
            lblDisplayName.ForeColor = UIHelper.TextPrimary;
            lblDisplayName.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblUsername.ForeColor = UIHelper.TextMuted;
            lblUsername.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // ── Draw Avatar Glow Ring ──
            var rect = picAvatar.Bounds;
            rect.Inflate(4, 4); // 4px larger than the avatar
            using var ringPen = new Pen(new LinearGradientBrush(rect, UIHelper.AccentPrimary, UIHelper.AccentTertiary, 45f), 2f);
            g.DrawEllipse(ringPen, rect);

            // ── Draw Gradient Separator ──
            int sepY = pnlSeparator.Location.Y;
            var sepRect = new Rectangle(20, sepY, this.Width - 40, 1);
            using var sepBrush = new LinearGradientBrush(sepRect, Color.Transparent, UIHelper.Border, 0f);
            // Center is opaque, edges are transparent
            var cb = new ColorBlend
            {
                Positions = new[] { 0f, 0.5f, 1f },
                Colors = new[] { Color.Transparent, UIHelper.Border, Color.Transparent }
            };
            sepBrush.InterpolationColors = cb;
            g.FillRectangle(sepBrush, sepRect);

            // ── Draw Right Border ──
            using var rightPen = new Pen(Color.FromArgb(15, 255, 255, 255), 1);
            g.DrawLine(rightPen, this.Width - 1, 0, this.Width - 1, this.Height);
        }

        public void LoadUserInfo()
        {
            if (SessionManager.IsLoggedIn)
            {
                lblDisplayName.Text = SessionManager.CurrentUser?.DisplayName;
                lblUsername.Text = "@" + SessionManager.CurrentUser?.Username;

                if (!string.IsNullOrEmpty(SessionManager.CurrentUser?.AvatarPath))
                {
                    string fullPath = FileHelper.GetFullPath(SessionManager.CurrentUser.AvatarPath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        try 
                        {
                            var img = FileHelper.LoadImageSafe(fullPath);
                            picAvatar.Image = new Bitmap(img);
                            img.Dispose();
                        } 
                        catch { }
                    }
                }
            }
        }

        private void ResetButtons()
        {
            btnHome.Checked = false;
            btnOnlineMovies.Checked = false;
            btnLocalMovies.Checked = false;
            btnAudio.Checked = false;
            btnFavorites.Checked = false;
            btnPlaylist.Checked = false;
            btnRecycleBin.Checked = false;
            btnProfile.Checked = false;
            btnBackup.Checked = false;
            btnActor.Checked = false;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            var btn = (Guna.UI2.WinForms.Guna2Button)sender;
            
            if (btn.Name == "btnLogout")
            {
                MenuItemClicked?.Invoke(this, "Logout");
                return;
            }

            ResetButtons();
            btn.Checked = true;

            string pageName = btn.Name.Replace("btn", "");
            MenuItemClicked?.Invoke(this, pageName);
        }
    }
}
