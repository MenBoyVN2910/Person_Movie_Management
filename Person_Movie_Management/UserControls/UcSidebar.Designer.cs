namespace Person_Movie_Management.UserControls
{
    partial class UcSidebar
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblDisplayName = new System.Windows.Forms.Label();
            this.picAvatar = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.pnlSeparator = new System.Windows.Forms.Panel();
            this.btnHome = new Guna.UI2.WinForms.Guna2Button();
            this.btnOnlineMovies = new Guna.UI2.WinForms.Guna2Button();
            this.btnLocalMovies = new Guna.UI2.WinForms.Guna2Button();
            this.btnAudio = new Guna.UI2.WinForms.Guna2Button();
            this.btnFavorites = new Guna.UI2.WinForms.Guna2Button();
            this.btnActor = new Guna.UI2.WinForms.Guna2Button();
            this.btnPlaylist = new Guna.UI2.WinForms.Guna2Button();
            this.btnRecycleBin = new Guna.UI2.WinForms.Guna2Button();
            this.btnProfile = new Guna.UI2.WinForms.Guna2Button();
            this.btnBackup = new Guna.UI2.WinForms.Guna2Button();
            this.btnLogout = new Guna.UI2.WinForms.Guna2Button();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblUsername);
            this.pnlTop.Controls.Add(this.lblDisplayName);
            this.pnlTop.Controls.Add(this.picAvatar);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(0, 20, 0, 10);
            this.pnlTop.Size = new System.Drawing.Size(230, 180);
            this.pnlTop.TabIndex = 0;
            // 
            // picAvatar
            // 
            this.picAvatar.ImageRotate = 0F;
            this.picAvatar.Location = new System.Drawing.Point(75, 25);
            this.picAvatar.Name = "picAvatar";
            this.picAvatar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.picAvatar.ShadowDecoration.Enabled = true;
            this.picAvatar.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.picAvatar.ShadowDecoration.Depth = 8;
            this.picAvatar.Size = new System.Drawing.Size(80, 80);
            this.picAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAvatar.TabIndex = 0;
            this.picAvatar.TabStop = false;
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDisplayName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.lblDisplayName.Location = new System.Drawing.Point(0, 115);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(230, 28);
            this.lblDisplayName.TabIndex = 1;
            this.lblDisplayName.Text = "Display Name";
            this.lblDisplayName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUsername
            // 
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblUsername.Location = new System.Drawing.Point(0, 143);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(230, 20);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "@username";
            this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlSeparator
            // 
            this.pnlSeparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            this.pnlSeparator.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSeparator.Location = new System.Drawing.Point(0, 180);
            this.pnlSeparator.Margin = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.pnlSeparator.Name = "pnlSeparator";
            this.pnlSeparator.Size = new System.Drawing.Size(230, 1);
            this.pnlSeparator.TabIndex = 7;
            // 
            // btnHome
            // 
            this.btnHome.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnHome.Checked = true;
            this.btnHome.BorderRadius = 10;
            this.btnHome.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnHome.ForeColor = System.Drawing.Color.White;
            this.btnHome.Location = new System.Drawing.Point(0, 191);
            this.btnHome.Margin = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(230, 45);
            this.btnHome.TabIndex = 1;
            this.btnHome.Text = "    🏠  Trang chủ";
            this.btnHome.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnHome.TextOffset = new System.Drawing.Point(10, 0);
            this.btnHome.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // btnOnlineMovies
            // 
            this.btnOnlineMovies.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnOnlineMovies.BorderRadius = 10;
            this.btnOnlineMovies.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOnlineMovies.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnOnlineMovies.ForeColor = System.Drawing.Color.White;
            this.btnOnlineMovies.Location = new System.Drawing.Point(0, 236);
            this.btnOnlineMovies.Name = "btnOnlineMovies";
            this.btnOnlineMovies.Size = new System.Drawing.Size(230, 45);
            this.btnOnlineMovies.TabIndex = 2;
            this.btnOnlineMovies.Text = "    🌐  Phim Online";
            this.btnOnlineMovies.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnOnlineMovies.TextOffset = new System.Drawing.Point(10, 0);
            this.btnOnlineMovies.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // btnLocalMovies
            // 
            this.btnLocalMovies.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnLocalMovies.BorderRadius = 10;
            this.btnLocalMovies.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnLocalMovies.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnLocalMovies.ForeColor = System.Drawing.Color.White;
            this.btnLocalMovies.Location = new System.Drawing.Point(0, 281);
            this.btnLocalMovies.Name = "btnLocalMovies";
            this.btnLocalMovies.Size = new System.Drawing.Size(230, 45);
            this.btnLocalMovies.TabIndex = 3;
            this.btnLocalMovies.Text = "    📁  Phim Trên Máy";
            this.btnLocalMovies.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnLocalMovies.TextOffset = new System.Drawing.Point(10, 0);
            this.btnLocalMovies.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // btnAudio
            // 
            this.btnAudio.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnAudio.BorderRadius = 10;
            this.btnAudio.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAudio.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnAudio.ForeColor = System.Drawing.Color.White;
            this.btnAudio.Location = new System.Drawing.Point(0, 326);
            this.btnAudio.Name = "btnAudio";
            this.btnAudio.Size = new System.Drawing.Size(230, 45);
            this.btnAudio.TabIndex = 4;
            this.btnAudio.Text = "    🎵  Âm thanh";
            this.btnAudio.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnAudio.TextOffset = new System.Drawing.Point(10, 0);
            this.btnAudio.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // btnFavorites
            // 
            this.btnFavorites.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnFavorites.BorderRadius = 10;
            this.btnFavorites.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnFavorites.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnFavorites.ForeColor = System.Drawing.Color.White;
            this.btnFavorites.Location = new System.Drawing.Point(0, 371);
            this.btnFavorites.Name = "btnFavorites";
            this.btnFavorites.Size = new System.Drawing.Size(230, 45);
            this.btnFavorites.TabIndex = 5;
            this.btnFavorites.Text = "    ❤️  Yêu Thích";
            this.btnFavorites.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnFavorites.TextOffset = new System.Drawing.Point(10, 0);
            this.btnFavorites.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // btnActor
            // 
            this.btnActor.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnActor.BorderRadius = 10;
            this.btnActor.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnActor.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnActor.ForeColor = System.Drawing.Color.White;
            this.btnActor.Location = new System.Drawing.Point(0, 416);
            this.btnActor.Name = "btnActor";
            this.btnActor.Size = new System.Drawing.Size(230, 45);
            this.btnActor.TabIndex = 6;
            this.btnActor.Text = "    💃  Diễn viên";
            this.btnActor.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnActor.TextOffset = new System.Drawing.Point(10, 0);
            this.btnActor.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // btnPlaylist
            // 
            this.btnPlaylist.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPlaylist.FillColor = System.Drawing.Color.Transparent;
            this.btnPlaylist.BorderRadius = 10;
            this.btnPlaylist.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnPlaylist.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnPlaylist.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnPlaylist.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnPlaylist.Location = new System.Drawing.Point(0, 310);
            this.btnPlaylist.Name = "btnPlaylist";
            this.btnPlaylist.Size = new System.Drawing.Size(230, 50);
            this.btnPlaylist.TabIndex = 9;
            this.btnPlaylist.Text = "    📋  Danh sách phát";
            this.btnPlaylist.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPlaylist.TextOffset = new System.Drawing.Point(10, 0);
            this.btnPlaylist.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // btnRecycleBin
            // 
            this.btnRecycleBin.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRecycleBin.FillColor = System.Drawing.Color.Transparent;
            this.btnRecycleBin.BorderRadius = 10;
            this.btnRecycleBin.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnRecycleBin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnRecycleBin.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnRecycleBin.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnRecycleBin.Location = new System.Drawing.Point(0, 360);
            this.btnRecycleBin.Name = "btnRecycleBin";
            this.btnRecycleBin.Size = new System.Drawing.Size(230, 50);
            this.btnRecycleBin.TabIndex = 8;
            this.btnRecycleBin.Text = "    🗑️  Thùng rác";
            this.btnRecycleBin.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnRecycleBin.TextOffset = new System.Drawing.Point(10, 0);
            this.btnRecycleBin.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // btnProfile
            // 
            this.btnProfile.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnProfile.BorderRadius = 10;
            this.btnProfile.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnProfile.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnProfile.ForeColor = System.Drawing.Color.White;
            this.btnProfile.Location = new System.Drawing.Point(0, 416);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(230, 45);
            this.btnProfile.TabIndex = 6;
            this.btnProfile.Text = "    👤  Hồ sơ cá nhân";
            this.btnProfile.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnProfile.TextOffset = new System.Drawing.Point(10, 0);
            this.btnProfile.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // 
            // btnBackup
            // 
            this.btnBackup.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnBackup.BorderRadius = 10;
            this.btnBackup.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnBackup.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnBackup.ForeColor = System.Drawing.Color.White;
            this.btnBackup.Location = new System.Drawing.Point(0, 461);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(230, 45);
            this.btnBackup.TabIndex = 7;
            this.btnBackup.Text = "    💾  Sao lưu";
            this.btnBackup.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnBackup.TextOffset = new System.Drawing.Point(10, 0);
            this.btnBackup.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnLogout.FillColor = System.Drawing.Color.Transparent;
            this.btnLogout.BorderRadius = 10;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnLogout.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(248)))), ((int)(((byte)(113)))), ((int)(((byte)(113)))));
            this.btnLogout.HoverState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(113)))), ((int)(((byte)(113)))));
            this.btnLogout.Location = new System.Drawing.Point(0, 650);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(230, 50);
            this.btnLogout.TabIndex = 6;
            this.btnLogout.Text = "    🚪  Đăng xuất";
            this.btnLogout.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnLogout.TextOffset = new System.Drawing.Point(10, 0);
            this.btnLogout.Click += new System.EventHandler(this.MenuButton_Click);
            // 
            // UcSidebar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(21)))), ((int)(((byte)(42)))));
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.btnProfile);
            this.Controls.Add(this.btnRecycleBin);
            this.Controls.Add(this.btnPlaylist);
            this.Controls.Add(this.btnActor);
            this.Controls.Add(this.btnFavorites);
            this.Controls.Add(this.btnAudio);
            this.Controls.Add(this.btnLocalMovies);
            this.Controls.Add(this.btnOnlineMovies);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.pnlSeparator);
            this.Controls.Add(this.pnlTop);
            this.Name = "UcSidebar";
            this.Size = new System.Drawing.Size(230, 700);
            this.pnlTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlTop;
        private Guna.UI2.WinForms.Guna2CirclePictureBox picAvatar;
        private System.Windows.Forms.Label lblDisplayName;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Panel pnlSeparator;
        private Guna.UI2.WinForms.Guna2Button btnHome;
        private Guna.UI2.WinForms.Guna2Button btnOnlineMovies;
        private Guna.UI2.WinForms.Guna2Button btnLocalMovies;
        private Guna.UI2.WinForms.Guna2Button btnAudio;
        private Guna.UI2.WinForms.Guna2Button btnFavorites;
        private Guna.UI2.WinForms.Guna2Button btnActor;
        private Guna.UI2.WinForms.Guna2Button btnPlaylist;
        private Guna.UI2.WinForms.Guna2Button btnRecycleBin;
        private Guna.UI2.WinForms.Guna2Button btnProfile;
        private Guna.UI2.WinForms.Guna2Button btnBackup;
        private Guna.UI2.WinForms.Guna2Button btnLogout;
    }
}
