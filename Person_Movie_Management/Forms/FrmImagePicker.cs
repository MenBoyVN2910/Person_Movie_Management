using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Guna.UI2.WinForms;

namespace Person_Movie_Management.Forms
{
    public partial class FrmImagePicker : Form
    {
        public List<string> SelectedUrls { get; private set; } = new List<string>();
        private List<string> _allUrls;
        private HashSet<string> _selectedSet = new HashSet<string>();
        private string _refererDomain;

        public FrmImagePicker(List<string> imageUrls, string? refererUrl = null)
        {
            InitializeComponent();
            _allUrls = imageUrls;
            _refererDomain = refererUrl ?? "";
            
            this.BackColor = UIHelper.BgDark;
            pnlTop.BackColor = UIHelper.BgPanel;
            flpImages.BackColor = UIHelper.BgDark;
            
            lblTitle.ForeColor = UIHelper.TextPrimary;
            lblTitle.Font = UIHelper.FontH2;
            
            btnSave.FillColor = UIHelper.GradEmerald1;
            btnCancel.FillColor = UIHelper.GradRose1;

            this.Load += FrmImagePicker_Load;
        }

        private async void FrmImagePicker_Load(object? sender, EventArgs e)
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                UseCookies = true
            };
            using var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "image/avif,image/webp,image/apng,image/svg+xml,image/*,*/*;q=0.8");
            if (!string.IsNullOrEmpty(_refererDomain))
            {
                try
                {
                    var refUri = new Uri(_refererDomain);
                    client.DefaultRequestHeaders.Add("Referer", $"{refUri.Scheme}://{refUri.Host}/");
                }
                catch { }
            }
            client.Timeout = TimeSpan.FromSeconds(10);

            bool isFirst = true;
            foreach (var url in _allUrls)
            {
                var card = new Guna2Panel
                {
                    Size = new Size(160, 200),
                    Margin = new Padding(10),
                    BorderRadius = 8,
                    BackColor = Color.Transparent,
                    FillColor = UIHelper.BgCard,
                    Cursor = Cursors.Hand
                };

                var pic = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Location = new Point(10, 10),
                    Size = new Size(140, 140),
                    BackColor = Color.Transparent
                };

                var chk = new Guna2CheckBox
                {
                    Text = "Chọn",
                    ForeColor = UIHelper.TextPrimary,
                    Location = new Point(10, 160),
                    AutoSize = true,
                    Cursor = Cursors.Hand,
                    Tag = url
                };

                chk.CheckedChanged += (s, ev) =>
                {
                    if (chk.Checked) _selectedSet.Add(url);
                    else _selectedSet.Remove(url);
                    
                    card.BorderThickness = chk.Checked ? 2 : 0;
                    card.BorderColor = chk.Checked ? UIHelper.AccentPrimary : Color.Transparent;
                };

                EventHandler clickHandler = (s, ev) => { chk.Checked = !chk.Checked; };
                card.Click += clickHandler;
                pic.Click += clickHandler;

                if (isFirst)
                {
                    chk.Checked = true;
                    isFirst = false;
                }

                card.Controls.Add(pic);
                card.Controls.Add(chk);
                flpImages.Controls.Add(card);

                // Download image with proper headers (fire-and-forget per card)
                _ = LoadImageAsync(client, url, pic, card);
            }
        }

        private async Task LoadImageAsync(HttpClient client, string url, PictureBox pic, Guna2Panel card)
        {
            string tempImagePath = Path.Combine(Path.GetTempPath(), $"temp_preview_{Guid.NewGuid()}.jpg");
            try
            {
                byte[]? data = null;
                try
                {
                    // Add Referer specifically for the request
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (!string.IsNullOrEmpty(_refererDomain))
                    {
                        request.Headers.Referrer = new Uri(_refererDomain);
                    }
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    data = await response.Content.ReadAsByteArrayAsync();
                }
                catch
                {
                    // Fallback: try with curl output to file
                    var refererArg = !string.IsNullOrEmpty(_refererDomain) 
                        ? $"-H \"Referer: {_refererDomain}\"" 
                        : "";
                    var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = "curl.exe";
                    process.StartInfo.Arguments = $"-s -L --max-time 15 -H \"User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36\" {refererArg} -o \"{tempImagePath}\" \"{url}\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    await process.WaitForExitAsync();
                    
                    if (File.Exists(tempImagePath))
                    {
                        data = File.ReadAllBytes(tempImagePath);
                    }
                }

                if (data != null && data.Length > 500) // Skip tiny/empty responses
                {
                    Image? img = null;
                    using var ms = new MemoryStream(data);
                    
                    try
                    {
                        // Try standard WinForms loading first
                        img = Image.FromStream(ms);
                    }
                    catch
                    {
                        // Fallback to Magick.NET for WEBP and other formats not supported natively
                        try
                        {
                            ms.Position = 0;
                            using var magickImage = new ImageMagick.MagickImage(ms);
                            // Convert to JPEG in memory
                            using var outMs = new MemoryStream();
                            magickImage.Format = ImageMagick.MagickFormat.Jpeg;
                            magickImage.Write(outMs);
                            outMs.Position = 0;
                            img = Image.FromStream(outMs);
                        }
                        catch { }
                    }

                    if (img != null)
                    {
                        if (!this.IsDisposed && !pic.IsDisposed)
                        {
                            this.Invoke((MethodInvoker)delegate { pic.Image = img; });
                        }
                    }
                    else
                    {
                        // If all decoding fails, hide the card
                        if (!this.IsDisposed && !card.IsDisposed)
                        {
                            this.Invoke((MethodInvoker)delegate { card.Visible = false; });
                        }
                    }
                }
                else
                {
                    // Hide cards with failed images
                    if (!this.IsDisposed && !card.IsDisposed)
                    {
                        this.Invoke((MethodInvoker)delegate { card.Visible = false; });
                    }
                }
            }
            catch
            {
                // Hide cards with failed images
                try
                {
                    if (!this.IsDisposed && !card.IsDisposed)
                    {
                        this.Invoke((MethodInvoker)delegate { card.Visible = false; });
                    }
                }
                catch { }
            }
            finally
            {
                try { if (File.Exists(tempImagePath)) File.Delete(tempImagePath); } catch { }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SelectedUrls = new List<string>(_selectedSet);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (Control card in flpImages.Controls)
            {
                foreach (Control c in card.Controls)
                {
                    if (c is Guna2CheckBox chk)
                    {
                        chk.Checked = true;
                    }
                }
            }
        }
    }
}
