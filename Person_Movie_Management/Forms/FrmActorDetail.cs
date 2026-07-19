using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Person_Movie_Management.Models;
using Person_Movie_Management.Services;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.Forms
{
    public partial class FrmActorDetail : Form
    {
        private Actor? _currentActor;
        private string? _avatarPath;
        private List<string> _galleryPaths = new List<string>();
        private List<ActorImage> _existingImages = new List<ActorImage>();
        private List<int> _imagesToDelete = new List<int>();

        public FrmActorDetail(Actor? actor)
        {
            InitializeComponent();
            _currentActor = actor;
            
            LoadData();
        }

        private void LoadData()
        {
            if (_currentActor != null)
            {
                lblTitle.Text = "💃 Chỉnh sửa Diễn Viên";
                btnDelete.Visible = true;
                
                txtName.Text = _currentActor.Name;
                txtDateOfBirth.Text = _currentActor.DateOfBirth;
                txtNationality.Text = _currentActor.Nationality;
                txtBio.Text = _currentActor.Bio;
                
                _avatarPath = _currentActor.AvatarPath;
                if (!string.IsNullOrEmpty(_avatarPath))
                {
                    LoadAvatar(_avatarPath);
                }

                // Load gallery
                _existingImages = AppServices.ActorRepo.GetImages(_currentActor.Id);
                foreach (var img in _existingImages)
                {
                    AddGalleryImageToUI(img.ImagePath, img.Id);
                }
            }
            else
            {
                lblTitle.Text = "💃 Thêm Diễn Viên Mới";
                btnDelete.Visible = false;
            }
        }

        private void LoadAvatar(string relativePath)
        {
            string fullPath = FileHelper.GetFullPath(relativePath);
            if (File.Exists(fullPath))
            {
                try
                {
                    var img = FileHelper.LoadImageSafe(fullPath);
                    picAvatar.Image = new Bitmap(img);
                    img.Dispose();
                }
                catch { }
            }
        }

        private void AddGalleryImageToUI(string path, int? existingId = null)
        {
            string fullPath = path;
            if (existingId.HasValue)
            {
                fullPath = FileHelper.GetFullPath(path);
            }

            if (!File.Exists(fullPath)) return;

            var pic = new PictureBox
            {
                Width = 100,
                Height = 100,
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(5),
                Cursor = Cursors.Hand,
                Tag = existingId.HasValue ? existingId.Value.ToString() : path
            };

            try
            {
                var img = FileHelper.LoadImageSafe(fullPath);
                pic.Image = new Bitmap(img);
                img.Dispose();
            }
            catch { return; }

            // Click to remove
            pic.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn muốn xoá ảnh này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    pnlGallery.Controls.Remove(pic);
                    if (existingId.HasValue)
                    {
                        _imagesToDelete.Add(existingId.Value);
                    }
                    else
                    {
                        _galleryPaths.Remove(path);
                    }
                }
            };

            pnlGallery.Controls.Add(pic);
        }

        private void picAvatar_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.webp;*.gif";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _avatarPath = ofd.FileName; // Temporary hold full path
                try
                {
                    var img = FileHelper.LoadImageSafe(_avatarPath);
                    picAvatar.Image = new Bitmap(img);
                    img.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải ảnh: " + ex.Message);
                }
            }
        }

        private void btnAddGalleryImage_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.webp;*.gif";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in ofd.FileNames)
                {
                    _galleryPaths.Add(file);
                    AddGalleryImageToUI(file);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên diễn viên.");
                return;
            }

            bool isNew = false;
            if (_currentActor == null)
            {
                isNew = true;
                _currentActor = new Actor
                {
                    UserId = SessionManager.CurrentUser!.Id
                };
            }

            _currentActor.Name = txtName.Text.Trim();
            _currentActor.DateOfBirth = txtDateOfBirth.Text.Trim();
            _currentActor.Nationality = txtNationality.Text.Trim();
            _currentActor.Bio = txtBio.Text.Trim();

            // Save Avatar
            if (_avatarPath != null && Path.IsPathRooted(_avatarPath))
            {
                string savedPath = FileHelper.CopyCoverImage(_avatarPath, "actor_" + _currentActor.Name);
                _currentActor.AvatarPath = savedPath;
            }

            if (isNew)
            {
                AppServices.ActorRepo.Insert(_currentActor);
            }
            else
            {
                AppServices.ActorRepo.Update(_currentActor);
            }

            // Save Gallery
            foreach (var newPath in _galleryPaths)
            {
                string savedGalleryPath = FileHelper.CopyDetailImage(newPath, "actor_" + _currentActor.Name);
                AppServices.ActorRepo.AddImage(new ActorImage
                {
                    ActorId = _currentActor.Id,
                    ImagePath = savedGalleryPath,
                    SortOrder = 0
                });
            }

            // Delete removed images
            foreach (var imgId in _imagesToDelete)
            {
                AppServices.ActorRepo.DeleteImage(imgId);
                // We should also delete the file physically but leaving it for now
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn xoá diễn viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (_currentActor != null)
                {
                    AppServices.ActorRepo.Delete(_currentActor.Id);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
