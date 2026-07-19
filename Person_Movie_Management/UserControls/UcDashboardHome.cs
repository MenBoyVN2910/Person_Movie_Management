using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Repositories;

namespace Person_Movie_Management.UserControls
{
    public partial class UcDashboardHome : UserControl
    {
        private readonly MovieRepository _movieRepo;
        
        // Cache stats
        private int _totalMovies;
        private int _totalFavorites;
        private int _totalOnline;

        // Hitboxes for instructions
        private Rectangle _tip1Rect;
        private Rectangle _tip2Rect;
        private Rectangle _tip3Rect;
        private Rectangle _tip4Rect;

        public UcDashboardHome()
        {
            InitializeComponent();
            _movieRepo = new MovieRepository();
            
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = UIHelper.BgDark;

            LoadStats();
        }

        private void LoadStats()
        {
            if (SessionManager.IsLoggedIn)
            {
                int userId = SessionManager.CurrentUser!.Id;
                _totalMovies = _movieRepo.GetAllByUser(userId).Count;
                _totalFavorites = _movieRepo.GetFavorites(userId).Count;
                _totalOnline = _movieRepo.GetAllByUser(userId, 0).Count; // online
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 1. Draw Greeting
            DrawGreeting(g);

            // 2. Draw Stats Cards
            DrawStatsCards(g);

            // 3. Draw Tips Section
            DrawTipsSection(g);
        }

        private void DrawGreeting(Graphics g)
        {
            string userName = SessionManager.CurrentUser?.DisplayName ?? "User";
            int hour = DateTime.Now.Hour;
            string greeting = hour < 12 ? "🌅 Chào buổi sáng" : hour < 18 ? "☀️ Chào buổi chiều" : "🌙 Chào buổi tối";
            
            var font = UIHelper.FontTitle;
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
            using var shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
            g.FillPath(shadowBrush, shadowPath);

            // Draw Gradient Background
            using var path = CreateRoundedRectPath(rect, 20);
            using var brush = new LinearGradientBrush(rect, c1, c2, 135f);
            g.FillPath(brush, path);

            // Glass border (white edge)
            using var glassPen = new Pen(Color.FromArgb(30, 255, 255, 255), 1f);
            g.DrawPath(glassPen, path);

            // Subtle overlay circle for depth
            using var overlayPath = new GraphicsPath();
            overlayPath.AddEllipse(x + w - 100, y - 50, 150, 150);
            using var overlayBrush = new SolidBrush(Color.FromArgb(40, 255, 255, 255));
            g.SetClip(path);
            g.FillPath(overlayBrush, overlayPath);
            g.ResetClip();

            // Draw Icon
            using var iconFont = new Font("Segoe UI Emoji", 24F);
            g.DrawString(icon, iconFont, Brushes.White, x + 25, y + 20);

            // Draw Title
            using var titleFont = new Font("Segoe UI", 12F, FontStyle.Regular);
            using var titleBrush = new SolidBrush(Color.FromArgb(230, 255, 255, 255));
            g.DrawString(title, titleFont, titleBrush, x + 25, y + 65);

            // Draw Value
            var valFont = UIHelper.FontStatNum;
            g.DrawString(value, valFont, Brushes.White, x + 25, y + 90);
        }

        private void DrawTipsSection(Graphics g)
        {
            int startX = 40;
            int startY = 340;

            var h2Font = UIHelper.FontH2;
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
            
            if (_tip1Rect.Contains(e.Location))
            {
                new Forms.FrmDetailDialog("Thêm Phim Online", 
                    "Bạn có thể vào mục Phim Online, nhấn nút Thêm Phim.\r\n\r\n" +
                    "Bạn có thể copy URL của phim hoặc website, dán vào ô Media URL. " +
                    "Khi click đúp vào thẻ phim, ứng dụng sẽ tự động mở trình duyệt và truy cập vào trang phim này.").ShowDialog();
            }
            else if (_tip2Rect.Contains(e.Location))
            {
                new Forms.FrmDetailDialog("Quét Thư Mục Tự Động", 
                    "Thay vì thêm từng phim thủ công, bạn vào mục Phim Trên Máy, nhấn Quét Thư Mục.\r\n\r\n" +
                    "Chọn một thư mục chứa phim trên máy tính, hệ thống sẽ tự động lọc ra các định dạng video (mp4, mkv, avi...) " +
                    "và tạo từng thẻ phim riêng biệt cho bạn.").ShowDialog();
            }
            else if (_tip3Rect.Contains(e.Location))
            {
                new Forms.FrmDetailDialog("Quản lý Thẻ Phim", 
                    "Mỗi thẻ phim đều có các tương tác nhanh:\r\n\r\n" +
                    "- Click chuột phải: Mở menu để Chỉnh sửa hoặc Xóa phim.\r\n" +
                    "- Click đúp (2 lần): Mở phim hoặc trang web chứa phim.\r\n" +
                    "- Nút 'i' góc trên: Xem chi tiết phim.\r\n").ShowDialog();
            }
            else if (_tip4Rect.Contains(e.Location))
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
