using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;

namespace Person_Movie_Management.Forms
{
    public partial class FrmAudioDetail : Form
    {
        private Audio _audio;
        private readonly AudioRepository _audioRepo;
        private string? _selectedCoverPath;
        private string? _selectedAudioPath;
        private byte[]? _pendingAudioData;

        public FrmAudioDetail(Audio? audio = null)
        {
            InitializeComponent();
            _audioRepo = new AudioRepository();
            
            this.BackColor = UIHelper.BgDark;
            pnlMain.FillColor = UIHelper.BgCard;
            pnlMain.FillColor2 = UIHelper.BgPanel;
            
            btnSave.FillColor = UIHelper.GradEmerald1;
            btnCancel.FillColor = UIHelper.GradRose1;
            
            if (audio == null)
            {
                _audio = new Audio { UserId = SessionManager.CurrentUser!.Id };
                lblTitle.Text = "🎵 Thêm Âm Thanh";
            }
            else
            {
                _audio = audio;
                lblTitle.Text = "🎵 Sửa Âm Thanh";
                LoadAudioData();
            }
        }

        private void LoadAudioData()
        {
            txtAudioCode.Text = _audio.AudioCode;
            txtNote.Text = _audio.Note;
            
            if (!string.IsNullOrEmpty(_audio.CoverImage))
            {
                string fullPath = FileHelper.GetFullPath(_audio.CoverImage);
                if (System.IO.File.Exists(fullPath))
                {
                    try { picCover.Image = FileHelper.LoadImageSafe(fullPath); } catch { }
                }
            }
            
            if (_audio.Id > 0)
            {
                lblSelectedAudio.Text = "Đã có file âm thanh trong hệ thống (chọn lại nếu muốn đổi)";
                lblSelectedAudio.ForeColor = UIHelper.TextMuted;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string newCode = txtAudioCode.Text.Trim();
            if (string.IsNullOrWhiteSpace(newCode))
            {
                MessageBox.Show("Vui lòng nhập tên/mã âm thanh.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (_audio.Id == 0 && _pendingAudioData == null)
            {
                MessageBox.Show("Vui lòng chọn một file âm thanh (.mp3) để tải lên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existing = _audioRepo.GetByCode(SessionManager.CurrentUser!.Id, newCode);
            if (existing != null && existing.Id != _audio.Id)
            {
                MessageBox.Show("Tên âm thanh này đã tồn tại trong danh sách của bạn! Vui lòng chọn tên khác.", "Trùng lặp dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _audio.AudioCode = newCode;
            _audio.Note = txtNote.Text;

            if (_pendingAudioData != null)
            {
                _audio.AudioData = _pendingAudioData;
            }

            if (!string.IsNullOrEmpty(_selectedCoverPath))
            {
                _audio.CoverImage = FileHelper.CopyCoverImage(_selectedCoverPath, _audio.AudioCode);
            }

            if (_audio.Id == 0)
            {
                _audioRepo.Insert(_audio);
            }
            else
            {
                _audioRepo.Update(_audio);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void picCover_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _selectedCoverPath = ofd.FileName;
                picCover.Image = FileHelper.LoadImageSafe(_selectedCoverPath);
            }
        }

        private void btnChooseAudio_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Audio Files|*.mp3";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var fileInfo = new FileInfo(ofd.FileName);
                if (fileInfo.Length > 50 * 1024 * 1024)
                {
                    MessageBox.Show("File quá lớn! Dung lượng tối đa cho phép là 50MB.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                try 
                {
                    var tfile = TagLib.File.Create(ofd.FileName);
                    if (tfile.Properties.Duration.TotalMinutes > 20)
                    {
                        MessageBox.Show($"File quá dài ({tfile.Properties.Duration.TotalMinutes:F1} phút)! Thời lượng tối đa là 20 phút.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể đọc thông tin file âm thanh. File có thể bị lỗi hoặc định dạng không đúng. Chi tiết: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _selectedAudioPath = ofd.FileName;
                lblSelectedAudio.Text = Path.GetFileName(_selectedAudioPath);
                lblSelectedAudio.ForeColor = UIHelper.Success;
                
                if (string.IsNullOrWhiteSpace(txtAudioCode.Text))
                {
                    txtAudioCode.Text = Path.GetFileNameWithoutExtension(_selectedAudioPath);
                }
                
                _pendingAudioData = System.IO.File.ReadAllBytes(_selectedAudioPath);
            }
        }
    }
}
