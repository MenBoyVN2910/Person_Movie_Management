using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;

namespace Person_Movie_Management.Forms
{
    public partial class FrmOmnibox : Form
    {
        private readonly MovieRepository _movieRepo;
        private readonly AudioRepository _audioRepo;
        private readonly PlaylistRepository _playlistRepo;
        private System.Windows.Forms.Timer _debounceTimer;

        public event EventHandler<object>? ItemSelected; // object = Movie or Audio or Playlist

        public FrmOmnibox()
        {
            InitializeComponent();
            _movieRepo = new MovieRepository();
            _audioRepo = new AudioRepository();
            _playlistRepo = new PlaylistRepository();

            _debounceTimer = new System.Windows.Forms.Timer();
            _debounceTimer.Interval = 300;
            _debounceTimer.Tick += DebounceTimer_Tick;
        }

        private void FrmOmnibox_Load(object sender, EventArgs e)
        {
            // Add a slight transparency to the whole form for the glass effect
            this.Opacity = 0.95;
            txtSearch.Focus();
        }

        private void FrmOmnibox_Deactivate(object sender, EventArgs e)
        {
            this.Close(); // Auto close when clicking outside
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private void DebounceTimer_Tick(object? sender, EventArgs e)
        {
            _debounceTimer.Stop();
            PerformSearch();
        }

        private void PerformSearch()
        {
            flpResults.Controls.Clear();
            string query = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(query)) return;

            int currentUserId = SessionManager.CurrentUser!.Id;
            int resultCount = 0;

            // Search Movies
            var movies = _movieRepo.GetAllByUser(currentUserId)
                .Where(m => m.MovieCode.Contains(query, StringComparison.OrdinalIgnoreCase) || 
                            (!string.IsNullOrEmpty(m.Note) && m.Note.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .Take(5).ToList();

            foreach (var m in movies)
            {
                AddResultItem($"🎬 Phim: {m.MovieCode}", m.CoverImage, m);
                resultCount++;
            }

            // Search Audios
            var audios = _audioRepo.GetAllByUser(currentUserId)
                .Where(a => a.AudioCode.Contains(query, StringComparison.OrdinalIgnoreCase) || 
                            (!string.IsNullOrEmpty(a.Note) && a.Note.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .Take(5).ToList();

            foreach (var a in audios)
            {
                AddResultItem($"🎵 Nhạc: {a.AudioCode}", a.CoverImage, a);
                resultCount++;
            }

            // Search Playlists
            var playlists = _playlistRepo.GetAllByUser(currentUserId)
                .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Take(3).ToList();

            foreach (var p in playlists)
            {
                AddResultItem($"📁 Playlist: {p.Name}", null, p);
                resultCount++;
            }

            if (resultCount == 0)
            {
                var lbl = new Label
                {
                    Text = "Không tìm thấy kết quả nào...",
                    ForeColor = Color.Gray,
                    Font = new Font("Segoe UI", 12),
                    AutoSize = true,
                    Margin = new Padding(20)
                };
                flpResults.Controls.Add(lbl);
            }
        }

        private void AddResultItem(string title, string? imagePath, object dataItem)
        {
            var btn = new Guna.UI2.WinForms.Guna2Button();
            btn.Size = new Size(flpResults.Width - 25, 60);
            btn.FillColor = Color.Transparent;
            btn.HoverState.FillColor = Color.FromArgb(30, 41, 59); // Slate 800
            btn.BorderRadius = 10;
            btn.TextAlign = HorizontalAlignment.Left;
            btn.TextOffset = new Point(60, 0);
            btn.Text = title;
            btn.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;
            btn.Margin = new Padding(5);

            if (!string.IsNullOrEmpty(imagePath))
            {
                string fullPath = FileHelper.GetFullPath(imagePath);
                if (System.IO.File.Exists(fullPath))
                {
                    try
                    {
                        var img = FileHelper.LoadImageSafe(fullPath, 50, 50);
                        btn.Image = img;
                        btn.ImageSize = new Size(40, 40);
                        btn.ImageAlign = HorizontalAlignment.Left;
                        btn.ImageOffset = new Point(10, 0);
                    }
                    catch { }
                }
            }

            btn.Click += (s, e) =>
            {
                ItemSelected?.Invoke(this, dataItem);
                this.Close();
            };

            flpResults.Controls.Add(btn);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Down)
            {
                // Simple focus forward logic
                if (flpResults.Controls.Count > 0)
                {
                    flpResults.Controls[0].Focus();
                }
            }
        }
    }
}
