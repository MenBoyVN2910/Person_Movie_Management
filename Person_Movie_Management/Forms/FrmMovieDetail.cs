using System;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Person_Movie_Management.Services;

namespace Person_Movie_Management.Forms
{
    public partial class FrmMovieDetail : Form
    {
        private Movie _movie;
        private readonly MovieRepository _movieRepo;
        private readonly MovieImageRepository _movieImageRepo;
        private string? _selectedCoverPath;
        private List<string> _galleryImagePaths = new();
        private readonly TagRepository _tagRepo;
        private List<int> _currentTagIds = new();

        public FrmMovieDetail(Movie? movie = null, string? initialUrl = null)
        {
            InitializeComponent();
            _movieRepo = new MovieRepository();
            _movieImageRepo = new MovieImageRepository();
            _tagRepo = new TagRepository();
            
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

            if (!string.IsNullOrEmpty(initialUrl))
            {
                txtMediaUrl.Text = initialUrl;
                this.Load += (s, e) => btnFetchUrl_Click(btnFetchUrl, EventArgs.Empty);
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
                    _selectedCoverPath = fullPath;
                    picCover.Image = FileHelper.LoadImageSafe(fullPath);
                }
            }

            // Load Gallery
            var gallery = _movieImageRepo.GetByMovieId(_movie.Id);
            foreach (var img in gallery)
            {
                string fullPath = FileHelper.GetFullPath(img.ImagePath);
                if (System.IO.File.Exists(fullPath))
                {
                    _galleryImagePaths.Add(fullPath);
                    AddGalleryThumbnail(fullPath);
                }
            }

            // Load Tags
            var tags = _tagRepo.GetTagsForMovie(_movie.Id);
            _currentTagIds = tags.Select(t => t.Id).ToList();
            RenderTags();
        }

        private void RenderTags()
        {
            flpTags.Controls.Clear();
            foreach (int tagId in _currentTagIds)
            {
                var tag = _tagRepo.GetById(tagId);
                if (tag != null)
                {
                    var lbl = new Label
                    {
                        Text = tag.TagName,
                        Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                        ForeColor = Color.White,
                        BackColor = ColorTranslator.FromHtml(tag.ColorHex ?? "#6366f1"),
                        AutoSize = true,
                        Padding = new Padding(5),
                        Margin = new Padding(3),
                        Cursor = Cursors.Default
                    };
                    // Make it rounded by painting if possible, but standard label with padding works too
                    flpTags.Controls.Add(lbl);
                }
            }
        }

        private void AddGalleryThumbnail(string path)
        {
            var pb = new PictureBox
            {
                Size = new Size(60, 80),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = FileHelper.LoadImageSafe(path),
                Margin = new Padding(3)
            };
            
            // Allow deleting image by clicking on it
            pb.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn muốn xóa ảnh này khỏi gallery?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    pnlGallery.Controls.Remove(pb);
                    _galleryImagePaths.Remove(path);
                    pb.Dispose();
                }
            };
            
            pnlGallery.Controls.Add(pb);
        }

        private void btnAddGalleryImage_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                {
                    _galleryImagePaths.Add(file);
                    AddGalleryThumbnail(file);
                }
            }
        }

        private void btnManageTags_Click(object sender, EventArgs e)
        {
            var frm = new FrmTagManager(SessionManager.CurrentUser!.Id, _currentTagIds);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                _currentTagIds = frm.SelectedTagIds;
                RenderTags();
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
                _movie.Id = _movieRepo.Insert(_movie);
            }
            else
            {
                _movieRepo.Update(_movie);
            }

            // Save Gallery
            _movieImageRepo.DeleteByMovieId(_movie.Id);
            for (int i = 0; i < _galleryImagePaths.Count; i++)
            {
                // Only copy if it's not already in the MovieVault/Covers folder?
                // Actually FileHelper.CopyCoverImage handles a unique name
                string newImagePath = FileHelper.CopyCoverImage(_galleryImagePaths[i], _movie.MovieCode + $"_gallery_{i}");
                _movieImageRepo.Insert(new MovieImage
                {
                    MovieId = _movie.Id,
                    ImagePath = newImagePath,
                    SortOrder = i
                });
            }
            
            // Save Tags
            _tagRepo.SetMovieTags(_movie.Id, _currentTagIds);

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

        private async void btnFetchUrl_Click(object sender, EventArgs e)
        {
            string url = txtMediaUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(url) || (!url.StartsWith("http://") && !url.StartsWith("https://")))
            {
                MessageBox.Show("Vui lòng nhập một URL hợp lệ (bắt đầu bằng http hoặc https).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnFetchUrl.Enabled = false;
            btnFetchUrl.Text = "⏳ Đang lấy...";

            try
            {
                var handler = new System.Net.Http.HttpClientHandler()
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                    UseCookies = true
                };
                using var client = new System.Net.Http.HttpClient(handler);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,vi;q=0.8");
                client.Timeout = TimeSpan.FromSeconds(15);
                
                // Dùng curl.exe làm phương pháp CHÍNH (đáng tin nhất, bypass bot protection)
                string html = "";
                try
                {
                    var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = "curl.exe";
                    process.StartInfo.Arguments = $"-s -L --max-time 20 -H \"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36\" -H \"Accept: text/html,application/xhtml+xml\" -H \"Accept-Language: en-US,en;q=0.9\" \"{url}\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    html = await process.StandardOutput.ReadToEndAsync();
                    await process.WaitForExitAsync();
                }
                catch { }

                // Fallback: HttpClient nếu curl thất bại
                if (string.IsNullOrWhiteSpace(html) || html.Length < 500)
                {
                    try
                    {
                        html = await client.GetStringAsync(url);
                    }
                    catch { }
                }

                if (string.IsNullOrWhiteSpace(html) || html.Length < 500)
                {
                    MessageBox.Show("Không thể tải nội dung trang web. Vui lòng kiểm tra lại URL.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // Try to get og:title
                var titleNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']");
                string? title = titleNode?.GetAttributeValue("content", null);
                if (string.IsNullOrEmpty(title))
                {
                    titleNode = doc.DocumentNode.SelectSingleNode("//title");
                    title = titleNode?.InnerText;
                }

                if (!string.IsNullOrEmpty(title) && string.IsNullOrWhiteSpace(txtMovieCode.Text))
                {
                    txtMovieCode.Text = System.Net.WebUtility.HtmlDecode(title);
                }

                // Phase 1: Smart Image Scraper
                // Chia làm 2 nhóm: ảnh bìa chính (priority) và ảnh phụ (extra)
                var priorityCoverUrls = new List<string>(); // og:image, twitter:image, JSON-LD, video poster
                var extraUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                
                // 1. og:image (most reliable if present)
                var ogImageNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
                string? ogImageUrl = ogImageNode?.GetAttributeValue("content", null);
                if (!string.IsNullOrEmpty(ogImageUrl)) priorityCoverUrls.Add(ogImageUrl);

                // 2. twitter:image
                var twitterImageNode = doc.DocumentNode.SelectSingleNode("//meta[@name='twitter:image'] | //meta[@property='twitter:image']");
                string? twitterImageUrl = twitterImageNode?.GetAttributeValue("content", null);
                if (!string.IsNullOrEmpty(twitterImageUrl) && !priorityCoverUrls.Contains(twitterImageUrl, StringComparer.OrdinalIgnoreCase)) 
                    priorityCoverUrls.Add(twitterImageUrl);

                // 3. JSON-LD thumbnailUrl / image
                var jsonLdNodes = doc.DocumentNode.SelectNodes("//script[@type='application/ld+json']");
                if (jsonLdNodes != null)
                {
                    foreach (var jsonNode in jsonLdNodes)
                    {
                        try
                        {
                            string jsonText = System.Net.WebUtility.HtmlDecode(jsonNode.InnerText);
                            var thumbMatch = System.Text.RegularExpressions.Regex.Match(jsonText, "\"thumbnailUrl\"\\s*:\\s*\"([^\"]+)\"");
                            if (thumbMatch.Success) 
                            {
                                string u = thumbMatch.Groups[1].Value.Replace("\\/", "/");
                                if (!priorityCoverUrls.Contains(u, StringComparer.OrdinalIgnoreCase)) priorityCoverUrls.Add(u);
                            }
                            var imgMatch = System.Text.RegularExpressions.Regex.Match(jsonText, "\"image\"\\s*:\\s*\"([^\"]+)\"");
                            if (imgMatch.Success)
                            {
                                string u = imgMatch.Groups[1].Value.Replace("\\/", "/");
                                if (!priorityCoverUrls.Contains(u, StringComparer.OrdinalIgnoreCase)) priorityCoverUrls.Add(u);
                            }
                        }
                        catch { }
                    }
                }

                // 4. <video poster="..."> attribute
                var videoNodes = doc.DocumentNode.SelectNodes("//video[@poster]");
                if (videoNodes != null)
                {
                    foreach (var vid in videoNodes)
                    {
                        string poster = vid.GetAttributeValue("poster", "");
                        if (!string.IsNullOrWhiteSpace(poster) && !priorityCoverUrls.Contains(poster, StringComparer.OrdinalIgnoreCase))
                            priorityCoverUrls.Add(poster);
                    }
                }

                // 5. Regex trong inline <script> - TÌM ẢNH BÌA CHÍNH của video (chỉ lấy match ĐẦU TIÊN mỗi pattern)
                var scriptNodes = doc.DocumentNode.SelectNodes("//script[not(@src)]");
                if (scriptNodes != null)
                {
                    // Patterns cho ảnh bìa chính - chỉ lấy FIRST match
                    var coverPatterns = new[]
                    {
                        "\"thumbURL\"\\s*:\\s*\"([^\"]+)\"",
                        "\"thumbnailUrl\"\\s*:\\s*\"([^\"]+)\"",
                        "\"previewImageUrl\"\\s*:\\s*\"([^\"]+)\"",
                        "\"posterUrl\"\\s*:\\s*\"([^\"]+)\"",
                        "\"poster\"\\s*:\\s*\"(https?:[^\"]+)\"",
                        "\"image_url\"\\s*:\\s*\"([^\"]+)\"",
                        "\"coverImageURL\"\\s*:\\s*\"([^\"]+)\"",
                    };

                    foreach (var scriptNode in scriptNodes)
                    {
                        try
                        {
                            string scriptText = scriptNode.InnerText;
                            if (string.IsNullOrWhiteSpace(scriptText) || scriptText.Length < 50) continue;

                            foreach (var pattern in coverPatterns)
                            {
                                // Chỉ lấy match ĐẦU TIÊN để tránh lấy thumbnail video đề xuất
                                var m = System.Text.RegularExpressions.Regex.Match(scriptText, pattern);
                                if (m.Success)
                                {
                                    string found = m.Groups[1].Value.Replace("\\/", "/");
                                    if (found.StartsWith("http"))
                                    {
                                        if (!priorityCoverUrls.Contains(found, StringComparer.OrdinalIgnoreCase))
                                            priorityCoverUrls.Add(found);
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }

                // 6. <img> tags - CHỈ khi chưa tìm thấy ảnh bìa nào
                if (priorityCoverUrls.Count == 0)
                {
                    var imgNodes = doc.DocumentNode.SelectNodes("//img[@src or @data-src]");
                    if (imgNodes != null)
                    {
                        foreach (var img in imgNodes)
                        {
                            foreach (var attr in new[] { "src", "data-src", "data-lazy-src" })
                            {
                                string src = img.GetAttributeValue(attr, "");
                                if (!string.IsNullOrWhiteSpace(src) && !src.StartsWith("data:image"))
                                {
                                    src = src.Split(' ')[0].Trim();
                                    extraUrls.Add(src);
                                }
                            }
                        }
                    }
                }

                // Tổng hợp: priority trước, extra sau
                var allRawUrls = new List<string>(priorityCoverUrls);
                foreach (var eu in extraUrls)
                {
                    if (!allRawUrls.Contains(eu, StringComparer.OrdinalIgnoreCase))
                        allRawUrls.Add(eu);
                }

                // Process URLs: resolve relative, filter junk
                var finalUrls = new List<string>();
                var baseUri = new Uri(url);
                foreach (var imgUrl in allRawUrls)
                {
                    try
                    {
                        string finalUrl;
                        if (imgUrl.StartsWith("http://") || imgUrl.StartsWith("https://"))
                        {
                            finalUrl = imgUrl; // Giữ nguyên URL tuyệt đối, không qua Uri.TryCreate (tránh lỗi encode)
                        }
                        else if (Uri.TryCreate(baseUri, imgUrl, out Uri absoluteUri))
                        {
                            finalUrl = absoluteUri.ToString();
                        }
                        else continue;

                        // Filter out SVG, tiny icons/logos/trackers
                        if (!finalUrl.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) &&
                            !finalUrl.Contains("icon", StringComparison.OrdinalIgnoreCase) &&
                            !finalUrl.Contains("logo", StringComparison.OrdinalIgnoreCase) &&
                            !finalUrl.Contains("pixel", StringComparison.OrdinalIgnoreCase) &&
                            !finalUrl.Contains("beacon", StringComparison.OrdinalIgnoreCase) &&
                            !finalUrl.Contains("avatar", StringComparison.OrdinalIgnoreCase) &&
                            !finalUrl.Contains("emoji", StringComparison.OrdinalIgnoreCase) &&
                            !finalUrl.Contains("flag", StringComparison.OrdinalIgnoreCase) &&
                            !finalUrls.Contains(finalUrl, StringComparer.OrdinalIgnoreCase))
                        {
                            finalUrls.Add(finalUrl);
                        }
                    }
                    catch { }
                }

                // Giới hạn tối đa 10 ảnh
                if (finalUrls.Count > 10)
                    finalUrls = finalUrls.Take(10).ToList();

                if (finalUrls.Count > 0)
                {
                    var frmPicker = new FrmImagePicker(finalUrls, url);
                    frmPicker.TopMost = true; // Make sure it cascades TopMost
                    if (frmPicker.ShowDialog(this) == DialogResult.OK)
                    {
                        var selectedUrls = frmPicker.SelectedUrls;
                        for (int i = 0; i < selectedUrls.Count; i++)
                        {
                            string tempImagePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"temp_gallery_{Guid.NewGuid()}.jpg");
                            bool downloaded = false;

                            // Phương pháp 1: curl.exe (đáng tin nhất, bypass hotlink protection)
                            try
                            {
                                var process = new System.Diagnostics.Process();
                                process.StartInfo.FileName = "curl.exe";
                                process.StartInfo.Arguments = $"-s -L --max-time 20 -H \"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36\" -H \"Referer: {url}\" -o \"{tempImagePath}\" \"{selectedUrls[i]}\"";
                                process.StartInfo.UseShellExecute = false;
                                process.StartInfo.CreateNoWindow = true;
                                process.Start();
                                await process.WaitForExitAsync();

                                if (System.IO.File.Exists(tempImagePath) && new System.IO.FileInfo(tempImagePath).Length > 500)
                                {
                                    downloaded = true;
                                }
                            }
                            catch { }

                            // Phương pháp 2: HttpClient (dự phòng)
                            if (!downloaded)
                            {
                                try
                                {
                                    var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, selectedUrls[i]);
                                    request.Headers.Referrer = new Uri(url);
                                    request.Headers.Add("Accept", "image/avif,image/webp,image/apng,image/*,*/*;q=0.8");
                                    var response = await client.SendAsync(request);
                                    response.EnsureSuccessStatusCode();
                                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                                    if (imageBytes != null && imageBytes.Length > 500)
                                    {
                                        System.IO.File.WriteAllBytes(tempImagePath, imageBytes);
                                        downloaded = true;
                                    }
                                }
                                catch { }
                            }

                            if (downloaded)
                            {
                                if (i == 0)
                                {
                                    // Ảnh đầu tiên luôn dùng làm ảnh bìa
                                    _selectedCoverPath = tempImagePath;
                                    picCover.Image = FileHelper.LoadImageSafe(_selectedCoverPath);
                                }
                                else
                                {
                                    _galleryImagePaths.Add(tempImagePath);
                                    AddGalleryThumbnail(tempImagePath);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy thông tin: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnFetchUrl.Enabled = true;
                btnFetchUrl.Text = "🔍 Lấy thông tin";
            }
        }

        private async void btnFetchTMDB_Click(object sender, EventArgs e)
        {
            string query = txtMovieCode.Text.Trim();
            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Vui lòng nhập tên phim vào ô Mã phim để tìm kiếm trên TMDB.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnFetchTMDB.Enabled = false;
            btnFetchTMDB.Text = "⏳ Đang tìm...";

            try
            {
                var tmdbService = new TMDBService();
                var results = await tmdbService.SearchMoviesAsync(query);

                if (results.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy kết quả nào trên TMDB.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Pick the first, most relevant result
                var movieInfo = results[0];
                
                txtNote.Text = movieInfo.Overview;
                if (!string.IsNullOrEmpty(movieInfo.ReleaseDate))
                {
                    txtNote.Text += $"\n\nPhát hành: {movieInfo.ReleaseDate}";
                }
                
                int userId = SessionManager.CurrentUser.Id;
                var allTags = _tagRepo.GetAllByUser(userId);
                foreach (var genre in movieInfo.Genres)
                {
                    var existingTag = allTags.FirstOrDefault(t => t.TagName.Equals(genre, StringComparison.OrdinalIgnoreCase));
                    if (existingTag != null)
                    {
                        if (!_currentTagIds.Contains(existingTag.Id)) _currentTagIds.Add(existingTag.Id);
                    }
                    else
                    {
                        int newTagId = _tagRepo.Insert(new Tag { TagName = genre, ColorHex = "#8b5cf6" }); // default purple
                        _currentTagIds.Add(newTagId);
                    }
                }
                RenderTags();

                if (!string.IsNullOrEmpty(movieInfo.PosterUrl))
                {
                    try
                    {
                        using var client = new System.Net.Http.HttpClient();
                        byte[] imageBytes = await client.GetByteArrayAsync(movieInfo.PosterUrl);
                        string tempImagePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"tmdb_poster_{Guid.NewGuid()}.jpg");
                        System.IO.File.WriteAllBytes(tempImagePath, imageBytes);
                        
                        _selectedCoverPath = tempImagePath;
                        picCover.Image = FileHelper.LoadImageSafe(_selectedCoverPath);
                    }
                    catch { }
                }
                
                // If the user hasn't set an initial rating, maybe we could set it but we don't have it on this form.
                // It's in the Movie object, but rating is updated via UcMovieCard click. We can pre-fill the _movie.Rating!
                _movie.Rating = (int)Math.Round(movieInfo.Rating);
                if (_movie.Rating > 5) _movie.Rating = 5;
                if (_movie.Rating < 0) _movie.Rating = 0;

                MessageBox.Show($"Đã tự động điền thông tin cho phim: {movieInfo.Title}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối TMDB: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnFetchTMDB.Enabled = true;
                btnFetchTMDB.Text = "🎬 TMDB API";
            }
        }
    }
}
