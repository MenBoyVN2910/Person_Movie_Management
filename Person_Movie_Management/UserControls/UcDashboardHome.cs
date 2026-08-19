using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Services;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;

namespace Person_Movie_Management.UserControls
{
    public partial class UcDashboardHome : UserControl
    {
        private readonly MovieRepository _movieRepo;
        
        private int _totalMovies;
        private int _totalFavorites;
        private int _totalOnline;
        
        // Tag Stats
        private System.Collections.Generic.Dictionary<string, int> _tagDist = new();

        // Hitboxes for guide cards
        private readonly List<Rectangle> _guideCardRects = new List<Rectangle>();
        private int _hoveredGuideCard = -1;

        private FlowLayoutPanel _pnlRecent;
        private FlowLayoutPanel _pnlContinue;

        public UcDashboardHome()
        {
            InitializeComponent();
            _movieRepo = new MovieRepository();
            
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = UIHelper.BgDark;
            this.AutoScroll = true;

            _pnlContinue = new FlowLayoutPanel
            {
                Location = new Point(40, 390),
                Size = new Size(900, 330),
                AutoScroll = false,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            this.Controls.Add(_pnlContinue);

            // Setup FlowLayoutPanel for recent movies
            _pnlRecent = new FlowLayoutPanel
            {
                Location = new Point(40, 780),
                Size = new Size(900, 330),
                AutoScroll = false,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            this.Controls.Add(_pnlRecent);

            LoadStats();
            LoadRecentMovies();

            DataCache.DataInvalidated += OnDataInvalidated;
            this.Disposed += (s, e) => { DataCache.DataInvalidated -= OnDataInvalidated; };
        }

        private void OnDataInvalidated()
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.BeginInvoke(() =>
                {
                    LoadStats();
                    LoadRecentMovies();
                    this.Invalidate();
                });
            }
        }

        // Enable WS_EX_COMPOSITED for ultra smooth 240Hz scrolling
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }

        private void LoadStats()
        {
            if (SessionManager.IsLoggedIn)
            {
                int userId = SessionManager.CurrentUser!.Id;
                var stats = _movieRepo.GetStats(userId);
                _totalMovies = stats.Total;
                _totalFavorites = stats.Favorites;
                _totalOnline = stats.Online;
                
                _tagDist = AppServices.TagRepo.GetTagDistribution(userId);
            }
        }

        private void LoadRecentMovies()
        {
            if (!SessionManager.IsLoggedIn) return;

            int userId = SessionManager.CurrentUser!.Id;
            var allMovies = _movieRepo.GetAllByUser(userId);
            
            // Continue Watching: WatchProgress > 0 and < 100, ordered by LastWatched
            var continueMovies = allMovies
                .Where(m => m.WatchProgress > 0 && m.WatchProgress < 100 && m.LastWatched.HasValue)
                .OrderByDescending(m => m.LastWatched)
                .Take(2).ToList();

            while (_pnlContinue.Controls.Count > 0)
            {
                var oldCtrl = _pnlContinue.Controls[0];
                _pnlContinue.Controls.RemoveAt(0);
                oldCtrl.Dispose();
            }

            if (continueMovies.Count == 0)
            {
                var lbl = new Label { Text = "Chưa có phim nào đang xem dở.", ForeColor = UIHelper.TextMuted, Font = UIHelper.FontBody, AutoSize = true, Margin = new Padding(10) };
                _pnlContinue.Controls.Add(lbl);
            }
            else
            {
                foreach (var movie in continueMovies)
                {
                    var card = new UcMovieCard(movie);
                    card.MovieClicked += (s, m) => { if (!string.IsNullOrEmpty(m.MediaUrl)) MediaLauncher.LaunchMedia(m.MediaUrl, m.SourceType); };
                    card.EditClicked += (s, m) => { if (new Forms.FrmMovieDetail(m).ShowDialog() == DialogResult.OK) { LoadStats(); LoadRecentMovies(); this.Invalidate(); } };
                    _pnlContinue.Controls.Add(card);
                }
            }

            // Recent Movies
            var recentMovies = allMovies.OrderByDescending(m => m.CreatedAt).Take(2).ToList();

            while (_pnlRecent.Controls.Count > 0)
            {
                var oldCtrl = _pnlRecent.Controls[0];
                _pnlRecent.Controls.RemoveAt(0);
                oldCtrl.Dispose();
            }

            foreach (var movie in recentMovies)
            {
                var card = new UcMovieCard(movie);
                card.MovieClicked += (s, m) => { if (!string.IsNullOrEmpty(m.MediaUrl)) MediaLauncher.LaunchMedia(m.MediaUrl, m.SourceType); };
                card.EditClicked += (s, m) => { if (new Forms.FrmMovieDetail(m).ShowDialog() == DialogResult.OK) { LoadStats(); LoadRecentMovies(); this.Invalidate(); } };
                _pnlRecent.Controls.Add(card);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Adjust for AutoScroll
            g.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

            // 1. Draw Greeting
            DrawGreeting(g);

            // 2. Draw Stats Cards
            DrawStatsCards(g);

            // 3. Draw Recent Movies Title
            DrawRecentTitle(g);

            // 4. Draw Tips Section
            DrawTipsSection(g);
        }

        private void DrawGreeting(Graphics g)
        {
            string userName = SessionManager.CurrentUser?.DisplayName ?? "User";
            int hour = DateTime.Now.Hour;
            string greeting = hour < 12 ? "Chào buổi sáng" : hour < 18 ? "Chào buổi chiều" : "Chào buổi tối";
            
            using var font = new Font("Segoe UI", 22F, FontStyle.Bold);
            using var brush = new SolidBrush(UIHelper.TextPrimary);
            g.DrawString($"{greeting}, {userName}!", font, brush, 40, 40);

            var subFont = UIHelper.FontBody;
            using var subBrush = new SolidBrush(UIHelper.TextMuted);
            g.DrawString("Chào mừng bạn trở lại với kho phim cá nhân.", subFont, subBrush, 45, 80);
        }

        private void DrawStatsCards(Graphics g)
        {
            int startX = 40;
            int startY = 140;
            int cardWidth = 280;
            int cardHeight = 150;
            int spacing = 30;

            DrawCard(g, startX, startY, cardWidth, cardHeight, 
                     "Tổng Số Phim", _totalMovies.ToString(), "🎬", 
                     UIHelper.GradViolet1, UIHelper.GradViolet2);

            DrawCard(g, startX + cardWidth + spacing, startY, cardWidth, cardHeight, 
                     "Yêu Thích", _totalFavorites.ToString(), "❤️", 
                     UIHelper.GradRose1, UIHelper.GradRose2);

            DrawCard(g, startX + (cardWidth + spacing) * 2, startY, cardWidth, cardHeight, 
                     "Phim Online", _totalOnline.ToString(), "🌐", 
                     UIHelper.GradSky1, UIHelper.GradSky2);
        }

        private void DrawCard(Graphics g, int x, int y, int w, int h, string title, string value, string icon, Color c1, Color c2)
        {
            var rect = new Rectangle(x, y, w, h);
            
            // Drop shadow
            var shadowRect = new Rectangle(x + 3, y + 5, w - 6, h - 4);
            using var shadowPath = CreateRoundedRectPath(shadowRect, 20);
            using var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0));
            g.FillPath(shadowBrush, shadowPath);

            // Draw Gradient Background
            using var path = CreateRoundedRectPath(rect, 20);
            using var brush = new LinearGradientBrush(rect, c1, c2, 135f);
            g.FillPath(brush, path);

            // Glass border
            using var glassPen = new Pen(Color.FromArgb(40, 255, 255, 255), 1.5f);
            g.DrawPath(glassPen, path);

            // Subtle overlay circle for depth
            using var overlayPath = new GraphicsPath();
            overlayPath.AddEllipse(x + w - 100, y - 50, 150, 150);
            using var overlayBrush = new SolidBrush(Color.FromArgb(30, 255, 255, 255));
            g.SetClip(path);
            g.FillPath(overlayBrush, overlayPath);
            g.ResetClip();

            // Draw Icon on the right side
            using var iconFont = new Font("Segoe UI Emoji", 36F);
            using var iconBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 255));
            g.DrawString(icon, iconFont, iconBrush, x + w - 85, y + 35);

            // Draw Title on the top left
            using var titleFont = new Font("Segoe UI", 12.5F, FontStyle.Regular);
            using var titleBrush = new SolidBrush(Color.FromArgb(230, 255, 255, 255));
            g.DrawString(title, titleFont, titleBrush, x + 25, y + 30);

            // Draw Value on the bottom left
            var valFont = UIHelper.FontStatNum;
            g.DrawString(value, valFont, Brushes.White, x + 21, y + 65);
        }

        private void DrawRecentTitle(Graphics g)
        {
            using var h2Font = new Font("Segoe UI Emoji", 16F, FontStyle.Bold);
            using var h2Brush = new SolidBrush(UIHelper.TextPrimary);
            
            g.DrawString("▶️ Tiếp tục xem", h2Font, h2Brush, 40, 340);
            
            g.DrawString("🔥 Phim mới thêm gần đây", h2Font, h2Brush, 40, 730);
        }

        private struct GuideCardItem
        {
            public string Badge;
            public string Title;
            public string Desc;
            public Color AccentColor;
        }

        private void DrawTipsSection(Graphics g)
        {
            int startX = 40;
            int startY = 1140; // Adjusted for movie sections

            using var h2Font = new Font("Segoe UI Emoji", 16F, FontStyle.Bold);
            using var h2Brush = new SolidBrush(UIHelper.TextPrimary);
            g.DrawString("💡 Hướng dẫn sử dụng theo từng trang", h2Font, h2Brush, startX, startY);

            using var subFont = new Font("Segoe UI", 9.5F);
            using var subBrush = new SolidBrush(UIHelper.TextMuted);
            g.DrawString("Nhấp vào bất kỳ thẻ nào để xem cẩm nang chi tiết từng tính năng, thao tác chuột và phím tắt.", subFont, subBrush, startX, startY + 32);

            _guideCardRects.Clear();

            var cards = new GuideCardItem[]
            {
                new GuideCardItem { Badge = "🌐", Title = "1. Trang Phim Online", Desc = "Lưu URL web, kéo thả link/ảnh, lọc tag/diễn viên/sao...", AccentColor = Color.FromArgb(99, 102, 241) },
                new GuideCardItem { Badge = "📁", Title = "2. Trang Phim Trên Máy", Desc = "Quản lý video offline, quét thư mục tự động hàng loạt...", AccentColor = Color.FromArgb(59, 130, 246) },
                new GuideCardItem { Badge = "🎵", Title = "3. Trang Âm Thanh", Desc = "Phát nhạc nền toàn cục, phím tắt Space/Mũi tên, quản lý OST...", AccentColor = Color.FromArgb(236, 72, 153) },
                new GuideCardItem { Badge = "👥", Title = "4. Trang Diễn Viên", Desc = "Hồ sơ nghệ sĩ, album ảnh chi tiết, xem phim đã tham gia...", AccentColor = Color.FromArgb(245, 158, 11) },
                new GuideCardItem { Badge = "📑", Title = "5. Trang Danh Sách Phát", Desc = "Tạo playlist kết hợp phim + nhạc, tùy chỉnh thứ tự phát...", AccentColor = Color.FromArgb(168, 85, 247) },
                new GuideCardItem { Badge = "🗑️", Title = "6. Trang Thùng Rác", Desc = "Xóa an toàn Soft-Delete, khôi phục nguyên trạng, dọn sạch...", AccentColor = Color.FromArgb(239, 68, 68) },
                new GuideCardItem { Badge = "💾", Title = "7. Trang Sao Lưu & Khôi Phục", Desc = "Đa thư mục sao lưu, tự động backup, phục hồi 1-click...", AccentColor = Color.FromArgb(16, 185, 129) },
                new GuideCardItem { Badge = "⚡", Title = "8. Tiện Ích & Phím Tắt", Desc = "Drop Widget ngoài Desktop, Omnibox Ctrl+K, phím tắt media...", AccentColor = Color.FromArgb(6, 182, 212) }
            };

            // Responsive 2-column layout with safe boundary
            int totalContentWidth = Math.Max(this.ClientSize.Width - 80, 900);
            int gapX = 24;
            int gapY = 16;
            int colWidth = Math.Max(430, (totalContentWidth - gapX) / 2);
            int colHeight = 86;
            int baseTop = startY + 68;

            for (int i = 0; i < cards.Length; i++)
            {
                int col = i % 2;
                int row = i / 2;

                int x = startX + col * (colWidth + gapX);
                int y = baseTop + row * (colHeight + gapY);

                bool isHovered = (_hoveredGuideCard == i);
                var rect = DrawGuideCard(g, x, y, colWidth, colHeight, cards[i], isHovered);
                _guideCardRects.Add(rect);
            }

            int totalBottom = baseTop + ((cards.Length + 1) / 2) * (colHeight + gapY) + 60;
            this.AutoScrollMinSize = new Size(0, totalBottom);
        }

        private Rectangle DrawGuideCard(Graphics g, int x, int y, int w, int h, GuideCardItem item, bool isHovered)
        {
            var rect = new Rectangle(x, y, w, h);
            using var path = CreateRoundedRectPath(rect, 14);

            // Card background
            Color bg = isHovered ? Color.FromArgb(26, 36, 58) : UIHelper.BgCard;
            using var brush = new SolidBrush(bg);
            g.FillPath(brush, path);

            // Glass border with hover glow
            Color borderColor = isHovered ? item.AccentColor : Color.FromArgb(25, 255, 255, 255);
            using var glassPen = new Pen(borderColor, isHovered ? 1.5f : 1f);
            g.DrawPath(glassPen, path);

            // Left accent bar
            var accentRect = new Rectangle(x, y + 10, 4, h - 20);
            using var accentPath = CreateRoundedRectPath(accentRect, 2);
            using var accentBrush = new SolidBrush(item.AccentColor);
            g.FillPath(accentBrush, accentPath);

            // Left Icon Badge Box
            int iconBoxSize = 54;
            var iconRect = new Rectangle(x + 14, y + (h - iconBoxSize) / 2, iconBoxSize, iconBoxSize);
            using var iconBgPath = CreateRoundedRectPath(iconRect, 12);
            using var iconBgBrush = new SolidBrush(Color.FromArgb(isHovered ? 50 : 25, item.AccentColor));
            g.FillPath(iconBgBrush, iconBgPath);

            // Left Icon Emoji
            using var iconFont = new Font("Segoe UI Emoji", 20F);
            using var sfCenter = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(item.Badge, iconFont, Brushes.White, iconRect, sfCenter);

            // Text Content Boundaries (Strictly bounded so it NEVER overflows)
            float textStartX = x + 78;
            float textAvailableWidth = w - 78 - 50; // Leave 50px for circular arrow on right

            // Title
            var titleRect = new RectangleF(textStartX, y + 16, textAvailableWidth, 24);
            using var titleFont = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            using var titleBrush = new SolidBrush(isHovered ? Color.White : UIHelper.TextPrimary);
            using var titleSf = new StringFormat
            {
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(item.Title, titleFont, titleBrush, titleRect, titleSf);

            // Description (Wrapped with clean word trimming)
            var descRect = new RectangleF(textStartX, y + 42, textAvailableWidth, 34);
            using var descFont = new Font("Segoe UI", 9F);
            using var descBrush = new SolidBrush(isHovered ? Color.FromArgb(203, 213, 225) : UIHelper.TextMuted);
            using var descSf = new StringFormat
            {
                Trimming = StringTrimming.EllipsisWord,
                LineAlignment = StringAlignment.Near
            };
            g.DrawString(item.Desc, descFont, descBrush, descRect, descSf);

            // Right Circular Chevron Button
            int btnSize = 32;
            int btnX = x + w - btnSize - 14;
            int btnY = y + (h - btnSize) / 2;
            var btnCircleRect = new Rectangle(btnX, btnY, btnSize, btnSize);

            using var circlePath = new GraphicsPath();
            circlePath.AddEllipse(btnCircleRect);

            if (isHovered)
            {
                using var circleBrush = new SolidBrush(item.AccentColor);
                g.FillPath(circleBrush, circlePath);
            }
            else
            {
                using var circleBrush = new SolidBrush(Color.FromArgb(18, 255, 255, 255));
                g.FillPath(circleBrush, circlePath);
            }

            using var chevronFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            using var chevronBrush = new SolidBrush(isHovered ? Color.White : Color.FromArgb(148, 163, 184));
            g.DrawString("›", chevronFont, chevronBrush, btnCircleRect, sfCenter);

            return rect;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var p = new Point(e.X - this.AutoScrollPosition.X, e.Y - this.AutoScrollPosition.Y);
            int newHovered = -1;

            for (int i = 0; i < _guideCardRects.Count; i++)
            {
                if (_guideCardRects[i].Contains(p))
                {
                    newHovered = i;
                    break;
                }
            }

            if (newHovered != _hoveredGuideCard)
            {
                _hoveredGuideCard = newHovered;
                this.Cursor = (_hoveredGuideCard != -1) ? Cursors.Hand : Cursors.Default;
                this.Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoveredGuideCard != -1)
            {
                _hoveredGuideCard = -1;
                this.Cursor = Cursors.Default;
                this.Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            // Translate mouse coordinates based on scroll position
            var p = new Point(e.X - this.AutoScrollPosition.X, e.Y - this.AutoScrollPosition.Y);

            for (int i = 0; i < _guideCardRects.Count; i++)
            {
                if (_guideCardRects[i].Contains(p))
                {
                    var topic = (Forms.GuideTopic)i;
                    new Forms.FrmGuideDetail(topic).ShowDialog(this);
                    return;
                }
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
    }
}
