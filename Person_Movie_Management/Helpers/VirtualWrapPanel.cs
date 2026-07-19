using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Person_Movie_Management.Models;
using Person_Movie_Management.UserControls;

namespace Person_Movie_Management.Helpers
{
    /// <summary>
    /// Panel ảo hóa giao diện (UI Virtualization / Control Pooling).
    /// Chỉ tạo ra số lượng UserControl đủ để lấp đầy màn hình hiện tại.
    /// DoubleBuffered + Throttled Render (~60fps) = mượt nhất.
    /// </summary>
    public class VirtualWrapPanel : Panel
    {
        [DllImport("winmm.dll")]
        private static extern uint timeBeginPeriod(uint period);
        [DllImport("winmm.dll")]
        private static extern uint timeEndPeriod(uint period);

        private List<object> _items = new List<object>();
        private Dictionary<int, Control> _activeControlsMap = new Dictionary<int, Control>();
        
        private Queue<UcMovieCard> _movieCardPool = new Queue<UcMovieCard>();
        private Queue<UcAudioCard> _audioCardPool = new Queue<UcAudioCard>();

        // Smooth scroll state
        private bool _isAnimating = false;
        private float _targetScrollY = 0;
        private float _currentScrollY = 0;

        // Throttle render: chỉ render mỗi 16ms (~60fps)
        private long _lastRenderTick = 0;

        // Cached layout
        private int _cachedDynamicMargin;
        private int _lastRenderedStartIndex = -1;
        private int _lastRenderedEndIndex = -1;
        
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int ItemWidth { get; set; } = 360;
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int ItemHeight { get; set; } = 320;
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int ItemMargin { get; set; } = 12;
        
        private int _columns = 1;
        private int _totalRows = 0;
        
        // Events
        public event EventHandler<Movie> MovieClicked;
        public event EventHandler<Movie> MovieFavoriteToggled;
        public event EventHandler<Movie> MovieEditClicked;
        public event EventHandler<Movie> MovieDeleteClicked;
        
        public event EventHandler<Audio> AudioClicked;
        public event EventHandler<Audio> AudioFavoriteToggled;
        public event EventHandler<Audio> AudioEditClicked;
        public event EventHandler<Audio> AudioDeleteClicked;

        private Dictionary<int, List<Tag>> _movieTagsDict = new();

        public VirtualWrapPanel()
        {
            this.DoubleBuffered = true;
            this.AutoScroll = true;
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
            this.Resize += VirtualWrapPanel_Resize;
        }

        // BỎ WS_EX_COMPOSITED: flag này buộc render TẤT CẢ child controls vào buffer mỗi frame.
        // Kết hợp smooth scroll chạy liên tục → overhead rất lớn.
        // DoubleBuffered + OptimizedDoubleBuffer đã đủ mượt.
        // (Không override CreateParams nữa)

        public void SetData(List<object> items, Dictionary<int, List<Tag>> movieTagsDict)
        {
            _items = items;
            _movieTagsDict = movieTagsDict ?? new Dictionary<int, List<Tag>>();
            
            foreach (var kvp in _activeControlsMap)
            {
                kvp.Value.Visible = false;
                ReturnToPool(kvp.Value);
            }
            _activeControlsMap.Clear();

            _lastRenderedStartIndex = -1;
            _lastRenderedEndIndex = -1;
            UpdateLayoutSize();
            RenderVisibleItems();
        }

        private void VirtualWrapPanel_Resize(object sender, EventArgs e)
        {
            _lastRenderedStartIndex = -1;
            _lastRenderedEndIndex = -1;
            UpdateLayoutSize();
            RenderVisibleItems();
        }

        // Cuộn native - chỉ cần render lại các thẻ hiển thị
        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            if (!_isAnimating)
            {
                _currentScrollY = -this.AutoScrollPosition.Y;
                _targetScrollY = _currentScrollY;
            }
            RenderVisibleItems();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!_isAnimating)
            {
                _currentScrollY = -this.AutoScrollPosition.Y;
                _targetScrollY = _currentScrollY;
            }

            // Quãng đường cuộn vừa phải — đủ momentum nhưng kết thúc nhanh
            int scrollAmount = SystemInformation.MouseWheelScrollLines * 80;
            if (e.Delta > 0)
                _targetScrollY -= scrollAmount;
            else
                _targetScrollY += scrollAmount;

            int maxScroll = this.AutoScrollMinSize.Height - this.ClientSize.Height;
            if (maxScroll < 0) maxScroll = 0;

            _targetScrollY = Math.Max(0, Math.Min(_targetScrollY, maxScroll));

            if (!_isAnimating)
            {
                StartSmoothScroll();
            }

            if (e is HandledMouseEventArgs he)
                he.Handled = true;
        }

        private async void StartSmoothScroll()
        {
            _isAnimating = true;
            
            // Ép hệ thống chuyển bộ đếm giờ về 1ms
            timeBeginPeriod(1);
            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                float lastTime = 0;
                _lastRenderTick = 0;

                while (Math.Abs(_targetScrollY - _currentScrollY) >= 0.5f)
                {
                    float currentTime = (float)sw.Elapsed.TotalSeconds;
                    float dt = currentTime - lastTime;
                    lastTime = currentTime;

                    if (dt > 0.05f) dt = 0.05f; // Cap at 20fps equivalent if lagging

                    float diff = _targetScrollY - _currentScrollY;
                    
                    // Lerp factor mạnh hơn (0.005) → momentum mềm mà kết thúc nhanh
                    float lerpFactor = 1.0f - (float)Math.Pow(0.005, dt); 
                    _currentScrollY += diff * lerpFactor;

                    this.AutoScrollPosition = new Point(0, (int)Math.Round(_currentScrollY));

                    // THROTTLE: Chỉ gọi RenderVisibleItems mỗi 16ms (~60fps)
                    // Giữa các frame chỉ update vị trí card (cực nhẹ)
                    long nowMs = sw.ElapsedMilliseconds;
                    if (nowMs - _lastRenderTick >= 16)
                    {
                        RenderVisibleItems();
                        _lastRenderTick = nowMs;
                    }
                    else
                    {
                        UpdatePositionsOnly(_cachedDynamicMargin);
                    }

                    await System.Threading.Tasks.Task.Delay(1);
                }

                _currentScrollY = _targetScrollY;
                this.AutoScrollPosition = new Point(0, (int)Math.Round(_currentScrollY));
                RenderVisibleItems();
            }
            finally
            {
                timeEndPeriod(1); // Phục hồi bộ đếm giờ để tiết kiệm pin
                _isAnimating = false;
            }
        }

        private void UpdateLayoutSize()
        {
            if (this.ClientSize.Width == 0) return;
            
            int totalItemWidth = ItemWidth + ItemMargin;
            _columns = Math.Max(1, this.ClientSize.Width / totalItemWidth);
            _totalRows = (int)Math.Ceiling(_items.Count / (double)_columns);
            
            int totalGridWidthWithoutMargins = _columns * ItemWidth;
            int remainingSpace = this.ClientSize.Width - totalGridWidthWithoutMargins;
            _cachedDynamicMargin = Math.Max(ItemMargin, remainingSpace / (_columns + 1));

            int totalHeight = _totalRows * (ItemHeight + _cachedDynamicMargin) + _cachedDynamicMargin;
            this.AutoScrollMinSize = new Size(0, totalHeight);
        }

        private void RenderVisibleItems()
        {
            if (_items.Count == 0 || _columns == 0)
            {
                ClearActiveControls();
                return;
            }

            int dynamicMargin = _cachedDynamicMargin;
            int scrollY = -this.AutoScrollPosition.Y;
            int totalItemHeight = ItemHeight + dynamicMargin;
            
            // Buffer 2 hàng trên/dưới (tăng từ 1 → 2) → pre-render card trước khi user cuộn tới
            int startRow = Math.Max(0, scrollY / totalItemHeight - 2);
            int endRow = Math.Min(_totalRows - 1, (scrollY + this.ClientSize.Height) / totalItemHeight + 2);

            int startIndex = startRow * _columns;
            int endIndex = Math.Min(_items.Count - 1, (endRow + 1) * _columns - 1);

            // Skip nếu visible range không đổi - chỉ update vị trí
            if (startIndex == _lastRenderedStartIndex && endIndex == _lastRenderedEndIndex)
            {
                UpdatePositionsOnly(dynamicMargin);
                return;
            }
            _lastRenderedStartIndex = startIndex;
            _lastRenderedEndIndex = endIndex;

            Dictionary<int, Control> newActiveControlsMap = new Dictionary<int, Control>();

            this.SuspendLayout();

            for (int i = startIndex; i <= endIndex; i++)
            {
                int row = i / _columns;
                int col = i % _columns;

                int x = dynamicMargin + col * (ItemWidth + dynamicMargin);
                int y = dynamicMargin + row * (ItemHeight + dynamicMargin);

                var item = _items[i];
                Control card;

                if (_activeControlsMap.TryGetValue(i, out var existingCard))
                {
                    card = existingCard;
                    _activeControlsMap.Remove(i);
                }
                else
                {
                    card = GetFromPool(item);
                    if (!this.Controls.Contains(card))
                    {
                        this.Controls.Add(card);
                    }
                }

                Point targetLoc = new Point(x, y + this.AutoScrollPosition.Y);
                if (card.Location != targetLoc) card.Location = targetLoc;
                if (!card.Visible) card.Visible = true;
                
                newActiveControlsMap[i] = card;
            }

            // Trả pool các control cũ
            foreach (var kvp in _activeControlsMap)
            {
                var oldCard = kvp.Value;
                oldCard.Visible = false;
                ReturnToPool(oldCard);
            }

            _activeControlsMap = newActiveControlsMap;
            this.ResumeLayout(false);
        }

        private void UpdatePositionsOnly(int dynamicMargin)
        {
            int scrollOffsetY = this.AutoScrollPosition.Y;
            foreach (var kvp in _activeControlsMap)
            {
                int i = kvp.Key;
                var card = kvp.Value;
                int row = i / _columns;
                int col = i % _columns;
                int x = dynamicMargin + col * (ItemWidth + dynamicMargin);
                int y = dynamicMargin + row * (ItemHeight + dynamicMargin);
                
                Point targetLoc = new Point(x, y + scrollOffsetY);
                if (card.Location != targetLoc) card.Location = targetLoc;
            }
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
            
            foreach(var c in _movieCardPool) { this.Controls.Remove(c); c.Dispose(); }
            _movieCardPool.Clear();
            
            foreach(var c in _audioCardPool) { this.Controls.Remove(c); c.Dispose(); }
            _audioCardPool.Clear();

            _lastRenderedStartIndex = -1;
            _lastRenderedEndIndex = -1;
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
                
                var tags = _movieTagsDict.ContainsKey(movie.Id) ? _movieTagsDict[movie.Id] : new List<Tag>();
                card.BindData(movie, tags);
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
                card.BindData(audio);
                return card;
            }
            
            return new Control();
        }

        private void ReturnToPool(Control ctrl)
        {
            if (ctrl is UcMovieCard mCard)
            {
                _movieCardPool.Enqueue(mCard);
            }
            else if (ctrl is UcAudioCard aCard)
            {
                _audioCardPool.Enqueue(aCard);
            }
            else
            {
                ctrl.Dispose();
            }
        }
    }
}
