namespace Person_Movie_Management.Forms
{
    partial class FrmMovieDetail
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
            this.picCover = new System.Windows.Forms.PictureBox();
            this.lblCoverHint = new System.Windows.Forms.Label();
            this.txtMovieCode = new Guna.UI2.WinForms.Guna2TextBox();
            this.cboSourceType = new Guna.UI2.WinForms.Guna2ComboBox();
            this.txtMediaUrl = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnFetchUrl = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnFetchTMDB = new Guna.UI2.WinForms.Guna2GradientButton();
            this.pnlNoteContainer = new Guna.UI2.WinForms.Guna2Panel();
            this.txtNote = new System.Windows.Forms.RichTextBox();
            this.btnSave = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnCancel = new Guna.UI2.WinForms.Guna2Button();
            this.btnManageTags = new Guna.UI2.WinForms.Guna2Button();
            this.flpTags = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlGallery = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddGalleryImage = new Guna.UI2.WinForms.Guna2Button();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCover)).BeginInit();
            this.pnlNoteContainer.SuspendLayout();
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
            this.pnlMain.Controls.Add(this.btnCancel);
            this.pnlMain.Controls.Add(this.btnSave);
            this.pnlMain.Controls.Add(this.btnCancel);
            this.pnlMain.Controls.Add(this.btnManageTags);
            this.pnlMain.Controls.Add(this.flpTags);
            this.pnlMain.Controls.Add(this.pnlNoteContainer);
            this.pnlMain.Controls.Add(this.btnFetchTMDB);
            this.pnlMain.Controls.Add(this.btnFetchUrl);
            this.pnlMain.Controls.Add(this.txtMediaUrl);
            this.pnlMain.Controls.Add(this.cboSourceType);
            this.pnlMain.Controls.Add(this.txtMovieCode);
            this.pnlMain.Controls.Add(this.btnAddGalleryImage);
            this.pnlMain.Controls.Add(this.pnlGallery);
            this.pnlMain.Controls.Add(this.lblCoverHint);
            this.pnlMain.Controls.Add(this.picCover);
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
            this.lblTitle.Text = "🎬  Thêm Phim Mới";
            // 
            // picCover
            // 
            this.picCover.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(38)))), ((int)(((byte)(72)))));
            this.picCover.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picCover.Location = new System.Drawing.Point(25, 80);
            this.picCover.Name = "picCover";
            this.picCover.Size = new System.Drawing.Size(220, 310);
            this.picCover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCover.TabIndex = 1;
            this.picCover.TabStop = false;
            this.picCover.Click += new System.EventHandler(this.picCover_Click);
            // 
            // lblCoverHint
            // 
            this.lblCoverHint.BackColor = System.Drawing.Color.Transparent;
            this.lblCoverHint.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblCoverHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblCoverHint.Location = new System.Drawing.Point(25, 395);
            this.lblCoverHint.Name = "lblCoverHint";
            this.lblCoverHint.Size = new System.Drawing.Size(220, 20);
            this.lblCoverHint.TabIndex = 2;
            this.lblCoverHint.Text = "📷 Click để chọn ảnh bìa";
            this.lblCoverHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCoverHint.Click += new System.EventHandler(this.picCover_Click);
            // 
            // pnlGallery
            // 
            this.pnlGallery.AutoScroll = true;
            this.pnlGallery.BackColor = System.Drawing.Color.Transparent;
            this.pnlGallery.Location = new System.Drawing.Point(25, 420);
            this.pnlGallery.Name = "pnlGallery";
            this.pnlGallery.Size = new System.Drawing.Size(220, 100);
            this.pnlGallery.TabIndex = 10;
            // 
            // btnAddGalleryImage
            // 
            this.btnAddGalleryImage.BorderRadius = 8;
            this.btnAddGalleryImage.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(38)))), ((int)(((byte)(72)))));
            this.btnAddGalleryImage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnAddGalleryImage.ForeColor = System.Drawing.Color.White;
            this.btnAddGalleryImage.Location = new System.Drawing.Point(25, 530);
            this.btnAddGalleryImage.Name = "btnAddGalleryImage";
            this.btnAddGalleryImage.Size = new System.Drawing.Size(220, 30);
            this.btnAddGalleryImage.TabIndex = 11;
            this.btnAddGalleryImage.Text = "+ Thêm ảnh phụ";
            this.btnAddGalleryImage.Click += new System.EventHandler(this.btnAddGalleryImage_Click);
            // 
            // txtMovieCode
            // 
            this.txtMovieCode.BorderRadius = 10;
            this.txtMovieCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMovieCode.DefaultText = "";
            this.txtMovieCode.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.txtMovieCode.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.txtMovieCode.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtMovieCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtMovieCode.Location = new System.Drawing.Point(280, 80);
            this.txtMovieCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtMovieCode.Name = "txtMovieCode";
            this.txtMovieCode.PasswordChar = '\0';
            this.txtMovieCode.PlaceholderText = "🎞️  Mã/Tên phim (VD: Avengers)";
            this.txtMovieCode.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.txtMovieCode.SelectedText = "";
            this.txtMovieCode.Size = new System.Drawing.Size(290, 42);
            this.txtMovieCode.TabIndex = 3;
            // 
            // btnFetchTMDB
            // 
            this.btnFetchTMDB.BorderRadius = 10;
            this.btnFetchTMDB.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.btnFetchTMDB.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(39)))), ((int)(((byte)(119)))));
            this.btnFetchTMDB.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnFetchTMDB.ForeColor = System.Drawing.Color.White;
            this.btnFetchTMDB.Location = new System.Drawing.Point(580, 80);
            this.btnFetchTMDB.Name = "btnFetchTMDB";
            this.btnFetchTMDB.Size = new System.Drawing.Size(130, 42);
            this.btnFetchTMDB.TabIndex = 12;
            this.btnFetchTMDB.Text = "🎬 TMDB API";
            this.btnFetchTMDB.Click += new System.EventHandler(this.btnFetchTMDB_Click);
            // 
            // cboSourceType
            // 
            this.cboSourceType.BackColor = System.Drawing.Color.Transparent;
            this.cboSourceType.BorderRadius = 10;
            this.cboSourceType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSourceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSourceType.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.cboSourceType.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.cboSourceType.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cboSourceType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.cboSourceType.ItemHeight = 30;
            this.cboSourceType.Items.AddRange(new object[] {
            "🌐  Phim Online",
            "📁  Phim Trên Máy (Local)"});
            this.cboSourceType.Location = new System.Drawing.Point(280, 140);
            this.cboSourceType.Name = "cboSourceType";
            this.cboSourceType.Size = new System.Drawing.Size(430, 36);
            this.cboSourceType.StartIndex = 0;
            this.cboSourceType.TabIndex = 4;
            // 
            // txtMediaUrl
            // 
            this.txtMediaUrl.BorderRadius = 10;
            this.txtMediaUrl.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMediaUrl.DefaultText = "";
            this.txtMediaUrl.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.txtMediaUrl.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.txtMediaUrl.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtMediaUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtMediaUrl.Location = new System.Drawing.Point(280, 200);
            this.txtMediaUrl.Margin = new System.Windows.Forms.Padding(4);
            this.txtMediaUrl.Name = "txtMediaUrl";
            this.txtMediaUrl.PasswordChar = '\0';
            this.txtMediaUrl.PlaceholderText = "🔗  Đường dẫn File hoặc Link Web";
            this.txtMediaUrl.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.txtMediaUrl.SelectedText = "";
            this.txtMediaUrl.Size = new System.Drawing.Size(290, 42);
            this.txtMediaUrl.TabIndex = 5;
            // 
            // btnFetchUrl
            // 
            this.btnFetchUrl.BorderRadius = 10;
            this.btnFetchUrl.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnFetchUrl.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(165)))), ((int)(((byte)(250)))));
            this.btnFetchUrl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnFetchUrl.ForeColor = System.Drawing.Color.White;
            this.btnFetchUrl.Location = new System.Drawing.Point(580, 200);
            this.btnFetchUrl.Name = "btnFetchUrl";
            this.btnFetchUrl.Size = new System.Drawing.Size(130, 42);
            this.btnFetchUrl.TabIndex = 9;
            this.btnFetchUrl.Text = "🔍 Lấy thông tin";
            this.btnFetchUrl.Click += new System.EventHandler(this.btnFetchUrl_Click);
            // 
            // pnlNoteContainer
            // 
            this.pnlNoteContainer.BorderRadius = 10;
            this.pnlNoteContainer.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.pnlNoteContainer.Location = new System.Drawing.Point(280, 260);
            this.pnlNoteContainer.Name = "pnlNoteContainer";
            this.pnlNoteContainer.Size = new System.Drawing.Size(430, 190);
            this.pnlNoteContainer.TabIndex = 6;
            this.pnlNoteContainer.Padding = new System.Windows.Forms.Padding(10);
            this.pnlNoteContainer.Controls.Add(this.txtNote);
            // 
            // txtNote
            // 
            this.txtNote.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(62)))));
            this.txtNote.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNote.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.txtNote.Location = new System.Drawing.Point(10, 10);
            this.txtNote.Name = "txtNote";
            this.txtNote.Size = new System.Drawing.Size(410, 170);
            this.txtNote.TabIndex = 0;
            this.txtNote.Text = "";
            // 
            // 
            // btnManageTags
            // 
            this.btnManageTags.BorderRadius = 8;
            this.btnManageTags.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnManageTags.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnManageTags.ForeColor = System.Drawing.Color.White;
            this.btnManageTags.Location = new System.Drawing.Point(280, 460);
            this.btnManageTags.Name = "btnManageTags";
            this.btnManageTags.Size = new System.Drawing.Size(120, 35);
            this.btnManageTags.TabIndex = 10;
            this.btnManageTags.Text = "+ Gắn Tag";
            this.btnManageTags.Click += new System.EventHandler(this.btnManageTags_Click);
            // 
            // flpTags
            // 
            this.flpTags.AutoScroll = true;
            this.flpTags.Location = new System.Drawing.Point(410, 460);
            this.flpTags.Name = "flpTags";
            this.flpTags.Size = new System.Drawing.Size(300, 35);
            this.flpTags.TabIndex = 11;
            // 
            // btnSave
            // 
            this.btnSave.BorderRadius = 12;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnSave.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(530, 510);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(180, 45);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "✓  LƯU LẠI";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BorderRadius = 12;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btnCancel.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(113)))), ((int)(((byte)(113)))));
            this.btnCancel.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(280, 510);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(180, 45);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "✕  HỦY";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FrmMovieDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(29)))));
            this.ClientSize = new System.Drawing.Size(750, 600);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmMovieDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Movie Detail";
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCover)).EndInit();
            this.ResumeLayout(false);

        }

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2Button btnManageTags;
        private System.Windows.Forms.FlowLayoutPanel flpTags;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private Guna.UI2.WinForms.Guna2GradientPanel pnlMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox picCover;
        private System.Windows.Forms.Label lblCoverHint;
        private System.Windows.Forms.FlowLayoutPanel pnlGallery;
        private Guna.UI2.WinForms.Guna2Button btnAddGalleryImage;
        private Guna.UI2.WinForms.Guna2TextBox txtMovieCode;
        private Guna.UI2.WinForms.Guna2ComboBox cboSourceType;
        private Guna.UI2.WinForms.Guna2TextBox txtMediaUrl;
        private Guna.UI2.WinForms.Guna2GradientButton btnFetchTMDB;
        private Guna.UI2.WinForms.Guna2GradientButton btnFetchUrl;
        private Guna.UI2.WinForms.Guna2Panel pnlNoteContainer;
        private System.Windows.Forms.RichTextBox txtNote;
        private Guna.UI2.WinForms.Guna2GradientButton btnSave;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
    }
}
