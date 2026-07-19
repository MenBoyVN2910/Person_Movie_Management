using System;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Guna.UI2.WinForms;

namespace Person_Movie_Management.Forms
{
    public partial class FrmSelectPlaylist : Form
    {
        private PlaylistRepository _playlistRepo;
        private int _userId;
        private int _itemId;
        private PlaylistItemType _itemType;

        public FrmSelectPlaylist(int userId, int itemId, PlaylistItemType itemType)
        {
            InitializeComponent();
            _playlistRepo = new PlaylistRepository();
            _userId = userId;
            _itemId = itemId;
            _itemType = itemType;

            LoadPlaylists();
        }

        private void LoadPlaylists()
        {
            flpPlaylists.Controls.Clear();
            var playlists = _playlistRepo.GetAllByUser(_userId);

            if (playlists.Count == 0)
            {
                var lblEmpty = new Label
                {
                    Text = "Bạn chưa có playlist nào.\nHãy tạo mới ở bên dưới!",
                    Font = new Font("Segoe UI", 11, FontStyle.Regular),
                    ForeColor = UIHelper.TextMuted,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false,
                    Width = 340,
                    Height = 80,
                    Margin = new Padding(5, 30, 5, 5)
                };
                flpPlaylists.Controls.Add(lblEmpty);
                return;
            }

            foreach (var pl in playlists)
            {
                bool alreadyExists = _playlistRepo.ItemExists(pl.Id, _itemId, _itemType);
                var btnPlaylist = CreatePlaylistButton(pl, alreadyExists);
                flpPlaylists.Controls.Add(btnPlaylist);
            }
        }

        private Guna2Button CreatePlaylistButton(Playlist playlist, bool alreadyExists)
        {
            int itemCount = _playlistRepo.GetItemCount(playlist.Id);
            string statusText = alreadyExists ? "  ✓ Đã thêm" : "";

            var btn = new Guna2Button
            {
                Text = $"  {playlist.Name}   ({itemCount} mục){statusText}",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = alreadyExists ? UIHelper.TextMuted : UIHelper.TextPrimary,
                FillColor = alreadyExists ? Color.FromArgb(20, UIHelper.Success) : UIHelper.BgCard,
                BorderRadius = 10,
                Width = 340,
                Height = 50,
                Margin = new Padding(5, 5, 5, 5),
                Cursor = alreadyExists ? Cursors.No : Cursors.Hand,
                TextAlign = HorizontalAlignment.Left,
                ImageAlign = HorizontalAlignment.Left,
                Enabled = !alreadyExists
            };

            btn.HoverState.FillColor = UIHelper.BgCardHover;
            btn.HoverState.ForeColor = Color.White;

            if (!alreadyExists)
            {
                int plId = playlist.Id;
                btn.Click += (s, e) =>
                {
                    int sortOrder = _playlistRepo.GetNextSortOrder(plId);
                    _playlistRepo.AddItem(new PlaylistItem
                    {
                        PlaylistId = plId,
                        ItemId = _itemId,
                        ItemType = _itemType,
                        SortOrder = sortOrder
                    });

                    MessageBox.Show($"Đã thêm vào playlist \"{playlist.Name}\"!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };
            }

            return btn;
        }

        private void btnNewPlaylist_Click(object sender, EventArgs e)
        {
            var frmInput = new FrmInputBox("Tạo Playlist", "Nhập tên Playlist mới:");
            if (frmInput.ShowDialog() == DialogResult.OK)
            {
                string name = frmInput.InputValue;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    var pl = new Playlist
                    {
                        UserId = _userId,
                        Name = name,
                        Description = "",
                        CreatedAt = DateTime.Now
                    };
                    int newId = _playlistRepo.Insert(pl);

                    // Also add item to this new playlist
                    int sortOrder = _playlistRepo.GetNextSortOrder(newId);
                    _playlistRepo.AddItem(new PlaylistItem
                    {
                        PlaylistId = newId,
                        ItemId = _itemId,
                        ItemType = _itemType,
                        SortOrder = sortOrder
                    });

                    MessageBox.Show($"Đã tạo playlist \"{name}\" và thêm mục vào!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }
}
