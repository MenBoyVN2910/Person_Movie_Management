using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
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
            lblPlaylistName.ForeColor = UIHelper.TextPrimary;
            lblItemCount.ForeColor = UIHelper.TextSecondary;
            lblPlaylistName.Text = $"🎵  {_playlist.Name}";

            btnBack.FillColor = UIHelper.BgCard;
            btnBack.ForeColor = UIHelper.TextPrimary;
            btnBack.HoverState.FillColor = UIHelper.BgCardHover;

            LoadItems();
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

        public void LoadItems()
        {
            flpItems.Controls.Clear();
            var items = _playlistRepo.GetItems(_playlist.Id);

            lblItemCount.Text = $"{items.Count} mục";
            lblEmpty.Visible = items.Count == 0;
            flpItems.Visible = items.Count > 0;

            int index = 0;
            foreach (var item in items)
            {
                var row = CreateItemRow(item, index);
                flpItems.Controls.Add(row);
                index++;
            }
        }

        private Guna2Panel CreateItemRow(PlaylistItem item, int index)
        {
            string title = "";
            string coverPath = "";
            string typeBadge = "";

            if (item.ItemType == PlaylistItemType.Movie)
            {
                var movie = _movieRepo.GetById(item.ItemId);
                if (movie != null)
                {
                    title = movie.MovieCode;
                    coverPath = movie.CoverImage ?? "";
                    typeBadge = "🎬 Phim";
                }
                else
                {
                    title = "(Phim đã bị xóa)";
                    typeBadge = "🎬 Phim";
                }
            }
            else
            {
                var audio = _audioRepo.GetById(item.ItemId, false);
                if (audio != null)
                {
                    title = audio.AudioCode;
                    coverPath = audio.CoverImage ?? "";
                    typeBadge = "🎵 Audio";
                }
                else
                {
                    title = "(Audio đã bị xóa)";
                    typeBadge = "🎵 Audio";
                }
            }

            var pnlRow = new Guna2Panel
            {
                Width = flpItems.Width - 60,
                Height = 80,
                BorderRadius = 12,
                FillColor = UIHelper.BgCard,
                Margin = new Padding(5, 5, 5, 5),
                Cursor = Cursors.Default
            };

            // Thumbnail
            var picThumb = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = UIHelper.BgPanel
            };
            if (!string.IsNullOrEmpty(coverPath))
            {
                string fullPath = FileHelper.GetFullPath(coverPath);
                picThumb.Image = FileHelper.LoadImageSafe(fullPath);
            }

            // Title
            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = UIHelper.TextPrimary,
                BackColor = Color.Transparent,
                Location = new Point(85, 12),
                AutoSize = false,
                AutoEllipsis = true,
                Size = new Size(pnlRow.Width - 360, 25)
            };

            // Type badge
            var lblType = new Label
            {
                Text = typeBadge,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = UIHelper.TextMuted,
                BackColor = Color.Transparent,
                Location = new Point(85, 42),
                AutoSize = true
            };

            // Order label
            var lblOrder = new Label
            {
                Text = $"#{index + 1}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = UIHelper.AccentSecondary,
                BackColor = Color.Transparent,
                AutoSize = true
            };
            lblOrder.Location = new Point(pnlRow.Width - 260, 28);

            // Move Up button
            var btnUp = new Guna2Button
            {
                Text = "▲",
                Size = new Size(36, 36),
                BorderRadius = 8,
                FillColor = UIHelper.BgPanel,
                ForeColor = UIHelper.TextSecondary,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnUp.Location = new Point(pnlRow.Width - 200, 22);
            btnUp.HoverState.FillColor = UIHelper.AccentPrimary;
            btnUp.HoverState.ForeColor = Color.White;
            int currentIndex = index;
            btnUp.Click += (s, e) => MoveItem(currentIndex, -1);

            // Move Down button
            var btnDown = new Guna2Button
            {
                Text = "▼",
                Size = new Size(36, 36),
                BorderRadius = 8,
                FillColor = UIHelper.BgPanel,
                ForeColor = UIHelper.TextSecondary,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDown.Location = new Point(pnlRow.Width - 158, 22);
            btnDown.HoverState.FillColor = UIHelper.AccentPrimary;
            btnDown.HoverState.ForeColor = Color.White;
            btnDown.Click += (s, e) => MoveItem(currentIndex, 1);

            // Delete button
            var btnRemove = new Guna2Button
            {
                Text = "Xóa",
                Size = new Size(70, 36),
                BorderRadius = 8,
                FillColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRemove.Location = new Point(pnlRow.Width - 100, 22);
            int itemDbId = item.Id;
            btnRemove.Click += (s, e) =>
            {
                _playlistRepo.RemoveItem(itemDbId);
                LoadItems();
            };

            // Hover effect
            pnlRow.MouseEnter += (s, e) => pnlRow.FillColor = UIHelper.BgCardHover;
            pnlRow.MouseLeave += (s, e) => pnlRow.FillColor = UIHelper.BgCard;

            pnlRow.Controls.Add(picThumb);
            pnlRow.Controls.Add(lblTitle);
            pnlRow.Controls.Add(lblType);
            pnlRow.Controls.Add(lblOrder);
            pnlRow.Controls.Add(btnUp);
            pnlRow.Controls.Add(btnDown);
            pnlRow.Controls.Add(btnRemove);

            return pnlRow;
        }

        private void MoveItem(int currentIndex, int direction)
        {
            var items = _playlistRepo.GetItems(_playlist.Id);
            int targetIndex = currentIndex + direction;
            if (targetIndex < 0 || targetIndex >= items.Count) return;

            // Swap sort orders
            var currentItem = items[currentIndex];
            var targetItem = items[targetIndex];

            int tempOrder = currentItem.SortOrder;
            _playlistRepo.UpdateSortOrder(currentItem.Id, targetItem.SortOrder);
            _playlistRepo.UpdateSortOrder(targetItem.Id, tempOrder);

            LoadItems();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
