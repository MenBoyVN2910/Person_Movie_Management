using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Person_Movie_Management.Services;

namespace Person_Movie_Management.Forms
{
    public partial class FrmBatchImport : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int WM_SETREDRAW = 0x0B;

        private readonly MovieRepository _movieRepo;
        private readonly TagRepository _tagRepo;
        private readonly MovieImageRepository _imageRepo;
        private CancellationTokenSource? _cts;
        private bool _isHighlighting = false;
        private readonly System.Windows.Forms.Timer _syntaxTimer;

        public FrmBatchImport()
        {
            InitializeComponent();
            _movieRepo = new MovieRepository();
            _tagRepo = new TagRepository();
            _imageRepo = new MovieImageRepository();

            this.BackColor = UIHelper.BgDark;
            pnlMain.FillColor = UIHelper.BgCard;
            pnlMain.FillColor2 = UIHelper.BgCard;
            lblTitle.BackColor = UIHelper.BgCard;

            _syntaxTimer = new System.Windows.Forms.Timer();
            _syntaxTimer.Interval = 250;
            _syntaxTimer.Tick += (s, e) =>
            {
                _syntaxTimer.Stop();
                HighlightUrls();
            };

            txtUrls.TextChanged += (s, e) =>
            {
                if (!_isHighlighting)
                {
                    _syntaxTimer.Stop();
                    _syntaxTimer.Start();
                }
            };

            txtUrls.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.V)
                {
                    e.SuppressKeyPress = true;
                    PasteWithAutoNewline();
                }
            };
        }

        private void PasteWithAutoNewline()
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                if (!string.IsNullOrEmpty(text))
                {
                    string pasteText = text.TrimEnd('\r', '\n') + "\r\n";
                    txtUrls.SelectedText = pasteText;
                }
            }
        }

        private void HighlightUrls()
        {
            if (_isHighlighting) return;
            _isHighlighting = true;

            int selStart = txtUrls.SelectionStart;
            int selLength = txtUrls.SelectionLength;

            SendMessage(txtUrls.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

            try
            {
                var seenUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                int validCount = 0;
                int duplicateCount = 0;
                int invalidCount = 0;

                int charIndex = 0;
                string[] lines = txtUrls.Lines;

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    int lineLen = line.Length;
                    string trimmed = line.Trim();

                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        bool isValidUrl = Uri.TryCreate(trimmed, UriKind.Absolute, out Uri? uri) &&
                                          (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

                        Color lineColor;
                        if (!isValidUrl)
                        {
                            lineColor = UIHelper.Danger;
                            invalidCount++;
                        }
                        else if (seenUrls.Contains(trimmed))
                        {
                            lineColor = UIHelper.Warning;
                            duplicateCount++;
                        }
                        else
                        {
                            lineColor = UIHelper.Success;
                            seenUrls.Add(trimmed);
                            validCount++;
                        }

                        txtUrls.Select(charIndex, lineLen);
                        txtUrls.SelectionColor = lineColor;
                    }

                    charIndex += lineLen + 1;
                }

                txtUrls.Select(selStart, selLength);
                txtUrls.SelectionColor = UIHelper.TextPrimary;

                lblStatus.Text = $"🟢 Hợp lệ: {validCount}  |  🟡 Trùng lặp: {duplicateCount}  |  🔴 Không hợp lệ: {invalidCount}";
            }
            finally
            {
                SendMessage(txtUrls.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
                txtUrls.Invalidate();
                _isHighlighting = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                btnCancel.Enabled = false;
                btnCancel.Text = "Đang dừng...";
                return;
            }
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            var rawLines = txtUrls.Lines
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct()
                .ToList();

            var validUrls = rawLines
                .Where(u => Uri.TryCreate(u, UriKind.Absolute, out Uri? uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                .ToList();

            if (validUrls.Count == 0)
            {
                MessageBox.Show("Vui lòng nhập ít nhất một đường dẫn URL hợp lệ (bắt đầu bằng http hoặc https).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnStart.Enabled = false;
            txtUrls.ReadOnly = true;
            btnCancel.Text = "Dừng lại";
            progressBar.Visible = true;
            progressBar.Maximum = validUrls.Count;
            progressBar.Value = 0;

            _cts = new CancellationTokenSource();
            int successCount = 0;
            int failedCount = 0;
            int userId = SessionManager.CurrentUser!.Id;

            for (int i = 0; i < validUrls.Count; i++)
            {
                if (_cts.IsCancellationRequested) break;

                string url = validUrls[i];
                lblStatus.Text = $"Đang xử lý ({i + 1}/{validUrls.Count}): {url}";

                try
                {
                    var movie = new Movie
                    {
                        UserId = userId,
                        SourceType = 0,
                        MediaUrl = url,
                        CreatedAt = DateTime.Now
                    };

                    string movieTitle = "";
                    string? posterUrl = null;

                    // Thử qua SiteAdapter trước (YouTube, Vimeo, Dailymotion...)
                    var adapter = SiteAdapterRegistry.FindAdapter(url);
                    SiteMetadata? meta = null;
                    if (adapter != null)
                    {
                        try
                        {
                            meta = await adapter.ExtractMetadataAsync(url, "");
                            if (meta != null)
                            {
                                if (!string.IsNullOrWhiteSpace(meta.Title)) movieTitle = meta.Title;
                                if (!string.IsNullOrWhiteSpace(meta.CoverImageUrl)) posterUrl = meta.CoverImageUrl;
                            }
                        }
                        catch { }
                    }

                    if (string.IsNullOrWhiteSpace(movieTitle))
                    {
                        var doc = await Task.Run(() =>
                        {
                            var web = new HtmlAgilityPack.HtmlWeb();
                            return web.Load(url);
                        });

                        var ogTitle = doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']")?.GetAttributeValue("content", "");
                        var titleTag = doc.DocumentNode.SelectSingleNode("//title")?.InnerText;
                        movieTitle = !string.IsNullOrWhiteSpace(ogTitle) ? ogTitle : (!string.IsNullOrWhiteSpace(titleTag) ? titleTag : "Movie " + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                        movieTitle = System.Net.WebUtility.HtmlDecode(movieTitle).Trim();

                        if (string.IsNullOrEmpty(posterUrl))
                        {
                            posterUrl = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']")?.GetAttributeValue("content", "");
                        }
                    }

                    if (string.IsNullOrWhiteSpace(movieTitle) || movieTitle.StartsWith("Movie 20"))
                    {
                        var match = Regex.Match(url, @"/([^/?#]+)[/?#]?", RegexOptions.RightToLeft);
                        if (match.Success) movieTitle = match.Groups[1].Value.Replace("-", " ").Replace("_", " ");
                    }

                    movie.MovieCode = movieTitle;

                    var existing = _movieRepo.GetByCode(userId, movie.MovieCode);
                    if (existing != null)
                    {
                        movie.MovieCode += " (" + DateTime.Now.ToString("HHmmss") + ")";
                    }

                    if (!string.IsNullOrEmpty(posterUrl))
                    {
                        try
                        {
                            using var client = new HttpClient();
                            byte[] imgBytes = await client.GetByteArrayAsync(posterUrl);
                            string tempFile = Path.Combine(Path.GetTempPath(), $"batch_poster_{Guid.NewGuid()}.jpg");
                            await File.WriteAllBytesAsync(tempFile, imgBytes);
                            movie.CoverImage = FileHelper.CopyCoverImage(tempFile, movie.MovieCode);
                        }
                        catch { }
                    }

                    int movieId = _movieRepo.Insert(movie);

                    successCount++;
                }
                catch
                {
                    failedCount++;
                }

                progressBar.Value = i + 1;
            }

            DataCache.Invalidate();

            MessageBox.Show($"Hoàn tất nhập hàng loạt!\nThành công: {successCount}\nThất bại/Bỏ qua: {failedCount}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}