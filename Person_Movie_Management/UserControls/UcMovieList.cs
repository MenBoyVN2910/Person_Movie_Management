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
        private readonly MovieRepository _movieRepo;
        private readonly AudioRepository _audioRepo;
        private readonly MovieService _movieService;
        private List<Movie> _allMovies = new();
        private List<Audio> _allAudios = new();

        public UcMovieList(MovieListMode mode)
        {
            InitializeComponent();
            _mode = mode;
            _movieRepo = new MovieRepository();
            _audioRepo = new AudioRepository();
            _movieService = new MovieService();

            this.BackColor = UIHelper.BgDark;
            pnlTop.BackColor = UIHelper.BgDark;
            flowLayoutPanel.BackColor = UIHelper.BgDark;

            // Style title
            lblTitle.ForeColor = UIHelper.TextPrimary;
            lblTitle.Font = UIHelper.FontH2;

            // Style search box
            txtSearch.FillColor = UIHelper.BgCard;
            txtSearch.ForeColor = UIHelper.TextPrimary;
            txtSearch.BorderRadius = 12;
            txtSearch.FocusedState.BorderColor = UIHelper.AccentPrimary;
            txtSearch.Font = new Font("Segoe UI", 10F);

            // Style action button
            btnAction.BorderRadius = 12;
            btnAction.FillColor = UIHelper.AccentPrimary;
            btnAction.FillColor2 = UIHelper.AccentTertiary;
            btnAction.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnAction.Animated = true;

            // Empty state label
            lblEmpty.ForeColor = UIHelper.TextMuted;
            lblEmpty.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
            lblEmpty.Visible = false;

            SetupUIForMode();
            LoadData();
        }

        private void SetupUIForMode()
        {
            if (_mode == MovieListMode.Online)
            {
                lblTitle.Text = "🌐  Phim Online";
                btnAction.Text = "＋  Thêm Phim";
                btnAction.Visible = true;
                btnExport.Visible = true;
                btnImport.Visible = true;
                lblEmpty.Text = "Chưa có phim Online nào.\nNhấn \"Thêm Phim\" để bắt đầu.";
            }
            else if (_mode == MovieListMode.Local)
            {
                lblTitle.Text = "📁  Phim Trên Máy";
                btnAction.Text = "📂  Quét Thư Mục";
                btnAction.Visible = true;
                btnExport.Visible = false;
                btnImport.Visible = false;
                lblEmpty.Text = "Chưa có phim Local nào.\nNhấn \"Quét Thư Mục\" để nhập phim từ máy tính.";
            }
            else
            {
                lblTitle.Text = "❤️  Phim Yêu Thích";
                btnAction.Visible = false;
                btnExport.Visible = false;
                btnImport.Visible = false;
                lblEmpty.Text = "Chưa có phim yêu thích nào.\nNhấn vào trái tim trên thẻ phim để thêm.";
            }
        }

        private void LoadData()
        {
            if (!SessionManager.IsLoggedIn) return;

            int userId = SessionManager.CurrentUser!.Id;

            if (_mode == MovieListMode.Online)
                _allMovies = _movieRepo.GetAllByUser(userId, 0);
            else if (_mode == MovieListMode.Local)
                _allMovies = _movieRepo.GetAllByUser(userId, 1);
            else if (_mode == MovieListMode.Favorites)
            {
                _allMovies = _movieRepo.GetFavorites(userId);
                _allAudios = _audioRepo.GetFavorites(userId);
            }

            DisplayData(_allMovies, _allAudios);
        }

        private void DisplayData(List<Movie> movies, List<Audio> audios = null)
        {
            flowLayoutPanel.Controls.Clear();
            lblEmpty.Visible = movies.Count == 0 && (audios == null || audios.Count == 0);

            foreach (var movie in movies)
            {
                var card = new UcMovieCard(movie);
                card.MovieClicked += Card_MovieClicked;
                card.FavoriteToggled += Card_FavoriteToggled;
                card.EditClicked += Card_EditClicked;
                card.DeleteClicked += Card_DeleteClicked;
                flowLayoutPanel.Controls.Add(card);
            }

            if (audios != null)
            {
                foreach (var audio in audios)
                {
                    var card = new UcAudioCard(audio);
                    card.AudioClicked += Card_AudioClicked;
                    card.FavoriteToggled += Card_AudioFavoriteToggled;
                    card.EditClicked += Card_AudioEditClicked;
                    card.DeleteClicked += Card_AudioDeleteClicked;
                    flowLayoutPanel.Controls.Add(card);
                }
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
                LoadData();
            }
        }

        private void Card_EditClicked(object? sender, Movie movie)
        {
            Forms.FrmMovieDetail frm = new Forms.FrmMovieDetail(movie);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void Card_DeleteClicked(object? sender, Movie movie)
        {
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa phim '{movie.MovieCode}' không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_movieRepo.Delete(movie.Id))
                {
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Xóa phim thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Card_AudioClicked(object? sender, Audio audio)
        {
            var fullAudio = _audioRepo.GetById(audio.Id, true);
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
                LoadData();
            }
        }

        private void Card_AudioEditClicked(object? sender, Audio audio)
        {
            Forms.FrmAudioDetail frm = new Forms.FrmAudioDetail(audio);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void Card_AudioDeleteClicked(object? sender, Audio audio)
        {
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa âm thanh '{audio.AudioCode}' không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_audioRepo.Delete(audio.Id))
                {
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Xóa âm thanh thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                DisplayData(_allMovies, _allAudios);
            }
            else
            {
                var filteredMovies = _allMovies.Where(m => 
                    m.MovieCode.ToLower().Contains(keyword) || 
                    (m.Note != null && m.Note.ToLower().Contains(keyword))
                ).ToList();

                var filteredAudios = _allAudios.Where(a => 
                    a.AudioCode.ToLower().Contains(keyword) || 
                    (a.Note != null && a.Note.ToLower().Contains(keyword))
                ).ToList();

                DisplayData(filteredMovies, filteredAudios);
            }
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
                        var newMovies = _movieService.AutoScanLocalFolder(SessionManager.CurrentUser!.Id, fbd.SelectedPath);
                        MessageBox.Show($"Đã quét và thêm {newMovies.Count} phim mới.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                }
            }
            else if (_mode == MovieListMode.Online)
            {
                Forms.FrmMovieDetail frm = new Forms.FrmMovieDetail();
                if (frm.ShowDialog() == DialogResult.OK) LoadData();
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
                            var existingMovie = _movieRepo.GetByCode(currentUserId, movie.MovieCode);
                            if (existingMovie == null)
                            {
                                // Reset IDs and link to current user
                                movie.Id = 0;
                                movie.UserId = currentUserId;
                                movie.CreatedAt = DateTime.Now;
                                movie.UpdatedAt = null;
                                
                                _movieRepo.Insert(movie);
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
                        LoadData();
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
