using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;

namespace Person_Movie_Management.UserControls
{
    public partial class UcMovieCard : UserControl
    {
        private Movie _movie;
        private readonly MovieRepository _movieRepo;
        private bool _isHovered = false;
        private int _hoverRating = 0;
        private Rectangle _starsRect;
        private Rectangle _infoRect;
        private Image? _coverImage;
        
        public event EventHandler<Movie>? MovieClicked;
        public event EventHandler<Movie>? FavoriteToggled;
        public event EventHandler<Movie>? EditClicked;
        public event EventHandler<Movie>? DeleteClicked;

        public UcMovieCard(Movie movie)
        {
            InitializeComponent();
            _movie = movie;
            _movieRepo = new MovieRepository();
            
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.Cursor = Cursors.Hand;
            this.BackColor = UIHelper.BgCard;
            
            LoadCoverImage();
        }

        private void LoadCoverImage()
        {
            if (!string.IsNullOrEmpty(_movie.CoverImage))
            {
                string fullPath = FileHelper.GetFullPath(_movie.CoverImage);
                if (System.IO.File.Exists(fullPath))
                {
                    try { _coverImage = FileHelper.LoadImageSafe(fullPath); } catch { }
                }
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
            var imgRect = new Rectangle(6, 6, this.Width - 12, 160);
            using var imgClip = CreateRoundedRectPath(imgRect, 10);

            if (_coverImage != null)
            {
                g.SetClip(imgClip);
                // Draw image maintaining aspect ratio (UniformToFill style)
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
                // Premium placeholder — brighter gradient
                g.SetClip(imgClip);
                using var placeholderBrush = new LinearGradientBrush(imgRect, 
                    Color.FromArgb(51, 65, 85), Color.FromArgb(71, 85, 105), 135f);
                g.FillRectangle(placeholderBrush, imgRect);
                
                // Film icon
                using var iconFont = new Font("Segoe UI Emoji", 36F);
                using var iconBrush = new SolidBrush(Color.FromArgb(80, 255, 255, 255));
                var iconSize = g.MeasureString("🎬", iconFont);
                g.DrawString("🎬", iconFont, iconBrush, 
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
            g.ResetClip();

            // ── Badge (ONLINE / LOCAL) ──
            string badgeText = _movie.SourceType == 0 ? "ONLINE" : "LOCAL";
            Color badgeColor = _movie.SourceType == 0 ? UIHelper.AccentPrimary : UIHelper.Success;
            using var badgeFont = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            var badgeSize = g.MeasureString(badgeText, badgeFont);
            var badgeRect = new Rectangle(12, 14, (int)badgeSize.Width + 12, 20);
            using var badgePath = CreateRoundedRectPath(badgeRect, 6);
            using var badgeBrush = new SolidBrush(badgeColor);
            g.FillPath(badgeBrush, badgePath);
            g.DrawString(badgeText, badgeFont, Brushes.White, badgeRect.X + 6, badgeRect.Y + 2);

            // ── Info Button ──
            _infoRect = new Rectangle(this.Width - 34, 12, 24, 24);
            using var infoBg = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
            g.FillEllipse(infoBg, _infoRect);
            using var infoIconFont = new Font("Segoe UI", 11F, FontStyle.Bold);
            g.DrawString("i", infoIconFont, Brushes.White, _infoRect.X + 7, _infoRect.Y + 1);

            // ── Movie Code ──
            int textY = imgRect.Bottom + 12;
            using var nameFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            using var nameBrush = new SolidBrush(UIHelper.TextPrimary);
            var nameRect = new RectangleF(10, textY, this.Width - 20, 28);
            using var sf = new StringFormat { Trimming = StringTrimming.EllipsisCharacter };
            g.DrawString(_movie.MovieCode, nameFont, nameBrush, nameRect, sf);

            // ── Rating Stars ──
            int ratingY = textY + 28;
            using var starFont = new Font("Segoe UI", 14F);
            
            // Draw 5 stars, color depends on hover or actual rating
            int displayRating = _hoverRating > 0 ? _hoverRating : _movie.Rating;
            
            float currentX = 10;
            for (int i = 1; i <= 5; i++)
            {
                string starChar = i <= displayRating ? "★" : "☆";
                using var starBrush = new SolidBrush(i <= displayRating ? UIHelper.AccentGold : UIHelper.TextMuted);
                g.DrawString(starChar, starFont, starBrush, currentX, ratingY);
                currentX += g.MeasureString("★", starFont).Width - 8; // Adjust spacing
            }
            
            // Save stars rect for hit testing
            _starsRect = new Rectangle(10, ratingY, (int)(currentX - 10), 30);

            // ── Favorite Heart ──
            string heart = _movie.IsFavorite ? "❤️" : "🤍";
            using var heartFont = new Font("Segoe UI Emoji", 14F);
            g.DrawString(heart, heartFont, Brushes.White, this.Width - 38, ratingY - 2);

            // ── Hover overlay animation hint ──
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
            MovieClicked?.Invoke(this, _movie);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            // Info button click
            if (_infoRect.Contains(e.Location))
            {
                string type = _movie.SourceType == 0 ? "🌐 Online" : "📁 Trên Máy";
                string details = $"Mã phim: {_movie.MovieCode}\r\n" +
                                 $"Nguồn: {type}\r\n" +
                                 $"Ngày thêm: {_movie.CreatedAt:dd/MM/yyyy HH:mm}\r\n" +
                                 $"Đánh giá: {_movie.Rating} sao\r\n" +
                                 $"Đường dẫn / URL: {_movie.MediaUrl}\r\n\r\n" +
                                 $"Mô tả & Ghi chú:\r\n{_movie.Note ?? "Chưa có mô tả."}";
                
                new Forms.FrmDetailDialog($"Thông tin: {_movie.MovieCode}", details).ShowDialog();
                return;
            }

            // Stars area click
            if (_starsRect.Contains(e.Location))
            {
                int starWidth = _starsRect.Width / 5;
                int clickedRating = ((e.X - _starsRect.X) / starWidth) + 1;
                clickedRating = Math.Max(1, Math.Min(5, clickedRating));
                
                _movie.Rating = clickedRating;
                _movieRepo.Update(_movie); // Save to DB
                this.Invalidate();
                return;
            }

            // Heart area click (bottom right)
            if (e.X > this.Width - 45 && e.Y > 200)
            {
                _movieRepo.ToggleFavorite(_movie.Id);
                _movie.IsFavorite = !_movie.IsFavorite;
                this.Invalidate();
                FavoriteToggled?.Invoke(this, _movie);
            }
        }

        // Context menu handlers
        private void menuEdit_Click(object sender, EventArgs e)
        {
            EditClicked?.Invoke(this, _movie);
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            DeleteClicked?.Invoke(this, _movie);
        }
    }
}
