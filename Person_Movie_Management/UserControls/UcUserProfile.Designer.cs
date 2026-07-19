namespace Person_Movie_Management.UserControls
{
    partial class UcUserProfile
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
            this.pnlInfo = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lblTitleInfo = new System.Windows.Forms.Label();
            this.picAvatar = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.btnChooseAvatar = new Guna.UI2.WinForms.Guna2Button();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtDisplayName = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtEmail = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblCreatedAt = new System.Windows.Forms.Label();
            this.btnSaveInfo = new Guna.UI2.WinForms.Guna2GradientButton();
            
            this.pnlSeparator = new System.Windows.Forms.Panel();
            
            this.lblTitlePassword = new System.Windows.Forms.Label();
            this.txtOldPassword = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtNewPassword = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtConfirmPassword = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnSavePassword = new Guna.UI2.WinForms.Guna2GradientButton();
            
            // New DropWidget Toggle
            this.lblToggleWidget = new System.Windows.Forms.Label();
            this.tglWidget = new System.Windows.Forms.CheckBox();
            
            this.pnlInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlInfo
            // 
            this.pnlInfo.BorderRadius = 16;
            this.pnlInfo.Controls.Add(this.btnSavePassword);
            this.pnlInfo.Controls.Add(this.txtConfirmPassword);
            this.pnlInfo.Controls.Add(this.txtNewPassword);
            this.pnlInfo.Controls.Add(this.txtOldPassword);
            this.pnlInfo.Controls.Add(this.lblTitlePassword);
            
            this.pnlInfo.Controls.Add(this.pnlSeparator);
            
            this.pnlInfo.Controls.Add(this.btnSaveInfo);
            this.pnlInfo.Controls.Add(this.lblCreatedAt);
            this.pnlInfo.Controls.Add(this.txtEmail);
            this.pnlInfo.Controls.Add(this.txtDisplayName);
            this.pnlInfo.Controls.Add(this.lblUsername);
            this.pnlInfo.Controls.Add(this.btnChooseAvatar);
            this.pnlInfo.Controls.Add(this.picAvatar);
            this.pnlInfo.Controls.Add(this.lblTitleInfo);
            
            // Add Toggle controls
            this.pnlInfo.Controls.Add(this.lblToggleWidget);
            this.pnlInfo.Controls.Add(this.tglWidget);
            
            this.pnlInfo.Location = new System.Drawing.Point(150, 20);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(650, 630);
            this.pnlInfo.TabIndex = 0;
            // 
            // lblTitleInfo
            // 
            this.lblTitleInfo.AutoSize = true;
            this.lblTitleInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitleInfo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitleInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.lblTitleInfo.Location = new System.Drawing.Point(30, 20);
            this.lblTitleInfo.Name = "lblTitleInfo";
            this.lblTitleInfo.Size = new System.Drawing.Size(220, 32);
            this.lblTitleInfo.TabIndex = 0;
            this.lblTitleInfo.Text = "👤  Thông tin cá nhân";
            // 
            // picAvatar
            // 
            this.picAvatar.BackColor = System.Drawing.Color.Transparent;
            this.picAvatar.FillColor = System.Drawing.Color.White;
            this.picAvatar.ImageRotate = 0F;
            this.picAvatar.Location = new System.Drawing.Point(30, 70);
            this.picAvatar.Name = "picAvatar";
            this.picAvatar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.picAvatar.Size = new System.Drawing.Size(100, 100);
            this.picAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAvatar.TabIndex = 11;
            this.picAvatar.TabStop = false;
            // 
            // btnChooseAvatar
            // 
            this.btnChooseAvatar.BorderRadius = 8;
            this.btnChooseAvatar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnChooseAvatar.ForeColor = System.Drawing.Color.White;
            this.btnChooseAvatar.Location = new System.Drawing.Point(145, 100);
            this.btnChooseAvatar.Name = "btnChooseAvatar";
            this.btnChooseAvatar.Size = new System.Drawing.Size(100, 35);
            this.btnChooseAvatar.TabIndex = 12;
            this.btnChooseAvatar.Text = "Đổi Avatar";
            this.btnChooseAvatar.Click += new System.EventHandler(this.btnChooseAvatar_Click);
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.BackColor = System.Drawing.Color.Transparent;
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblUsername.Location = new System.Drawing.Point(265, 75);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(115, 20);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Tên đăng nhập:";
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.BorderRadius = 10;
            this.txtDisplayName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtDisplayName.DefaultText = "";
            this.txtDisplayName.Location = new System.Drawing.Point(265, 110);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.PasswordChar = '\0';
            this.txtDisplayName.PlaceholderText = "Tên hiển thị";
            this.txtDisplayName.SelectedText = "";
            this.txtDisplayName.Size = new System.Drawing.Size(350, 42);
            this.txtDisplayName.TabIndex = 2;
            // 
            // txtEmail
            // 
            this.txtEmail.BorderRadius = 10;
            this.txtEmail.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEmail.DefaultText = "";
            this.txtEmail.Location = new System.Drawing.Point(265, 160);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.PasswordChar = '\0';
            this.txtEmail.PlaceholderText = "Email";
            this.txtEmail.SelectedText = "";
            this.txtEmail.Size = new System.Drawing.Size(350, 42);
            this.txtEmail.TabIndex = 3;
            // 
            // lblCreatedAt
            // 
            this.lblCreatedAt.AutoSize = true;
            this.lblCreatedAt.BackColor = System.Drawing.Color.Transparent;
            this.lblCreatedAt.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCreatedAt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblCreatedAt.Location = new System.Drawing.Point(30, 215);
            this.lblCreatedAt.Name = "lblCreatedAt";
            this.lblCreatedAt.Size = new System.Drawing.Size(117, 20);
            this.lblCreatedAt.TabIndex = 4;
            this.lblCreatedAt.Text = "Ngày tham gia:";
            // 
            // btnSaveInfo
            // 
            this.btnSaveInfo.BorderRadius = 12;
            this.btnSaveInfo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSaveInfo.ForeColor = System.Drawing.Color.White;
            this.btnSaveInfo.Location = new System.Drawing.Point(30, 255);
            this.btnSaveInfo.Name = "btnSaveInfo";
            this.btnSaveInfo.Size = new System.Drawing.Size(585, 45);
            this.btnSaveInfo.TabIndex = 13;
            this.btnSaveInfo.Text = "CẬP NHẬT THÔNG TIN";
            this.btnSaveInfo.Click += new System.EventHandler(this.btnSaveInfo_Click);
            // 
            // pnlSeparator
            // 
            this.pnlSeparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            this.pnlSeparator.Location = new System.Drawing.Point(30, 330);
            this.pnlSeparator.Name = "pnlSeparator";
            this.pnlSeparator.Size = new System.Drawing.Size(585, 1);
            this.pnlSeparator.TabIndex = 10;
            // 
            // lblTitlePassword
            // 
            this.lblTitlePassword.AutoSize = true;
            this.lblTitlePassword.BackColor = System.Drawing.Color.Transparent;
            this.lblTitlePassword.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitlePassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.lblTitlePassword.Location = new System.Drawing.Point(30, 360);
            this.lblTitlePassword.Name = "lblTitlePassword";
            this.lblTitlePassword.Size = new System.Drawing.Size(180, 30);
            this.lblTitlePassword.TabIndex = 5;
            this.lblTitlePassword.Text = "🔐  Đổi mật khẩu";
            // 
            // txtOldPassword
            // 
            this.txtOldPassword.BorderRadius = 10;
            this.txtOldPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtOldPassword.DefaultText = "";
            this.txtOldPassword.Location = new System.Drawing.Point(30, 410);
            this.txtOldPassword.Name = "txtOldPassword";
            this.txtOldPassword.PasswordChar = '●';
            this.txtOldPassword.PlaceholderText = "🔒  Mật khẩu cũ";
            this.txtOldPassword.SelectedText = "";
            this.txtOldPassword.Size = new System.Drawing.Size(585, 42);
            this.txtOldPassword.TabIndex = 6;
            this.txtOldPassword.UseSystemPasswordChar = true;
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.BorderRadius = 10;
            this.txtNewPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNewPassword.DefaultText = "";
            this.txtNewPassword.Location = new System.Drawing.Point(30, 465);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '●';
            this.txtNewPassword.PlaceholderText = "🔑  Mật khẩu mới";
            this.txtNewPassword.SelectedText = "";
            this.txtNewPassword.Size = new System.Drawing.Size(585, 42);
            this.txtNewPassword.TabIndex = 7;
            this.txtNewPassword.UseSystemPasswordChar = true;
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.BorderRadius = 10;
            this.txtConfirmPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtConfirmPassword.DefaultText = "";
            this.txtConfirmPassword.Location = new System.Drawing.Point(30, 520);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '●';
            this.txtConfirmPassword.PlaceholderText = "🔑  Xác nhận mật khẩu mới";
            this.txtConfirmPassword.SelectedText = "";
            this.txtConfirmPassword.Size = new System.Drawing.Size(585, 42);
            this.txtConfirmPassword.TabIndex = 8;
            this.txtConfirmPassword.UseSystemPasswordChar = true;
            // 
            // btnSavePassword
            // 
            this.btnSavePassword.BorderRadius = 12;
            this.btnSavePassword.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSavePassword.ForeColor = System.Drawing.Color.White;
            this.btnSavePassword.Location = new System.Drawing.Point(30, 575);
            this.btnSavePassword.Name = "btnSavePassword";
            this.btnSavePassword.Size = new System.Drawing.Size(585, 45);
            this.btnSavePassword.TabIndex = 9;
            this.btnSavePassword.Text = "CẬP NHẬT MẬT KHẨU";
            this.btnSavePassword.Click += new System.EventHandler(this.btnSavePassword_Click);
            // 
            // lblToggleWidget
            // 
            this.lblToggleWidget.AutoSize = true;
            this.lblToggleWidget.BackColor = System.Drawing.Color.Transparent;
            this.lblToggleWidget.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblToggleWidget.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.lblToggleWidget.Location = new System.Drawing.Point(350, 28);
            this.lblToggleWidget.Name = "lblToggleWidget";
            this.lblToggleWidget.Size = new System.Drawing.Size(200, 21);
            this.lblToggleWidget.TabIndex = 10;
            this.lblToggleWidget.Text = "Bong bóng thả phim";
            // 
            // tglWidget
            // 
            this.tglWidget.AutoSize = true;
            this.tglWidget.Location = new System.Drawing.Point(520, 31);
            this.tglWidget.Name = "tglWidget";
            this.tglWidget.Size = new System.Drawing.Size(15, 14);
            this.tglWidget.TabIndex = 11;
            this.tglWidget.UseVisualStyleBackColor = true;
            this.tglWidget.CheckedChanged += new System.EventHandler(this.tglWidget_CheckedChanged);
            // 
            // UcUserProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(29)))));
            this.Controls.Add(this.pnlInfo);
            this.Name = "UcUserProfile";
            this.Size = new System.Drawing.Size(950, 710);
            this.pnlInfo.ResumeLayout(false);
            this.pnlInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.ResumeLayout(false);

        }

        private Guna.UI2.WinForms.Guna2GradientPanel pnlInfo;
        private System.Windows.Forms.Label lblTitleInfo;
        private Guna.UI2.WinForms.Guna2CirclePictureBox picAvatar;
        private Guna.UI2.WinForms.Guna2Button btnChooseAvatar;
        private System.Windows.Forms.Label lblUsername;
        private Guna.UI2.WinForms.Guna2TextBox txtDisplayName;
        private Guna.UI2.WinForms.Guna2TextBox txtEmail;
        private System.Windows.Forms.Label lblCreatedAt;
        private Guna.UI2.WinForms.Guna2GradientButton btnSaveInfo;
        
        private System.Windows.Forms.Panel pnlSeparator;
        
        private System.Windows.Forms.Label lblTitlePassword;
        private Guna.UI2.WinForms.Guna2TextBox txtOldPassword;
        private Guna.UI2.WinForms.Guna2TextBox txtNewPassword;
        private Guna.UI2.WinForms.Guna2TextBox txtConfirmPassword;
        private Guna.UI2.WinForms.Guna2GradientButton btnSavePassword;
        
        private System.Windows.Forms.Label lblToggleWidget;
        private System.Windows.Forms.CheckBox tglWidget;
    }
}
