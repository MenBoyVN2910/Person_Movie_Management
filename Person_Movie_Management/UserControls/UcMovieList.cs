using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Person_Movie_Management.Services;
using Person_Movie_Management.Forms;

namespace Person_Movie_Management.UserControls
{
    public enum MovieListMode
    {
        Online = 0,
        Local = 1,
        Favorites = 2
    }

    public partial class UcMovieList : UserControl
    {
        private readonly MovieListMode _mode;
        private List<Movie> _allMovies = new();
        private List<Audio> _allAudios = new();
        private List<Movie> _filteredMovies = new();
        private List<Audio> _filteredAudios = new();
        private bool _isLoading = false; // Flag tránh LoadDataAsync chạy đồng thời

        private Guna.UI2.WinForms.Guna2ComboBox cmbSort;
        private Guna.UI2.WinForms.Guna2ComboBox cmbFilterTag;
        private Guna.UI2.WinForms.Guna2GradientButton btnBatchImport;
        private Guna.UI2.WinForms.Guna2GradientButton btnDeleteAll;
        private Guna.UI2.WinForms.Guna2GradientButton btnRandom;
        private Guna.UI2.WinForms.Guna2GradientButton btnToggleSearchFilter;
        private Guna.UI2.WinForms.Guna2GradientButton btnToggleWatcher;
        private Panel pnlSearchFilterPopup;
        private ToolTip _btnToolTip;

        public UcMovieList(MovieListMode mode)
        {
            InitializeComponent();
            _mode = mode;

            this.BackColor = UIHelper.BgDark;
            pnlTop.BackColor = UIHelper.BgDark;
            flowLayoutPanel.BackColor = UIHelper.BgDark;

            // Style title
            lblTitle.ForeColor = UIHelper.TextPrimary;
            lblTitle.Font = UIHelper.FontH2;

            pnlTop.Height = 80;

            // Style search box
            txtSearch.FillColor = UIHelper.BgCard;
            txtSearch.ForeColor = UIHelper.TextPrimary;
            txtSearch.BorderRadius = 12;
            txtSearch.FocusedState.BorderColor = UIHelper.AccentPrimary;
            txtSearch.Font = new Font("Segoe UI", 10F);
            
            // Adjust search box position and add sort combo box
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txtSearch.Size = new Size(300, 42); 
            
            cmbSort = new Guna.UI2.WinForms.Guna2ComboBox();
            cmbSort.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            cmbSort.Size = new Size(250, 42);
            cmbSort.BorderRadius = 12;
            cmbSort.FillColor = UIHelper.BgCard;
            cmbSort.ForeColor = UIHelper.TextPrimary;
            cmbSort.FocusedState.BorderColor = UIHelper.AccentPrimary;
            cmbSort.Font = new Font("Segoe UI", 10F);
            cmbSort.Items.AddRange(new string[] { "Ngày thêm (Mới nhất)", "Ngày thêm (Cũ nhất)", "Rating (Cao -> Thấp)", "Tên (A-Z)" });
            cmbSort.SelectedIndex = 0;
            cmbSort.SelectedIndexChanged += (s, e) => { ApplySearchAndSort(true); };
            pnlTop.Controls.Add(cmbSort);

            // Phase 2: Smart Filter by Tag
            cmbFilterTag = new Guna.UI2.WinForms.Guna2ComboBox();
            cmbFilterTag.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            cmbFilterTag.Size = new Size(180, 42);
            cmbFilterTag.BorderRadius = 12;
            cmbFilterTag.FillColor = UIHelper.BgCard;
            cmbFilterTag.ForeColor = UIHelper.TextPrimary;
            cmbFilterTag.FocusedState.BorderColor = UIHelper.AccentPrimary;
            cmbFilterTag.Font = new Font("Segoe UI", 10F);
            cmbFilterTag.Items.Add("Tất cả Thể loại");
            cmbFilterTag.SelectedIndex = 0;
            cmbFilterTag.SelectedIndexChanged += CmbFilterTag_SelectedIndexChanged;
            pnlTop.Controls.Add(cmbFilterTag);

            pnlSearchFilterPopup = new Panel();
            pnlSearchFilterPopup.BackColor = UIHelper.BgDark;
            pnlSearchFilterPopup.Size = new Size(320, 165);
            pnlSearchFilterPopup.Visible = false;
            pnlSearchFilterPopup.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pnlSearchFilterPopup);

            btnToggleSearchFilter = new Guna.UI2.WinForms.Guna2GradientButton();
            btnToggleSearchFilter.Text = "🔍";
            btnToggleSearchFilter.Size = new Size(42, 42);
            btnToggleSearchFilter.BorderRadius = 12;
            btnToggleSearchFilter.FillColor = UIHelper.BgCard;
            btnToggleSearchFilter.FillColor2 = UIHelper.BgCard;
            btnToggleSearchFilter.ForeColor = UIHelper.TextPrimary;
            btnToggleSearchFilter.Font = new Font("Segoe UI", 16F);
            btnToggleSearchFilter.Visible = false;
            btnToggleSearchFilter.Click += (s, e) => {
                pnlSearchFilterPopup.Visible = !pnlSearchFilterPopup.Visible;
                if (pnlSearchFilterPopup.Visible) {
                    pnlSearchFilterPopup.Location = new Point(btnToggleSearchFilter.Left, pnlTop.Bottom);
                    pnlSearchFilterPopup.BringToFront();
                }
            };
            pnlTop.Controls.Add(btnToggleSearchFilter);

            // Center them initially
            CenterFilterControls();
            
            pnlTop.Resize += (s, e) => {
                CenterFilterControls();
            };

            // Style action button
            btnAction.Size = new Size(150, 42);
            btnAction.BorderRadius = 12;
            btnAction.FillColor = UIHelper.AccentPrimary;
            btnAction.FillColor2 = UIHelper.AccentTertiary;
            btnAction.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnAction.Animated = true;

            // Phase 1: Batch Import button
            btnBatchImport = new Guna.UI2.WinForms.Guna2GradientButton();
            btnBatchImport.Text = "⚡";
            btnBatchImport.Size = new Size(50, 42);
            btnBatchImport.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnBatchImport.BorderRadius = 12;
            btnBatchImport.FillColor = Color.FromArgb(139, 92, 246); // Purple
            btnBatchImport.FillColor2 = Color.FromArgb(167, 139, 250);
            btnBatchImport.Font = new Font("Segoe UI", 16F);
            btnBatchImport.ForeColor = Color.White;
            btnBatchImport.Click += BtnBatchImport_Click;
            if (_mode == MovieListMode.Favorites) btnBatchImport.Visible = false;
            pnlTop.Controls.Add(btnBatchImport);
            
            btnDeleteAll = new Guna.UI2.WinForms.Guna2GradientButton();
            btnDeleteAll.Text = "🗑";
            btnDeleteAll.Size = new Size(50, 42);
            btnDeleteAll.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnDeleteAll.BorderRadius = 12;
            btnDeleteAll.FillColor = Color.FromArgb(239, 68, 68); // Red-500
            btnDeleteAll.FillColor2 = Color.FromArgb(220, 38, 38); // Red-600
            btnDeleteAll.Font = new Font("Segoe UI", 16F);
            btnDeleteAll.ForeColor = Color.White;
            btnDeleteAll.Visible = _mode != MovieListMode.Favorites;
            btnDeleteAll.Click += BtnDeleteAll_Click;
            pnlTop.Controls.Add(btnDeleteAll);
            
            btnRandom = new Guna.UI2.WinForms.Guna2GradientButton();
            btnRandom.Text = "🎲";
            btnRandom.Size = new Size(50, 42);
            btnRandom.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnRandom.BorderRadius = 12;
            btnRandom.FillColor = Color.FromArgb(245, 158, 11); // Amber-500
            btnRandom.FillColor2 = Color.FromArgb(217, 119, 6); // Amber-600
            btnRandom.Font = new Font("Segoe UI", 16F);
            btnRandom.ForeColor = Color.White;
            btnRandom.Visible = _mode != MovieListMode.Favorites;
            btnRandom.Click += BtnRandom_Click;
            pnlTop.Controls.Add(btnRandom);

            // Folder Watcher toggle button for Local mode
            btnToggleWatcher = new Guna.UI2.WinForms.Guna2GradientButton();
            btnToggleWatcher.Size = new Size(205, 42);
            btnToggleWatcher.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnToggleWatcher.BorderRadius = 12;
            btnToggleWatcher.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnToggleWatcher.Animated = true;
            btnToggleWatcher.Cursor = Cursors.Hand;
            btnToggleWatcher.Visible = false;
            btnToggleWatcher.Click += BtnToggleWatcher_Click;
            pnlTop.Controls.Add(btnToggleWatcher);
            
            // Adjust buttons to NOT rely on designer anchors which might cause conflict
            btnAction.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnImport.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnImport.Size = new Size(50, 42);
            btnImport.Text = "📥";
            btnImport.Font = new Font("Segoe UI", 16F);
            btnExport.Size = new Size(50, 42);
            btnExport.Text = "📤";
            btnExport.Font = new Font("Segoe UI", 16F);

            // Add tooltips
            _btnToolTip = new ToolTip();
            _btnToolTip.SetToolTip(btnBatchImport, "Thêm Nhiều Phim Nhanh");
            _btnToolTip.SetToolTip(btnDeleteAll, "Xóa Tất Cả Phim");
            _btnToolTip.SetToolTip(btnRandom, "Chọn Ngẫu Nhiên 5 Phim");
            _btnToolTip.SetToolTip(btnImport, "Nhập Dữ Liệu Từ File");
            _btnToolTip.SetToolTip(btnExport, "Xuất Dữ Liệu Ra File");
            _btnToolTip.SetToolTip(btnAction, "Thêm Phim Mới");
            
            pnlTop.Resize += (s, e) => {
                LayoutButtons();
            };

            // Empty state label
            lblEmpty.ForeColor = UIHelper.TextMuted;
            lblEmpty.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            lblEmpty.Visible = false;

            SetupUIForMode();
            this.Load += async (s, e) => { await LoadDataAsync(); };
            
            DataCache.DataInvalidated += DataCache_DataInvalidated;
            this.Disposed += (s, e) => {
                DataCache.DataInvalidated -= DataCache_DataInvalidated;
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
            };
        }

        private void DataCache_DataInvalidated()
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.Invoke((MethodInvoker)delegate {
                    _ = LoadDataAsync();
                });
            }
        }

        private void LayoutButtons()
        {
            int currentX = pnlTop.Width - 25;

            if (btnAction != null && btnAction.Visible)
            {
                currentX -= btnAction.Width;
                btnAction.Location = new Point(currentX, 18);
                currentX -= 10;
            }
            if (btnBatchImport != null && btnBatchImport.Visible)
            {
                currentX -= btnBatchImport.Width;
                btnBatchImport.Location = new Point(currentX, 18);
                currentX -= 10;
            }
            if (btnImport != null && btnImport.Visible)
            {
                currentX -= btnImport.Width;
                btnImport.Location = new Point(currentX, 18);
                currentX -= 10;
            }
            if (btnExport != null && btnExport.Visible)
            {
                currentX -= btnExport.Width;
                btnExport.Location = new Point(currentX, 18);
                currentX -= 10;
            }
            if (btnDeleteAll != null && btnDeleteAll.Visible)
            {
                currentX -= btnDeleteAll.Width;
                btnDeleteAll.Location = new Point(currentX, 18);
                currentX -= 10;
            }
            if (btnRandom != null && btnRandom.Visible)
            {
                currentX -= btnRandom.Width;
                btnRandom.Location = new Point(currentX, 18);
                currentX -= 10;
            }
            if (btnToggleWatcher != null && btnToggleWatcher.Visible)
            {
                currentX -= btnToggleWatcher.Width;
                btnToggleWatcher.Location = new Point(currentX, 18);
            }
            
            // Re-center filters after button layout changes
            CenterFilterControls();
        }

        private void CenterFilterControls()
        {
            if (cmbSort == null || txtSearch == null || cmbFilterTag == null) return;
            
            int titleRight = lblTitle.Right + 30;
            
            // Determine leftmost button X by checking visibility from left to right
            int leftmostButtonX = pnlTop.Width;
            if (btnToggleWatcher != null && btnToggleWatcher.Visible) leftmostButtonX = btnToggleWatcher.Left;
            else if (btnRandom != null && btnRandom.Visible) leftmostButtonX = btnRandom.Left;
            else if (btnDeleteAll != null && btnDeleteAll.Visible) leftmostButtonX = btnDeleteAll.Left;
            else if (btnExport != null && btnExport.Visible) leftmostButtonX = btnExport.Left;
            else if (btnImport != null && btnImport.Visible) leftmostButtonX = btnImport.Left;
            else if (btnBatchImport != null && btnBatchImport.Visible) leftmostButtonX = btnBatchImport.Left;
            else if (btnAction != null && btnAction.Visible) leftmostButtonX = btnAction.Left;
            
            int availableSpace = leftmostButtonX - titleRight - 20; // 20px padding
            if (availableSpace < 100) availableSpace = 100; // prevent crashing

            if (btnToggleSearchFilter == null || pnlSearchFilterPopup == null) return;

            int defaultWidth = 300 + 10 + 180 + 10 + 250;
            
            if (availableSpace < defaultWidth)
            {
                // Collapsed mode
                btnToggleSearchFilter.Visible = true;
                btnToggleSearchFilter.Location = new Point(titleRight, 18);
                
                if (pnlSearchFilterPopup.Visible)
                {
                    pnlSearchFilterPopup.Location = new Point(btnToggleSearchFilter.Left, pnlTop.Bottom);
                }

                if (txtSearch.Parent != pnlSearchFilterPopup)
                {
                    txtSearch.Parent = pnlSearchFilterPopup;
                    cmbFilterTag.Parent = pnlSearchFilterPopup;
                    cmbSort.Parent = pnlSearchFilterPopup;
                    
                    txtSearch.Width = 300;
                    cmbFilterTag.Width = 300;
                    cmbSort.Width = 300;
                    
                    txtSearch.Location = new Point(10, 10);
                    cmbFilterTag.Location = new Point(10, 60);
                    cmbSort.Location = new Point(10, 110);
                }
            }
            else
            {
                // Expanded mode
                btnToggleSearchFilter.Visible = false;
                pnlSearchFilterPopup.Visible = false;
                
                if (txtSearch.Parent != pnlTop)
                {
                    txtSearch.Parent = pnlTop;
                    cmbFilterTag.Parent = pnlTop;
                    cmbSort.Parent = pnlTop;
                }

                txtSearch.Width = 300;
                cmbFilterTag.Width = 180;
                cmbSort.Width = 250;
                int totalWidth = txtSearch.Width + 10 + cmbFilterTag.Width + 10 + cmbSort.Width;
                int startX = titleRight + (availableSpace - totalWidth) / 2;
                int rowY = 18;
                txtSearch.Location = new Point(startX, rowY);
                cmbFilterTag.Location = new Point(txtSearch.Right + 10, rowY);
                cmbSort.Location = new Point(cmbFilterTag.Right + 10, rowY);
            }
        }

        private void SetupUIForMode()
        {
            if (_mode == MovieListMode.Online)
            {
                lblTitle.Text = "🌐  Phim Online";
                btnAction.Text = "＋ Thêm";
                btnAction.Size = new Size(130, 42);
                btnAction.Visible = true;
                btnExport.Visible = true;
                btnImport.Visible = true;
                btnBatchImport.Visible = true;
                if (btnToggleWatcher != null) btnToggleWatcher.Visible = false;
                lblEmpty.Text = "Chưa có phim Online nào.\nNhấn \"Thêm\" để bắt đầu.";
            }
            else if (_mode == MovieListMode.Local)
            {
                lblTitle.Text = "📁  Phim Trên Máy";
                btnAction.Text = "📂  Quét Thư Mục";
                btnAction.Size = new Size(180, 42);
                btnAction.Visible = true;
                btnExport.Visible = false;
                btnImport.Visible = false;
                btnBatchImport.Visible = false;
                if (btnToggleWatcher != null)
                {
                    btnToggleWatcher.Visible = true;
                    UpdateWatcherButtonUI();
                }
                lblEmpty.Text = "Chưa có phim Local nào.\nNhấn \"Quét Thư Mục\" để nhập phim từ máy tính.";
            }
            else
            {
                lblTitle.Text = "❤️  Phim Yêu Thích";
                btnAction.Visible = false;
                btnExport.Visible = false;
                btnImport.Visible = false;
                btnBatchImport.Visible = false;
                if (btnToggleWatcher != null) btnToggleWatcher.Visible = false;
                if (btnDeleteAll != null) btnDeleteAll.Visible = false;
                lblEmpty.Text = "Chưa có phim yêu thích nào.\nNhấn vào trái tim trên thẻ phim để thêm.";
            }

            LayoutButtons();
        }

        private void BtnToggleWatcher_Click(object? sender, EventArgs e)
        {
            bool newState = !SessionManager.IsFolderWatcherEnabled;
            SessionManager.IsFolderWatcherEnabled = newState;
            UpdateWatcherButtonUI();

            var frmMain = this.FindForm() as Forms.FrmMain;
            if (frmMain != null)
            {
                frmMain.ToggleFolderWatcher(newState);
            }

            if (newState)
            {
                FrmToastNotification.ShowNotification("🔔 Folder Watcher", "Đã BẬT tự động theo dõi thư mục Videos.");
            }
            else
            {
                FrmToastNotification.ShowNotification("🔕 Folder Watcher", "Đã TẮT theo dõi thư mục Videos (tránh lưu video nhạy cảm).");
            }
        }

        private void UpdateWatcherButtonUI()
        {
            if (btnToggleWatcher == null) return;
            bool isEnabled = SessionManager.IsFolderWatcherEnabled;
            if (isEnabled)
            {
                btnToggleWatcher.Text = "🔔 Quét Videos: BẬT";
                btnToggleWatcher.FillColor = Color.FromArgb(16, 185, 129); // Emerald-500
                btnToggleWatcher.FillColor2 = Color.FromArgb(5, 150, 105); // Emerald-600
                btnToggleWatcher.ForeColor = Color.White;
                _btnToolTip?.SetToolTip(btnToggleWatcher, "Đang TỰ ĐỘNG theo dõi thư mục Videos.\nNhấp để TẮT (tránh lưu các video nhạy cảm).");
            }
            else
            {
                btnToggleWatcher.Text = "🔕 Quét Videos: TẮT";
                btnToggleWatcher.FillColor = Color.FromArgb(71, 85, 105); // Slate-600
                btnToggleWatcher.FillColor2 = Color.FromArgb(51, 65, 85); // Slate-700
                btnToggleWatcher.ForeColor = Color.FromArgb(203, 213, 225);
                _btnToolTip?.SetToolTip(btnToggleWatcher, "Đang TẮT theo dõi thư mục Videos.\nNhấp để BẬT tính năng tự động quét.");
            }
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            if (!SessionManager.IsLoggedIn) return;
            if (_isLoading) return; // Tránh chạy đồng thời nhiều lần
            _isLoading = true;

            try
            {
            int userId = SessionManager.CurrentUser!.Id;

            if (_mode == MovieListMode.Favorites)
            {
                _allMovies = await DataCache.GetFavoriteMoviesAsync(userId);
                _allAudios = await DataCache.GetFavoriteAudiosAsync(userId);
            }
            else
            {
                var allCached = await DataCache.GetMoviesAsync(userId);
                int type = _mode == MovieListMode.Online ? 0 : 1;
                _allMovies = allCached.Where(m => m.SourceType == type).ToList();
                _allAudios = new List<Audio>();
            }

            // Dynamically update cmbFilterTag based on the loaded movies
            string? currentSelectedTag = cmbFilterTag.SelectedItem?.ToString();
            
            cmbFilterTag.SelectedIndexChanged -= CmbFilterTag_SelectedIndexChanged;
            cmbFilterTag.Items.Clear();
            cmbFilterTag.Items.Add("Tất cả Thể loại");

            if (_allMovies != null && _allMovies.Count > 0)
            {
                var movieIds = _allMovies.Select(m => m.Id).ToList();
                var tagsDict = await AppServices.TagRepo.GetTagsForMoviesAsync(movieIds);
                var uniqueTags = tagsDict.Values.SelectMany(t => t).Select(t => t.TagName).Distinct().OrderBy(t => t).ToList();
                foreach (var tag in uniqueTags)
                {
                    cmbFilterTag.Items.Add(tag);
                }
            }

            if (currentSelectedTag != null && cmbFilterTag.Items.Contains(currentSelectedTag))
            {
                cmbFilterTag.SelectedItem = currentSelectedTag;
            }
            else
            {
                cmbFilterTag.SelectedIndex = 0;
            }

            cmbFilterTag.SelectedIndexChanged += CmbFilterTag_SelectedIndexChanged;

            ApplySearchAndSort(false);
            } // end try
            finally
            {
                _isLoading = false;
            }
        }

        private void CmbFilterTag_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ApplySearchAndSort(true);
        }

        private async System.Threading.Tasks.Task DisplayDataAsync(bool resetScroll = false)
        {
            var combinedItems = new List<object>();
            combinedItems.AddRange(_filteredMovies);
            combinedItems.AddRange(_filteredAudios);

            lblEmpty.Visible = combinedItems.Count == 0;

            if (combinedItems.Count > 0)
            {
                var movieIds = _filteredMovies.Select(m => m.Id).ToList();
                var tagsDict = await AppServices.TagRepo.GetTagsForMoviesAsync(movieIds);
                
                // Wire up events once via panel
                flowLayoutPanel.MovieClicked -= Card_MovieClicked;
                flowLayoutPanel.MovieFavoriteToggled -= Card_FavoriteToggled;
                flowLayoutPanel.MovieEditClicked -= Card_EditClicked;
                flowLayoutPanel.MovieDeleteClicked -= Card_DeleteClicked;
                
                flowLayoutPanel.AudioClicked -= Card_AudioClicked;
                flowLayoutPanel.AudioFavoriteToggled -= Card_AudioFavoriteToggled;
                flowLayoutPanel.AudioEditClicked -= Card_AudioEditClicked;
                flowLayoutPanel.AudioDeleteClicked -= Card_AudioDeleteClicked;

                flowLayoutPanel.MovieClicked += Card_MovieClicked;
                flowLayoutPanel.MovieFavoriteToggled += Card_FavoriteToggled;
                flowLayoutPanel.MovieEditClicked += Card_EditClicked;
                flowLayoutPanel.MovieDeleteClicked += Card_DeleteClicked;
                
                flowLayoutPanel.AudioClicked += Card_AudioClicked;
                flowLayoutPanel.AudioFavoriteToggled += Card_AudioFavoriteToggled;
                flowLayoutPanel.AudioEditClicked += Card_AudioEditClicked;
                flowLayoutPanel.AudioDeleteClicked += Card_AudioDeleteClicked;

                flowLayoutPanel.SetData(combinedItems, tagsDict, resetScroll);
            }
            else
            {
                flowLayoutPanel.SetData(new List<object>(), new Dictionary<int, List<Tag>>(), resetScroll);
            }
        }

        private void Card_MovieClicked(object? sender, Movie movie)
        {
            if (movie.SourceType == 0)
            {
                if (!string.IsNullOrEmpty(movie.MediaUrl))
                    MediaLauncher.LaunchMedia(movie.MediaUrl, 0);
            }
            else
            {
                if (!string.IsNullOrEmpty(movie.MediaUrl))
                    MediaLauncher.LaunchMedia(movie.MediaUrl, 1);
            }
        }

        private void Card_FavoriteToggled(object? sender, Movie movie)
        {
            if (_mode == MovieListMode.Favorites && !movie.IsFavorite)
            {
                _ = LoadDataAsync();
            }
        }

        private void Card_EditClicked(object? sender, Movie movie)
        {
            Forms.FrmMovieDetail frm = new Forms.FrmMovieDetail(movie);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                _ = LoadDataAsync();
            }
        }

        private void Card_DeleteClicked(object? sender, Movie movie)
        {
            int movieId = movie.Id; // Capture ID trước để tránh closure bug
            string movieCode = movie.MovieCode;
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa phim '{movieCode}' không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (AppServices.MovieRepo.Delete(movieId))
                {
                    // DataCache.Invalidate() sẽ tự trigger DataCache_DataInvalidated → LoadDataAsync
                    // Không cần gọi LoadDataAsync thêm lần nữa để tránh race condition
                    DataCache.Invalidate();
                }
                else
                {
                    MessageBox.Show("Xóa phim thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Card_AudioClicked(object? sender, Audio audio)
        {
            var fullAudio = AppServices.AudioRepo.GetById(audio.Id, true);
            if (fullAudio != null && fullAudio.AudioData != null && fullAudio.AudioData.Length > 0)
            {
                try
                {
                    string tempFile = Path.Combine(Path.GetTempPath(), $"temp_audio_{Guid.NewGuid()}.mp3");
                    System.IO.File.WriteAllBytes(tempFile, fullAudio.AudioData);
                    MediaLauncher.LaunchMedia(tempFile, 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể phát âm thanh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy dữ liệu âm thanh.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Card_AudioFavoriteToggled(object? sender, Audio audio)
        {
            if (_mode == MovieListMode.Favorites && !audio.IsFavorite)
            {
                _ = LoadDataAsync();
            }
        }

        private void Card_AudioEditClicked(object? sender, Audio audio)
        {
            Forms.FrmAudioDetail frm = new Forms.FrmAudioDetail(audio);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                _ = LoadDataAsync();
            }
        }

        private void Card_AudioDeleteClicked(object? sender, Audio audio)
        {
            int audioId = audio.Id; // Capture ID trước để tránh closure bug
            string audioCode = audio.AudioCode;
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa âm thanh '{audioCode}' không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (AppServices.AudioRepo.Delete(audioId))
                {
                    // DataCache.Invalidate() sẽ tự trigger DataCache_DataInvalidated → LoadDataAsync
                    DataCache.Invalidate();
                }
                else
                {
                    MessageBox.Show("Xóa âm thanh thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private System.Windows.Forms.Timer? _searchDebounceTimer;

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (_searchDebounceTimer == null)
            {
                _searchDebounceTimer = new System.Windows.Forms.Timer();
                _searchDebounceTimer.Interval = 300; // 300ms delay
                _searchDebounceTimer.Tick += (s, ev) =>
                {
                    _searchDebounceTimer.Stop();
                    ApplySearchAndSort(true);
                };
            }
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private void ApplySearchAndSort(bool resetScroll = false)
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            string? selectedTag = cmbFilterTag.SelectedIndex > 0 ? cmbFilterTag.SelectedItem?.ToString() : null;

            var filteredMovies = _allMovies.AsEnumerable();
            var filteredAudios = _allAudios.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                filteredMovies = filteredMovies.Where(m => 
                    m.MovieCode.ToLower().Contains(keyword) || 
                    (m.Note != null && m.Note.ToLower().Contains(keyword)));

                filteredAudios = filteredAudios.Where(a => 
                    a.AudioCode.ToLower().Contains(keyword) || 
                    (a.Note != null && a.Note.ToLower().Contains(keyword)));
            }

            // Apply Tag Filter
            if (selectedTag != null)
            {
                filteredMovies = filteredMovies.Where(m => 
                {
                    var movieTags = AppServices.TagRepo.GetTagsForMovie(m.Id);
                    return movieTags.Any(t => t.TagName == selectedTag);
                });
                filteredAudios = Enumerable.Empty<Audio>();
            }

            int sortMode = cmbSort?.SelectedIndex ?? 0;
            switch (sortMode)
            {
                case 0: // Ngày thêm (Mới nhất)
                    filteredMovies = filteredMovies.OrderByDescending(m => m.CreatedAt);
                    filteredAudios = filteredAudios.OrderByDescending(a => a.CreatedAt);
                    break;
                case 1: // Ngày thêm (Cũ nhất)
                    filteredMovies = filteredMovies.OrderBy(m => m.CreatedAt);
                    filteredAudios = filteredAudios.OrderBy(a => a.CreatedAt);
                    break;
                case 2: // Rating (Cao -> Thấp)
                    filteredMovies = filteredMovies.OrderByDescending(m => m.Rating);
                    filteredAudios = filteredAudios.OrderByDescending(a => a.Rating);
                    break;
                case 3: // Tên (A-Z)
                    filteredMovies = filteredMovies.OrderBy(m => m.MovieCode);
                    filteredAudios = filteredAudios.OrderBy(a => a.AudioCode);
                    break;
            }

            _filteredMovies = filteredMovies.ToList();
            _filteredAudios = filteredAudios.ToList();
            
            _ = DisplayDataAsync(resetScroll);
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            if (_mode == MovieListMode.Local)
            {
                using var fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    if (SessionManager.IsLoggedIn)
                    {
                        var newMovies = AppServices.MovieSvc.AutoScanLocalFolder(SessionManager.CurrentUser!.Id, fbd.SelectedPath);
                        
                        // Register scanned folder into real-time FolderWatcher
                        var frmMain = this.FindForm() as Forms.FrmMain;
                        if (frmMain != null)
                        {
                            frmMain.AddWatchFolder(fbd.SelectedPath);
                        }

                        MessageBox.Show($"Đã quét và thêm {newMovies.Count} phim mới.\n✦ Thư mục này hiện đã được đưa vào danh sách TỰ ĐỘNG THEO DÕI thời gian thực!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DataCache.Invalidate();
                        _ = LoadDataAsync();
                    }
                }
            }
            else if (_mode == MovieListMode.Online)
            {
                Forms.FrmMovieDetail frm = new Forms.FrmMovieDetail();
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    DataCache.Invalidate();
                    _ = LoadDataAsync();
                }
            }
        }

        private void BtnRandom_Click(object? sender, EventArgs e)
        {
            if (_allMovies == null || _allMovies.Count == 0)
            {
                MessageBox.Show("Không có phim nào để chọn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var rnd = new Random();
            int countToTake = Math.Min(5, _allMovies.Count);
            _filteredMovies = _allMovies.OrderBy(x => rnd.Next()).Take(countToTake).ToList();
            _filteredAudios = new List<Audio>(); // Xóa audio để không hiển thị lẫn
            _ = DisplayDataAsync(false);

            if (_allMovies.Count < 5)
            {
                Forms.FrmToastNotification.ShowNotification("🎲 Trộn Ngẫu Nhiên", $"Kho hiện có {_allMovies.Count} phim. Đã chọn ngẫu nhiên tất cả {_allMovies.Count} phim.", "", null);
            }
            else
            {
                Forms.FrmToastNotification.ShowNotification("🎲 Trộn Ngẫu Nhiên", $"Đã chọn ngẫu nhiên 5 phim trong tổng số {_allMovies.Count} phim.", "", null);
            }
        }

        private void BtnBatchImport_Click(object? sender, EventArgs e)
        {
            var frm = new Forms.FrmBatchImport();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                DataCache.Invalidate();
                _ = LoadDataAsync();
            }
        }

        private void BtnDeleteAll_Click(object? sender, EventArgs e)
        {
            if (_filteredMovies.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var inputDialog = new FrmInputBox("Xác nhận xóa", "Nhập 'delete' để xóa TẤT CẢ mục trên trang này:", showHardDelete: true);
            if (inputDialog.ShowDialog() == DialogResult.OK)
            {
                if (inputDialog.InputValue.Trim().ToLower() == "delete")
                {
                    if (SessionManager.IsLoggedIn)
                    {
                        int userId = SessionManager.CurrentUser!.Id;
                        int? sourceType = (int)_mode;

                        if (inputDialog.IsHardDelete)
                        {
                            AppServices.MovieRepo.HardDeleteAll(userId, sourceType);
                            MessageBox.Show("Đã xóa vĩnh viễn tất cả các mục thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            AppServices.MovieRepo.DeleteAll(userId, sourceType);
                            MessageBox.Show("Đã xóa tất cả thành công. Các mục này đã được đưa vào Thùng Rác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        DataCache.Invalidate();
                        _ = LoadDataAsync();
                    }
                }
                else
                {
                    MessageBox.Show("Xác nhận không hợp lệ. Hủy thao tác xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (_allMovies == null || _allMovies.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "Backup files (*.zip)|*.zip|JSON files (*.json)|*.json";
            sfd.FileName = "PhimOnline_Export.zip";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string jsonString = JsonSerializer.Serialize(_allMovies, options);

                    if (sfd.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                        Directory.CreateDirectory(tempDir);

                        File.WriteAllText(Path.Combine(tempDir, "data.json"), jsonString);

                        foreach (var movie in _allMovies)
                        {
                            if (!string.IsNullOrEmpty(movie.CoverImage))
                            {
                                string imgPath = FileHelper.GetFullPath(movie.CoverImage);
                                if (File.Exists(imgPath))
                                {
                                    string destImg = Path.Combine(tempDir, movie.CoverImage);
                                    Directory.CreateDirectory(Path.GetDirectoryName(destImg)!);
                                    File.Copy(imgPath, destImg, true);
                                }
                            }
                        }

                        if (File.Exists(sfd.FileName)) File.Delete(sfd.FileName);
                        System.IO.Compression.ZipFile.CreateFromDirectory(tempDir, sfd.FileName);
                        Directory.Delete(tempDir, true);
                    }
                    else
                    {
                        File.WriteAllText(sfd.FileName, jsonString);
                    }
                    
                    MessageBox.Show("Xuất dữ liệu thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra khi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsLoggedIn) return;

            using var ofd = new OpenFileDialog();
            ofd.Filter = "Backup files (*.zip)|*.zip|JSON files (*.json)|*.json";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string jsonString = "";
                    string? tempDir = null;

                    if (ofd.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                        System.IO.Compression.ZipFile.ExtractToDirectory(ofd.FileName, tempDir);

                        string jsonFile = Path.Combine(tempDir, "data.json");
                        if (File.Exists(jsonFile))
                        {
                            jsonString = File.ReadAllText(jsonFile);
                        }
                        else
                        {
                            throw new Exception("Không tìm thấy file data.json trong bản sao lưu.");
                        }
                    }
                    else
                    {
                        jsonString = File.ReadAllText(ofd.FileName);
                    }

                    var importedMovies = JsonSerializer.Deserialize<List<Movie>>(jsonString);

                    if (importedMovies != null && importedMovies.Count > 0)
                    {
                        int currentUserId = SessionManager.CurrentUser!.Id;
                        int importedCount = 0;

                        foreach (var movie in importedMovies)
                        {
                            // Avoid duplicates by MovieCode
                            var existingMovie = AppServices.MovieRepo.GetByCode(currentUserId, movie.MovieCode);
                            if (existingMovie == null)
                            {
                                // Reset IDs and link to current user
                                movie.Id = 0;
                                movie.UserId = currentUserId;
                                movie.CreatedAt = DateTime.Now;
                                movie.UpdatedAt = null;
                                
                                AppServices.MovieRepo.Insert(movie);
                                importedCount++;
                            }
                        }

                        // Copy images if from zip
                        if (tempDir != null)
                        {
                            string sourceImagesDir = Path.Combine(tempDir, "App_Data", "CoverImages");
                            if (Directory.Exists(sourceImagesDir))
                            {
                                string destImagesDir = FileHelper.GetFullPath("App_Data\\CoverImages");
                                if (!Directory.Exists(destImagesDir)) Directory.CreateDirectory(destImagesDir);
                                
                                foreach (string file in Directory.GetFiles(sourceImagesDir))
                                {
                                    string destFile = Path.Combine(destImagesDir, Path.GetFileName(file));
                                    File.Copy(file, destFile, true);
                                }
                            }
                            Directory.Delete(tempDir, true);
                        }

                        MessageBox.Show($"Nhập dữ liệu thành công! Đã thêm {importedCount} phim mới.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DataCache.Invalidate();
                        _ = LoadDataAsync();
                    }
                    else
                    {
                        MessageBox.Show("File không chứa dữ liệu hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (tempDir != null && Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra khi nhập file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
