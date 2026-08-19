using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Services;

namespace Person_Movie_Management.Forms
{
    public class FrmToastNotification : Form
    {
        private System.Windows.Forms.Timer _autoCloseTimer = null!;
        private System.Windows.Forms.Timer _fadeTimer = null!;
        private string? _mediaPath;
        private bool _isFadingOut = false;
        private PictureBox _picThumb = null!;
        private Label _lblTitle = null!;
        private Label _lblMovieName = null!;
        private Label _lblStatus = null!;
        private Label _btnClose = null!;

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
                return cp;
            }
        }

        public FrmToastNotification(string title, string movieName, string? mediaPath = null, Image? thumbnail = null)
        {
            _mediaPath = mediaPath;
            InitializeUI(title, movieName, thumbnail);
        }

        public static void ShowNotification(string title, string movieName, string? mediaPath = null, Image? thumbnail = null)
        {
            try
            {
                // Run on UI thread if possible
                var toast = new FrmToastNotification(title, movieName, mediaPath, thumbnail);
                toast.Show();
            }
            catch { }
        }

        private void InitializeUI(string title, string movieName, Image? thumbnail)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Size = new Size(390, 95);
            this.BackColor = Color.FromArgb(20, 26, 50);
            this.DoubleBuffered = true;
            this.Opacity = 0.0;
            this.Cursor = Cursors.Hand;

            // Thumbnail / Icon
            _picThumb = new PictureBox
            {
                Size = new Size(65, 65),
                Location = new Point(14, 15),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(30, 38, 72),
                Cursor = Cursors.Hand
            };

            if (thumbnail != null)
            {
                _picThumb.Image = new Bitmap(thumbnail);
            }
            else
            {
                // Draw a fallback video icon
                var bmp = new Bitmap(65, 65);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using var brush = new LinearGradientBrush(new Rectangle(0, 0, 65, 65), Color.FromArgb(139, 92, 246), Color.FromArgb(99, 102, 241), 135f);
                    g.FillRectangle(brush, 0, 0, 65, 65);
                    using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("🎬", new Font("Segoe UI Emoji", 22f), Brushes.White, new RectangleF(0, 0, 65, 65), sf);
                }
                _picThumb.Image = bmp;
            }

            // Title
            _lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(167, 139, 250), // Purple-400
                Location = new Point(88, 14),
                AutoSize = false,
                Size = new Size(260, 22),
                AutoEllipsis = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            // Movie Name
            _lblMovieName = new Label
            {
                Text = movieName,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(241, 245, 249),
                Location = new Point(88, 38),
                AutoSize = false,
                Size = new Size(260, 22),
                AutoEllipsis = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            // Subtitle
            _lblStatus = new Label
            {
                Text = "✦ Đã tự động thêm vào Phim Trên Máy",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(74, 222, 128), // Emerald-400
                Location = new Point(88, 62),
                AutoSize = false,
                Size = new Size(260, 18),
                AutoEllipsis = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            // Close button
            _btnClose = new Label
            {
                Text = "✕",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(356, 10),
                Size = new Size(24, 24),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };

            _btnClose.MouseEnter += (s, e) => { _btnClose.ForeColor = Color.FromArgb(239, 68, 68); };
            _btnClose.MouseLeave += (s, e) => { _btnClose.ForeColor = Color.FromArgb(148, 163, 184); };
            _btnClose.Click += (s, e) => { StartFadeOut(); };

            // Click anywhere to play movie
            this.Click += OnToastClicked;
            _picThumb.Click += OnToastClicked;
            _lblTitle.Click += OnToastClicked;
            _lblMovieName.Click += OnToastClicked;
            _lblStatus.Click += OnToastClicked;

            this.Controls.Add(_picThumb);
            this.Controls.Add(_lblTitle);
            this.Controls.Add(_lblMovieName);
            this.Controls.Add(_lblStatus);
            this.Controls.Add(_btnClose);

            // Position at bottom-right of primary screen above taskbar
            Rectangle workingArea = Screen.PrimaryScreen?.WorkingArea ?? Screen.GetWorkingArea(this);
            int margin = 20;
            this.Location = new Point(workingArea.Right - this.Width - margin, workingArea.Bottom - this.Height - margin);

            // Auto close timer (5 seconds)
            _autoCloseTimer = new System.Windows.Forms.Timer();
            _autoCloseTimer.Interval = 5000;
            _autoCloseTimer.Tick += (s, e) =>
            {
                _autoCloseTimer.Stop();
                StartFadeOut();
            };

            // Pause auto-close on mouse hover
            this.MouseEnter += (s, e) => _autoCloseTimer.Stop();
            this.MouseLeave += (s, e) => { if (!_isFadingOut) _autoCloseTimer.Start(); };

            // Fade in animation
            _fadeTimer = new System.Windows.Forms.Timer();
            _fadeTimer.Interval = 20;
            _fadeTimer.Tick += (s, e) =>
            {
                if (!_isFadingOut)
                {
                    if (this.Opacity < 0.96)
                    {
                        this.Opacity += 0.08;
                    }
                    else
                    {
                        this.Opacity = 0.96;
                        _fadeTimer.Stop();
                        _autoCloseTimer.Start();
                    }
                }
                else
                {
                    if (this.Opacity > 0.08)
                    {
                        this.Opacity -= 0.08;
                    }
                    else
                    {
                        _fadeTimer.Stop();
                        this.Close();
                    }
                }
            };

            this.Load += (s, e) => { _fadeTimer.Start(); };
        }

        private void OnToastClicked(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_mediaPath) && File.Exists(_mediaPath))
            {
                try
                {
                    MediaLauncher.LaunchMedia(_mediaPath, 1);
                }
                catch { }
            }
            StartFadeOut();
        }

        private void StartFadeOut()
        {
            if (_isFadingOut) return;
            _isFadingOut = true;
            _autoCloseTimer.Stop();
            _fadeTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw rounded border with glowing accent gradient
            var rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            using var path = GetRoundedRectanglePath(rect, 14);
            using var pen = new Pen(Color.FromArgb(139, 92, 246), 1.5f);
            e.Graphics.DrawPath(pen, path);
        }

        private static GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _autoCloseTimer?.Dispose();
                _fadeTimer?.Dispose();
                _picThumb?.Image?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
