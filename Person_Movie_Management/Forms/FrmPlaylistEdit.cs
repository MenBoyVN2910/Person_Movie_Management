using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;

namespace Person_Movie_Management.Forms
{
    public partial class FrmPlaylistEdit : Form
    {
        private Playlist? _editingPlaylist;
        private string? _selectedCoverPath;

        /// <summary>Result playlist after Save. Null if cancelled.</summary>
        public Playlist? ResultPlaylist { get; private set; }

        /// <summary>Create mode</summary>
        public FrmPlaylistEdit(int userId)
        {
            InitializeComponent();
            txtName.MaxLength = 50;
            _editingPlaylist = new Playlist { UserId = userId, CreatedAt = DateTime.Now, IsPrivate = false };
            lblTitle.Text = "✨ Tạo Playlist Mới";
            SetPlaceholderCover();
        }

        /// <summary>Edit mode</summary>
        public FrmPlaylistEdit(Playlist existing)
        {
            InitializeComponent();
            _editingPlaylist = existing;
            lblTitle.Text = "✏️ Chỉnh Sửa Playlist";
            LoadExistingData();
        }

        private void SetPlaceholderCover()
        {
            if (string.IsNullOrEmpty(_selectedCoverPath))
            {
                var bmp = new Bitmap(130, 98);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using var brush = new LinearGradientBrush(
                        new Rectangle(0, 0, 130, 98),
                        Color.FromArgb(49, 46, 129),
                        Color.FromArgb(30, 27, 75),
                        45f);
                    g.FillRectangle(brush, 0, 0, 130, 98);

                    using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("🖼️", new Font("Segoe UI Emoji", 20F), Brushes.White, new RectangleF(0, 15, 130, 40), sf);
                    g.DrawString("Chọn ảnh", new Font("Segoe UI", 8.5F, FontStyle.Bold), new SolidBrush(Color.FromArgb(199, 210, 254)), new RectangleF(0, 58, 130, 25), sf);
                }
                picCover.Image?.Dispose();
                picCover.Image = bmp;
                picCover.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void LoadExistingData()
        {
            if (_editingPlaylist == null) return;
            txtName.Text = _editingPlaylist.Name;
            txtDesc.Text = _editingPlaylist.Description ?? "";
            _selectedCoverPath = _editingPlaylist.CoverImage;

            if (!string.IsNullOrEmpty(_selectedCoverPath))
            {
                string fullPath = FileHelper.GetFullPath(_selectedCoverPath);
                var img = FileHelper.LoadImageSafe(fullPath);
                if (img != null)
                {
                    picCover.Image = UIHelper.CropToFill(img, 130, 98);
                    picCover.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else
                {
                    SetPlaceholderCover();
                }
            }
            else
            {
                SetPlaceholderCover();
            }

            int len = txtDesc.Text.Length;
            lblDescCount.Text = $"{len} / 200";
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            int len = txtDesc.Text.Length;
            lblDescCount.Text = $"{len} / 200";
            lblDescCount.ForeColor = len >= 180
                ? Color.FromArgb(239, 68, 68)
                : Color.FromArgb(100, 116, 139);
        }

        private void picCover_Click(object sender, EventArgs e)
        {
            btnPickCover_Click(sender, e);
        }

        private void btnPickCover_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Chọn ảnh bìa",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.webp;*.gif|All Files|*.*"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _selectedCoverPath = dlg.FileName;
                var img = FileHelper.LoadImageSafe(_selectedCoverPath);
                if (img != null)
                {
                    picCover.Image?.Dispose();
                    picCover.Image = UIHelper.CropToFill(img, 130, 98);
                    picCover.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }

        private void btnClearCover_Click(object sender, EventArgs e)
        {
            _selectedCoverPath = null;
            picCover.Image?.Dispose();
            picCover.Image = null;
            SetPlaceholderCover();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Tên playlist không được để trống!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            if (_editingPlaylist == null) return;

            _editingPlaylist.Name = name;
            _editingPlaylist.Description = string.IsNullOrWhiteSpace(txtDesc.Text) ? null : txtDesc.Text.Trim();
            _editingPlaylist.IsPrivate = false;
            _editingPlaylist.CoverImage = _selectedCoverPath;

            ResultPlaylist = _editingPlaylist;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
