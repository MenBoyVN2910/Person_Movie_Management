using System;
using System.Drawing;
using System.Windows.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.UserControls;

namespace Person_Movie_Management.Forms
{
    public partial class FrmMain : Form
    {
        private UcSidebar _sidebar;
        private FrmDropWidget _dropWidget;
        private Services.FolderWatcherService _folderWatcher;
        private bool _isClosing = false;

        public FrmMain()
        {
            InitializeComponent();
            this.BackColor = UIHelper.BgDark;
            
            // Add Sidebar
            _sidebar = new UcSidebar();
            _sidebar.Dock = DockStyle.Left;
            _sidebar.MenuItemClicked += Sidebar_MenuItemClicked;
            this.Controls.Add(_sidebar);
            
            // Load Home by default
            LoadPage("Home");
            
            // Add Audio Player
            _audioPlayer = new UcAudioPlayer();
            _audioPlayer.Visible = false; // Hide until something is played
            this.Controls.Add(_audioPlayer);
            _audioPlayer.BringToFront(); // Ensure it's on top of sidebar and content

            // Phase 5: Background AI Prefetching
            if (SessionManager.IsLoggedIn)
            {
                int currentUserId = SessionManager.CurrentUser!.Id;
                System.Threading.Tasks.Task.Run(async () => 
                {
                    try 
                    {
                        await DataCache.GetMoviesAsync(currentUserId);
                        await DataCache.GetFavoriteAudiosAsync(currentUserId);
                    } 
                    catch { }
                });
            }

            // Phase 2: DropWidget
            this.Load += (s, e) => {
                if (_dropWidget == null)
                {
                    _dropWidget = new FrmDropWidget();
                    if (SessionManager.IsDropWidgetEnabled)
                    {
                        _dropWidget.Show(); 
                    }
                }

                // Phase 3: Folder Watcher
                if (SessionManager.IsLoggedIn)
                {
                    _folderWatcher = new Services.FolderWatcherService(SessionManager.CurrentUser!.Id, this);
                    Services.AppServices.BackupSvc.Start(SessionManager.CurrentUser!.Id);
                }
            };
        }


        private UcAudioPlayer _audioPlayer;

        public void PlayGlobalAudio(byte[] audioData, string title)
        {
            if (audioData == null || audioData.Length == 0) return;
            _audioPlayer.Visible = true;
            _audioPlayer.Play(audioData, title);
            _audioPlayer.Focus();
        }

        public void ToggleDropWidget(bool show)
        {
            if (_dropWidget != null)
            {
                if (show && !_dropWidget.Visible)
                    _dropWidget.Show();
                else if (!show && _dropWidget.Visible)
                    _dropWidget.Hide();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_audioPlayer != null && _audioPlayer.IsActive)
            {
                switch (keyData)
                {
                    case Keys.Space:
                        _audioPlayer.TogglePlayPause();
                        return true;
                    case Keys.Right:
                        _audioPlayer.AdjustVolume(0.05f);
                        return true;
                    case Keys.Left:
                        _audioPlayer.AdjustVolume(-0.05f);
                        return true;
                }
            }
            
            // Feature 2: Global Omnibox Shortcut (Ctrl+K)
            if (keyData == (Keys.Control | Keys.K))
            {
                ShowOmnibox();
                return true;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ShowOmnibox()
        {
            var omnibox = new FrmOmnibox();
            omnibox.ItemSelected += (s, item) =>
            {
                if (item is Models.Movie movie)
                {
                    // Open Movie Detail
                    var frm = new FrmMovieDetail(movie);
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadPage("Home"); // Refresh
                    }
                }
                else if (item is Models.Audio audio)
                {
                    var frm = new FrmAudioDetail(audio);
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadPage("Audio");
                    }
                }
                else if (item is Models.Playlist playlist)
                {
                    LoadPage("Playlist");
                }
            };
            omnibox.ShowDialog(this);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_isClosing)
            {
                e.Cancel = true;
                _isClosing = true;
                
                // Hide the main form immediately to give the user the impression it's closed
                this.Hide();
                
                _dropWidget?.Close();
                _folderWatcher?.Stop();
                
                if (Services.AppServices.BackupSvc != null)
                {
                    await Services.AppServices.BackupSvc.PerformBackupAsync();
                    Services.AppServices.BackupSvc.Stop();
                }

                Application.Exit();
            }
            base.OnFormClosing(e);
        }



        private void Sidebar_MenuItemClicked(object? sender, string pageName)
        {
            if (pageName == "Logout")
            {
                Services.AppServices.BackupSvc.Stop();
                SessionManager.Logout();
                FrmLogin login = new FrmLogin();
                login.Show();
                this.Hide();
                return;
            }

            LoadPage(pageName);
        }

        private void LoadPage(string pageName)
        {
            pnlContent.Controls.Clear();
            UserControl uc = null;

            switch (pageName)
            {
                case "Home":
                    uc = new Person_Movie_Management.UserControls.UcDashboardHome();
                    break;
                case "OnlineMovies":
                    uc = new Person_Movie_Management.UserControls.UcMovieList(Person_Movie_Management.UserControls.MovieListMode.Online);
                    break;
                case "LocalMovies":
                    uc = new Person_Movie_Management.UserControls.UcMovieList(Person_Movie_Management.UserControls.MovieListMode.Local);
                    break;
                case "Audio":
                    uc = new Person_Movie_Management.UserControls.UcAudioList();
                    break;
                case "Favorites":
                    uc = new Person_Movie_Management.UserControls.UcMovieList(Person_Movie_Management.UserControls.MovieListMode.Favorites);
                    break;
                case "Playlist":
                    uc = new Person_Movie_Management.UserControls.UcPlaylist(SessionManager.CurrentUser.Id);
                    break;
                case "RecycleBin":
                    uc = new Person_Movie_Management.UserControls.UcRecycleBin(SessionManager.CurrentUser.Id);
                    break;
                case "Profile":
                    uc = new Person_Movie_Management.UserControls.UcUserProfile();
                    break;
                case "Backup":
                    uc = new Person_Movie_Management.UserControls.UcBackupManager();
                    break;
                case "Actor":
                    uc = new Person_Movie_Management.UserControls.UcActorList();
                    break;
            }

            if (uc != null)
            {
                uc.Dock = DockStyle.Fill;
                pnlContent.Controls.Add(uc);
            }
            else
            {
                // Placeholder
                var lbl = new Label
                {
                    Text = $"Trang {pageName} đang được xây dựng...",
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 16, FontStyle.Regular),
                    AutoSize = true,
                    Location = new Point(50, 50)
                };
                pnlContent.Controls.Add(lbl);
            }
        }

        public void RefreshSidebarUserInfo()
        {
            _sidebar?.LoadUserInfo();
        }
    }
}
