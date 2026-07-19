using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Person_Movie_Management.Services;

namespace Person_Movie_Management.Forms
{
    public partial class FrmDropWidget : Form
    {
        private Point _dragCursorPoint;
        private Point _dragFormPoint;
        private bool _dragging;
        private MovieRepository _movieRepo;
        private System.Windows.Forms.Timer _fadeTimer;
        private double _targetOpacity = 0.3;
        private bool _isHovered = false;

        public FrmDropWidget()
        {
            InitializeComponent();
            _movieRepo = new MovieRepository();
            
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.AllowDrop = true;
            this.ShowInTaskbar = false;
            
            // Set size to a perfect circle (60x60)
            this.Size = new Size(70, 70);
            this.StartPosition = FormStartPosition.Manual;
            this.Opacity = 0.3;
            
            var screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Right - this.Width - 20, screen.Bottom - this.Height - 20);

            this.BackColor = Color.Magenta; // Will be set to transparent
            this.TransparencyKey = Color.Magenta;
            
            // Force region to circle to bypass Windows min width
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, 70, 70);
            this.Region = new Region(path);

            SetupUI();
        }

        private void SetupUI()
        {
            // Use custom painting on the form itself, no child controls
            this.Paint += FrmDropWidget_Paint;
            this.MouseEnter += FrmDropWidget_MouseEnter;
            this.MouseLeave += FrmDropWidget_MouseLeave;

            // Dragging the widget
            this.MouseDown += (s, e) =>
            {
                _dragging = true;
                _dragCursorPoint = Cursor.Position;
                _dragFormPoint = this.Location;
            };

            this.MouseMove += (s, e) =>
            {
                if (_dragging)
                {
                    Point dif = Point.Subtract(Cursor.Position, new Size(_dragCursorPoint));
                    this.Location = Point.Add(_dragFormPoint, new Size(dif));
                }
            };

            this.MouseUp += (s, e) => { _dragging = false; };
            
            // Allow Drop
            this.DragEnter += FrmDropWidget_DragEnter;
            this.DragDrop += FrmDropWidget_DragDrop;
            
            // Setup Fade Timer
            _fadeTimer = new System.Windows.Forms.Timer { Interval = 15 };
            _fadeTimer.Tick += (s, e) =>
            {
                if (Math.Abs(this.Opacity - _targetOpacity) < 0.05)
                {
                    this.Opacity = _targetOpacity;
                    _fadeTimer.Stop();
                }
                else if (this.Opacity < _targetOpacity)
                {
                    this.Opacity += 0.05;
                }
                else
                {
                    this.Opacity -= 0.05;
                }
            };
        }
        
        private void FrmDropWidget_MouseEnter(object sender, EventArgs e)
        {
            _isHovered = true;
            _targetOpacity = 1.0;
            _fadeTimer.Start();
            this.Invalidate();
        }
        
        private void FrmDropWidget_MouseLeave(object sender, EventArgs e)
        {
            // Verify mouse is actually outside (MouseLeave can fire if mouse moves over border)
            Point clientMouse = this.PointToClient(Cursor.Position);
            if (!this.ClientRectangle.Contains(clientMouse))
            {
                _isHovered = false;
                _targetOpacity = 0.3;
                _fadeTimer.Start();
                this.Invalidate();
            }
        }
        
        private void FrmDropWidget_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Draw gradient background
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(this.ClientRectangle, 
                   _isHovered ? UIHelper.GradEmerald1 : UIHelper.AccentPrimary, 
                   _isHovered ? UIHelper.GradEmerald2 : UIHelper.AccentTertiary, 
                   45f))
            {
                g.FillEllipse(brush, 0, 0, 68, 68);
            }
            
            // Draw border
            using (var pen = new Pen(Color.FromArgb(50, Color.White), 2f))
            {
                g.DrawEllipse(pen, 1, 1, 66, 66);
            }
            
            // Draw icon
            string icon = "📥";
            using (var font = new Font("Segoe UI Emoji", 20f))
            using (var textBrush = new SolidBrush(Color.White))
            {
                var stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(icon, font, textBrush, new Rectangle(0, 0, 70, 70), stringFormat);
            }
        }

        private void FrmDropWidget_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private async void FrmDropWidget_DragDrop(object sender, DragEventArgs e)
        {
            if (!SessionManager.IsLoggedIn) return;
            int userId = SessionManager.CurrentUser!.Id;

            // Handle Text/URL
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.Data.GetData(DataFormats.Text);
                if (text.StartsWith("http://") || text.StartsWith("https://"))
                {
                    var frm = new FrmMovieDetail(null, text);
                    frm.TopMost = true; // Make sure it shows on top of browser
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        DataCache.Invalidate();
                    }
                }
            }
            // Handle Files
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                int added = 0;
                foreach (string file in files)
                {
                    if (File.Exists(file) && (file.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase)))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(file);
                        if (_movieRepo.GetByCode(userId, fileName) == null)
                        {
                            var movie = new Movie
                            {
                                UserId = userId,
                                MovieCode = fileName,
                                SourceType = 1, // Local
                                MediaUrl = file
                            };
                            _movieRepo.Insert(movie);
                            added++;
                        }
                    }
                }
                
                if (added > 0)
                {
                    MessageBox.Show($"Đã thêm {added} phim từ file kéo thả.", "DropZone", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DataCache.Invalidate();
                }
            }
        }
    }
}
