using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.UserControls
{
    public partial class UcAudioPlayer : UserControl
    {
        private WaveOutEvent? _outputDevice;
        private AudioFileReader? _audioFile;
        private bool _isDraggingProgress = false;
        private bool _isDraggingVolume = false;
        private string? _tempFilePath;
        private string _songTitle = "";
        private float _volume = 0.7f;
        private bool _isHoveringProgress = false;
        private bool _isHoveringVolume = false;
        private int _hoverProgressX = 0;

        // Layout constants
        private const int PLAYER_HEIGHT = 110; // Taller audio bar
        private const int PROGRESS_BAR_HEIGHT = 4;
        private const int PROGRESS_BAR_HOVER_HEIGHT = 6;
        private const int THUMB_RADIUS = 6;
        private const int BTN_SIZE = 44;
        private const int VOLUME_BAR_WIDTH = 100;
        private const int VOLUME_BAR_HEIGHT = 4;
        private const float VOL_STEP = 0.05f;

        // Colors
        private static readonly Color BgColor = Color.FromArgb(12, 15, 30);
        private static readonly Color ProgressBg = Color.FromArgb(40, 50, 70);
        private static readonly Color ProgressFill = Color.FromArgb(139, 92, 246); // Purple accent
        private static readonly Color ProgressHover = Color.FromArgb(167, 130, 255);
        private static readonly Color VolumeFill = Color.FromArgb(16, 185, 129); // Green accent
        private static readonly Color TextPrimary = Color.FromArgb(230, 237, 243);
        private static readonly Color TextMuted = Color.FromArgb(120, 135, 155);
        private static readonly Color BtnPlay = Color.FromArgb(139, 92, 246);
        private static readonly Color BtnPlayHover = Color.FromArgb(160, 120, 255);
        private static readonly Color BtnStop = Color.FromArgb(220, 60, 60);
        private static readonly Color BtnStopHover = Color.FromArgb(240, 85, 85);
        private static readonly Color SeparatorLine = Color.FromArgb(30, 38, 60);

        // Hover states for buttons
        private bool _hoverPlay = false;
        private bool _hoverStop = false;
        private bool _hoverClose = false;

        // Rectangles for hit testing
        private RectangleF _progressBarRect;
        private RectangleF _playBtnRect;
        private RectangleF _stopBtnRect;
        private RectangleF _volumeIconRect;
        private RectangleF _volumeBarRect;
        private RectangleF _closeBtnRect;

        public UcAudioPlayer()
        {
            InitializeComponent();
            this.Height = PLAYER_HEIGHT;
            this.Dock = DockStyle.Bottom;
            this.BackColor = BgColor;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Selectable, true);
            this.TabStop = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int w = this.Width;
            int h = this.Height;

            // ── Top separator line ──
            using var sepPen = new Pen(SeparatorLine, 1);
            g.DrawLine(sepPen, 0, 0, w, 0);

            // ── Progress bar (top area) ──
            DrawProgressBar(g, w);

            // ── Song title (left side) ──
            DrawSongInfo(g);

            // ── Control buttons (center) ──
            DrawControlButtons(g, w);

            // ── Time display (right of buttons) ──
            DrawTimeDisplay(g, w);

            // ── Volume control (far right) ──
            DrawVolumeControl(g, w);

            // ── Close button (very far right) ──
            DrawCloseButton(g, w);
        }

        private void DrawCloseButton(Graphics g, int w)
        {
            int btnSize = 24;
            int x = w - btnSize - 15; // Right margin 15
            int y = 50;
            _closeBtnRect = new RectangleF(x, y, btnSize, btnSize);

            if (_hoverClose)
            {
                using var hoverBrush = new SolidBrush(Color.FromArgb(40, 255, 255, 255));
                using var hoverPath = CreateRoundedRect(_closeBtnRect, 4);
                g.FillPath(hoverBrush, hoverPath);
            }

            using var pen = new Pen(TextMuted, 2) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            if (_hoverClose) pen.Color = Color.White;
            g.DrawLine(pen, x + 7, y + 7, x + btnSize - 7, y + btnSize - 7);
            g.DrawLine(pen, x + btnSize - 7, y + 7, x + 7, y + btnSize - 7);
        }

        private void DrawProgressBar(Graphics g, int w)
        {
            int barY = 14;
            int barX = 15;
            int barW = w - 30;
            int barH = _isHoveringProgress || _isDraggingProgress ? PROGRESS_BAR_HOVER_HEIGHT : PROGRESS_BAR_HEIGHT;

            _progressBarRect = new RectangleF(barX, barY - 6, barW, 16); // Larger hit area

            // Background track
            using var bgBrush = new SolidBrush(ProgressBg);
            var bgRect = new RectangleF(barX, barY, barW, barH);
            using var bgPath = CreateRoundedRect(bgRect, barH / 2f);
            g.FillPath(bgBrush, bgPath);

            // Filled portion
            float progress = GetProgress();
            if (progress > 0)
            {
                float fillW = Math.Max(barH, barW * progress);
                var fillRect = new RectangleF(barX, barY, fillW, barH);
                var fillColor = _isHoveringProgress || _isDraggingProgress ? ProgressHover : ProgressFill;
                using var fillBrush = new SolidBrush(fillColor);
                using var fillPath = CreateRoundedRect(fillRect, barH / 2f);
                g.FillPath(fillBrush, fillPath);
            }

            // Thumb (only when hovering or dragging)
            if (_isHoveringProgress || _isDraggingProgress)
            {
                float thumbX = barX + barW * progress;
                float thumbY = barY + barH / 2f;
                using var thumbBrush = new SolidBrush(Color.White);
                g.FillEllipse(thumbBrush, thumbX - THUMB_RADIUS, thumbY - THUMB_RADIUS, THUMB_RADIUS * 2, THUMB_RADIUS * 2);
            }

            // Hover time tooltip
            if (_isHoveringProgress && _audioFile != null && !_isDraggingProgress)
            {
                float hoverProgress = Math.Max(0, Math.Min(1, (_hoverProgressX - barX) / (float)barW));
                TimeSpan hoverTime = TimeSpan.FromSeconds(_audioFile.TotalTime.TotalSeconds * hoverProgress);
                string hoverText = hoverTime.ToString(@"mm\:ss");
                using var tipFont = new Font("Segoe UI", 8F);
                var tipSize = g.MeasureString(hoverText, tipFont);
                float tipX = _hoverProgressX - tipSize.Width / 2;
                tipX = Math.Max(barX, Math.Min(tipX, barX + barW - tipSize.Width));
                using var tipBgBrush = new SolidBrush(Color.FromArgb(200, 20, 25, 45));
                using var tipTextBrush = new SolidBrush(Color.White);
                g.FillRectangle(tipBgBrush, tipX - 2, barY - 18, tipSize.Width + 4, tipSize.Height + 2);
                g.DrawString(hoverText, tipFont, tipTextBrush, tipX, barY - 17);
            }
        }

        private void DrawSongInfo(Graphics g)
        {
            string displayTitle = string.IsNullOrEmpty(_songTitle) ? "Chưa có bài nào" : _songTitle;
            using var titleFont = new Font("Segoe UI", 11F, FontStyle.Bold);
            using var titleBrush = new SolidBrush(string.IsNullOrEmpty(_songTitle) ? TextMuted : TextPrimary);
            var titleRect = new RectangleF(20, 35, Math.Min(320, this.Width / 3f), 45);
            var sf = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap, LineAlignment = StringAlignment.Center };
            g.DrawString(displayTitle, titleFont, titleBrush, titleRect, sf);
        }

        private void DrawControlButtons(Graphics g, int w)
        {
            int centerX = w / 2;
            int btnY = 35;

            // Play/Pause button
            float playX = centerX - BTN_SIZE - 8;
            _playBtnRect = new RectangleF(playX, btnY, BTN_SIZE, BTN_SIZE);
            var playColor = _hoverPlay ? BtnPlayHover : BtnPlay;
            using var playBrush = new SolidBrush(playColor);
            g.FillEllipse(playBrush, _playBtnRect);

            // Play/Pause icon
            bool isPlaying = _outputDevice?.PlaybackState == PlaybackState.Playing;
            using var iconFont = new Font("Segoe UI", 14F, FontStyle.Bold);
            using var iconBrush = new SolidBrush(Color.White);
            var iconSf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            if (isPlaying)
            {
                // Draw pause bars manually
                using var pausePen = new Pen(Color.White, 3) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                float cx = _playBtnRect.X + _playBtnRect.Width / 2;
                float cy = _playBtnRect.Y + _playBtnRect.Height / 2;
                g.DrawLine(pausePen, cx - 5, cy - 7, cx - 5, cy + 7);
                g.DrawLine(pausePen, cx + 5, cy - 7, cx + 5, cy + 7);
            }
            else
            {
                // Draw play triangle manually
                float cx = _playBtnRect.X + _playBtnRect.Width / 2 + 2;
                float cy = _playBtnRect.Y + _playBtnRect.Height / 2;
                var triangle = new PointF[]
                {
                    new PointF(cx - 6, cy - 8),
                    new PointF(cx - 6, cy + 8),
                    new PointF(cx + 8, cy)
                };
                using var triBrush = new SolidBrush(Color.White);
                g.FillPolygon(triBrush, triangle);
            }

            // Stop button
            float stopX = centerX + 8;
            _stopBtnRect = new RectangleF(stopX, btnY, BTN_SIZE, BTN_SIZE);
            var stopColor = _hoverStop ? BtnStopHover : BtnStop;
            using var stopBrush = new SolidBrush(stopColor);
            g.FillEllipse(stopBrush, _stopBtnRect);

            // Stop icon (square)
            using var stopIconBrush = new SolidBrush(Color.White);
            float sx = _stopBtnRect.X + _stopBtnRect.Width / 2 - 6;
            float sy = _stopBtnRect.Y + _stopBtnRect.Height / 2 - 6;
            g.FillRectangle(stopIconBrush, sx, sy, 12, 12);
        }

        private void DrawTimeDisplay(Graphics g, int w)
        {
            if (_audioFile == null)
            {
                DrawTimeText(g, w, "00:00 / 00:00");
                return;
            }

            TimeSpan current = _isDraggingProgress ? GetDragTime() : _audioFile.CurrentTime;
            TimeSpan total = _audioFile.TotalTime;
            string timeText = $"{current:mm\\:ss} / {total:mm\\:ss}";
            DrawTimeText(g, w, timeText);
        }

        private void DrawTimeText(Graphics g, int w, string text)
        {
            using var font = new Font("Segoe UI", 9.5F);
            using var brush = new SolidBrush(TextMuted);
            var size = g.MeasureString(text, font);
            float x = w / 2f + BTN_SIZE + 30;
            g.DrawString(text, font, brush, x, 50);
        }

        private void DrawVolumeControl(Graphics g, int w)
        {
            int volBarW = 80;
            int volX = w - 170; // Icon is left of the bar, far enough from close button
            int volY = 55;
            _volumeIconRect = new RectangleF(volX - 10, volY - 10, 40, 40);

            // Hover effect on icon
            if (_volumeIconRect.Contains(this.PointToClient(Cursor.Position)))
            {
                using var hoverBrush = new SolidBrush(Color.FromArgb(20, 255, 255, 255));
                g.FillEllipse(hoverBrush, _volumeIconRect);
            }

            // Speaker icon (drawn with GDI+)
            DrawSpeakerIcon(g, volX, volY);

            // Volume bar
            int barX = volX + 35;
            int barY = volY + 6;
            _volumeBarRect = new RectangleF(barX, barY - 6, volBarW, 16); // Larger hit area

            // Background track
            using var bgBrush = new SolidBrush(ProgressBg);
            var bgRect = new RectangleF(barX, barY, volBarW, VOLUME_BAR_HEIGHT);
            using var bgPath = CreateRoundedRect(bgRect, VOLUME_BAR_HEIGHT / 2f);
            g.FillPath(bgBrush, bgPath);

            // Filled portion
            if (_volume > 0)
            {
                float fillW = Math.Max(VOLUME_BAR_HEIGHT, volBarW * _volume);
                var fillRect = new RectangleF(barX, barY, fillW, VOLUME_BAR_HEIGHT);
                using var fillBrush = new SolidBrush(VolumeFill);
                using var fillPath = CreateRoundedRect(fillRect, VOLUME_BAR_HEIGHT / 2f);
                g.FillPath(fillBrush, fillPath);

                // Thumb
                if (_isDraggingVolume || _volumeBarRect.Contains(this.PointToClient(Cursor.Position)))
                {
                    float thumbX = barX + fillW;
                    float thumbY = barY + VOLUME_BAR_HEIGHT / 2f;
                    using var thumbBrush = new SolidBrush(Color.White);
                    g.FillEllipse(thumbBrush, thumbX - THUMB_RADIUS, thumbY - THUMB_RADIUS, THUMB_RADIUS * 2, THUMB_RADIUS * 2);
                }
            }
        }

        private void DrawSpeakerIcon(Graphics g, float x, float y)
        {
            // Speaker body (small rectangle + triangle)
            using var speakerBrush = new SolidBrush(TextMuted);
            using var speakerPen = new Pen(TextMuted, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round };

            // Speaker body
            g.FillRectangle(speakerBrush, x + 2, y + 3, 4, 6);

            // Speaker cone (triangle)
            var cone = new PointF[]
            {
                new PointF(x + 6, y + 2),
                new PointF(x + 12, y - 1),
                new PointF(x + 12, y + 13),
                new PointF(x + 6, y + 10)
            };
            g.FillPolygon(speakerBrush, cone);

            // Sound waves based on volume level
            if (_volume > 0)
            {
                float cx = x + 14;
                float cy = y + 6;
                // Small wave
                using var wavePen = new Pen(VolumeFill, 1.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                g.DrawArc(wavePen, cx, cy - 4, 6, 8, -45, 90);

                if (_volume > 0.35f)
                {
                    // Medium wave
                    g.DrawArc(wavePen, cx + 2, cy - 6, 8, 12, -50, 100);
                }
                if (_volume > 0.7f)
                {
                    // Large wave
                    g.DrawArc(wavePen, cx + 4, cy - 8, 10, 16, -55, 110);
                }
            }
            else
            {
                // Muted X
                using var mutePen = new Pen(BtnStop, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round };
                g.DrawLine(mutePen, x + 15, y + 2, x + 21, y + 10);
                g.DrawLine(mutePen, x + 21, y + 2, x + 15, y + 10);
            }
        }

        private float GetProgress()
        {
            if (_audioFile == null) return 0;
            if (_isDraggingProgress) return GetDragProgressRatio();
            return (float)(_audioFile.CurrentTime.TotalSeconds / Math.Max(1, _audioFile.TotalTime.TotalSeconds));
        }

        private float GetDragProgressRatio()
        {
            float barX = 15;
            float barW = this.Width - 30;
            return Math.Max(0, Math.Min(1, (_hoverProgressX - barX) / barW));
        }

        private TimeSpan GetDragTime()
        {
            if (_audioFile == null) return TimeSpan.Zero;
            return TimeSpan.FromSeconds(_audioFile.TotalTime.TotalSeconds * GetDragProgressRatio());
        }

        private static GraphicsPath CreateRoundedRect(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            float d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // ── Mouse Events ──

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool needsRepaint = false;

            bool wasHoverProgress = _isHoveringProgress;
            _isHoveringProgress = _progressBarRect.Contains(e.Location);
            if (wasHoverProgress != _isHoveringProgress) needsRepaint = true;

            if (_isHoveringProgress || _isDraggingProgress)
            {
                _hoverProgressX = e.X;
                needsRepaint = true;
            }

            bool wasHoverVolume = _isHoveringVolume;
            _isHoveringVolume = _volumeIconRect.Contains(e.Location) || _volumeBarRect.Contains(e.Location);
            if (wasHoverVolume != _isHoveringVolume) needsRepaint = true;

            bool wasHoverPlay = _hoverPlay;
            _hoverPlay = _playBtnRect.Contains(e.Location);
            if (wasHoverPlay != _hoverPlay) needsRepaint = true;

            bool wasHoverStop = _hoverStop;
            _hoverStop = _stopBtnRect.Contains(e.Location);
            if (wasHoverStop != _hoverStop) needsRepaint = true;

            bool wasHoverClose = _hoverClose;
            _hoverClose = _closeBtnRect.Contains(e.Location);
            if (wasHoverClose != _hoverClose) needsRepaint = true;

            if (_isDraggingProgress)
            {
                needsRepaint = true;
            }

            if (_isDraggingVolume)
            {
                float ratio = Math.Max(0, Math.Min(1, (e.X - _volumeBarRect.X) / _volumeBarRect.Width));
                _volume = ratio;
                ApplyVolume();
                needsRepaint = true;
            }

            this.Cursor = (_isHoveringProgress || _isHoveringVolume || _hoverPlay || _hoverStop || _hoverClose)
                ? Cursors.Hand : Cursors.Default;

            if (needsRepaint) this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;

            if (_progressBarRect.Contains(e.Location))
            {
                _isDraggingProgress = true;
                _hoverProgressX = e.X;
                this.Invalidate();
                return;
            }

            if (_volumeBarRect.Contains(e.Location))
            {
                _isDraggingVolume = true;
                float ratio = Math.Max(0, Math.Min(1, (e.X - _volumeBarRect.X) / _volumeBarRect.Width));
                _volume = ratio;
                ApplyVolume();
                this.Invalidate();
                return;
            }

            if (_volumeIconRect.Contains(e.Location))
            {
                // Toggle Mute
                if (_volume > 0)
                {
                    _volume = 0;
                }
                else
                {
                    _volume = 0.7f;
                }
                ApplyVolume();
                this.Invalidate();
                return;
            }

            if (_playBtnRect.Contains(e.Location))
            {
                TogglePlayPause();
                return;
            }

            if (_stopBtnRect.Contains(e.Location))
            {
                StopPlayback();
                return;
            }

            if (_closeBtnRect.Contains(e.Location))
            {
                StopPlayback();
                this.Visible = false;
                return;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_isDraggingProgress && _audioFile != null)
            {
                float barX = 15;
                float barW = this.Width - 30;
                float ratio = Math.Max(0, Math.Min(1, (e.X - barX) / barW));
                _audioFile.CurrentTime = TimeSpan.FromSeconds(_audioFile.TotalTime.TotalSeconds * ratio);
                _isDraggingProgress = false;
                this.Invalidate();
            }

            if (_isDraggingVolume)
            {
                _isDraggingVolume = false;
                this.Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            bool needsRepaint = _isHoveringProgress || _isHoveringVolume || _hoverPlay || _hoverStop || _hoverClose;
            _isHoveringProgress = false;
            _isHoveringVolume = false;
            _hoverPlay = false;
            _hoverStop = false;
            _hoverClose = false;
            this.Cursor = Cursors.Default;
            if (needsRepaint) this.Invalidate();
        }

        // ── Keyboard Events ──

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!this.Visible || _outputDevice == null) return base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                case Keys.Space:
                    TogglePlayPause();
                    return true;
                case Keys.Right:
                    _volume = Math.Min(1f, _volume + VOL_STEP);
                    ApplyVolume();
                    this.Invalidate();
                    return true;
                case Keys.Left:
                    _volume = Math.Max(0f, _volume - VOL_STEP);
                    ApplyVolume();
                    this.Invalidate();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ApplyVolume()
        {
            if (_outputDevice != null) _outputDevice.Volume = _volume;
        }

        // ── Playback Logic ──

        public void Play(byte[] audioData, string title)
        {
            CleanUp();
            if (audioData == null || audioData.Length == 0) return;

            _songTitle = title;

            try
            {
                _tempFilePath = Path.Combine(Path.GetTempPath(), $"naudio_temp_{Guid.NewGuid()}.mp3");
                File.WriteAllBytes(_tempFilePath, audioData);

                _audioFile = new AudioFileReader(_tempFilePath);
                _outputDevice = new WaveOutEvent();
                _outputDevice.Init(_audioFile);
                _outputDevice.Volume = _volume;
                _outputDevice.PlaybackStopped += OnPlaybackStopped;

                _outputDevice.Play();
                timerPlayback.Start();
                this.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi phát âm thanh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CleanUp();
            }
        }

        public void TogglePlayPause()
        {
            if (_outputDevice == null) return;

            if (_outputDevice.PlaybackState == PlaybackState.Playing)
            {
                _outputDevice.Pause();
                timerPlayback.Stop();
            }
            else if (_outputDevice.PlaybackState == PlaybackState.Paused)
            {
                _outputDevice.Play();
                timerPlayback.Start();
            }
            this.Invalidate();
        }

        public void AdjustVolume(float delta)
        {
            _volume = Math.Max(0f, Math.Min(1f, _volume + delta));
            ApplyVolume();
            this.Invalidate();
        }

        public bool IsActive => _outputDevice != null && this.Visible;

        private void OnPlaybackStopped(object? sender, StoppedEventArgs args)
        {
            if (InvokeRequired)
                BeginInvoke(new Action(() => StopPlayback()));
            else
                StopPlayback();
        }

        private void StopPlayback()
        {
            _outputDevice?.Stop();
            timerPlayback.Stop();
            if (_audioFile != null) _audioFile.CurrentTime = TimeSpan.Zero;
            this.Invalidate();
        }

        private void timerPlayback_Tick(object? sender, EventArgs e)
        {
            if (_audioFile != null && !_isDraggingProgress)
            {
                this.Invalidate();
            }
        }

        public void CleanUp()
        {
            timerPlayback.Stop();
            if (_outputDevice != null)
            {
                _outputDevice.Dispose();
                _outputDevice = null;
            }
            if (_audioFile != null)
            {
                _audioFile.Dispose();
                _audioFile = null;
            }
            if (!string.IsNullOrEmpty(_tempFilePath) && File.Exists(_tempFilePath))
            {
                try { File.Delete(_tempFilePath); } catch { }
            }
            _songTitle = "";
            this.Invalidate();
        }
    }
}
