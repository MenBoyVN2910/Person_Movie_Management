using System;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using System.Linq;
using Person_Movie_Management.Services;
using System.Collections.Generic;

namespace Person_Movie_Management.UserControls
{
    public partial class UcMovieCard : UserControl
    {
        private Movie _movie;
        private List<Tag> _tags = new();
        private int _currentLoadId = 0;
        private int _boundMovieId = -1; // Track để skip bind nếu cùng movie
        
        // Phase 2: Hover Preview
        private System.Windows.Forms.Timer _hoverTimer;
        private List<string> _galleryImages = null;
        private bool _galleryLoaded = false; // Load gallery 1 lần duy nhất per movie
        private int _currentGalleryIndex = 0;
        private Image _originalCover;

        public event EventHandler<Movie>? MovieClicked;
        public event EventHandler<Movie>? FavoriteToggled;
        public event EventHandler<Movie>? EditClicked;
        public event EventHandler<Movie>? DeleteClicked;

        public UcMovieCard(Movie movie, System.Collections.Generic.List<Tag> tags = null)
        {
            InitializeComponent();
            
            this.Cursor = Cursors.Hand;
            
            menuEdit.Image = UIHelper.CreateIcon("\uE70F", 12f);
            menuDelete.Image = UIHelper.CreateIcon("\uE74D", 12f);
            menuAddToPlaylist.Image = UIHelper.CreateIcon("\uE710", 12f);

            lblRating.MouseClick += lblRating_MouseClick;

            BindData(movie, tags);

            // Setup Hover Timer
            _hoverTimer = new System.Windows.Forms.Timer();
            _hoverTimer.Interval = 1000; // Change image every 1 second
            _hoverTimer.Tick += HoverTimer_Tick;

            AttachHoverEvents(pnlBase);
        }

        private void AttachHoverEvents(Control parent)
        {
            parent.MouseEnter += Card_MouseEnter;
            parent.MouseLeave += Card_MouseLeave;
            foreach (Control child in parent.Controls)
            {
                if (child != lblFavorite && child != lblInfo && child != lblRating)
                {
                    AttachHoverEvents(child);
                }
            }
        }

        private void Card_MouseEnter(object? sender, EventArgs e)
        {
            var hoverColor = Color.FromArgb(35, 42, 85);
            pnlBase.FillColor = hoverColor; // Sáng hơn một chút
            lblTitle.BackColor = hoverColor;
            flpTags.BackColor = hoverColor;
            lblRating.BackColor = hoverColor;
            lblFavorite.BackColor = hoverColor;
            
            // Start hover preview — load gallery ASYNC, chỉ 1 lần per movie
            if (!_galleryLoaded)
            {
                _galleryLoaded = true;
                LoadGalleryAsync();
            }
            
            if (_galleryImages != null && _galleryImages.Count > 1)
            {
                _originalCover = picCover.Image;
                _currentGalleryIndex = 0;
                _hoverTimer.Start();
            }
        }

        private async void LoadGalleryAsync()
        {
            try
            {
                int movieId = _movie.Id;
                var images = await System.Threading.Tasks.Task.Run(() =>
                {
                    var repo = new MovieImageRepository();
                    return repo.GetByMovieId(movieId);
                });
                
                if (_movie.Id != movieId || this.IsDisposed) return; // Card đã bị recycle
                
                _galleryImages = images.Select(img => img.ImagePath).ToList();
                if (!string.IsNullOrEmpty(_movie.CoverImage))
                {
                    _galleryImages.Insert(0, _movie.CoverImage);
                }
            }
            catch { }
        }

        private void HoverTimer_Tick(object? sender, EventArgs e)
        {
            if (_galleryImages != null && _galleryImages.Count > 0)
            {
                _currentGalleryIndex = (_currentGalleryIndex + 1) % _galleryImages.Count;
                var rawImage = FileHelper.LoadImageSafe(_galleryImages[_currentGalleryIndex]);
                if (rawImage != null)
                {
                    if (picCover.Image != null && picCover.Image.Tag?.ToString() == "cropped" && picCover.Image != _originalCover)
                    {
                        picCover.Image.Dispose();
                    }
                    var cropped = UIHelper.CropToFill(rawImage, picCover.Width, picCover.Height);
                    if (cropped != null)
                    {
                        cropped.Tag = "cropped";
                        picCover.Image = cropped;
                        rawImage.Dispose();
                    }
                    else
                    {
                        picCover.Image = rawImage;
                    }
                }
            }
        }

        private void Card_MouseLeave(object? sender, EventArgs e)
        {
            // Chỉ trả về màu cũ khi chuột thực sự rời khỏi toàn bộ thẻ
            var rect = pnlBase.RectangleToScreen(pnlBase.ClientRectangle);
            if (!rect.Contains(Cursor.Position))
            {
                var normalColor = Color.FromArgb(22, 28, 56);
                pnlBase.FillColor = normalColor; // Màu gốc
                lblTitle.BackColor = normalColor;
                flpTags.BackColor = normalColor;
                lblRating.BackColor = normalColor;
                lblFavorite.BackColor = normalColor;
                
                // Stop hover preview
                _hoverTimer.Stop();
                if (_originalCover != null)
                {
                    picCover.Image = _originalCover;
                }
            }
        }

        public void BindData(Movie movie, System.Collections.Generic.List<Tag> tags = null)
        {
            // SKIP hoàn toàn nếu đang hiển thị cùng movie (cuộn lên rồi xuống lại) và dữ liệu hiển thị không đổi
            if (_boundMovieId == movie.Id && _movie != null 
                && _movie.Rating == movie.Rating 
                && _movie.IsFavorite == movie.IsFavorite
                && _movie.CoverImage == movie.CoverImage
                && _movie.MovieCode == movie.MovieCode)
            {
                _movie = movie; // Cập nhật reference nhưng không bind lại UI
                if (_movie.WatchProgress > 0)
                {
                    pgbWatchProgress.Value = _movie.WatchProgress;
                    pgbWatchProgress.Visible = true;
                }
                else
                {
                    pgbWatchProgress.Visible = false;
                }
                return;
            }

            _movie = movie;
            _boundMovieId = movie.Id;
            
            // Reset gallery khi bind movie mới
            _galleryImages = null;
            _galleryLoaded = false;
            _hoverTimer?.Stop();

            if (tags != null)
            {
                _tags = tags;
            }
            else
            {
                _tags = AppServices.TagRepo.GetTagsForMovie(_movie.Id);
            }
            // Bind Title
            lblTitle.Text = _movie.MovieCode;

            // Bind Source (ONLINE/LOCAL)
            lblSource.Text = _movie.SourceType == 0 ? "ONLINE" : "LOCAL";
            lblSource.BackColor = _movie.SourceType == 0 ? UIHelper.AccentPrimary : UIHelper.Success;

            // Bind Rating
            string stars = "";
            for (int i = 1; i <= 5; i++)
            {
                stars += i <= _movie.Rating ? "★" : "☆";
            }
            lblRating.Text = stars;

            // Bind Favorite
            lblFavorite.Text = _movie.IsFavorite ? "❤️" : "🤍";

            // Bind Tags (Tối ưu hóa: Tái sử dụng control thay vì tạo mới liên tục gây lag)
            this.SuspendLayout();
            flpTags.SuspendLayout();
            
            int maxTags = Math.Min(_tags.Count, 4); // Tối đa hiển thị 4 tag
            for (int i = 0; i < maxTags; i++)
            {
                var tag = _tags[i];
                Color tagColor = UIHelper.AccentPrimary;
                try { if (!string.IsNullOrEmpty(tag.ColorHex)) tagColor = ColorTranslator.FromHtml(tag.ColorHex); } catch { }

                Label lblTag;
                if (i < flpTags.Controls.Count)
                {
                    lblTag = (Label)flpTags.Controls[i];
                }
                else
                {
                    lblTag = new Label
                    {
                        AutoSize = true,
                        Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                        Padding = new Padding(4, 2, 4, 2),
                        Margin = new Padding(0, 0, 6, 0)
                    };
                    flpTags.Controls.Add(lblTag);
                }

                lblTag.Text = tag.TagName;
                lblTag.BackColor = tagColor;
                lblTag.ForeColor = Color.White;
                lblTag.Visible = true;
            }

            // Ẩn các tag thừa
            for (int i = maxTags; i < flpTags.Controls.Count; i++)
            {
                flpTags.Controls[i].Visible = false;
            }
            
            flpTags.ResumeLayout(false); // false: không ép layout recalculation
            this.ResumeLayout(false);    // false: tránh layout overhead khi cuộn

            // Bind Cover Image
            picCover.Image = null; // Clear old image
            LoadCoverImageAsync();

            if (_movie.WatchProgress > 0)
            {
                pgbWatchProgress.Value = _movie.WatchProgress;
                pgbWatchProgress.Visible = true;
            }
            else
            {
                pgbWatchProgress.Visible = false;
            }
        }

        private async void LoadCoverImageAsync()
        {
            int loadId = ++_currentLoadId;
            if (!string.IsNullOrEmpty(_movie.CoverImage))
            {
                string fullPath = FileHelper.GetFullPath(_movie.CoverImage);
                if (System.IO.File.Exists(fullPath))
                {
                    try
                    {
                        // Dùng ImageCache.GetAsync trực tiếp (check cache trước, nếu hit thì không tạo Task)
                        var img = await ImageCache.GetAsync(fullPath);
                        if (img != null && !this.IsDisposed && _currentLoadId == loadId)
                        {
                            if (picCover.Image != null && picCover.Image.Tag?.ToString() == "cropped")
                            {
                                picCover.Image.Dispose();
                            }
                            var cropped = UIHelper.CropToFill(img, picCover.Width, picCover.Height);
                            if (cropped != null)
                            {
                                cropped.Tag = "cropped";
                                picCover.Image = cropped;
                            }
                            else
                            {
                                picCover.Image = img;
                            }
                        }
                    }
                    catch { }
                }
            }
            else
            {
                if (_currentLoadId == loadId) picCover.Image = null;
            }
        }

        private void MainControl_Click(object sender, EventArgs e)
        {
            MovieClicked?.Invoke(this, _movie);
        }

        private void lblFavorite_Click(object sender, EventArgs e)
        {
            AppServices.MovieRepo.ToggleFavorite(_movie.Id);
            DataCache.Invalidate();
            _movie.IsFavorite = !_movie.IsFavorite;
            BindData(_movie, _tags); // Rebind to update heart icon
            FavoriteToggled?.Invoke(this, _movie);
        }

        private void lblInfo_Click(object sender, EventArgs e)
        {
            string type = _movie.SourceType == 0 ? "🌐 Online" : "📁 Trên Máy";
            string details = $"Mã phim: {_movie.MovieCode}\r\n" +
                             $"Nguồn: {type}\r\n" +
                             $"Ngày thêm: {_movie.CreatedAt:dd/MM/yyyy HH:mm}\r\n" +
                             $"Đánh giá: {_movie.Rating} sao\r\n" +
                             $"Đường dẫn / URL: {_movie.MediaUrl}\r\n\r\n" +
                             $"Mô tả & Ghi chú:\r\n{_movie.Note ?? "Chưa có mô tả."}";
            
            new Forms.FrmDetailDialog($"Thông tin: {_movie.MovieCode}", details).ShowDialog();
        }

        private void lblRating_MouseClick(object sender, MouseEventArgs e)
        {
            // Simple rating calculation based on click position
            int starWidth = 22; // rough width of one star character
            int clickedRating = (e.X / starWidth) + 1;
            clickedRating = Math.Max(1, Math.Min(5, clickedRating));
            
            _movie.Rating = clickedRating;
            AppServices.MovieRepo.Update(_movie); // Save to DB
            DataCache.Invalidate();
            BindData(_movie, _tags);
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

        private void menuAddToPlaylist_Click(object sender, EventArgs e)
        {
            int userId = _movie.UserId;
            var frm = new Person_Movie_Management.Forms.FrmSelectPlaylist(userId, _movie.Id, Person_Movie_Management.Models.PlaylistItemType.Movie);
            frm.ShowDialog();
        }

        private void menuUpdateProgress_Click(object sender, EventArgs e)
        {
            string input = UIHelper.ShowInputBox("Cập nhật tiến độ", "Nhập phần trăm đã xem (0-100):", _movie.WatchProgress.ToString());
            if (int.TryParse(input, out int progress))
            {
                if (progress < 0) progress = 0;
                if (progress > 100) progress = 100;

                _movie.WatchProgress = progress;
                AppServices.MovieRepo.UpdateProgress(_movie.Id, progress);
                DataCache.Invalidate();
                this.Invalidate();
                BindData(_movie, _tags);
            }
        }
    }
}
