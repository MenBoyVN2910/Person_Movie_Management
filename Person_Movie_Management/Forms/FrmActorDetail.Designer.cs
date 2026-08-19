namespace Person_Movie_Management.Forms
{
    partial class FrmActorDetail
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
            this.components = new System.ComponentModel.Container();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.pnlMain = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.picAvatar = new System.Windows.Forms.PictureBox();
            this.lblAvatarHint = new System.Windows.Forms.Label();
            this.txtName = new Guna.UI2.WinForms.Guna2TextBox();
            this.dtpDateOfBirth = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.cboNationality = new Guna.UI2.WinForms.Guna2ComboBox();
            this.btnManageNationality = new Guna.UI2.WinForms.Guna2Button();
            this.pnlBioContainer = new Guna.UI2.WinForms.Guna2Panel();
            this.txtBio = new System.Windows.Forms.RichTextBox();
            this.lblCharCount = new System.Windows.Forms.Label();
            this.btnSave = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnCancel = new Guna.UI2.WinForms.Guna2Button();
            this.btnDelete = new Guna.UI2.WinForms.Guna2Button();
            this.pnlGallery = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddGalleryImage = new Guna.UI2.WinForms.Guna2Button();
            this.lblGalleryTitle = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            this.pnlBioContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.TargetControl = this;
            this.guna2Elipse1.BorderRadius = 16;
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2DragControl1.TargetControl = this;
            this.guna2DragControl1.UseTransparentDrag = true;
            // 
            // pnlMain
            // 
            this.pnlMain.BorderRadius = 16;
            this.pnlMain.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.pnlMain.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.pnlMain.Controls.Add(this.btnDelete);
            this.pnlMain.Controls.Add(this.btnCancel);
            this.pnlMain.Controls.Add(this.btnSave);
            this.pnlMain.Controls.Add(this.lblCharCount);
            this.pnlMain.Controls.Add(this.pnlBioContainer);
            this.pnlMain.Controls.Add(this.btnManageNationality);
            this.pnlMain.Controls.Add(this.cboNationality);
            this.pnlMain.Controls.Add(this.dtpDateOfBirth);
            this.pnlMain.Controls.Add(this.txtName);
            this.pnlMain.Controls.Add(this.btnAddGalleryImage);
            this.pnlMain.Controls.Add(this.pnlGallery);
            this.pnlMain.Controls.Add(this.lblGalleryTitle);
            this.pnlMain.Controls.Add(this.lblAvatarHint);
            this.pnlMain.Controls.Add(this.picAvatar);
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(750, 600);
            this.pnlMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.lblTitle.Location = new System.Drawing.Point(25, 25);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(230, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "💃 Thông tin Diễn Viên";
            // 
            // picAvatar
            // 
            this.picAvatar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(38)))), ((int)(((byte)(72)))));
            this.picAvatar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picAvatar.Location = new System.Drawing.Point(25, 80);
            this.picAvatar.Name = "picAvatar";
            this.picAvatar.Size = new System.Drawing.Size(220, 310);
            this.picAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAvatar.TabIndex = 1;
            this.picAvatar.TabStop = false;
            this.picAvatar.Click += new System.EventHandler(this.picAvatar_Click);
            // 
            // lblAvatarHint
            // 
            this.lblAvatarHint.BackColor = System.Drawing.Color.Transparent;
            this.lblAvatarHint.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblAvatarHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblAvatarHint.Location = new System.Drawing.Point(25, 395);
            this.lblAvatarHint.Name = "lblAvatarHint";
            this.lblAvatarHint.Size = new System.Drawing.Size(220, 20);
            this.lblAvatarHint.TabIndex = 2;
            this.lblAvatarHint.Text = "📷 Click để chọn ảnh đại diện";
            this.lblAvatarHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAvatarHint.Click += new System.EventHandler(this.picAvatar_Click);
            // 
            // txtName
            // 
            this.txtName.BorderRadius = 10;
            this.txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtName.DefaultText = "";
            this.txtName.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.txtName.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtName.Location = new System.Drawing.Point(280, 80);
            this.txtName.Margin = new System.Windows.Forms.Padding(4);
            this.txtName.Name = "txtName";
            this.txtName.PasswordChar = '\0';
            this.txtName.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.txtName.PlaceholderText = "Tên diễn viên *";
            this.txtName.SelectedText = "";
            this.txtName.Size = new System.Drawing.Size(435, 45);
            this.txtName.TabIndex = 3;
            // 
            // dtpDateOfBirth
            // 
            this.dtpDateOfBirth.BorderRadius = 10;
            this.dtpDateOfBirth.BorderThickness = 1;
            this.dtpDateOfBirth.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(75)))), ((int)(((byte)(115)))));
            this.dtpDateOfBirth.Checked = false;
            this.dtpDateOfBirth.CustomFormat = "yyyy-MM-dd";
            this.dtpDateOfBirth.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.dtpDateOfBirth.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.dtpDateOfBirth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.dtpDateOfBirth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDateOfBirth.Location = new System.Drawing.Point(280, 140);
            this.dtpDateOfBirth.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtpDateOfBirth.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtpDateOfBirth.Name = "dtpDateOfBirth";
            this.dtpDateOfBirth.ShowCheckBox = true;
            this.dtpDateOfBirth.Size = new System.Drawing.Size(205, 40);
            this.dtpDateOfBirth.TabIndex = 4;
            this.dtpDateOfBirth.Value = new System.DateTime(1990, 1, 1, 0, 0, 0, 0);
            // 
            // cboNationality
            // 
            this.cboNationality.BackColor = System.Drawing.Color.Transparent;
            this.cboNationality.BorderRadius = 10;
            this.cboNationality.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(75)))), ((int)(((byte)(115)))));
            this.cboNationality.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboNationality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cboNationality.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.cboNationality.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.cboNationality.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.cboNationality.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cboNationality.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cboNationality.ItemHeight = 34;
            this.cboNationality.Location = new System.Drawing.Point(500, 140);
            this.cboNationality.Name = "cboNationality";
            this.cboNationality.Size = new System.Drawing.Size(165, 40);
            this.cboNationality.TabIndex = 5;
            // 
            // btnManageNationality
            // 
            this.btnManageNationality.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(75)))), ((int)(((byte)(115)))));
            this.btnManageNationality.BorderRadius = 10;
            this.btnManageNationality.BorderThickness = 1;
            this.btnManageNationality.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnManageNationality.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(42)))), ((int)(((byte)(78)))));
            this.btnManageNationality.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnManageNationality.ForeColor = System.Drawing.Color.White;
            this.btnManageNationality.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnManageNationality.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnManageNationality.Location = new System.Drawing.Point(670, 140);
            this.btnManageNationality.Name = "btnManageNationality";
            this.btnManageNationality.Size = new System.Drawing.Size(45, 40);
            this.btnManageNationality.TabIndex = 14;
            this.btnManageNationality.Text = "⚙️";
            this.btnManageNationality.Click += new System.EventHandler(this.btnManageNationality_Click);
            // 
            // pnlBioContainer
            // 
            this.pnlBioContainer.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.pnlBioContainer.BorderRadius = 10;
            this.pnlBioContainer.BorderThickness = 1;
            this.pnlBioContainer.Controls.Add(this.txtBio);
            this.pnlBioContainer.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.pnlBioContainer.Location = new System.Drawing.Point(280, 195);
            this.pnlBioContainer.Name = "pnlBioContainer";
            this.pnlBioContainer.Padding = new System.Windows.Forms.Padding(10);
            this.pnlBioContainer.Size = new System.Drawing.Size(435, 115);
            this.pnlBioContainer.TabIndex = 6;
            // 
            // txtBio
            // 
            this.txtBio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.txtBio.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBio.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtBio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtBio.Location = new System.Drawing.Point(10, 10);
            this.txtBio.MaxLength = 500;
            this.txtBio.Name = "txtBio";
            this.txtBio.Size = new System.Drawing.Size(415, 95);
            this.txtBio.TabIndex = 0;
            this.txtBio.Text = "";
            // 
            // lblCharCount
            // 
            this.lblCharCount.BackColor = System.Drawing.Color.Transparent;
            this.lblCharCount.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCharCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblCharCount.Location = new System.Drawing.Point(565, 313);
            this.lblCharCount.Name = "lblCharCount";
            this.lblCharCount.Size = new System.Drawing.Size(150, 18);
            this.lblCharCount.TabIndex = 13;
            this.lblCharCount.Text = "0 / 500 ký tự";
            this.lblCharCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblGalleryTitle
            // 
            this.lblGalleryTitle.AutoSize = true;
            this.lblGalleryTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblGalleryTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblGalleryTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.lblGalleryTitle.Location = new System.Drawing.Point(280, 335);
            this.lblGalleryTitle.Name = "lblGalleryTitle";
            this.lblGalleryTitle.Size = new System.Drawing.Size(73, 21);
            this.lblGalleryTitle.TabIndex = 7;
            this.lblGalleryTitle.Text = "Ảnh phụ";
            // 
            // pnlGallery
            // 
            this.pnlGallery.AutoScroll = true;
            this.pnlGallery.BackColor = System.Drawing.Color.Transparent;
            this.pnlGallery.Location = new System.Drawing.Point(280, 370);
            this.pnlGallery.Name = "pnlGallery";
            this.pnlGallery.Size = new System.Drawing.Size(435, 130);
            this.pnlGallery.TabIndex = 8;
            // 
            // btnAddGalleryImage
            // 
            this.btnAddGalleryImage.BorderRadius = 8;
            this.btnAddGalleryImage.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(38)))), ((int)(((byte)(72)))));
            this.btnAddGalleryImage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnAddGalleryImage.ForeColor = System.Drawing.Color.White;
            this.btnAddGalleryImage.Location = new System.Drawing.Point(370, 332);
            this.btnAddGalleryImage.Name = "btnAddGalleryImage";
            this.btnAddGalleryImage.Size = new System.Drawing.Size(120, 30);
            this.btnAddGalleryImage.TabIndex = 9;
            this.btnAddGalleryImage.Text = "+ Thêm ảnh";
            this.btnAddGalleryImage.Click += new System.EventHandler(this.btnAddGalleryImage_Click);
            // 
            // btnSave
            // 
            this.btnSave.BorderRadius = 12;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnSave.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(575, 530);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(140, 45);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Lưu Thay Đổi";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BorderRadius = 12;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(440, 530);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 45);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BorderRadius = 12;
            this.btnDelete.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(25, 530);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 45);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.Text = "Xóa";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnDelete.Visible = false;
            // 
            // FrmActorDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 600);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmActorDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chi Tiết Diễn Viên";
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.pnlBioContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2GradientPanel pnlMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox picAvatar;
        private System.Windows.Forms.Label lblAvatarHint;
        private Guna.UI2.WinForms.Guna2TextBox txtName;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpDateOfBirth;
        private Guna.UI2.WinForms.Guna2ComboBox cboNationality;
        private Guna.UI2.WinForms.Guna2Button btnManageNationality;
        private Guna.UI2.WinForms.Guna2Panel pnlBioContainer;
        private System.Windows.Forms.RichTextBox txtBio;
        private System.Windows.Forms.Label lblCharCount;
        private Guna.UI2.WinForms.Guna2GradientButton btnSave;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
        private Guna.UI2.WinForms.Guna2Button btnDelete;
        private System.Windows.Forms.FlowLayoutPanel pnlGallery;
        private System.Windows.Forms.Label lblGalleryTitle;
        private Guna.UI2.WinForms.Guna2Button btnAddGalleryImage;
    }
}
