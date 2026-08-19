using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Person_Movie_Management.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Guna.UI2.WinForms;

namespace Person_Movie_Management.UserControls
{
    public partial class UcPlaylist : UserControl
    {
        private PlaylistRepository _playlistRepo;
        private int _currentUserId;
        private List<Playlist> _allPlaylists = new();
        private string _currentSort = "newest";

        private const int CARD_W = 250;
        private const int CARD_H = 310;
        private const int CARD_IMG_H = 155;

        public UcPlaylist(int userId)
        {
            InitializeComponent();
            _currentUserId = userId;
            _playlistRepo = new PlaylistRepository();

            this.BackColor = UIHelper.BgDark;
            pnlTop.BackColor = UIHelper.BgDark;
            flpPlaylists.BackColor = UIHelper.BgDark;

            cboSort.Items.AddRange(new string[] {
                "🕐 Mới nhất",
                "📅 Cũ nhất",
                "🔤 Tên A-Z",
                "🔤 Tên Z-A",
                "📊 Nhiều mục nhất"
            });
            cboSort.SelectedIndex = 0;
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

        private void UcPlaylist_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            _allPlaylists = LoadSorted();
            RenderGrid(_allPlaylists);
        }

        private List<Playlist> LoadSorted()
        {
            return _currentSort switch
            {
                "oldest" => _playlistRepo.GetAllByUser(_currentUserId, "oldest"),
                "name_az" => _playlistRepo.GetAllByUser(_currentUserId, "name_az"),
                "name_za" => _playlistRepo.GetAllByUser(_currentUserId, "name_za"),
                "most_items" => _playlistRepo.GetAllByUserSortedByCount(_currentUserId),
                _ => _playlistRepo.GetAllByUser(_currentUserId, "newest"),
            };
        }

        private void RenderGrid(List<Playlist> list)
        {
            flpPlaylists.SuspendLayout();
            foreach (Control c in flpPlaylists.Controls)
            {
                c.Dispose();
            }
            flpPlaylists.Controls.Clear();

            string filter = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(filter))
            {
                list = list.Where(p => p.Name.ToLower().Contains(filter)).ToList();
            }

            if (list.Count == 0)
            {
                lblEmpty.Visible = true;
                lblEmpty.Text = string.IsNullOrEmpty(filter)
                    ? "Chưa có playlist nào.\r\nNhấn \"+ Tạo mới\" để bắt đầu! 🎵"
                    : "Không tìm thấy playlist nào phù hợp.";
                flpPlaylists.ResumeLayout();
                return;
            }

            lblEmpty.Visible = false;

            foreach (var pl in list)
            {
                var card = CreatePlaylistCard(pl);
                flpPlaylists.Controls.Add(card);
            }

            flpPlaylists.ResumeLayout();
        }

        private Guna2Panel CreatePlaylistCard(Playlist playlist)
        {
            var (movieCount, audioCount) = _playlistRepo.GetStats(playlist.Id);
            int totalCount = movieCount + audioCount;

            var pnlCard = new Guna2Panel
            {
                Width = CARD_W,
                Height = CARD_H,
                BorderRadius = 16,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(42, 53, 90),
                FillColor = Color.FromArgb(20, 26, 48),
                Margin = new Padding(10, 10, 10, 10),
                Cursor = Cursors.Hand,
                Tag = playlist
            };

            // ── Top Cover PictureBox ──────────────────────────
            var picCover = new PictureBox
            {
                Size = new Size(CARD_W, CARD_IMG_H),
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.FromArgb(15, 20, 40),
                Cursor = Cursors.Hand
            };

            Image? coverImg = null;
            if (!string.IsNullOrEmpty(playlist.CoverImage))
            {
                string fullPath = FileHelper.GetFullPath(playlist.CoverImage);
                var raw = FileHelper.LoadImageSafe(fullPath);
                if (raw != null)
                {
                    coverImg = UIHelper.CropToFill(raw, CARD_W, CARD_IMG_H);
                    raw.Dispose();
                }
            }

            if (coverImg == null && totalCount > 0)
            {
                coverImg = BuildMosaicImage(playlist.Id, CARD_W, CARD_IMG_H);
            }

            if (coverImg == null)
            {
                coverImg = BuildGradientPlaceholder(CARD_W, CARD_IMG_H, playlist.Name);
            }

            picCover.Image = coverImg;

            // ── Overlay Gradient at bottom of cover ───────────
            picCover.Paint += (s, e) =>
            {
                using var brush = new LinearGradientBrush(
                    new Rectangle(0, CARD_IMG_H - 45, CARD_W, 45),
                    Color.Transparent,
                    Color.FromArgb(160, 15, 20, 40),
                    LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(brush, 0, CARD_IMG_H - 45, CARD_W, 45);
            };

            // Quick Edit Button
            var btnEdit = new Guna2Button
            {
                Text = "✏️",
                Size = new Size(30, 30),
                Location = new Point(CARD_W - 74, 8),
                BorderRadius = 8,
                FillColor = Color.FromArgb(200, 20, 26, 48),
                ForeColor = Color.FromArgb(224, 231, 255),
                Font = new Font("Segoe UI Emoji", 9F),
                Cursor = Cursors.Hand
            };
            btnEdit.HoverState.FillColor = Color.FromArgb(99, 102, 241);
            btnEdit.HoverState.ForeColor = Color.White;
            btnEdit.Click += (s, e) => EditPlaylist(playlist);
            picCover.Controls.Add(btnEdit);

            // Quick Delete Button
            var btnDelete = new Guna2Button
            {
                Text = "🗑️",
                Size = new Size(30, 30),
                Location = new Point(CARD_W - 38, 8),
                BorderRadius = 8,
                FillColor = Color.FromArgb(200, 50, 20, 30),
                ForeColor = Color.FromArgb(252, 165, 165),
                Font = new Font("Segoe UI Emoji", 9F),
                Cursor = Cursors.Hand
            };
            btnDelete.HoverState.FillColor = Color.FromArgb(239, 68, 68);
            btnDelete.HoverState.ForeColor = Color.White;
            btnDelete.Click += (s, e) => DeletePlaylist(playlist);
            picCover.Controls.Add(btnDelete);

            // ── Info Area (Bottom) ────────────────────────────
            var lblName = new Label
            {
                Text = playlist.Name,
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(248, 250, 252),
                BackColor = Color.Transparent,
                Location = new Point(14, CARD_IMG_H + 10),
                Size = new Size(CARD_W - 28, 24),
                AutoEllipsis = true,
                Cursor = Cursors.Hand
            };

            var lblDesc = new Label
            {
                Text = string.IsNullOrWhiteSpace(playlist.Description) ? "Chưa có mô tả" : playlist.Description,
                Font = new Font("Segoe UI", 8.5F, string.IsNullOrWhiteSpace(playlist.Description) ? FontStyle.Italic : FontStyle.Regular),
                ForeColor = string.IsNullOrWhiteSpace(playlist.Description) ? Color.FromArgb(80, 95, 125) : Color.FromArgb(148, 163, 184),
                BackColor = Color.Transparent,
                Location = new Point(14, CARD_IMG_H + 34),
                Size = new Size(CARD_W - 28, 30),
                AutoEllipsis = true,
                Cursor = Cursors.Hand
            };

            // Stat pills
            var btnMoviePill = new Guna2Button
            {
                Text = $"🎬 {movieCount}",
                Size = new Size(62, 24),
                Location = new Point(14, CARD_IMG_H + 72),
                BorderRadius = 6,
                FillColor = Color.FromArgb(35, 99, 102, 241),
                ForeColor = Color.FromArgb(199, 210, 254),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnMoviePill.Click += (s, e) => ViewPlaylist(playlist);

            var btnAudioPill = new Guna2Button
            {
                Text = $"🎵 {audioCount}",
                Size = new Size(62, 24),
                Location = new Point(82, CARD_IMG_H + 72),
                BorderRadius = 6,
                FillColor = Color.FromArgb(35, 236, 72, 153),
                ForeColor = Color.FromArgb(251, 207, 232),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAudioPill.Click += (s, e) => ViewPlaylist(playlist);

            var lblDate = new Label
            {
                Text = $"📅 {playlist.CreatedAt:dd/MM/yyyy}",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(100, 116, 139),
                BackColor = Color.Transparent,
                Location = new Point(14, CARD_IMG_H + 110),
                Size = new Size(CARD_W - 28, 18),
                Cursor = Cursors.Hand
            };

            // Assemble Card
            pnlCard.Controls.Add(picCover);
            pnlCard.Controls.Add(lblName);
            pnlCard.Controls.Add(lblDesc);
            pnlCard.Controls.Add(btnMoviePill);
            pnlCard.Controls.Add(btnAudioPill);
            pnlCard.Controls.Add(lblDate);

            // Hover interactions
            Action onHover = () =>
            {
                pnlCard.BorderColor = Color.FromArgb(139, 92, 246);
                pnlCard.FillColor = Color.FromArgb(28, 36, 68);
            };

            Action onLeave = () =>
            {
                pnlCard.BorderColor = Color.FromArgb(42, 53, 90);
                pnlCard.FillColor = Color.FromArgb(20, 26, 48);
            };

            AttachHoverEvents(pnlCard, onHover, onLeave, playlist);

            return pnlCard;
        }

        private void AttachHoverEvents(Guna2Panel card, Action onHover, Action onLeave, Playlist playlist)
        {
            card.MouseEnter += (s, e) => onHover();
            card.MouseLeave += (s, e) =>
            {
                if (!card.RectangleToScreen(card.ClientRectangle).Contains(Cursor.Position))
                    onLeave();
            };

            foreach (Control c in card.Controls)
            {
                if (c is Guna2Button btn && (btn.Text == "✏️" || btn.Text == "🗑️"))
                    continue;

                c.MouseEnter += (s, e) => onHover();
                c.MouseLeave += (s, e) =>
                {
                    if (!card.RectangleToScreen(card.ClientRectangle).Contains(Cursor.Position))
                        onLeave();
                };

                if (c is not Guna2Button)
                {
                    c.Click += (s, e) => ViewPlaylist(playlist);
                }
            }

            card.Click += (s, e) => ViewPlaylist(playlist);
        }

        private Image BuildMosaicImage(int playlistId, int width, int height)
        {
            var thumbPaths = _playlistRepo.GetCoverThumbnails(playlistId, 4);
            var images = new List<Image>();

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

            if (images.Count == 0) return BuildGradientPlaceholder(width, height, "");

            var bmp = new Bitmap(width, height);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (images.Count == 1)
            {
                using var cropped = UIHelper.CropToFill(images[0], width, height);
                if (cropped != null) g.DrawImage(cropped, 0, 0, width, height);
            }
            else if (images.Count == 2)
            {
                int hw = width / 2;
                using var c1 = UIHelper.CropToFill(images[0], hw, height);
                using var c2 = UIHelper.CropToFill(images[1], hw, height);
                if (c1 != null) g.DrawImage(c1, 0, 0, hw, height);
                if (c2 != null) g.DrawImage(c2, hw, 0, hw, height);
            }
            else
            {
                int hw = width / 2;
                int hh = height / 2;
                using var c1 = UIHelper.CropToFill(images[0], hw, hh);
                using var c2 = UIHelper.CropToFill(images[1], hw, hh);
                using var c3 = UIHelper.CropToFill(images[2], hw, hh);
                if (c1 != null) g.DrawImage(c1, 0, 0, hw, hh);
                if (c2 != null) g.DrawImage(c2, hw, 0, hw, hh);
                if (c3 != null) g.DrawImage(c3, 0, hh, hw, hh);

                if (images.Count >= 4)
                {
                    using var c4 = UIHelper.CropToFill(images[3], hw, hh);
                    if (c4 != null) g.DrawImage(c4, hw, hh, hw, hh);
                }
                else
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(22, 28, 56)), hw, hh, hw, hh);
                }
            }

            foreach (var img in images) img.Dispose();
            return bmp;
        }

        private Image BuildGradientPlaceholder(int width, int height, string name)
        {
            var bmp = new Bitmap(width, height);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int hash = Math.Abs((name ?? "").GetHashCode());
            var colors = new[]
            {
                (Color.FromArgb(79, 70, 229), Color.FromArgb(124, 58, 237)),   // indigo-violet
                (Color.FromArgb(219, 39, 119), Color.FromArgb(124, 58, 237)),  // pink-violet
                (Color.FromArgb(37, 99, 235), Color.FromArgb(79, 70, 229)),   // blue-indigo
                (Color.FromArgb(5, 150, 105), Color.FromArgb(37, 99, 235)),   // emerald-blue
                (Color.FromArgb(217, 119, 6), Color.FromArgb(225, 29, 72)),    // amber-rose
                (Color.FromArgb(8, 145, 178), Color.FromArgb(79, 70, 229)),   // cyan-indigo
            };
            var (c1, c2) = colors[hash % colors.Length];

            using var brush = new LinearGradientBrush(
                new Rectangle(0, 0, width, height), c1, c2, 135f);
            g.FillRectangle(brush, 0, 0, width, height);

            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString("🎵", new Font("Segoe UI Emoji", 28F), Brushes.White,
                new RectangleF(0, 0, width, height), sf);

            return bmp;
        }

        private void EditPlaylist(Playlist playlist)
        {
            using var frm = new FrmPlaylistEdit(playlist);
            if (frm.ShowDialog(this.ParentForm) == DialogResult.OK && frm.ResultPlaylist != null)
            {
                _playlistRepo.Update(frm.ResultPlaylist);
                LoadData();
            }
        }

        private void DeletePlaylist(Playlist playlist)
        {
            int count = _playlistRepo.GetItemCount(playlist.Id);
            string msg = count > 0
                ? $"Xóa playlist \"{playlist.Name}\"?\n\n⚠️ Playlist này đang có {count} mục bên trong."
                : $"Xóa playlist \"{playlist.Name}\"?";

            if (MessageBox.Show(msg, "Xóa Playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _playlistRepo.Delete(playlist.Id);
                LoadData();
            }
        }

        private void ViewPlaylist(Playlist playlist)
        {
            var parentPanel = this.Parent;
            if (parentPanel == null) return;

            var detail = new UcPlaylistDetail(playlist);
            detail.Dock = DockStyle.Fill;
            detail.BackRequested += (s, e) =>
            {
                parentPanel.Controls.Clear();
                this.Dock = DockStyle.Fill;
                parentPanel.Controls.Add(this);
                LoadData();
            };

            parentPanel.Controls.Clear();
            parentPanel.Controls.Add(detail);
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            if (_allPlaylists.Count == 0)
            {
                MessageBox.Show("Không có playlist nào để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var inputDialog = new FrmInputBox(
                "Xác nhận xóa Playlist",
                $"Bạn sắp xóa TẤT CẢ {_allPlaylists.Count} playlist.",
                defaultValue: "",
                showHardDelete: false,
                note: "⚠️ Lưu ý: Nhập 'delete' để xác nhận xóa toàn bộ playlist.",
                placeholder: "Nhập 'delete' để xác nhận...");

            if (inputDialog.ShowDialog() == DialogResult.OK &&
                inputDialog.InputValue.Trim().ToLower() == "delete")
            {
                int deleted = _playlistRepo.DeleteAll(_currentUserId);
                MessageBox.Show($"Đã xóa thành công {deleted} playlist!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            else if (inputDialog.DialogResult == DialogResult.OK)
            {
                MessageBox.Show("Xác nhận không hợp lệ. Hủy thao tác xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using var frm = new FrmPlaylistEdit(_currentUserId);
            if (frm.ShowDialog(this.ParentForm) == DialogResult.OK && frm.ResultPlaylist != null)
            {
                _playlistRepo.Insert(frm.ResultPlaylist);
                LoadData();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private void _searchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            RenderGrid(_allPlaylists);
        }

        private void cboSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentSort = cboSort.SelectedIndex switch
            {
                0 => "newest",
                1 => "oldest",
                2 => "name_az",
                3 => "name_za",
                4 => "most_items",
                _ => "newest"
            };
            _allPlaylists = LoadSorted();
            RenderGrid(_allPlaylists);
        }
    }
}
