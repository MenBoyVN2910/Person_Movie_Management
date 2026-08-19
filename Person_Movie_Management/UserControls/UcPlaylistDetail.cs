using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Person_Movie_Management.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Person_Movie_Management.Services;
using Guna.UI2.WinForms;

namespace Person_Movie_Management.UserControls
{
    public partial class UcPlaylistDetail : UserControl
    {
        private PlaylistRepository _playlistRepo;
        private MovieRepository _movieRepo;
        private AudioRepository _audioRepo;
        private Playlist _playlist;

        public event EventHandler? BackRequested;

        public UcPlaylistDetail(Playlist playlist)
        {
            InitializeComponent();
            _playlist = playlist;
            _playlistRepo = new PlaylistRepository();
            _movieRepo = new MovieRepository();
            _audioRepo = new AudioRepository();

            this.BackColor = UIHelper.BgDark;
            pnlHeader.BackColor = UIHelper.BgDark;
            flpItems.BackColor = UIHelper.BgDark;

            SetupHeader();
            LoadItems();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }

        private void SetupHeader()
        {
            lblPlaylistName.Text = _playlist.Name;

            if (!string.IsNullOrWhiteSpace(_playlist.Description))
            {
                lblDescription.Text = _playlist.Description;
                lblDescription.Visible = true;
            }
            else
            {
                lblDescription.Text = "Chưa có mô tả cho playlist này";
                lblDescription.ForeColor = Color.FromArgb(80, 95, 125);
            }

            // Badges
            btnPrivacyBadge.Visible = false;

            var (movieCount, audioCount) = _playlistRepo.GetStats(_playlist.Id);
            btnMovieBadge.Text = $"🎬 {movieCount} phim";
            btnAudioBadge.Text = $"🎵 {audioCount} audio";
            lblDate.Text = $"📅 Ngày tạo: {_playlist.CreatedAt:dd/MM/yyyy}";

            // Cover picture
            Image? coverImg = null;
            if (!string.IsNullOrEmpty(_playlist.CoverImage))
            {
                string fullPath = FileHelper.GetFullPath(_playlist.CoverImage);
                var raw = FileHelper.LoadImageSafe(fullPath);
                if (raw != null)
                {
                    coverImg = UIHelper.CropToFill(raw, 108, 108);
                    raw.Dispose();
                }
            }

            if (coverImg == null && (movieCount + audioCount) > 0)
            {
                coverImg = BuildMosaicSquare(_playlist.Id, 108);
            }

            if (coverImg == null)
            {
                coverImg = BuildHeaderGradient(108, 108, _playlist.Name);
            }

            picCover.Image?.Dispose();
            picCover.Image = coverImg;
        }

        private Image BuildMosaicSquare(int playlistId, int size)
        {
            var thumbPaths = _playlistRepo.GetCoverThumbnails(playlistId, 4);
            var images = new System.Collections.Generic.List<Image>();

            foreach (var (path, _) in thumbPaths)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    string full = FileHelper.GetFullPath(path);
                    var img = FileHelper.LoadImageSafe(full);
                    if (img != null) images.Add(img);
                }
                if (images.Count >= 4) break;
            }

            if (images.Count == 0) return BuildHeaderGradient(size, size, "");

            var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (images.Count == 1)
            {
                using var cropped = UIHelper.CropToFill(images[0], size, size);
                if (cropped != null) g.DrawImage(cropped, 0, 0, size, size);
            }
            else if (images.Count == 2)
            {
                int half = size / 2;
                using var c1 = UIHelper.CropToFill(images[0], half, size);
                using var c2 = UIHelper.CropToFill(images[1], half, size);
                if (c1 != null) g.DrawImage(c1, 0, 0, half, size);
                if (c2 != null) g.DrawImage(c2, half, 0, half, size);
            }
            else
            {
                int half = size / 2;
                using var c1 = UIHelper.CropToFill(images[0], half, half);
                using var c2 = UIHelper.CropToFill(images[1], half, half);
                using var c3 = UIHelper.CropToFill(images[2], half, half);
                if (c1 != null) g.DrawImage(c1, 0, 0, half, half);
                if (c2 != null) g.DrawImage(c2, half, 0, half, half);
                if (c3 != null) g.DrawImage(c3, 0, half, half, half);

                if (images.Count >= 4)
                {
                    using var c4 = UIHelper.CropToFill(images[3], half, half);
                    if (c4 != null) g.DrawImage(c4, half, half, half, half);
                }
                else
                {
                    using var bgBrush = new SolidBrush(Color.FromArgb(20, 26, 48));
                    g.FillRectangle(bgBrush, half, half, half, half);
                }
            }

            foreach (var img in images) img.Dispose();
            return bmp;
        }

        private Image BuildHeaderGradient(int width, int height, string name)
        {
            var bmp = new Bitmap(width, height);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int hash = Math.Abs((name ?? "").GetHashCode());
            var colors = new[]
            {
                (Color.FromArgb(79, 70, 229), Color.FromArgb(124, 58, 237)),
                (Color.FromArgb(219, 39, 119), Color.FromArgb(124, 58, 237)),
                (Color.FromArgb(37, 99, 235), Color.FromArgb(79, 70, 229)),
                (Color.FromArgb(5, 150, 105), Color.FromArgb(37, 99, 235))
            };
            var (c1, c2) = colors[hash % colors.Length];

            using var brush = new LinearGradientBrush(new Rectangle(0, 0, width, height), c1, c2, 135f);
            g.FillRectangle(brush, 0, 0, width, height);

            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("🎵", new Font("Segoe UI Emoji", 24F), Brushes.White, new RectangleF(0, 0, width, height), sf);
            return bmp;
        }

        public void LoadItems()
        {
            flpItems.SuspendLayout();
            foreach (Control c in flpItems.Controls)
            {
                c.Dispose();
            }
            flpItems.Controls.Clear();

            var items = _playlistRepo.GetItems(_playlist.Id);
            var (movieCount, audioCount) = _playlistRepo.GetStats(_playlist.Id);

            btnMovieBadge.Text = $"🎬 {movieCount} phim";
            btnAudioBadge.Text = $"🎵 {audioCount} audio";

            lblEmpty.Visible = items.Count == 0;
            flpItems.Visible = items.Count > 0;

            int index = 0;
            foreach (var item in items)
            {
                var row = CreateItemRow(item, index);
                flpItems.Controls.Add(row);
                index++;
            }

            flpItems.ResumeLayout();
        }

        private Guna2Panel CreateItemRow(PlaylistItem item, int index)
        {
            string title = "";
            string coverPath = "";
            string subtitle = "";
            bool isMovie = item.ItemType == PlaylistItemType.Movie;

            if (isMovie)
            {
                var movie = _movieRepo.GetById(item.ItemId);
                if (movie != null)
                {
                    title = movie.MovieCode;
                    subtitle = !string.IsNullOrWhiteSpace(movie.Note) ? movie.Note : (movie.SourceType == 1 ? "Phim cục bộ (Local)" : "Phim trực tuyến (Online)");
                    coverPath = movie.CoverImage ?? "";
                }
                else
                {
                    title = "(Phim đã bị xóa)";
                    subtitle = "Mục này không còn tồn tại trong kho lưu trữ";
                }
            }
            else
            {
                var audio = _audioRepo.GetById(item.ItemId, false);
                if (audio != null)
                {
                    title = audio.AudioCode;
                    subtitle = !string.IsNullOrWhiteSpace(audio.Note) ? audio.Note : "Tệp âm thanh";
                    coverPath = audio.CoverImage ?? "";
                }
                else
                {
                    title = "(Audio đã bị xóa)";
                    subtitle = "Mục này không còn tồn tại trong kho lưu trữ";
                }
            }

            int rowWidth = Math.Max(600, flpItems.ClientSize.Width - 56);

            var pnlRow = new Guna2Panel
            {
                Width = rowWidth,
                Height = 72,
                BorderRadius = 12,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(42, 53, 90),
                FillColor = Color.FromArgb(20, 26, 48),
                Margin = new Padding(0, 4, 0, 4)
            };

            // Order Number Badge
            var btnOrder = new Guna2Button
            {
                Text = $"{index + 1}",
                Size = new Size(32, 32),
                Location = new Point(14, 20),
                BorderRadius = 16,
                FillColor = Color.FromArgb(30, 99, 102, 241),
                ForeColor = Color.FromArgb(199, 210, 254),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Enabled = false
            };

            // Thumbnail
            var picThumb = new Guna2PictureBox
            {
                Size = new Size(52, 52),
                Location = new Point(54, 10),
                BorderRadius = 8,
                FillColor = Color.FromArgb(30, 41, 68),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            if (!string.IsNullOrEmpty(coverPath))
            {
                string fullPath = FileHelper.GetFullPath(coverPath);
                var raw = FileHelper.LoadImageSafe(fullPath);
                if (raw != null)
                {
                    picThumb.Image = UIHelper.CropToFill(raw, 52, 52);
                    raw.Dispose();
                }
            }

            if (picThumb.Image == null)
            {
                picThumb.Image = BuildHeaderGradient(52, 52, title);
            }

            // Title Label
            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(241, 245, 249),
                BackColor = Color.Transparent,
                Location = new Point(116, 14),
                Size = new Size(Math.Max(180, rowWidth - 360), 22),
                AutoEllipsis = true
            };

            // Subtitle / Type badge
            var btnType = new Guna2Button
            {
                Text = isMovie ? "🎬 Phim" : "🎵 Audio",
                Size = new Size(68, 20),
                Location = new Point(116, 38),
                BorderRadius = 5,
                FillColor = isMovie ? Color.FromArgb(35, 99, 102, 241) : Color.FromArgb(35, 236, 72, 153),
                ForeColor = isMovie ? Color.FromArgb(199, 210, 254) : Color.FromArgb(251, 207, 232),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                Enabled = false
            };

            var lblSub = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(100, 116, 139),
                BackColor = Color.Transparent,
                Location = new Point(190, 40),
                Size = new Size(Math.Max(100, rowWidth - 440), 18),
                AutoEllipsis = true
            };

            // Play Button
            var btnPlay = new Guna2Button
            {
                Text = "▶",
                Size = new Size(32, 32),
                Location = new Point(rowWidth - 162, 20),
                BorderRadius = 8,
                FillColor = Color.FromArgb(30, 99, 102, 241),
                ForeColor = Color.FromArgb(199, 210, 254),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPlay.HoverState.FillColor = Color.FromArgb(99, 102, 241);
            btnPlay.HoverState.ForeColor = Color.White;
            btnPlay.Click += (s, e) => PlayItem(item);

            picThumb.Cursor = Cursors.Hand;
            picThumb.Click += (s, e) => PlayItem(item);
            lblTitle.Cursor = Cursors.Hand;
            lblTitle.Click += (s, e) => PlayItem(item);

            // Move Up Button
            var btnUp = new Guna2Button
            {
                Text = "▲",
                Size = new Size(32, 32),
                Location = new Point(rowWidth - 124, 20),
                BorderRadius = 8,
                FillColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.FromArgb(203, 213, 225),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnUp.HoverState.FillColor = Color.FromArgb(99, 102, 241);
            btnUp.HoverState.ForeColor = Color.White;
            int currentIndex = index;
            btnUp.Click += (s, e) => MoveItem(currentIndex, -1);

            // Move Down Button
            var btnDown = new Guna2Button
            {
                Text = "▼",
                Size = new Size(32, 32),
                Location = new Point(rowWidth - 86, 20),
                BorderRadius = 8,
                FillColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.FromArgb(203, 213, 225),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDown.HoverState.FillColor = Color.FromArgb(99, 102, 241);
            btnDown.HoverState.ForeColor = Color.White;
            btnDown.Click += (s, e) => MoveItem(currentIndex, 1);

            // Delete Button
            var btnRemove = new Guna2Button
            {
                Text = "🗑️",
                Size = new Size(32, 32),
                Location = new Point(rowWidth - 46, 20),
                BorderRadius = 8,
                FillColor = Color.FromArgb(45, 20, 30),
                ForeColor = Color.FromArgb(252, 165, 165),
                Font = new Font("Segoe UI Emoji", 9F),
                Cursor = Cursors.Hand
            };
            btnRemove.HoverState.FillColor = Color.FromArgb(239, 68, 68);
            btnRemove.HoverState.ForeColor = Color.White;
            int itemDbId = item.Id;
            btnRemove.Click += (s, e) =>
            {
                _playlistRepo.RemoveItem(itemDbId);
                LoadItems();
            };

            // Hover effect on the row
            pnlRow.MouseEnter += (s, e) =>
            {
                pnlRow.BorderColor = Color.FromArgb(139, 92, 246);
                pnlRow.FillColor = Color.FromArgb(28, 36, 68);
            };
            pnlRow.MouseLeave += (s, e) =>
            {
                if (!pnlRow.RectangleToScreen(pnlRow.ClientRectangle).Contains(Cursor.Position))
                {
                    pnlRow.BorderColor = Color.FromArgb(42, 53, 90);
                    pnlRow.FillColor = Color.FromArgb(20, 26, 48);
                }
            };

            pnlRow.Controls.Add(btnOrder);
            pnlRow.Controls.Add(picThumb);
            pnlRow.Controls.Add(lblTitle);
            pnlRow.Controls.Add(btnType);
            pnlRow.Controls.Add(lblSub);
            pnlRow.Controls.Add(btnPlay);
            pnlRow.Controls.Add(btnUp);
            pnlRow.Controls.Add(btnDown);
            pnlRow.Controls.Add(btnRemove);

            return pnlRow;
        }

        private void PlayItem(PlaylistItem item)
        {
            if (item.ItemType == PlaylistItemType.Movie)
            {
                var movie = _movieRepo.GetById(item.ItemId);
                if (movie != null && !string.IsNullOrWhiteSpace(movie.MediaUrl))
                {
                    MediaLauncher.LaunchMedia(movie.MediaUrl, movie.SourceType);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy phim hoặc đường dẫn phát.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                var fullAudio = _audioRepo.GetById(item.ItemId, true);
                if (fullAudio != null && fullAudio.AudioData != null && fullAudio.AudioData.Length > 0)
                {
                    try
                    {
                        if (this.FindForm() is FrmMain mainForm)
                        {
                            mainForm.PlayGlobalAudio(fullAudio.AudioData, fullAudio.AudioCode, fullAudio.Id);
                        }
                        else
                        {
                            string tempFile = Path.Combine(Path.GetTempPath(), $"temp_audio_{Guid.NewGuid()}.mp3");
                            System.IO.File.WriteAllBytes(tempFile, fullAudio.AudioData);
                            MediaLauncher.LaunchMedia(tempFile, 1);
                        }
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
        }

        private void MoveItem(int currentIndex, int direction)
        {
            var items = _playlistRepo.GetItems(_playlist.Id);
            int targetIndex = currentIndex + direction;
            if (targetIndex < 0 || targetIndex >= items.Count) return;

            var currentItem = items[currentIndex];
            var targetItem = items[targetIndex];

            int tempOrder = currentItem.SortOrder;
            _playlistRepo.UpdateSortOrder(currentItem.Id, targetItem.SortOrder);
            _playlistRepo.UpdateSortOrder(targetItem.Id, tempOrder);

            LoadItems();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            using var frm = new FrmPlaylistEdit(_playlist);
            if (frm.ShowDialog(this.ParentForm) == DialogResult.OK && frm.ResultPlaylist != null)
            {
                _playlist = frm.ResultPlaylist;
                _playlistRepo.Update(_playlist);
                SetupHeader();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        private void UcPlaylistDetail_Resize(object sender, EventArgs e)
        {
            int rowWidth = Math.Max(600, flpItems.ClientSize.Width - 56);
            foreach (Control c in flpItems.Controls)
            {
                if (c is Guna2Panel row)
                {
                    row.Width = rowWidth;
                    // Update positions of right action buttons
                    if (row.Controls.Count >= 9)
                    {
                        row.Controls[5].Location = new Point(rowWidth - 162, 20); // btnPlay
                        row.Controls[6].Location = new Point(rowWidth - 124, 20); // btnUp
                        row.Controls[7].Location = new Point(rowWidth - 86, 20);  // btnDown
                        row.Controls[8].Location = new Point(rowWidth - 46, 20);  // btnRemove
                    }
                }
            }
        }
    }
}
