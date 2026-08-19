using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Person_Movie_Management.Models;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Services;

namespace Person_Movie_Management.UserControls
{
    public partial class UcActorCard : UserControl
    {
        private Actor _actor = null!;
        public event EventHandler<Actor>? ActorClicked;

        // Hover Slideshow state
        private System.Windows.Forms.Timer _slideshowTimer = null!;
        private System.Windows.Forms.Timer? _copyResetTimer;
        private List<Image>? _preparedGalleryFrames;
        private int _currentFrameIndex = 0;
        private Image? _originalCover;
        private bool _isHovered = false;
        private int _galleryLoadId = 0;

        private int _boundActorId = -1;
        private int _currentLoadId = 0;

        private static readonly Color NormalBg = Color.FromArgb(20, 26, 48);
        private static readonly Color NormalBorder = Color.FromArgb(42, 53, 90);
        private static readonly Color HoverBg = Color.FromArgb(28, 36, 68);
        private static readonly Color HoverBorder = Color.FromArgb(139, 92, 246);

        public UcActorCard()
        {
            InitializeComponent();
            SetupCard();

            this.Disposed += (s, e) =>
            {
                _slideshowTimer?.Stop();
                _slideshowTimer?.Dispose();
                _copyResetTimer?.Stop();
                _copyResetTimer?.Dispose();
            };
        }

        public UcActorCard(Actor actor) : this()
        {
            BindData(actor);
        }

        private void SetupCard()
        {
            this.DoubleBuffered = true;
            this.BackColor = UIHelper.BgDark;
            this.pnlBase.BackColor = UIHelper.BgDark;

            _slideshowTimer = new System.Windows.Forms.Timer();
            _slideshowTimer.Interval = 750;
            _slideshowTimer.Tick += SlideshowTimer_Tick;

            btnCopyName.Click += (s, e) =>
            {
                CopyActorName();
            };
            btnCopyName.MouseEnter += (s, e) => btnCopyName.ForeColor = Color.White;
            btnCopyName.MouseLeave += (s, e) =>
            {
                if (btnCopyName.Text == "📋")
                    btnCopyName.ForeColor = Color.FromArgb(165, 180, 252);
            };

            var toolTip = new ToolTip();
            toolTip.SetToolTip(btnCopyName, "Sao chép tên diễn viên");

            AttachHoverAndClickEvents(this);
        }

        private void CopyActorName()
        {
            if (_actor == null || string.IsNullOrWhiteSpace(_actor.Name)) return;
            try
            {
                Clipboard.SetText(_actor.Name);
                btnCopyName.Text = "✅";
                btnCopyName.ForeColor = Color.FromArgb(74, 222, 128);

                _copyResetTimer?.Stop();
                _copyResetTimer = new System.Windows.Forms.Timer { Interval = 1500 };
                _copyResetTimer.Tick += (s, e) =>
                {
                    _copyResetTimer.Stop();
                    btnCopyName.Text = "📋";
                    btnCopyName.ForeColor = Color.FromArgb(165, 180, 252);
                };
                _copyResetTimer.Start();
            }
            catch { }
        }

        private void AttachHoverAndClickEvents(Control parent)
        {
            parent.MouseEnter += (s, e) => StartHover();
            parent.MouseMove += (s, e) => { if (!_isHovered) StartHover(); };
            parent.MouseLeave += OnControlMouseLeave;
            
            if (parent != btnCopyName)
            {
                parent.Click += (s, e) => Card_Click(s, e);
            }

            foreach (Control child in parent.Controls)
            {
                AttachHoverAndClickEvents(child);
            }
        }

        private void OnControlMouseLeave(object? sender, EventArgs e)
        {
            var rect = pnlBase.RectangleToScreen(pnlBase.ClientRectangle);
            if (!rect.Contains(Cursor.Position))
            {
                StopHover();
            }
        }

        public void BindData(Actor actor)
        {
            if (actor == null) return;

            // Fast Guard: Bỏ qua nếu đang hiển thị đúng actor này
            if (_boundActorId == actor.Id && _actor != null &&
                _actor.Name == actor.Name &&
                _actor.Nationality == actor.Nationality &&
                _actor.DateOfBirth == actor.DateOfBirth &&
                _actor.AvatarPath == actor.AvatarPath)
            {
                _actor = actor;
                return;
            }

            StopHover();
            _actor = actor;
            _boundActorId = actor.Id;
            _preparedGalleryFrames = null;
            _galleryLoadId++;

            lblName.Text = actor.Name;

            // Sub-info: Quốc tịch • Năm sinh
            string nat = string.IsNullOrWhiteSpace(actor.Nationality) ? "" : actor.Nationality.Trim();
            string birthYear = "";
            if (!string.IsNullOrWhiteSpace(actor.DateOfBirth) && DateTime.TryParse(actor.DateOfBirth, out var dt))
            {
                birthYear = dt.Year.ToString();
            }

            if (!string.IsNullOrEmpty(nat) && !string.IsNullOrEmpty(birthYear))
            {
                lblSubInfo.Text = $"{nat} • {birthYear}";
            }
            else if (!string.IsNullOrEmpty(nat))
            {
                lblSubInfo.Text = nat;
            }
            else if (!string.IsNullOrEmpty(birthYear))
            {
                lblSubInfo.Text = $"Sinh năm {birthYear}";
            }
            else
            {
                lblSubInfo.Text = "Chưa rõ";
            }

            // Load Primary Avatar with RAM Fast Path
            int targetW = picAvatar.Width > 0 ? picAvatar.Width : 230;
            int targetH = picAvatar.Height > 0 ? picAvatar.Height : 240;

            if (!string.IsNullOrEmpty(actor.AvatarPath))
            {
                string fullPath = FileHelper.GetFullPath(actor.AvatarPath);
                if (ImageCache.TryGetThumbnailFromMemory(fullPath, targetW, targetH, out var memImg) && memImg != null)
                {
                    picAvatar.Image = memImg;
                    _originalCover = memImg;
                }
                else
                {
                    picAvatar.Image = null;
                    _originalCover = null;
                    LoadAvatarAsync(fullPath, targetW, targetH);
                }
            }
            else
            {
                picAvatar.Image = null;
                _originalCover = null;
            }
        }

        private async void LoadAvatarAsync(string fullPath, int targetW, int targetH)
        {
            int loadId = ++_currentLoadId;
            if (File.Exists(fullPath))
            {
                try
                {
                    var img = await ImageCache.GetThumbnailAsync(fullPath, targetW, targetH);
                    if (img != null && !this.IsDisposed && _currentLoadId == loadId)
                    {
                        picAvatar.Image = img;
                        _originalCover = img;
                    }
                }
                catch { }
            }
            else
            {
                if (_currentLoadId == loadId)
                {
                    picAvatar.Image = null;
                    _originalCover = null;
                }
            }
        }

        private void StartHover()
        {
            if (_actor == null) return;
            _isHovered = true;
            pnlBase.FillColor = HoverBg;
            pnlBase.BorderColor = HoverBorder;
            pnlInfo.BackColor = HoverBg;

            if (_originalCover == null && picAvatar.Image != null)
            {
                _originalCover = picAvatar.Image;
            }

            if (_preparedGalleryFrames != null)
            {
                if (_preparedGalleryFrames.Count > 1 && !_slideshowTimer.Enabled)
                {
                    _currentFrameIndex = 0;
                    _slideshowTimer.Start();
                }
                return;
            }

            // Tải bất đồng bộ danh sách ảnh phụ của diễn viên để trình chiếu khi hover
            int currentActorId = _actor.Id;
            int loadId = ++_galleryLoadId;
            int targetWidth = picAvatar.Width > 0 ? picAvatar.Width : 230;
            int targetHeight = picAvatar.Height > 0 ? picAvatar.Height : 240;

            _ = Task.Run(async () =>
            {
                var subImages = AppServices.ActorRepo.GetImages(currentActorId);

                var paths = new List<string>();
                if (!string.IsNullOrEmpty(_actor.AvatarPath))
                {
                    string mainPath = FileHelper.GetFullPath(_actor.AvatarPath);
                    if (File.Exists(mainPath)) paths.Add(mainPath);
                }

                foreach (var sub in subImages)
                {
                    string subPath = FileHelper.GetFullPath(sub.ImagePath);
                    if (File.Exists(subPath) && !paths.Contains(subPath, StringComparer.OrdinalIgnoreCase))
                    {
                        paths.Add(subPath);
                    }
                }

                if (paths.Count <= 1)
                {
                    if (_actor.Id == currentActorId && _galleryLoadId == loadId && !this.IsDisposed)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            _preparedGalleryFrames = new List<Image>();
                        }));
                    }
                    return;
                }

                var frames = new List<Image>();
                foreach (var p in paths)
                {
                    try
                    {
                        var cropped = await ImageCache.GetThumbnailAsync(p, targetWidth, targetHeight);
                        if (cropped != null)
                        {
                            frames.Add(cropped);
                        }
                    }
                    catch { }
                }

                if (_actor.Id == currentActorId && _galleryLoadId == loadId && !this.IsDisposed)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        _preparedGalleryFrames = frames;
                        if (_isHovered && _preparedGalleryFrames.Count > 1 && !_slideshowTimer.Enabled)
                        {
                            _currentFrameIndex = 0;
                            _slideshowTimer.Start();
                        }
                    }));
                }
            });
        }

        private void StopHover()
        {
            _isHovered = false;
            _slideshowTimer.Stop();

            pnlBase.FillColor = NormalBg;
            pnlBase.BorderColor = NormalBorder;
            pnlInfo.BackColor = NormalBg;

            if (_originalCover != null)
            {
                picAvatar.Image = _originalCover;
            }
        }

        private void SlideshowTimer_Tick(object? sender, EventArgs e)
        {
            if (!_isHovered || _preparedGalleryFrames == null || _preparedGalleryFrames.Count <= 1)
            {
                _slideshowTimer.Stop();
                return;
            }

            _currentFrameIndex = (_currentFrameIndex + 1) % _preparedGalleryFrames.Count;
            picAvatar.Image = _preparedGalleryFrames[_currentFrameIndex];
        }

        private void Card_Click(object? sender, EventArgs e)
        {
            if (_actor != null)
            {
                ActorClicked?.Invoke(this, _actor);
            }
        }
    }
}
