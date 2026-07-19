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
    public partial class UcPlaylist : UserControl
    {
        private PlaylistRepository _playlistRepo;
        private int _currentUserId;

        public UcPlaylist(int userId)
        {
            InitializeComponent();
            _currentUserId = userId;
            _playlistRepo = new PlaylistRepository();
            
            this.BackColor = UIHelper.BgDark;
            flpPlaylists.BackColor = UIHelper.BgDark;
            pnlTop.BackColor = UIHelper.BgDark;
            lblTitle.ForeColor = UIHelper.TextPrimary;
            lblTitle.ForeColor = UIHelper.TextPrimary;
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

        private void UcPlaylist_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            flpPlaylists.Controls.Clear();
            var playlists = _playlistRepo.GetAllByUser(_currentUserId);
            
            foreach (var pl in playlists)
            {
                var card = CreatePlaylistCard(pl);
                flpPlaylists.Controls.Add(card);
            }
        }

        private Guna2Panel CreatePlaylistCard(Playlist playlist)
        {
            int itemCount = _playlistRepo.GetItemCount(playlist.Id);

            var pnlCard = new Guna2Panel
            {
                Width = 280,
                Height = 180,
                BorderRadius = 14,
                FillColor = UIHelper.BgCard,
                Margin = new Padding(15),
                Cursor = Cursors.Hand
            };

            // Header accent line
            var pnlAccent = new Panel
            {
                Size = new Size(pnlCard.Width - 30, 4),
                Location = new Point(15, 12),
                BackColor = UIHelper.AccentPrimary
            };

            var lblName = new Label
            {
                Text = playlist.Name,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = UIHelper.TextPrimary,
                BackColor = Color.Transparent,
                Location = new Point(15, 25),
                AutoSize = true
            };

            var lblDesc = new Label
            {
                Text = string.IsNullOrEmpty(playlist.Description) ? "Không có mô tả" : playlist.Description,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = UIHelper.TextMuted,
                BackColor = Color.Transparent,
                Location = new Point(15, 55),
                AutoSize = false,
                Width = 250,
                Height = 35
            };

            var lblCount = new Label
            {
                Text = $"📄 {itemCount} mục",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = UIHelper.TextSecondary,
                BackColor = Color.Transparent,
                Location = new Point(15, 95),
                AutoSize = true
            };

            var lblDate = new Label
            {
                Text = $"🕐 {playlist.CreatedAt:dd/MM/yyyy}",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = UIHelper.TextMuted,
                BackColor = Color.Transparent,
                Location = new Point(130, 95),
                AutoSize = true
            };

            // Edit button
            var btnEdit = new Guna2Button
            {
                Text = "Sửa",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderRadius = 8,
                FillColor = UIHelper.AccentPrimary,
                ForeColor = Color.White,
                Location = new Point(15, 130),
                Size = new Size(70, 32),
                Cursor = Cursors.Hand
            };
            btnEdit.Click += (s, e) => EditPlaylist(playlist);

            // Delete button
            var btnDelete = new Guna2Button
            {
                Text = "Xóa",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderRadius = 8,
                FillColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                Location = new Point(95, 130),
                Size = new Size(70, 32),
                Cursor = Cursors.Hand
            };
            btnDelete.Click += (s, e) => 
            {
                var confirm = MessageBox.Show($"Bạn muốn xóa Playlist '{playlist.Name}'?", "Xóa Playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    _playlistRepo.Delete(playlist.Id);
                    LoadData();
                }
            };

            pnlCard.Controls.Add(pnlAccent);
            pnlCard.Controls.Add(lblName);
            pnlCard.Controls.Add(lblDesc);
            pnlCard.Controls.Add(lblCount);
            pnlCard.Controls.Add(lblDate);
            pnlCard.Controls.Add(btnEdit);
            pnlCard.Controls.Add(btnDelete);

            // Hover effect
            pnlCard.MouseEnter += (s, e) => { pnlCard.FillColor = UIHelper.BgCardHover; };
            pnlCard.MouseLeave += (s, e) => { pnlCard.FillColor = UIHelper.BgCard; };

            // Click to view playlist detail
            pnlCard.Click += (s, e) => ViewPlaylist(playlist);
            lblName.Click += (s, e) => ViewPlaylist(playlist);
            lblDesc.Click += (s, e) => ViewPlaylist(playlist);
            lblCount.Click += (s, e) => ViewPlaylist(playlist);
            lblDate.Click += (s, e) => ViewPlaylist(playlist);

            return pnlCard;
        }

        private void EditPlaylist(Playlist playlist)
        {
            var frmInput = new Person_Movie_Management.Forms.FrmInputBox("Sửa Playlist", "Nhập tên mới:", playlist.Name);
            if (frmInput.ShowDialog() == DialogResult.OK)
            {
                string newName = frmInput.InputValue;
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    playlist.Name = newName;
                    _playlistRepo.Update(playlist);
                    LoadData();
                }
            }
        }
        
        private void ViewPlaylist(Playlist playlist)
        {
            // Get the parent panel (pnlContent in FrmMain)
            var parentPanel = this.Parent;
            if (parentPanel == null) return;

            var detail = new UcPlaylistDetail(playlist);
            detail.Dock = DockStyle.Fill;
            detail.BackRequested += (s, e) =>
            {
                parentPanel.Controls.Clear();
                this.Dock = DockStyle.Fill;
                parentPanel.Controls.Add(this);
                LoadData(); // Refresh after returning
            };

            parentPanel.Controls.Clear();
            parentPanel.Controls.Add(detail);
        }

        private void btnAddPlaylist_Click(object sender, EventArgs e)
        {
            var frmInput = new Person_Movie_Management.Forms.FrmInputBox("Tạo Playlist", "Nhập tên Playlist mới:");
            if (frmInput.ShowDialog() == DialogResult.OK)
            {
                string name = frmInput.InputValue;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var pl = new Playlist
                    {
                        UserId = _currentUserId,
                        Name = name,
                        Description = "",
                        CreatedAt = DateTime.Now
                    };
                    _playlistRepo.Insert(pl);
                    LoadData();
                }
            }
        }
    }
}
