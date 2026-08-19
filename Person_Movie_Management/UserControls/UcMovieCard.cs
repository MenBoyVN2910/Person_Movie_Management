using System;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using System.Linq;
using Person_Movie_Management.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Person_Movie_Management.UserControls
{
    public partial class UcMovieCard : UserControl
    {
        private Movie _movie = null!;
        private List<Tag> _tags = new();
        private int _currentLoadId = 0;
        private int _boundMovieId = -1;

        // Hover Slideshow Preview
        private readonly System.Windows.Forms.Timer _slideshowTimer;
        private List<Image>? _preparedGalleryFrames;
        private Image? _originalCover;
        private bool _isHovered = false;
        private int _currentFrameIndex = 0;
        private int _galleryLoadId = 0;

        public event EventHandler<Movie>? MovieClicked;
        public event EventHandler<Movie>? FavoriteToggled;
        public event EventHandler<Movie>? EditClicked;
        public event EventHandler<Movie>? DeleteClicked;

        public UcMovieCard(Movie movie, List<Tag>? tags = null)
        {
            InitializeComponent();
            
            this.DoubleBuffered = true;
            this.BackColor = UIHelper.BgDark;
            this.pnlBase.BackColor = UIHelper.BgDark;
            this.Cursor = Cursors.Hand;
            
            menuEdit.Image = UIHelper.CreateIcon("\uE70F", 12f);
            menuDelete.Image = UIHelper.CreateIcon("\uE74D", 12f);
            menuAddToPlaylist.Image = UIHelper.CreateIcon("\uE710", 12f);

            lblRating.Cursor = Cursors.Hand;
            lblRating.MouseClick += lblRating_MouseClick;
            lblRating.MouseMove += lblRating_MouseMove;
            lblRating.MouseLeave += lblRating_MouseLeave;

            // Slideshow Timer: 750ms interval for smooth preview
            _slideshowTimer = new System.Windows.Forms.Timer();
            _slideshowTimer.Interval = 750;
            _slideshowTimer.Tick += SlideshowTimer_Tick;

            this.Disposed += (s, e) =>
            {
                _slideshowTimer?.Stop();
                _slideshowTimer?.Dispose();
            };

            BindData(movie, tags);

            AttachHoverEvents(pnlBase);
        }

        private void AttachHoverEvents(Control control)
        {
            control.MouseEnter += OnControlMouseEnter;
            control.MouseLeave += OnControlMouseLeave;
            control.MouseMove += OnControlMouseMove;

            foreach (Control child in control.Controls)
            {
                if (child != lblFavorite && child != lblInfo && child != lblRating)
                {
                    AttachHoverEvents(child);
                }
            }
        }

        private void OnControlMouseEnter(object? sender, EventArgs e)
        {
            if (!_isHovered)
            {
                StartHover();
            }
        }

        private void OnControlMouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isHovered)
            {
                StartHover();
            }
        }

        private void OnControlMouseLeave(object? sender, EventArgs e)
        {
            var rect = pnlBase.RectangleToScreen(pnlBase.ClientRectangle);
            if (!rect.Contains(Cursor.Position))
            {
                StopHover();
            }
        }

        private void StartHover()
        {
            if (_movie == null) return;
            _isHovered = true;

            var hoverColor = Color.FromArgb(35, 42, 85);
            pnlBase.FillColor = hoverColor;
            lblTitle.BackColor = hoverColor;
            flpTags.BackColor = hoverColor;
            lblRating.BackColor = hoverColor;
            lblFavorite.BackColor = hoverColor;

            if (_originalCover == null && picCover.Image != null)
            {
                _originalCover = picCover.Image;
            }

            if (_preparedGalleryFrames != null)
            {
                if (_preparedGalleryFrames.Count > 1 && !_slideshowTimer.Enabled)
                {
                    _currentFrameIndex = 0;
                    _slideshowTimer.Start();
                }
                return;
            }

            // Asynchronously load and pre-crop all gallery frames
            int currentMovieId = _movie.Id;
            int loadId = ++_galleryLoadId;
            int targetWidth = picCover.Width > 0 ? picCover.Width : 348;
            int targetHeight = picCover.Height > 0 ? picCover.Height : 185;

            _ = Task.Run(async () =>
            {
                var repo = new MovieImageRepository();
                var subImages = repo.GetByMovieId(currentMovieId);

                var paths = new List<string>();
                if (!string.IsNullOrEmpty(_movie.CoverImage))
                {
                    string mainPath = FileHelper.GetFullPath(_movie.CoverImage);
                    if (System.IO.File.Exists(mainPath)) paths.Add(mainPath);
                }

                foreach (var sub in subImages)
                {
                    string subPath = FileHelper.GetFullPath(sub.ImagePath);
                    if (System.IO.File.Exists(subPath) && !paths.Contains(subPath, StringComparer.OrdinalIgnoreCase))
                    {
                        paths.Add(subPath);
                    }
                }

                if (paths.Count <= 1)
                {
                    if (_movie.Id == currentMovieId && _galleryLoadId == loadId && !this.IsDisposed)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            _preparedGalleryFrames = new List<Image>();
                        }));
                    }
                    return;
                }

                var frames = new List<Image>();
                foreach (var p in paths)
                {
                    try
                    {
                        var cropped = await ImageCache.GetThumbnailAsync(p, targetWidth, targetHeight);
                        if (cropped != null)
                        {
                            frames.Add(cropped);
                        }
                    }
                    catch { }
                }

                if (_movie.Id == currentMovieId && _galleryLoadId == loadId && !this.IsDisposed)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        _preparedGalleryFrames = frames;
                        if (_isHovered && _preparedGalleryFrames.Count > 1 && !_slideshowTimer.Enabled)
                        {
                            _currentFrameIndex = 0;
                            _slideshowTimer.Start();
                        }
                    }));
                }
            });
        }

        private void SlideshowTimer_Tick(object? sender, EventArgs e)
        {
            if (!_isHovered || _preparedGalleryFrames == null || _preparedGalleryFrames.Count <= 1 || this.IsDisposed)
            {
                _slideshowTimer.Stop();
                return;
            }

            // Watchdog: verify mouse is still inside the card
            var screenRect = pnlBase.RectangleToScreen(pnlBase.ClientRectangle);
            if (!screenRect.Contains(Cursor.Position))
            {
                StopHover();
                return;
            }

            _currentFrameIndex = (_currentFrameIndex + 1) % _preparedGalleryFrames.Count;
            picCover.Image = _preparedGalleryFrames[_currentFrameIndex];
        }

        private void StopHover()
        {
            _isHovered = false;
            _slideshowTimer.Stop();

            var normalColor = Color.FromArgb(22, 28, 56);
            pnlBase.FillColor = normalColor;
            lblTitle.BackColor = normalColor;
            flpTags.BackColor = normalColor;
            lblRating.BackColor = normalColor;
            lblFavorite.BackColor = normalColor;

            // Immediately reset to the original main cover
            if (_originalCover != null && !this.IsDisposed)
            {
                picCover.Image = _originalCover;
            }
            else if (_movie != null && !string.IsNullOrEmpty(_movie.CoverImage))
            {
                LoadCoverImageAsync();
            }
            _currentFrameIndex = 0;
        }

        public void BindData(Movie movie, List<Tag>? tags = null)
        {
            if (movie == null) return;

            // Fast Guard: Nếu thẻ đang hiển thị đúng movie này và không đổi trạng thái thì bỏ qua
            if (_boundMovieId == movie.Id && _movie != null &&
                _movie.MovieCode == movie.MovieCode &&
                _movie.Rating == movie.Rating &&
                _movie.IsFavorite == movie.IsFavorite &&
                _movie.WatchProgress == movie.WatchProgress &&
                _movie.CoverImage == movie.CoverImage &&
                _movie.SourceType == movie.SourceType)
            {
                _movie = movie;
                return;
            }

            _movie = movie;
            _boundMovieId = movie.Id;
            StopHover();

            _preparedGalleryFrames = null;
            _galleryLoadId++;

            if (tags != null)
            {
                _tags = tags;
            }
            else
            {
                _tags = AppServices.TagRepo.GetTagsForMovie(_movie.Id);
            }

            lblTitle.Text = _movie.MovieCode;

            // Site-aware badge: nhận dạng trang web cụ thể cho phim Online
            if (_movie.SourceType == 0)
            {
                var (siteName, siteIcon) = Helpers.SiteAdapterRegistry.IdentifySite(_movie.MediaUrl);
                lblSource.Text = $"{siteIcon} {siteName}";
                lblSource.BackColor = UIHelper.AccentPrimary;
            }
            else
            {
                lblSource.Text = "📁 LOCAL";
                lblSource.BackColor = UIHelper.Success;
            }

            UpdateRatingStarsDisplay(_movie.Rating);
            lblFavorite.Text = _movie.IsFavorite ? "❤️" : "🤍";

            this.SuspendLayout();
            flpTags.SuspendLayout();
            
            int maxTags = Math.Min(_tags.Count, 4);
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

            for (int i = maxTags; i < flpTags.Controls.Count; i++)
            {
                flpTags.Controls[i].Visible = false;
            }
            
            flpTags.ResumeLayout(false);
            this.ResumeLayout(false);

            // Fast RAM check for cover image
            int targetW = picCover.Width > 0 ? picCover.Width : 348;
            int targetH = picCover.Height > 0 ? picCover.Height : 185;

            if (!string.IsNullOrEmpty(_movie.CoverImage))
            {
                string fullPath = FileHelper.GetFullPath(_movie.CoverImage);
                if (ImageCache.TryGetThumbnailFromMemory(fullPath, targetW, targetH, out var memImg) && memImg != null)
                {
                    picCover.Image = memImg;
                    _originalCover = memImg;
                }
                else
                {
                    picCover.Image = null;
                    _originalCover = null;
                    LoadCoverImageAsync(fullPath, targetW, targetH);
                }
            }
            else
            {
                picCover.Image = null;
                _originalCover = null;
            }

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

        private async void LoadCoverImageAsync(string? fullPath = null, int targetW = 0, int targetH = 0)
        {
            int loadId = ++_currentLoadId;
            if (string.IsNullOrEmpty(fullPath) && !string.IsNullOrEmpty(_movie?.CoverImage))
            {
                fullPath = FileHelper.GetFullPath(_movie.CoverImage);
            }
            if (targetW <= 0) targetW = picCover.Width > 0 ? picCover.Width : 348;
            if (targetH <= 0) targetH = picCover.Height > 0 ? picCover.Height : 185;

            if (!string.IsNullOrEmpty(fullPath) && System.IO.File.Exists(fullPath))
            {
                try
                {
                    var finalImg = await ImageCache.GetThumbnailAsync(fullPath, targetW, targetH);
                    if (finalImg != null && !this.IsDisposed && _currentLoadId == loadId)
                    {
                        picCover.Image = finalImg;
                        _originalCover = finalImg;
                    }
                }
                catch { }
            }
            else
            {
                if (_currentLoadId == loadId)
                {
                    picCover.Image = null;
                    _originalCover = null;
                }
            }
        }

        private void MainControl_Click(object sender, EventArgs e)
        {
            MovieClicked?.Invoke(this, _movie);
        }

        private void lblFavorite_Click(object sender, EventArgs e)
        {
            if (_movie == null) return;
            _movie.IsFavorite = !_movie.IsFavorite;
            lblFavorite.Text = _movie.IsFavorite ? "❤️" : "🤍";
            AppServices.MovieRepo.ToggleFavorite(_movie.Id);
            DataCache.Invalidate();
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

        private void UpdateRatingStarsDisplay(int rating)
        {
            string stars = "";
            for (int i = 1; i <= 5; i++)
            {
                stars += i <= rating ? "★" : "☆";
            }
            lblRating.Text = stars;
        }

        private int GetRatingFromX(int x)
        {
            int starWidth = Math.Max(1, lblRating.Width / 5);
            int r = (x / starWidth) + 1;
            return Math.Max(1, Math.Min(5, r));
        }

        private void lblRating_MouseClick(object? sender, MouseEventArgs e)
        {
            int clickedRating = GetRatingFromX(e.X);
            
            if (_movie.Rating == clickedRating && clickedRating == 1)
            {
                _movie.Rating = 0;
            }
            else
            {
                _movie.Rating = clickedRating;
            }

            AppServices.MovieRepo.Update(_movie);
            DataCache.Invalidate();
            UpdateRatingStarsDisplay(_movie.Rating);
        }

        private void lblRating_MouseMove(object? sender, MouseEventArgs e)
        {
            int hoverRating = GetRatingFromX(e.X);
            UpdateRatingStarsDisplay(hoverRating);
        }

        private void lblRating_MouseLeave(object? sender, EventArgs e)
        {
            UpdateRatingStarsDisplay(_movie.Rating);
        }

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
            using var frm = new Forms.FrmUpdateProgress(_movie.WatchProgress, _movie.MovieCode);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                int progress = frm.SelectedProgress;
                _movie.WatchProgress = progress;
                AppServices.MovieRepo.UpdateProgress(_movie.Id, progress);
                DataCache.Invalidate();
                
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
        }
    }
}
