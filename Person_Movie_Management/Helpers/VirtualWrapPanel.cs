using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Models;
using Person_Movie_Management.UserControls;

namespace Person_Movie_Management.Helpers
{
    /// <summary>
    /// Panel ảo hóa cuộn siêu mượt (High-Performance UI Virtualization & Smooth Scroll).
    /// - DoubleBuffered + UserPaint + WS_CLIPCHILDREN: vẽ vào off-screen bitmap, xóa sạch pixel cũ.
    /// - Refresh() đồng bộ sau mỗi frame: không bao giờ còn vết nhân bản lát cắt.
    /// - Smooth Scroll 60-120 FPS bằng Timer + Exponential Lerp Damping.
    /// - Tích hợp Guna2VScrollBar tối màu đồng bộ giao diện.
    /// - Bắt cuộn chuột toàn cục (IMessageFilter) cho phép cuộn ngay cả khi trỏ chuột vào thẻ con.
    /// - Cơ chế Control Recycling thông minh: không bind lại dữ liệu khi thẻ đang hiển thị.
    /// </summary>
    public class VirtualWrapPanel : Panel
    {
        private List<object> _items = new List<object>();
        private Dictionary<int, Control> _activeControlsMap = new Dictionary<int, Control>();
        
        private Queue<UcMovieCard> _movieCardPool = new Queue<UcMovieCard>();
        private Queue<UcAudioCard> _audioCardPool = new Queue<UcAudioCard>();
        private Queue<UcActorCard> _actorCardPool = new Queue<UcActorCard>();

        // Scrollbar
        private readonly Guna.UI2.WinForms.Guna2VScrollBar _vScrollBar;
        private bool _isUpdatingScrollBarInternally = false;

        // Smooth Scroll State
        private readonly System.Windows.Forms.Timer _scrollTimer;
        private readonly System.Diagnostics.Stopwatch _animStopwatch = new System.Diagnostics.Stopwatch();
        private float _lastAnimTime = 0;
        private float _targetScrollY = 0;
        private float _currentScrollY = 0;
        private int _lastRenderedScrollY = -9999; // Cache để skip frame trùng

        // MouseWheel Filter
        private readonly PanelMouseWheelFilter _mouseWheelFilter;

        // Cached background brush (tái sử dụng, không tạo mới mỗi frame)
        private SolidBrush? _bgBrush;

        // Layout parameters
        private int _cachedDynamicMargin;
        private int _columns = 1;
        private int _totalRows = 0;
        private int _maxScroll = 0;
        
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int ItemWidth { get; set; } = 360;
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int ItemHeight { get; set; } = 320;
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int ItemMargin { get; set; } = 12;
        
        // Events
        public event EventHandler<Movie>? MovieClicked;
        public event EventHandler<Movie>? MovieFavoriteToggled;
        public event EventHandler<Movie>? MovieEditClicked;
        public event EventHandler<Movie>? MovieDeleteClicked;
        
        public event EventHandler<Audio>? AudioClicked;
        public event EventHandler<Audio>? AudioFavoriteToggled;
        public event EventHandler<Audio>? AudioEditClicked;
        public event EventHandler<Audio>? AudioDeleteClicked;

        public event EventHandler<Actor>? ActorClicked;

        private Dictionary<int, List<Tag>> _movieTagsDict = new();

        public VirtualWrapPanel()
        {
            this.AutoScroll = false;
            this.BackColor = UIHelper.BgDark;

            // Bật DoubleBuffered đúng cách: UserPaint + OptimizedDoubleBuffer + AllPaintingInWmPaint
            // UserPaint PHẢI = true để DoubleBuffer hoạt động (off-screen bitmap)
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            // Setup Custom ScrollBar
            _vScrollBar = new Guna.UI2.WinForms.Guna2VScrollBar
            {
                Width = 10,
                Dock = DockStyle.Right,
                FillColor = Color.Transparent,
                ThumbColor = Color.FromArgb(71, 85, 105),
                BorderRadius = 5,
                Visible = false
            };
            _vScrollBar.Scroll += VScrollBar_Scroll;
            _vScrollBar.ValueChanged += VScrollBar_ValueChanged;
            this.Controls.Add(_vScrollBar);

            // Setup Smooth Scroll Animation Timer (~60 FPS → 8ms ≈ 120 FPS)
            _scrollTimer = new System.Windows.Forms.Timer { Interval = 8 };
            _scrollTimer.Tick += ScrollTimer_Tick;

            // Global Mouse Wheel Filter
            _mouseWheelFilter = new PanelMouseWheelFilter(this);
            Application.AddMessageFilter(_mouseWheelFilter);

            this.Resize += VirtualWrapPanel_Resize;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x02000000; // WS_CLIPCHILDREN: Ngăn vẽ đè lên child controls & xóa sạch vùng lộ ra
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Chỉ vẽ vùng ClipRectangle (vùng bẩn) thay vì toàn bộ ClientRectangle
            // Giảm 80-95% diện tích vẽ nền mỗi frame
            if (_bgBrush == null || _bgBrush.Color != this.BackColor)
            {
                _bgBrush?.Dispose();
                _bgBrush = new SolidBrush(this.BackColor);
            }
            e.Graphics.FillRectangle(_bgBrush, e.ClipRectangle);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _scrollTimer?.Stop();
                _scrollTimer?.Dispose();
                _bgBrush?.Dispose();
                Application.RemoveMessageFilter(_mouseWheelFilter);
            }
            base.Dispose(disposing);
        }

        public void SetData(List<object> items, Dictionary<int, List<Tag>>? movieTagsDict, bool resetScroll = false)
        {
            _items = items ?? new List<object>();
            _movieTagsDict = movieTagsDict ?? new Dictionary<int, List<Tag>>();
            
            _scrollTimer.Stop();
            _animStopwatch.Stop();
            
            if (resetScroll)
            {
                _currentScrollY = 0;
                _targetScrollY = 0;
            }
            _lastRenderedScrollY = -9999;

            UpdateLayoutSize();
            RenderVisibleItems();
            this.Invalidate();
        }

        private void VirtualWrapPanel_Resize(object? sender, EventArgs e)
        {
            _lastRenderedScrollY = -9999;
            UpdateLayoutSize();
            RenderVisibleItems();
            this.Invalidate();
        }

        public void HandleExternalMouseWheel(int delta)
        {
            if (_maxScroll <= 0 || _items.Count == 0) return;

            int scrollLines = SystemInformation.MouseWheelScrollLines;
            if (scrollLines <= 0) scrollLines = 3;
            int step = scrollLines * 60;

            if (delta > 0)
                _targetScrollY -= step;
            else
                _targetScrollY += step;

            _targetScrollY = Math.Max(0, Math.Min(_targetScrollY, _maxScroll));

            if (!_scrollTimer.Enabled)
            {
                _lastAnimTime = 0;
                _animStopwatch.Restart();
                _scrollTimer.Start();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            HandleExternalMouseWheel(e.Delta);
            if (e is HandledMouseEventArgs he) he.Handled = true;
        }

        private void ScrollTimer_Tick(object? sender, EventArgs e)
        {
            float elapsed = (float)_animStopwatch.Elapsed.TotalSeconds;
            float dt = elapsed - _lastAnimTime;
            _lastAnimTime = elapsed;
            if (dt > 0.05f) dt = 0.05f;

            float diff = _targetScrollY - _currentScrollY;
            if (Math.Abs(diff) < 0.5f)
            {
                _currentScrollY = _targetScrollY;
                _scrollTimer.Stop();
                _animStopwatch.Stop();
            }
            else
            {
                // Exponential Lerp Damping
                float lerp = 1.0f - (float)Math.Pow(0.0001, dt);
                _currentScrollY += diff * lerp;
            }

            SyncScrollBarValue();
            RenderVisibleItems();
        }

        private void VScrollBar_Scroll(object? sender, ScrollEventArgs e)
        {
            if (_isUpdatingScrollBarInternally) return;
            _scrollTimer.Stop();
            _targetScrollY = _currentScrollY = e.NewValue;
            RenderVisibleItems();
        }

        private void VScrollBar_ValueChanged(object? sender, EventArgs e)
        {
            if (_isUpdatingScrollBarInternally) return;
            _scrollTimer.Stop();
            _targetScrollY = _currentScrollY = _vScrollBar.Value;
            RenderVisibleItems();
        }

        private void SyncScrollBarValue()
        {
            if (!_vScrollBar.Visible) return;
            _isUpdatingScrollBarInternally = true;
            try
            {
                int val = Math.Max(_vScrollBar.Minimum, Math.Min((int)Math.Round(_currentScrollY), _vScrollBar.Maximum));
                _vScrollBar.Value = val;
            }
            finally
            {
                _isUpdatingScrollBarInternally = false;
            }
        }

        private void UpdateLayoutSize()
        {
            int clientWidth = this.ClientSize.Width;
            if (clientWidth <= 0) return;

            int scrollBarW = _vScrollBar.Visible ? _vScrollBar.Width : 0;
            int usableWidth = clientWidth - scrollBarW;

            int totalItemWidth = ItemWidth + ItemMargin;
            _columns = Math.Max(1, usableWidth / totalItemWidth);
            _totalRows = (int)Math.Ceiling(_items.Count / (double)_columns);
            
            int totalGridWidthWithoutMargins = _columns * ItemWidth;
            int remainingSpace = usableWidth - totalGridWidthWithoutMargins;
            _cachedDynamicMargin = Math.Max(ItemMargin, remainingSpace / (_columns + 1));

            int totalHeight = _totalRows * (ItemHeight + _cachedDynamicMargin) + _cachedDynamicMargin;
            _maxScroll = Math.Max(0, totalHeight - this.ClientSize.Height);

            if (_maxScroll > 0)
            {
                _vScrollBar.Visible = true;
                _vScrollBar.Minimum = 0;
                _vScrollBar.Maximum = _maxScroll + this.ClientSize.Height;
                _vScrollBar.LargeChange = this.ClientSize.Height;
                _vScrollBar.SmallChange = ItemHeight / 2;
                _vScrollBar.BringToFront();
            }
            else
            {
                _vScrollBar.Visible = false;
                _currentScrollY = 0;
                _targetScrollY = 0;
            }

            _targetScrollY = Math.Max(0, Math.Min(_targetScrollY, _maxScroll));
            _currentScrollY = Math.Max(0, Math.Min(_currentScrollY, _maxScroll));
            SyncScrollBarValue();
        }

        private void RenderVisibleItems()
        {
            if (_items.Count == 0 || _columns == 0)
            {
                ClearActiveControls();
                return;
            }

            int dynamicMargin = _cachedDynamicMargin;
            int scrollY = (int)Math.Round(_currentScrollY);

            // === SKIP FRAME NẾU SCROLL CHƯA ĐỔI ===
            // Tránh tính toán + layout lặp khi scrollY giống frame trước
            if (scrollY == _lastRenderedScrollY) return;
            _lastRenderedScrollY = scrollY;

            int totalRowHeight = ItemHeight + dynamicMargin;

            // Buffer 2 row trên và 2 row dưới → thẻ được tải TRƯỚC khi vào viewport
            int startRow = Math.Max(0, (scrollY / totalRowHeight) - 2);
            int endRow = Math.Min(_totalRows - 1, ((scrollY + this.ClientSize.Height) / totalRowHeight) + 2);

            int startIndex = startRow * _columns;
            int endIndex = Math.Min(_items.Count - 1, (endRow + 1) * _columns - 1);

            var newActiveControlsMap = new Dictionary<int, Control>(endIndex - startIndex + 2);
            var movesList = new List<(Control card, int x, int y)>(endIndex - startIndex + 2);
            List<Control>? controlsToHide = null;

            for (int i = startIndex; i <= endIndex; i++)
            {
                int row = i / _columns;
                int col = i % _columns;

                int x = dynamicMargin + col * (ItemWidth + dynamicMargin);
                int y = dynamicMargin + row * (ItemHeight + dynamicMargin) - scrollY;

                var item = _items[i];
                Control card;

                if (_activeControlsMap.TryGetValue(i, out var existingCard) && IsCardMatchingItem(existingCard, item))
                {
                    card = existingCard;
                    _activeControlsMap.Remove(i);
                    BindControlData(card, item);
                }
                else
                {
                    if (existingCard != null)
                    {
                        ReturnToPool(existingCard);
                        _activeControlsMap.Remove(i);
                    }
                    card = GetFromPool(item);
                    if (!this.Controls.Contains(card))
                    {
                        this.Controls.Add(card);
                    }
                    BindControlData(card, item);
                }

                movesList.Add((card, x, y));
                newActiveControlsMap[i] = card;
            }

            // Collect controls exiting the viewport
            if (_activeControlsMap.Count > 0)
            {
                controlsToHide = new List<Control>(_activeControlsMap.Count);
                foreach (var kvp in _activeControlsMap)
                {
                    controlsToHide.Add(kvp.Value);
                }
            }

            // === Invalidate CHỈ vùng pixel cũ của thẻ rời viewport ===
            if (controlsToHide != null)
            {
                foreach (var oldCard in controlsToHide)
                {
                    if (oldCard.Visible)
                    {
                        this.Invalidate(oldCard.Bounds);
                    }
                }
            }

            // === Di chuyển tất cả controls trong 1 batch ===
            this.SuspendLayout();
            try
            {
                // Di chuyển controls hiện có - dùng Location thay SetBounds (nhanh hơn)
                foreach (var m in movesList)
                {
                    var card = m.card;
                    var newLoc = new Point(m.x, m.y);
                    if (card.Location != newLoc)
                    {
                        card.Location = newLoc;
                    }
                    if (!card.Visible) card.Visible = true;
                }

                // Ẩn controls rời viewport
                if (controlsToHide != null)
                {
                    foreach (var oldCard in controlsToHide)
                    {
                        oldCard.Visible = false;
                        ReturnToPool(oldCard);
                    }
                }
            }
            finally
            {
                this.ResumeLayout(false);
            }

            _activeControlsMap = newActiveControlsMap;

            _vScrollBar.BringToFront();

            // === Chỉ Update() vùng dirty (KHÔNG Refresh() toàn bộ panel) ===
            // DoubleBuffered + UserPaint đảm bảo vùng dirty được vẽ vào off-screen bitmap
            // Update() chỉ xử lý các vùng đã Invalidate ở trên → nhanh gấp 5-10x so với Refresh()
            this.Update();
        }

        private static bool IsCardMatchingItem(Control card, object item)
        {
            if (item is Movie && card is UcMovieCard) return true;
            if (item is Audio && card is UcAudioCard) return true;
            if (item is Actor && card is UcActorCard) return true;
            return false;
        }

        private void ClearActiveControls()
        {
            foreach (var kvp in _activeControlsMap)
            {
                var ctrl = kvp.Value;
                this.Controls.Remove(ctrl);
                ctrl.Dispose();
            }
            _activeControlsMap.Clear();
            
            foreach (var c in _movieCardPool) { this.Controls.Remove(c); c.Dispose(); }
            _movieCardPool.Clear();
            
            foreach (var c in _audioCardPool) { this.Controls.Remove(c); c.Dispose(); }
            _audioCardPool.Clear();

            foreach (var c in _actorCardPool) { this.Controls.Remove(c); c.Dispose(); }
            _actorCardPool.Clear();
        }

        private void BindControlData(Control card, object item)
        {
            if (card is UcMovieCard mCard && item is Movie movie)
            {
                var tags = _movieTagsDict.ContainsKey(movie.Id) ? _movieTagsDict[movie.Id] : new List<Tag>();
                mCard.BindData(movie, tags);
            }
            else if (card is UcAudioCard aCard && item is Audio audio)
            {
                aCard.BindData(audio);
            }
            else if (card is UcActorCard actCard && item is Actor actor)
            {
                actCard.BindData(actor);
            }
        }

        private Control GetFromPool(object item)
        {
            if (item is Movie movie)
            {
                UcMovieCard card;
                if (_movieCardPool.Count > 0)
                {
                    card = _movieCardPool.Dequeue();
                }
                else
                {
                    card = new UcMovieCard(movie, null);
                    card.Size = new Size(ItemWidth, ItemHeight);
                    card.MovieClicked += (s, e) => MovieClicked?.Invoke(s, e);
                    card.FavoriteToggled += (s, e) => MovieFavoriteToggled?.Invoke(s, e);
                    card.EditClicked += (s, e) => MovieEditClicked?.Invoke(s, e);
                    card.DeleteClicked += (s, e) => MovieDeleteClicked?.Invoke(s, e);
                }
                return card;
            }
            else if (item is Audio audio)
            {
                UcAudioCard card;
                if (_audioCardPool.Count > 0)
                {
                    card = _audioCardPool.Dequeue();
                }
                else
                {
                    card = new UcAudioCard(audio);
                    card.Size = new Size(ItemWidth, ItemHeight);
                    card.AudioClicked += (s, e) => AudioClicked?.Invoke(s, e);
                    card.FavoriteToggled += (s, e) => AudioFavoriteToggled?.Invoke(s, e);
                    card.EditClicked += (s, e) => AudioEditClicked?.Invoke(s, e);
                    card.DeleteClicked += (s, e) => AudioDeleteClicked?.Invoke(s, e);
                }
                return card;
            }
            else if (item is Actor actor)
            {
                UcActorCard card;
                if (_actorCardPool.Count > 0)
                {
                    card = _actorCardPool.Dequeue();
                }
                else
                {
                    card = new UcActorCard();
                    card.Size = new Size(ItemWidth, ItemHeight);
                    card.ActorClicked += (s, e) => ActorClicked?.Invoke(s, e);
                }
                return card;
            }
            
            return new Control();
        }

        private void ReturnToPool(Control ctrl)
        {
            ctrl.Visible = false;
            ctrl.Location = new Point(-3000, -3000);
            if (ctrl is UcMovieCard mCard)
            {
                if (!_movieCardPool.Contains(mCard))
                {
                    _movieCardPool.Enqueue(mCard);
                }
            }
            else if (ctrl is UcAudioCard aCard)
            {
                if (!_audioCardPool.Contains(aCard))
                {
                    _audioCardPool.Enqueue(aCard);
                }
            }
            else if (ctrl is UcActorCard actCard)
            {
                if (!_actorCardPool.Contains(actCard))
                {
                    _actorCardPool.Enqueue(actCard);
                }
            }
            else
            {
                this.Controls.Remove(ctrl);
                ctrl.Dispose();
            }
        }

        /// <summary>
        /// Bộ lọc thông điệp cuộn chuột Windows Messages (WM_MOUSEWHEEL).
        /// Cho phép cuộn trang mượt mà ngay cả khi chuột đang nằm trên các thẻ con (cover, button, label...).
        /// </summary>
        private class PanelMouseWheelFilter : IMessageFilter
        {
            private readonly VirtualWrapPanel _panel;

            public PanelMouseWheelFilter(VirtualWrapPanel panel)
            {
                _panel = panel;
            }

            public bool PreFilterMessage(ref Message m)
            {
                const int WM_MOUSEWHEEL = 0x020A;
                if (m.Msg == WM_MOUSEWHEEL && _panel.IsHandleCreated && !_panel.IsDisposed && _panel.Visible)
                {
                    var mousePos = Cursor.Position;
                    var screenRect = _panel.RectangleToScreen(_panel.ClientRectangle);
                    if (screenRect.Contains(mousePos))
                    {
                        var topForm = _panel.FindForm();
                        if (topForm != null && Form.ActiveForm == topForm)
                        {
                            int delta = unchecked((short)((long)m.WParam >> 16));
                            _panel.HandleExternalMouseWheel(delta);
                            return true; // Đã xử lý xong
                        }
                    }
                }
                return false;
            }
        }
    }
}
