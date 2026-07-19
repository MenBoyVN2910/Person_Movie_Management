using System;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;

namespace Person_Movie_Management.Forms
{
    public partial class FrmMovieDetail : Form
    {
        private Movie _movie;
        private readonly MovieRepository _movieRepo;
        private string? _selectedCoverPath;

        public FrmMovieDetail(Movie? movie = null)
        {
            InitializeComponent();
            _movieRepo = new MovieRepository();
            
            this.BackColor = UIHelper.BgDark;
            pnlMain.FillColor = UIHelper.BgCard;
            pnlMain.FillColor2 = UIHelper.BgPanel;
            
            btnSave.FillColor = UIHelper.GradEmerald1;
            btnCancel.FillColor = UIHelper.GradRose1;
            
            if (movie == null)
            {
                _movie = new Movie { UserId = SessionManager.CurrentUser!.Id };
                lblTitle.Text = "Thêm Phim Mới";
            }
            else
            {
                _movie = movie;
                lblTitle.Text = "Chỉnh Sửa Phim";
                LoadMovieData();
            }
        }

        private void LoadMovieData()
        {
            txtMovieCode.Text = _movie.MovieCode;
            cboSourceType.SelectedIndex = _movie.SourceType;
            txtMediaUrl.Text = _movie.MediaUrl;
            txtNote.Text = _movie.Note;
            
            if (!string.IsNullOrEmpty(_movie.CoverImage))
            {
                string fullPath = FileHelper.GetFullPath(_movie.CoverImage);
                if (System.IO.File.Exists(fullPath))
                {
                    picCover.Image = FileHelper.LoadImageSafe(fullPath);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string newCode = txtMovieCode.Text.Trim();
            if (string.IsNullOrWhiteSpace(newCode))
            {
                MessageBox.Show("Vui lòng nhập mã phim.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existing = _movieRepo.GetByCode(SessionManager.CurrentUser!.Id, newCode);
            if (existing != null && existing.Id != _movie.Id)
            {
                MessageBox.Show("Tên phim này đã tồn tại trong danh sách của bạn! Vui lòng chọn tên khác.", "Trùng lặp dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _movie.MovieCode = newCode;
            _movie.SourceType = cboSourceType.SelectedIndex;
            _movie.MediaUrl = txtMediaUrl.Text;
            _movie.Note = txtNote.Text;

            if (!string.IsNullOrEmpty(_selectedCoverPath))
            {
                _movie.CoverImage = FileHelper.CopyCoverImage(_selectedCoverPath, _movie.MovieCode);
            }

            if (_movie.Id == 0)
            {
                _movieRepo.Insert(_movie);
            }
            else
            {
                _movieRepo.Update(_movie);
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
    }
}
