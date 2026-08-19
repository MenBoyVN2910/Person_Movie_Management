using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Person_Movie_Management.Services;

namespace Person_Movie_Management.UserControls
{
    public partial class UcAudioCard : UserControl
    {
        private Audio _audio = null!;
        private bool _isHovered = false;
        private int _hoverRating = 0;
        private Rectangle _starsRect;
        private Rectangle _infoRect;
        private Image? _coverImage;
        private int _currentLoadId = 0;
        private int _boundAudioId = -1;
        
        public event EventHandler<Audio>? AudioClicked;
        public event EventHandler<Audio>? FavoriteToggled;
        public event EventHandler<Audio>? EditClicked;
        public event EventHandler<Audio>? DeleteClicked;

        public UcAudioCard(Audio audio)
        {
            InitializeComponent();
            
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.Cursor = Cursors.Hand;
            this.BackColor = UIHelper.BgCard;
            
            menuEdit.Image = UIHelper.CreateIcon("\uE70F", 12f);
            menuDownload.Image = UIHelper.CreateIcon("\uE896", 12f); // Download icon
            menuDelete.Image = UIHelper.CreateIcon("\uE74D", 12f);
            menuAddToPlaylist.Image = UIHelper.CreateIcon("\uE710", 12f);
            
            BindData(audio);
        }

        public void BindData(Audio audio)
        {
            if (_boundAudioId == audio.Id && _audio != null && _audio.AudioCode == audio.AudioCode && _audio.Rating == audio.Rating && _audio.IsFavorite == audio.IsFavorite && _audio.WatchProgress == audio.WatchProgress && _audio.CoverImage == audio.CoverImage)
            {
                _audio = audio;
                return;
            }

            _audio = audio;
            _boundAudioId = audio.Id;

            int targetW = this.Width > 0 ? this.Width - 12 : 348;
            int targetH = 190;

            if (!string.IsNullOrEmpty(_audio.CoverImage))
            {
                string fullPath = FileHelper.GetFullPath(_audio.CoverImage);
                if (ImageCache.TryGetThumbnailFromMemory(fullPath, targetW, targetH, out var memImg) && memImg != null)
                {
                    _coverImage = memImg;
                }
                else
                {
                    _coverImage = null;
                    LoadCoverImage(fullPath, targetW, targetH);
                }
            }
            else
            {
                _coverImage = null;
            }

            this.Invalidate();
        }

        private async void LoadCoverImage(string? fullPath = null, int targetW = 0, int targetH = 0)
        {
            int loadId = ++_currentLoadId;
            if (string.IsNullOrEmpty(fullPath) && !string.IsNullOrEmpty(_audio?.CoverImage))
            {
                fullPath = FileHelper.GetFullPath(_audio.CoverImage);
            }
            if (targetW <= 0) targetW = this.Width > 0 ? this.Width - 12 : 348;
            if (targetH <= 0) targetH = 190;

            if (!string.IsNullOrEmpty(fullPath) && System.IO.File.Exists(fullPath))
            {
                try
                {
                    var img = await ImageCache.GetThumbnailAsync(fullPath, targetW, targetH);
                    if (img != null && !this.IsDisposed && _currentLoadId == loadId)
                    {
                        _coverImage = img;
                        this.Invalidate();
                    }
                }
                catch { }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // ── Hover lift effect ──
            if (_isHovered)
            {
                g.TranslateTransform(0, -2);
            }

            var fullRect = new Rectangle(0, _isHovered ? 2 : 0, this.Width, this.Height - (_isHovered ? 2 : 0));

            // ── Drop Shadow (3-layer) ──
            for (int i = 3; i >= 1; i--)
            {
                var shadowRect = new Rectangle(fullRect.X + i, fullRect.Y + i + 2, fullRect.Width - i * 2, fullRect.Height - i * 2);
                using var shadowPath = CreateRoundedRectPath(shadowRect, 14);
                int alpha = _isHovered ? 30 + i * 10 : 15 + i * 5;
                using var shadowBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
                g.FillPath(shadowBrush, shadowPath);
            }

            // ── Background with rounded corners ──
            using var bgPath = CreateRoundedRectPath(fullRect, 14);
            using var bgBrush = new SolidBrush(_isHovered ? UIHelper.BgCardHover : UIHelper.BgCard);
            g.FillPath(bgBrush, bgPath);

            // ── Glass border (subtle white edge) ──
            using var glassPen = new Pen(Color.FromArgb(_isHovered ? 40 : 15, 255, 255, 255), 1f);
            g.DrawPath(glassPen, bgPath);

            // ── Border glow on hover ──
            if (_isHovered)
            {
                using var borderPen = new Pen(Color.FromArgb(100, UIHelper.AccentPrimary), 1.5f);
                g.DrawPath(borderPen, bgPath);
            }

            // ── Cover Image Area ──
            var imgRect = new Rectangle(6, 6, this.Width - 12, 190);
            using var imgClip = CreateRoundedRectPath(imgRect, 10);

            if (_coverImage != null)
            {
                g.SetClip(imgClip);
                float imgAspect = (float)_coverImage.Width / _coverImage.Height;
                float rectAspect = (float)imgRect.Width / imgRect.Height;
                float drawWidth, drawHeight;
                if (imgAspect > rectAspect)
                {
                    drawHeight = imgRect.Height;
                    drawWidth = drawHeight * imgAspect;
                }
                else
                {
                    drawWidth = imgRect.Width;
                    drawHeight = drawWidth / imgAspect;
                }
                float drawX = imgRect.X + (imgRect.Width - drawWidth) / 2;
                float drawY = imgRect.Y + (imgRect.Height - drawHeight) / 2;

                g.DrawImage(_coverImage, drawX, drawY, drawWidth, drawHeight);
                g.ResetClip();
            }
            else
            {
                g.SetClip(imgClip);
                using var placeholderBrush = new LinearGradientBrush(imgRect, 
                    Color.FromArgb(51, 65, 85), Color.FromArgb(71, 85, 105), 135f);
                g.FillRectangle(placeholderBrush, imgRect);
                
                using var iconFont = new Font("Segoe UI Emoji", 36F);
                using var iconBrush = new SolidBrush(Color.FromArgb(80, 255, 255, 255));
                var iconSize = g.MeasureString("🎵", iconFont);
                g.DrawString("🎵", iconFont, iconBrush, 
                    imgRect.X + (imgRect.Width - iconSize.Width) / 2, 
                    imgRect.Y + (imgRect.Height - iconSize.Height) / 2);
                g.ResetClip();
            }

            // ── Gradient overlay at bottom of image ──
            var gradientOverlay = new Rectangle(imgRect.X, imgRect.Bottom - 60, imgRect.Width, 60);
            using var overlayBrush = new LinearGradientBrush(gradientOverlay,
                Color.FromArgb(0, 0, 0, 0), Color.FromArgb(180, 0, 0, 0), 90f);
            g.SetClip(imgClip);
            g.FillRectangle(overlayBrush, gradientOverlay);

            // ── Listen / Watch Progress Bar ──
            if (_audio.WatchProgress > 0)
            {
                int pgbHeight = 4;
                var pgbRect = new Rectangle(imgRect.X, imgRect.Bottom - pgbHeight, imgRect.Width, pgbHeight);
                using var pgbBgBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0));
                g.FillRectangle(pgbBgBrush, pgbRect);
                
                int pgbWidth = (int)((Math.Min(100, _audio.WatchProgress) / 100f) * imgRect.Width);
                var pgbFillRect = new Rectangle(imgRect.X, imgRect.Bottom - pgbHeight, pgbWidth, pgbHeight);
                using var pgbFillBrush = new SolidBrush(UIHelper.AccentPrimary);
                g.FillRectangle(pgbFillBrush, pgbFillRect);
            }

            g.ResetClip();

            // ── Info Button ──
            _infoRect = new Rectangle(this.Width - 34, 12, 24, 24);
            using var infoBg = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
            g.FillEllipse(infoBg, _infoRect);
            using var infoIconFont = new Font("Segoe UI", 11F, FontStyle.Bold);
            g.DrawString("i", infoIconFont, Brushes.White, _infoRect.X + 7, _infoRect.Y + 1);

            // ── Audio Code ──
            int textY = imgRect.Bottom + 16;
            using var nameFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            using var nameBrush = new SolidBrush(UIHelper.TextPrimary);
            var nameRect = new RectangleF(10, textY, this.Width - 20, 28);
            using var sf = new StringFormat { Trimming = StringTrimming.EllipsisCharacter };
            g.DrawString(_audio.AudioCode, nameFont, nameBrush, nameRect, sf);

            // ── Rating Stars ──
            int ratingY = textY + 44;
            using var starFont = new Font("Segoe UI", 14F);
            
            int displayRating = _hoverRating > 0 ? _hoverRating : _audio.Rating;
            
            float currentX = 10;
            for (int i = 1; i <= 5; i++)
            {
                string starChar = i <= displayRating ? "★" : "☆";
                using var starBrush = new SolidBrush(i <= displayRating ? UIHelper.AccentGold : UIHelper.TextMuted);
                g.DrawString(starChar, starFont, starBrush, currentX, ratingY);
                currentX += g.MeasureString("★", starFont).Width - 8; 
            }
            
            _starsRect = new Rectangle(10, ratingY, (int)(currentX - 10), 30);

            // ── Favorite Heart ──
            string heart = _audio.IsFavorite ? "❤️" : "🤍";
            using var heartFont = new Font("Segoe UI Emoji", 14F);
            g.DrawString(heart, heartFont, Brushes.White, this.Width - 38, ratingY - 2);

            if (_isHovered)
            {
                using var hoverOverlay = new SolidBrush(Color.FromArgb(12, 99, 102, 241));
                g.FillPath(hoverOverlay, bgPath);
                g.ResetTransform();
            }
        }

        private GraphicsPath CreateRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            this.Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (_starsRect.Contains(e.Location))
            {
                int starWidth = _starsRect.Width / 5;
                int newHover = ((e.X - _starsRect.X) / starWidth) + 1;
                newHover = Math.Max(1, Math.Min(5, newHover));
                
                if (_hoverRating != newHover)
                {
                    _hoverRating = newHover;
                    this.Invalidate();
                }
            }
            else if (_hoverRating != 0)
            {
                _hoverRating = 0;
                this.Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            _hoverRating = 0;
            this.Invalidate();
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            AudioClicked?.Invoke(this, _audio);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            if (_infoRect.Contains(e.Location))
            {
                string details = $"Tên bài: {_audio.AudioCode}\r\n" +
                                 $"Ngày thêm: {_audio.CreatedAt:dd/MM/yyyy HH:mm}\r\n" +
                                 $"Đánh giá: {_audio.Rating} sao\r\n\r\n" +
                                 $"Mô tả & Ghi chú:\r\n{_audio.Note ?? "Chưa có mô tả."}";
                
                new Forms.FrmDetailDialog($"Thông tin: {_audio.AudioCode}", details).ShowDialog();
                return;
            }

            if (_starsRect.Contains(e.Location))
            {
                int starWidth = _starsRect.Width / 5;
                int clickedRating = ((e.X - _starsRect.X) / starWidth) + 1;
                clickedRating = Math.Max(1, Math.Min(5, clickedRating));
                
                _audio.Rating = clickedRating;
                AppServices.AudioRepo.Update(_audio);
                DataCache.Invalidate();
                this.Invalidate();
                return;
            }

            if (e.X > this.Width - 45 && e.Y > 200)
            {
                if (_audio == null) return;
                _audio.IsFavorite = !_audio.IsFavorite;
                this.Invalidate();
                AppServices.AudioRepo.ToggleFavorite(_audio.Id);
                DataCache.Invalidate();
                FavoriteToggled?.Invoke(this, _audio);
            }
        }

        private void menuEdit_Click(object sender, EventArgs e)
        {
            EditClicked?.Invoke(this, _audio);
        }

        private void menuDownload_Click(object sender, EventArgs e)
        {
            var fullAudio = AppServices.AudioRepo.GetById(_audio.Id, includeAudioData: true);
            if (fullAudio?.AudioData == null || fullAudio.AudioData.Length == 0)
            {
                MessageBox.Show("Không tìm thấy dữ liệu file âm thanh để tải về.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Title = "Tải bài hát về máy";
            sfd.FileName = $"{FileHelper.SanitizeFileName(_audio.AudioCode)}.mp3";
            sfd.Filter = "File Âm Thanh (*.mp3)|*.mp3|Tất cả tệp (*.*)|*.*";
            sfd.DefaultExt = "mp3";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.IO.File.WriteAllBytes(sfd.FileName, fullAudio.AudioData);
                    MessageBox.Show("Đã tải bài hát về máy thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            DeleteClicked?.Invoke(this, _audio);
        }

        private void menuAddToPlaylist_Click(object sender, EventArgs e)
        {
            int userId = _audio.UserId;
            var frm = new Person_Movie_Management.Forms.FrmSelectPlaylist(userId, _audio.Id, Person_Movie_Management.Models.PlaylistItemType.Audio);
            frm.ShowDialog();
        }
    }
}
