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
            
            txtName.MaxLength = 50; // Giới hạn tên diễn viên 50 ký tự
            InitNationalities();
            SetupBioCharCounter();
            LoadData();
        }

        private void InitNationalities()
        {
            string currentVal = cboNationality.Text;
            cboNationality.Items.Clear();
            var list = new List<string>();

            // Load distinct custom nationalities from DB (không dùng danh sách mặc định cứng)
            if (SessionManager.IsLoggedIn && SessionManager.CurrentUser != null)
            {
                list = AppServices.ActorRepo.GetDistinctNationalities(SessionManager.CurrentUser.Id);
            }

            foreach (var item in list)
            {
                cboNationality.Items.Add(item);
            }

            if (!string.IsNullOrEmpty(currentVal))
            {
                cboNationality.Text = currentVal;
            }
        }

        private void btnManageNationality_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsLoggedIn || SessionManager.CurrentUser == null) return;
            var frm = new FrmNationalityManager(SessionManager.CurrentUser.Id);
            frm.ShowDialog(this);
            InitNationalities();
        }

        private void SetupBioCharCounter()
        {
            txtBio.MaxLength = 500;
            txtBio.TextChanged += (s, e) =>
            {
                int count = txtBio.Text.Length;
                lblCharCount.Text = $"{count} / 500 ký tự";
                if (count > 500)
                {
                    lblCharCount.ForeColor = Color.FromArgb(239, 68, 68); // Red
                }
                else
                {
                    lblCharCount.ForeColor = Color.FromArgb(148, 163, 184); // Slate 400
                }
            };
        }

        private void LoadData()
        {
            if (_currentActor != null)
            {
                lblTitle.Text = "💃 Chỉnh sửa Diễn Viên";
                btnDelete.Visible = true;
                
                txtName.Text = _currentActor.Name;
                
                if (!string.IsNullOrWhiteSpace(_currentActor.DateOfBirth) && DateTime.TryParse(_currentActor.DateOfBirth, out var parsedDate))
                {
                    dtpDateOfBirth.Value = parsedDate;
                    dtpDateOfBirth.Checked = true;
                }
                else
                {
                    dtpDateOfBirth.Checked = false;
                }

                cboNationality.Text = _currentActor.Nationality ?? string.Empty;
                txtBio.Text = _currentActor.Bio ?? string.Empty;
                lblCharCount.Text = $"{txtBio.Text.Length} / 500 ký tự";
                
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
                dtpDateOfBirth.Checked = false;
                cboNationality.SelectedIndex = -1;
                cboNationality.Text = string.Empty;
                txtBio.Text = string.Empty;
                lblCharCount.Text = "0 / 500 ký tự";
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
                    if (img != null)
                    {
                        picAvatar.Image?.Dispose();
                        picAvatar.Image = new Bitmap(img);
                        img.Dispose();
                    }
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
                if (img != null)
                {
                    pic.Image = new Bitmap(img);
                    img.Dispose();
                }
                else return;
            }
            catch { return; }

            // Click to remove
            pic.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn muốn xoá ảnh này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    pnlGallery.Controls.Remove(pic);
                    pic.Image?.Dispose();
                    pic.Dispose();
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
                    if (img != null)
                    {
                        picAvatar.Image?.Dispose();
                        picAvatar.Image = new Bitmap(img);
                        img.Dispose();
                    }
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
            string trimmedName = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                MessageBox.Show("Vui lòng nhập tên diễn viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            int excludeId = _currentActor?.Id ?? 0;
            if (SessionManager.IsLoggedIn && SessionManager.CurrentUser != null)
            {
                var duplicate = AppServices.ActorRepo.GetByName(SessionManager.CurrentUser.Id, trimmedName, excludeId);
                if (duplicate != null)
                {
                    MessageBox.Show($"Diễn viên có tên '{trimmedName}' đã tồn tại trong danh sách của bạn. Vui lòng nhập tên khác.", "Trùng tên diễn viên", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }
            }

            if (txtBio.Text.Length > 500)
            {
                MessageBox.Show($"Ghi chú hiện tại có {txtBio.Text.Length} ký tự, đã vượt quá giới hạn 500 ký tự. Vui lòng rút gọn lại.", "Vượt quá giới hạn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBio.Focus();
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
            _currentActor.DateOfBirth = dtpDateOfBirth.Checked ? dtpDateOfBirth.Value.ToString("yyyy-MM-dd") : null;
            _currentActor.Nationality = string.IsNullOrWhiteSpace(cboNationality.Text) ? null : cboNationality.Text.Trim();
            _currentActor.Bio = string.IsNullOrWhiteSpace(txtBio.Text) ? null : txtBio.Text.Trim();

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
