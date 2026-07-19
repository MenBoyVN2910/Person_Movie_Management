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
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Sidebar_MenuItemClicked(object? sender, string pageName)
        {
            if (pageName == "Logout")
            {
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
                case "Profile":
                    uc = new Person_Movie_Management.UserControls.UcUserProfile();
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
