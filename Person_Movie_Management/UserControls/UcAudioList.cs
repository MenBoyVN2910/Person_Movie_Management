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

namespace Person_Movie_Management.UserControls
{
    public partial class UcAudioList : UserControl
    {
        private readonly AudioRepository _audioRepo;
        private List<Audio> _allAudios = new();

        public UcAudioList()
        {
            InitializeComponent();
            _audioRepo = new AudioRepository();

            this.BackColor = UIHelper.BgDark;
            pnlTop.BackColor = UIHelper.BgDark;
            flowLayoutPanel.BackColor = UIHelper.BgDark;

            // Style search box
            txtSearch.FillColor = UIHelper.BgCard;
            txtSearch.ForeColor = UIHelper.TextPrimary;
            txtSearch.BorderRadius = 12;
            txtSearch.FocusedState.BorderColor = UIHelper.AccentPrimary;
            txtSearch.Font = new Font("Segoe UI", 10F);

            // Style action button
            btnAction.BorderRadius = 12;
            btnAction.FillColor = UIHelper.AccentPrimary;
            btnAction.FillColor2 = UIHelper.AccentTertiary;
            btnAction.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnAction.Animated = true;

            lblEmpty.ForeColor = UIHelper.TextMuted;
            lblEmpty.Font = new Font("Segoe UI", 12F, FontStyle.Regular);

            LoadData();
        }

        private void LoadData()
        {
            if (!SessionManager.IsLoggedIn) return;

            int userId = SessionManager.CurrentUser!.Id;
            _allAudios = _audioRepo.GetAllByUser(userId, false); // Don't load BLOB for list view
            DisplayAudios(_allAudios);
        }

        private void DisplayAudios(List<Audio> audios)
        {
            flowLayoutPanel.Controls.Clear();
            lblEmpty.Visible = audios.Count == 0;

            foreach (var audio in audios)
            {
                var card = new UcAudioCard(audio);
                card.AudioClicked += Card_AudioClicked;
                card.FavoriteToggled += Card_FavoriteToggled;
                card.EditClicked += Card_EditClicked;
                card.DeleteClicked += Card_DeleteClicked;
                flowLayoutPanel.Controls.Add(card);
            }
        }

        private void Card_AudioClicked(object? sender, Audio audio)
        {
            // Load full audio including BLOB data
            var fullAudio = _audioRepo.GetById(audio.Id, true);
            if (fullAudio != null && fullAudio.AudioData != null && fullAudio.AudioData.Length > 0)
            {
                try
                {
                    string tempFile = Path.Combine(Path.GetTempPath(), $"temp_audio_{Guid.NewGuid()}.mp3");
                    File.WriteAllBytes(tempFile, fullAudio.AudioData);
                    MediaLauncher.LaunchMedia(tempFile, 1);
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
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa âm thanh '{audio.AudioCode}' không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_audioRepo.Delete(audio.Id))
                {
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Xóa âm thanh thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    var audiosToExport = _audioRepo.GetAllByUser(userId, true);

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
                            var existingAudio = _audioRepo.GetByCode(currentUserId, audio.AudioCode);
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

                                if (_audioRepo.Insert(audio)) importedCount++;
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
