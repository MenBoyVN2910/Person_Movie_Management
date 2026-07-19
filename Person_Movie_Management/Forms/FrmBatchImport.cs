using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Guna.UI2.WinForms;

namespace Person_Movie_Management.Forms
{
    public partial class FrmBatchImport : Form
    {
        private MovieRepository _movieRepo;
        
        public FrmBatchImport()
        {
            InitializeComponent();
            _movieRepo = new MovieRepository();
            
            var pnlTop = new Guna.UI2.WinForms.Guna2GradientPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                FillColor = UIHelper.AccentPrimary,
                FillColor2 = UIHelper.AccentTertiary
            };

            this.BackColor = UIHelper.BgDark;
            pnlMain.FillColor = UIHelper.BgCard;
            
            lblTitle.ForeColor = UIHelper.TextPrimary;
            lblTitle.Font = UIHelper.FontH2;
            
            txtUrls.BackColor = UIHelper.BgDark;
            txtUrls.ForeColor = UIHelper.TextPrimary;
            txtUrls.Padding = new Padding(8);
            
            btnStart.FillColor = UIHelper.GradEmerald1;
            btnCancel.FillColor = UIHelper.GradRose1;
            
            progressBar.Visible = false;
            lblStatus.ForeColor = UIHelper.TextMuted;
            lblStatus.Text = "";
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            var lines = txtUrls.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(l => l.Trim())
                                    .Where(l => l.StartsWith("http://") || l.StartsWith("https://"))
                                    .Distinct()
                                    .ToList();

            if (lines.Count == 0)
            {
                MessageBox.Show("Vui lòng dán ít nhất 1 đường link hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnStart.Enabled = false;
            txtUrls.Enabled = false;
            progressBar.Visible = true;
            progressBar.Maximum = lines.Count;
            progressBar.Value = 0;
            
            int successCount = 0;
            int failCount = 0;
            int userId = SessionManager.CurrentUser!.Id;

            var handler = new System.Net.Http.HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                UseCookies = true
            };
            using var client = new System.Net.Http.HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
            client.Timeout = TimeSpan.FromSeconds(15);

            for (int i = 0; i < lines.Count; i++)
            {
                string url = lines[i];
                lblStatus.Text = $"Đang xử lý ({i + 1}/{lines.Count}): {url}";
                
                try
                {
                    string html = "";
                    try
                    {
                        var process = new System.Diagnostics.Process();
                        process.StartInfo.FileName = "curl.exe";
                        process.StartInfo.Arguments = $"-s -L --max-time 20 -H \"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36\" \"{url}\"";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                        html = await process.StandardOutput.ReadToEndAsync();
                        await process.WaitForExitAsync();
                    }
                    catch { }

                    if (string.IsNullOrWhiteSpace(html) || html.Length < 500)
                    {
                        try { html = await client.GetStringAsync(url); }
                        catch { }
                    }

                    if (string.IsNullOrWhiteSpace(html) || html.Length < 500)
                    {
                        failCount++;
                        continue;
                    }

                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    string? title = doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']")?.GetAttributeValue("content", null) 
                                 ?? doc.DocumentNode.SelectSingleNode("//title")?.InnerText;

                    if (!string.IsNullOrEmpty(title))
                    {
                        title = System.Net.WebUtility.HtmlDecode(title);
                        
                        // Check if already exists
                        if (_movieRepo.GetByCode(userId, title) == null)
                        {
                            var movie = new Movie
                            {
                                UserId = userId,
                                MovieCode = title,
                                SourceType = 0, // Online
                                MediaUrl = url,
                                Note = "Thêm hàng loạt"
                            };

                            string? imageUrl = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']")?.GetAttributeValue("content", null);
                            if (!string.IsNullOrEmpty(imageUrl))
                            {
                                if (imageUrl.StartsWith("/"))
                                {
                                    var uri = new Uri(url);
                                    imageUrl = $"{uri.Scheme}://{uri.Host}{imageUrl}";
                                }
                                
                                string tempImagePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"temp_batch_{Guid.NewGuid()}.jpg");
                                bool imgOk = false;

                                // Phương pháp 1: curl.exe (đáng tin nhất)
                                try
                                {
                                    var process2 = new System.Diagnostics.Process();
                                    process2.StartInfo.FileName = "curl.exe";
                                    process2.StartInfo.Arguments = $"-s -L --max-time 15 -H \"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36\" -H \"Referer: {url}\" -o \"{tempImagePath}\" \"{imageUrl}\"";
                                    process2.StartInfo.UseShellExecute = false;
                                    process2.StartInfo.CreateNoWindow = true;
                                    process2.Start();
                                    await process2.WaitForExitAsync();
                                    if (System.IO.File.Exists(tempImagePath) && new System.IO.FileInfo(tempImagePath).Length > 500)
                                        imgOk = true;
                                }
                                catch { }

                                // Phương pháp 2: HttpClient (dự phòng)
                                if (!imgOk)
                                {
                                    try
                                    {
                                        byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);
                                        if (imageBytes != null && imageBytes.Length > 500)
                                        {
                                            System.IO.File.WriteAllBytes(tempImagePath, imageBytes);
                                            imgOk = true;
                                        }
                                    }
                                    catch { }
                                }
                                
                                if (imgOk)
                                {
                                    movie.CoverImage = FileHelper.CopyCoverImage(tempImagePath, title);
                                    try { System.IO.File.Delete(tempImagePath); } catch { }
                                }
                            }

                            _movieRepo.Insert(movie);
                            successCount++;
                        }
                        else
                        {
                            failCount++;
                        }
                    }
                    else
                    {
                        failCount++;
                    }
                }
                catch
                {
                    failCount++;
                }

                progressBar.Value = i + 1;
            }

            lblStatus.Text = $"Hoàn tất: Thêm thành công {successCount} phim, thất bại hoặc trùng lặp {failCount} phim.";
            btnStart.Enabled = true;
            btnStart.Text = "Xong";
            btnStart.Click -= btnStart_Click;
            btnStart.Click += (s, ev) => { this.DialogResult = DialogResult.OK; this.Close(); };
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
