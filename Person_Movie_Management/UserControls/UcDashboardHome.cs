using System;
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
        private System.Collections.Generic.Dictionary<string, int> _tagDist;

        // Hitboxes for instructions
        private Rectangle _tip1Rect;
        private Rectangle _tip2Rect;
        private Rectangle _tip3Rect;
        private Rectangle _tip4Rect;

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

            _pnlContinue.Controls.Clear();
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

            _pnlRecent.Controls.Clear();
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

        private void DrawTipsSection(Graphics g)
        {
            int startX = 40;
            int startY = 1140; // Adjusted for two movie sections

            using var h2Font = new Font("Segoe UI Emoji", 16F, FontStyle.Bold);
            using var h2Brush = new SolidBrush(UIHelper.TextPrimary);
            g.DrawString("💡 Hướng dẫn sử dụng", h2Font, h2Brush, startX, startY);

            int currentY = startY + 50;
            _tip1Rect = DrawTipItem(g, startX, currentY, "1. Thêm Phim Online", "Kéo thả link từ trình duyệt vào ô Thêm phim để nhanh chóng lưu phim Online.");
            currentY += 80;
            _tip2Rect = DrawTipItem(g, startX, currentY, "2. Quét Thư Mục", "Nhấn nút 'Quét thư mục' ở trang Phim Trên Máy để thêm hàng loạt phim.");
            currentY += 80;
            _tip3Rect = DrawTipItem(g, startX, currentY, "3. Quản lý Thẻ Phim", "Click chuột phải vào bất kỳ thẻ phim nào để sửa hoặc xóa phim.");
            currentY += 80;
            _tip4Rect = DrawTipItem(g, startX, currentY, "4. Đánh giá & Yêu thích", "Nhấn vào ngôi sao để đánh giá, nhấn trái tim để thêm vào danh sách Yêu thích.");
            
            // Set min auto scroll size based on content height
            this.AutoScrollMinSize = new Size(0, currentY + 100);
        }

        private Rectangle DrawTipItem(Graphics g, int x, int y, string title, string desc)
        {
            var rect = new Rectangle(x, y, 700, 70);
            using var path = CreateRoundedRectPath(rect, 12);
            using var brush = new SolidBrush(UIHelper.BgCard);
            g.FillPath(brush, path);

            // Glass border
            using var glassPen = new Pen(Color.FromArgb(15, 255, 255, 255), 1f);
            g.DrawPath(glassPen, path);

            // Left accent bar (gradient indigo -> pink)
            var accentRect = new Rectangle(x, y + 10, 4, 50);
            using var accentPath = CreateRoundedRectPath(accentRect, 2);
            using var accentBrush = new LinearGradientBrush(accentRect, UIHelper.AccentPrimary, UIHelper.AccentTertiary, 90f);
            g.FillPath(accentBrush, accentPath);

            // Icon background
            var iconRect = new Rectangle(x + 15, y + 15, 40, 40);
            using var iconBgPath = CreateRoundedRectPath(iconRect, 8);
            using var iconBgBrush = new SolidBrush(Color.FromArgb(25, UIHelper.AccentPrimary));
            g.FillPath(iconBgBrush, iconBgPath);

            using var checkFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            using var checkBrush = new SolidBrush(UIHelper.AccentSecondary);
            g.DrawString("✓", checkFont, checkBrush, x + 25, y + 25);

            using var titleBrush2 = new SolidBrush(UIHelper.TextPrimary);
            g.DrawString(title, UIHelper.FontBody, titleBrush2, x + 70, y + 15);
            using var descBrush = new SolidBrush(UIHelper.TextMuted);
            g.DrawString(desc, UIHelper.FontCaption, descBrush, x + 70, y + 40);
            
            return rect;
        }


        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            // Translate mouse coordinates based on scroll position
            var p = new Point(e.X - this.AutoScrollPosition.X, e.Y - this.AutoScrollPosition.Y);
            
            if (_tip1Rect.Contains(p))
            {
                new Forms.FrmDetailDialog("Thêm Phim Online", 
                    "Bạn có thể vào mục Phim Online, nhấn nút Thêm Phim.\r\n\r\n" +
                    "Bạn có thể copy URL của phim hoặc website, dán vào ô Media URL. " +
                    "Khi click đúp vào thẻ phim, ứng dụng sẽ tự động mở trình duyệt và truy cập vào trang phim này.").ShowDialog();
            }
            else if (_tip2Rect.Contains(p))
            {
                new Forms.FrmDetailDialog("Quét Thư Mục Tự Động", 
                    "Thay vì thêm từng phim thủ công, bạn vào mục Phim Trên Máy, nhấn Quét Thư Mục.\r\n\r\n" +
                    "Chọn một thư mục chứa phim trên máy tính, hệ thống sẽ tự động lọc ra các định dạng video (mp4, mkv, avi...) " +
                    "và tạo từng thẻ phim riêng biệt cho bạn.").ShowDialog();
            }
            else if (_tip3Rect.Contains(p))
            {
                new Forms.FrmDetailDialog("Quản lý Thẻ Phim", 
                    "Mỗi thẻ phim đều có các tương tác nhanh:\r\n\r\n" +
                    "- Click chuột phải: Mở menu để Chỉnh sửa hoặc Xóa phim.\r\n" +
                    "- Click đúp (2 lần): Mở phim hoặc trang web chứa phim.\r\n" +
                    "- Nút 'i' góc trên: Xem chi tiết phim.\r\n").ShowDialog();
            }
            else if (_tip4Rect.Contains(p))
            {
                new Forms.FrmDetailDialog("Đánh giá & Yêu thích", 
                    "Tính năng tương tác trực tiếp trên Thẻ phim:\r\n\r\n" +
                    "⭐ Đánh giá sao:\r\n" +
                    "Rẽ chuột (hover) vào các ngôi sao trên thẻ phim, số sao sẽ sáng lên.\r\n" +
                    "Nhấn chuột để lưu đánh giá 1-5 sao của bạn cho bộ phim đó.\r\n\r\n" +
                    "❤️ Phim Yêu thích:\r\n" +
                    "Nhấn vào biểu tượng trái tim ở góc dưới bên phải thẻ phim.\r\n" +
                    "Khi tim chuyển sang màu trắng/đỏ nghĩa là phim đã được lưu vào Danh sách Yêu Thích.\r\n" +
                    "Bạn có thể xem lại toàn bộ phim yêu thích bằng cách chọn mục 'Yêu Thích' ở menu bên trái.").ShowDialog();
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
