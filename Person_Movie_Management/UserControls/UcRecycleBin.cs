using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Guna.UI2.WinForms;
using System.Linq;
using Person_Movie_Management.Data;

namespace Person_Movie_Management.UserControls
{
    public partial class UcRecycleBin : UserControl
    {
        private MovieRepository _movieRepo;
        private AudioRepository _audioRepo;
        private int _currentUserId;

        public UcRecycleBin(int userId)
        {
            InitializeComponent();
            _currentUserId = userId;
            _movieRepo = new MovieRepository();
            _audioRepo = new AudioRepository();
            
            this.BackColor = UIHelper.BgDark;
            flpMovies.BackColor = UIHelper.BgDark;
            pnlTop.BackColor = UIHelper.BgDark;
            lblTitle.ForeColor = UIHelper.TextPrimary;
            txtSearch.FillColor = UIHelper.BgPanel;
            txtSearch.ForeColor = UIHelper.TextPrimary;
            cmbSort.FillColor = UIHelper.BgPanel;
            cmbSort.FillColor = UIHelper.BgPanel;
            cmbSort.ForeColor = UIHelper.TextPrimary;
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

        private void UcRecycleBin_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void pnlTop_Resize(object sender, EventArgs e)
        {
            int searchWidth = 300;
            int sortWidth = 200;
            int btnWidth = 120;
            int spacing = 20;
            
            int totalWidth = searchWidth + sortWidth + btnWidth + spacing * 2;
            int startX = (pnlTop.Width - totalWidth) / 2;

            txtSearch.Width = searchWidth;
            cmbSort.Width = sortWidth;
            btnEmptyTrash.Width = btnWidth;

            txtSearch.Location = new Point(startX, (pnlTop.Height - txtSearch.Height) / 2);
            cmbSort.Location = new Point(startX + searchWidth + spacing, (pnlTop.Height - cmbSort.Height) / 2);
            btnEmptyTrash.Location = new Point(startX + searchWidth + sortWidth + spacing * 2, (pnlTop.Height - btnEmptyTrash.Height) / 2);
        }

        public void LoadData(string keyword = "")
        {
            flpMovies.Controls.Clear();
            
            bool isAudio = cmbSort.SelectedIndex == 1;
            
            if (isAudio)
            {
                var audios = _audioRepo.GetDeleted(_currentUserId);
                if (!string.IsNullOrEmpty(keyword))
                {
                    audios = audios.Where(a => a.AudioCode.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
                                              (a.Note != null && a.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToList();
                }
                
                foreach (var audio in audios)
                {
                    var cleanCode = audio.AudioCode.Split(new[] { "_$DEL$_" }, StringSplitOptions.None)[0];
                    var card = CreateItemCard(audio.Id, cleanCode, audio.CoverImage, true);
                    flpMovies.Controls.Add(card);
                }
            }
            else
            {
                var movies = _movieRepo.GetDeleted(_currentUserId);
                if (!string.IsNullOrEmpty(keyword))
                {
                    movies = movies.Where(m => m.MovieCode.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
                                              (m.Note != null && m.Note.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToList();
                }
                
                foreach (var movie in movies)
                {
                    var cleanCode = movie.MovieCode.Split(new[] { "_$DEL$_" }, StringSplitOptions.None)[0];
                    var card = CreateItemCard(movie.Id, cleanCode, movie.CoverImage, false);
                    flpMovies.Controls.Add(card);
                }
            }
        }

        private Image CreatePlaceholder(int width, int height, string text)
        {
            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(UIHelper.BgPanel);
                using (var brush = new SolidBrush(UIHelper.TextMuted))
                using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
                {
                    var size = g.MeasureString(text, font);
                    g.DrawString(text, font, brush, (width - size.Width) / 2, (height - size.Height) / 2);
                }
            }
            return bmp;
        }

        private Guna2Panel CreateItemCard(int id, string code, string coverPath, bool isAudio)
        {
            var pnlCard = new Guna2Panel
            {
                Width = 200,
                Height = 350,
                BorderRadius = 12,
                FillColor = UIHelper.BgCard,
                Margin = new Padding(10),
                Cursor = Cursors.Default
            };

            var picCover = new PictureBox
            {
                Width = 180,
                Height = 250,
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = UIHelper.BgPanel,
                Cursor = Cursors.Hand
            };

            if (!string.IsNullOrEmpty(coverPath) && System.IO.File.Exists(coverPath))
            {
                try
                {
                    using (var fs = new System.IO.FileStream(coverPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        var img = Image.FromStream(fs);
                        picCover.Image = new Bitmap(img);
                    }
                }
                catch
                {
                    picCover.Image = CreatePlaceholder(180, 250, isAudio ? "AUDIO" : "NO IMAGE");
                }
            }
            else
            {
                picCover.Image = CreatePlaceholder(180, 250, isAudio ? "AUDIO" : "NO IMAGE");
            }

            var lblCode = new Label
            {
                Text = code,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = UIHelper.TextPrimary,
                BackColor = Color.Transparent,
                Location = new Point(10, 270),
                AutoSize = false,
                Width = 180,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            var btnRestore = new Guna2Button
            {
                Text = "Khôi phục",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderRadius = 8,
                FillColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                Location = new Point(10, 305),
                Size = new Size(85, 30),
                Cursor = Cursors.Hand
            };
            btnRestore.Click += (s, e) => 
            {
                RestoreItem(id, isAudio);
            };

            var btnDelete = new Guna2Button
            {
                Text = "Xóa hẳn",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderRadius = 8,
                FillColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                Location = new Point(105, 305),
                Size = new Size(85, 30),
                Cursor = Cursors.Hand
            };
            btnDelete.Click += (s, e) => 
            {
                HardDeleteItem(id, isAudio);
            };

            pnlCard.Controls.Add(picCover);
            pnlCard.Controls.Add(lblCode);
            pnlCard.Controls.Add(btnRestore);
            pnlCard.Controls.Add(btnDelete);
            
            picCover.Click += (s, e) => ShowItemDetails(id, isAudio);

            return pnlCard;
        }

        private void ShowItemDetails(int id, bool isAudio)
        {
            if (isAudio)
            {
                var audio = _audioRepo.GetById(id);
                if (audio != null)
                {
                    string details = $"Mã âm thanh: {audio.AudioCode}\r\n" +
                                     $"Ngày xóa: {audio.DeletedAt:dd/MM/yyyy HH:mm}\r\n" +
                                     $"Ngày tạo: {audio.CreatedAt:dd/MM/yyyy HH:mm}\r\n" +
                                     $"Đánh giá: {audio.Rating} sao\r\n\r\n" +
                                     $"Mô tả: {audio.Note ?? "Không có"}";
                    new Forms.FrmDetailDialog($"Chi tiết: {audio.AudioCode}", details).ShowDialog();
                }
            }
            else
            {
                var movie = _movieRepo.GetById(id);
                if (movie != null)
                {
                    string type = movie.SourceType == 0 ? "🌐 Online" : "📁 Trên Máy";
                    var cleanCode = movie.MovieCode.Split(new[] { "_$DEL$_" }, StringSplitOptions.None)[0];
                    string details = $"Mã phim: {cleanCode}\r\n" +
                                     $"Nguồn: {type}\r\n" +
                                     $"Ngày xóa: {movie.DeletedAt:dd/MM/yyyy HH:mm}\r\n" +
                                     $"Ngày tạo: {movie.CreatedAt:dd/MM/yyyy HH:mm}\r\n" +
                                     $"Đánh giá: {movie.Rating} sao\r\n" +
                                     $"Đường dẫn / URL: {movie.MediaUrl}\r\n\r\n" +
                                     $"Mô tả: {movie.Note ?? "Không có"}";
                    new Forms.FrmDetailDialog($"Chi tiết: {cleanCode}", details).ShowDialog();
                }
            }
        }
        
        private void RestoreItem(int id, bool isAudio)
        {
            if (!SessionManager.IsLoggedIn) return;
            int currentUserId = SessionManager.CurrentUser!.Id;

            if (isAudio)
            {
                var itemToRestore = _audioRepo.GetById(id, includeDeleted: true);
                if (itemToRestore != null)
                {
                    var cleanCode = itemToRestore.AudioCode.Split(new[] { "_$DEL$_" }, StringSplitOptions.None)[0];
                    var existing = _audioRepo.GetByCode(currentUserId, cleanCode);
                    if (existing != null)
                    {
                        MessageBox.Show($"Không thể khôi phục vì đã có âm thanh tên '{cleanCode}' đang hoạt động!", "Trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            else
            {
                var itemToRestore = _movieRepo.GetById(id, includeDeleted: true);
                if (itemToRestore != null)
                {
                    var cleanCode = itemToRestore.MovieCode.Split(new[] { "_$DEL$_" }, StringSplitOptions.None)[0];
                    var existing = _movieRepo.GetByCode(currentUserId, cleanCode);
                    if (existing != null)
                    {
                        MessageBox.Show($"Không thể khôi phục vì đã có phim tên '{cleanCode}' đang hoạt động!", "Trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var cmd = connection.CreateCommand();
            string table = isAudio ? "Audios" : "Movies";
            string col = isAudio ? "AudioCode" : "MovieCode";
            cmd.CommandText = $"UPDATE {table} SET IsDeleted = 0, DeletedAt = NULL, {col} = REPLACE({col}, '_$DEL$_' || Id, '') WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
            
            MessageBox.Show("Khôi phục thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData(txtSearch.Text);
        }
        
        private void HardDeleteItem(int id, bool isAudio)
        {
            var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa vĩnh viễn mục này? Không thể khôi phục lại!", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                if (isAudio) _audioRepo.HardDelete(id);
                else _movieRepo.HardDelete(id);
                
                MessageBox.Show("Xóa vĩnh viễn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(txtSearch.Text);
            }
        }

        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text);
        }

        private void btnEmptyTrash_Click(object sender, EventArgs e)
        {
            bool isAudio = cmbSort.SelectedIndex == 1;
            int count = flpMovies.Controls.Count;
            if (count == 0) return;

            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa vĩnh viễn toàn bộ {count} mục trong thùng rác? Không thể khôi phục lại!", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                if (isAudio)
                {
                    var audios = _audioRepo.GetDeleted(_currentUserId);
                    foreach (var item in audios) _audioRepo.HardDelete(item.Id);
                }
                else
                {
                    var movies = _movieRepo.GetDeleted(_currentUserId);
                    foreach (var item in movies) _movieRepo.HardDelete(item.Id);
                }
                
                MessageBox.Show("Đã dọn dẹp thùng rác thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(txtSearch.Text);
            }
        }
    }
}
