using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Person_Movie_Management.Services;
using Person_Movie_Management.Forms;

namespace Person_Movie_Management.UserControls
{
    public partial class UcAudioList : UserControl
    {
        private List<Audio> _allAudios = new();
        private Guna.UI2.WinForms.Guna2GradientButton btnDeleteAll;
        private ToolTip _btnToolTip;

        public UcAudioList()
        {
            InitializeComponent();

            this.BackColor = UIHelper.BgDark;
            pnlTop.BackColor = UIHelper.BgDark;
            flowLayoutPanel.BackColor = UIHelper.BgDark;

            // Style search box
            txtSearch.FillColor = UIHelper.BgCard;
            txtSearch.ForeColor = UIHelper.TextPrimary;
            txtSearch.BorderRadius = 12;
            txtSearch.FocusedState.BorderColor = UIHelper.AccentPrimary;
            txtSearch.Font = new Font("Segoe UI", 10F);
            
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txtSearch.Size = new Size(300, 42);
            
            pnlTop.Resize += (s, e) => {
                int startX = (pnlTop.Width - txtSearch.Width) / 2;
                if (startX < 240) startX = 240;
                txtSearch.Location = new Point(startX, 18);
            };
            
            int initStartX = (pnlTop.Width - 300) / 2;
            if (initStartX < 240) initStartX = 240;
            txtSearch.Location = new Point(initStartX, 18);

            // Style action button
            btnAction.BorderRadius = 12;
            btnAction.FillColor = UIHelper.AccentPrimary;
            btnAction.FillColor2 = UIHelper.AccentTertiary;
            btnAction.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnAction.Animated = true;

            lblEmpty.ForeColor = UIHelper.TextMuted;
            lblEmpty.Font = new Font("Segoe UI", 12F, FontStyle.Regular);

            btnDeleteAll = new Guna.UI2.WinForms.Guna2GradientButton();
            btnDeleteAll.Text = "🗑";
            btnDeleteAll.Size = new Size(50, 42);
            btnDeleteAll.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnDeleteAll.BorderRadius = 12;
            btnDeleteAll.FillColor = Color.FromArgb(239, 68, 68); // Red-500
            btnDeleteAll.FillColor2 = Color.FromArgb(220, 38, 38); // Red-600
            btnDeleteAll.Font = new Font("Segoe UI", 16F);
            btnDeleteAll.ForeColor = Color.White;
            btnDeleteAll.Click += BtnDeleteAll_Click;
            pnlTop.Controls.Add(btnDeleteAll);
            
            // Adjust buttons to NOT rely on designer anchors which might cause conflict
            btnAction.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnAction.Size = new Size(150, 42); // Adjust size for text

            btnImport.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnImport.Size = new Size(50, 42);
            btnImport.Text = "📥";
            btnImport.Font = new Font("Segoe UI", 16F);
            btnExport.Size = new Size(50, 42);
            btnExport.Text = "📤";
            btnExport.Font = new Font("Segoe UI", 16F);

            // Add tooltips
            _btnToolTip = new ToolTip();
            _btnToolTip.SetToolTip(btnDeleteAll, "Xóa Tất Cả Âm Thanh");
            _btnToolTip.SetToolTip(btnImport, "Nhập Dữ Liệu Từ File");
            _btnToolTip.SetToolTip(btnExport, "Xuất Dữ Liệu Ra File");
            _btnToolTip.SetToolTip(btnAction, "Thêm Âm Thanh Mới");
            
            pnlTop.Resize += (s, e) => {
                LayoutButtons();
            };
            
            LayoutButtons();

            LoadData();
        }

        private void LayoutButtons()
        {
            int currentX = pnlTop.Width - 25;

            if (btnAction != null && btnAction.Visible)
            {
                currentX -= btnAction.Width;
                btnAction.Location = new Point(currentX, 18);
                currentX -= 10;
            }
            if (btnImport != null && btnImport.Visible)
            {
                currentX -= btnImport.Width;
                btnImport.Location = new Point(currentX, 18);
                currentX -= 10;
            }
            if (btnExport != null && btnExport.Visible)
            {
                currentX -= btnExport.Width;
                btnExport.Location = new Point(currentX, 18);
                currentX -= 10;
            }
            if (btnDeleteAll != null && btnDeleteAll.Visible)
            {
                currentX -= btnDeleteAll.Width;
                btnDeleteAll.Location = new Point(currentX, 18);
            }
            
            // Re-center search box if needed
            if (txtSearch != null)
            {
                int leftmostButtonX = pnlTop.Width;
                if (btnDeleteAll != null && btnDeleteAll.Visible) leftmostButtonX = btnDeleteAll.Left;
                else if (btnExport != null && btnExport.Visible) leftmostButtonX = btnExport.Left;
                else if (btnImport != null && btnImport.Visible) leftmostButtonX = btnImport.Left;
                else if (btnAction != null && btnAction.Visible) leftmostButtonX = btnAction.Left;
                
                int availableSpace = leftmostButtonX - 240;
                if (availableSpace < 300) availableSpace = 300;
                
                int startX = 240 + (availableSpace - 300) / 2;
                if (startX < 240) startX = 240;
                
                txtSearch.Location = new Point(startX, 18);
            }
        }

        private void BtnDeleteAll_Click(object? sender, EventArgs e)
        {
            if (_allAudios.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var inputDialog = new FrmInputBox("Xác nhận xóa", "Nhập 'delete' để xóa TẤT CẢ âm thanh:");
            if (inputDialog.ShowDialog() == DialogResult.OK)
            {
                if (inputDialog.InputValue.Trim().ToLower() == "delete")
                {
                    if (SessionManager.IsLoggedIn)
                    {
                        AppServices.AudioRepo.DeleteAll(SessionManager.CurrentUser!.Id);
                        MessageBox.Show("Đã xóa tất cả thành công. Các mục này đã được đưa vào Thùng Rác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                }
                else
                {
                    MessageBox.Show("Xác nhận không hợp lệ. Hủy thao tác xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadData()
        {
            if (!SessionManager.IsLoggedIn) return;

            int userId = SessionManager.CurrentUser!.Id;
            _allAudios = AppServices.AudioRepo.GetAllByUser(userId, false); // Don't load BLOB for list view
            DisplayAudios(_allAudios);
        }

        private void DisplayAudios(List<Audio> audios)
        {
            lblEmpty.Visible = audios.Count == 0;
            
            flowLayoutPanel.AudioClicked -= Card_AudioClicked;
            flowLayoutPanel.AudioFavoriteToggled -= Card_FavoriteToggled;
            flowLayoutPanel.AudioEditClicked -= Card_EditClicked;
            flowLayoutPanel.AudioDeleteClicked -= Card_DeleteClicked;
            
            flowLayoutPanel.AudioClicked += Card_AudioClicked;
            flowLayoutPanel.AudioFavoriteToggled += Card_FavoriteToggled;
            flowLayoutPanel.AudioEditClicked += Card_EditClicked;
            flowLayoutPanel.AudioDeleteClicked += Card_DeleteClicked;

            var items = audios.Cast<object>().ToList();
            flowLayoutPanel.SetData(items, null);
        }

        private void Card_AudioClicked(object? sender, Audio audio)
        {
            // Load full audio including BLOB data
            var fullAudio = AppServices.AudioRepo.GetById(audio.Id, true);
            if (fullAudio != null && fullAudio.AudioData != null && fullAudio.AudioData.Length > 0)
            {
                try
                {
                    if (this.ParentForm is FrmMain mainForm)
                    {
                        mainForm.PlayGlobalAudio(fullAudio.AudioData, audio.AudioCode);
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

        private void Card_FavoriteToggled(object? sender, Audio audio)
        {
            // No strict favorites view right now, but handled if needed.
        }

        private void Card_EditClicked(object? sender, Audio audio)
        {
            // Need to load full audio so they don't overwrite AudioData with null by mistake?
            // Actually, Edit detail might just keep existing data unless they pick a new file.
            // Let's pass the audio without data, and FrmAudioDetail only updates AudioData if a new file is chosen.
            Forms.FrmAudioDetail frm = new Forms.FrmAudioDetail(audio);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void Card_DeleteClicked(object? sender, Audio audio)
        {
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa '{audio.AudioCode}' không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (AppServices.AudioRepo.Delete(audio.Id))
                {
                    DataCache.Invalidate();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                DisplayAudios(_allAudios);
            }
            else
            {
                var filtered = _allAudios.Where(a => 
                    a.AudioCode.ToLower().Contains(keyword) || 
                    (a.Note != null && a.Note.ToLower().Contains(keyword))
                ).ToList();
                DisplayAudios(filtered);
            }
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            Forms.FrmAudioDetail frm = new Forms.FrmAudioDetail();
            if (frm.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (_allAudios == null || _allAudios.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "Backup files (*.zip)|*.zip";
            sfd.FileName = "AmThanh_Export.zip";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Load full data for export
                    int userId = SessionManager.CurrentUser!.Id;
                    var audiosToExport = AppServices.AudioRepo.GetAllByUser(userId, true);

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string jsonString = JsonSerializer.Serialize(audiosToExport, options);

                    string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    Directory.CreateDirectory(tempDir);

                    File.WriteAllText(Path.Combine(tempDir, "data_audio.json"), jsonString);

                    foreach (var audio in audiosToExport)
                    {
                        // Save BLOBs as separate files to avoid massive JSON
                        if (audio.AudioData != null && audio.AudioData.Length > 0)
                        {
                            File.WriteAllBytes(Path.Combine(tempDir, $"audio_{audio.Id}.bin"), audio.AudioData);
                        }

                        // Save cover image
                        if (!string.IsNullOrEmpty(audio.CoverImage))
                        {
                            string imgPath = FileHelper.GetFullPath(audio.CoverImage);
                            if (File.Exists(imgPath))
                            {
                                string destImg = Path.Combine(tempDir, audio.CoverImage);
                                Directory.CreateDirectory(Path.GetDirectoryName(destImg)!);
                                File.Copy(imgPath, destImg, true);
                            }
                        }
                    }

                    if (File.Exists(sfd.FileName)) File.Delete(sfd.FileName);
                    System.IO.Compression.ZipFile.CreateFromDirectory(tempDir, sfd.FileName);
                    Directory.Delete(tempDir, true);
                    
                    MessageBox.Show("Xuất dữ liệu thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra khi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (!SessionManager.IsLoggedIn) return;

            using var ofd = new OpenFileDialog();
            ofd.Filter = "Backup files (*.zip)|*.zip";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string jsonString = "";
                    string? tempDir = null;

                    tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    System.IO.Compression.ZipFile.ExtractToDirectory(ofd.FileName, tempDir);

                    string jsonFile = Path.Combine(tempDir, "data_audio.json");
                    if (File.Exists(jsonFile))
                    {
                        jsonString = File.ReadAllText(jsonFile);
                    }
                    else
                    {
                        throw new Exception("Không tìm thấy file data_audio.json trong bản sao lưu.");
                    }

                    var importedAudios = JsonSerializer.Deserialize<List<Audio>>(jsonString);

                    if (importedAudios != null && importedAudios.Count > 0)
                    {
                        int currentUserId = SessionManager.CurrentUser!.Id;
                        int importedCount = 0;

                        foreach (var audio in importedAudios)
                        {
                            // Avoid duplicates by AudioCode
                            var existingAudio = AppServices.AudioRepo.GetByCode(currentUserId, audio.AudioCode);
                            if (existingAudio == null)
                            {
                                int oldId = audio.Id;
                                
                                // Load binary data
                                string binPath = Path.Combine(tempDir, $"audio_{oldId}.bin");
                                if (File.Exists(binPath))
                                {
                                    audio.AudioData = File.ReadAllBytes(binPath);
                                }

                                // Load cover image
                                if (!string.IsNullOrEmpty(audio.CoverImage))
                                {
                                    string srcImg = Path.Combine(tempDir, audio.CoverImage);
                                    if (File.Exists(srcImg))
                                    {
                                        string newCoverPath = FileHelper.CopyCoverImage(srcImg, audio.AudioCode);
                                        audio.CoverImage = newCoverPath;
                                    }
                                }

                                audio.Id = 0;
                                audio.UserId = currentUserId;
                                audio.CreatedAt = DateTime.Now;

                                if (AppServices.AudioRepo.Insert(audio)) importedCount++;
                            }
                        }

                        MessageBox.Show($"Đã nhập thành công {importedCount} âm thanh.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    
                    if (tempDir != null && Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra khi nhập file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
