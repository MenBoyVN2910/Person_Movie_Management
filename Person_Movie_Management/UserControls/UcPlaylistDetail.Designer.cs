namespace Person_Movie_Management.UserControls
{
    partial class UcPlaylistDetail
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnBack = new Guna.UI2.WinForms.Guna2Button();
            this.picCover = new Guna.UI2.WinForms.Guna2PictureBox();
            this.lblPlaylistName = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.pnlBadges = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPrivacyBadge = new Guna.UI2.WinForms.Guna2Button();
            this.btnMovieBadge = new Guna.UI2.WinForms.Guna2Button();
            this.btnAudioBadge = new Guna.UI2.WinForms.Guna2Button();
            this.lblDate = new System.Windows.Forms.Label();
            this.btnEdit = new Guna.UI2.WinForms.Guna2Button();
            this.pnlSeparator = new System.Windows.Forms.Panel();
            this.flpItems = new System.Windows.Forms.FlowLayoutPanel();
            this.lblEmpty = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCover)).BeginInit();
            this.pnlBadges.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.pnlHeader.Controls.Add(this.btnEdit);
            this.pnlHeader.Controls.Add(this.pnlBadges);
            this.pnlHeader.Controls.Add(this.lblDescription);
            this.pnlHeader.Controls.Add(this.lblPlaylistName);
            this.pnlHeader.Controls.Add(this.picCover);
            this.pnlHeader.Controls.Add(this.btnBack);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1000, 140);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnBack
            // 
            this.btnBack.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(53)))), ((int)(((byte)(90)))));
            this.btnBack.BorderRadius = 10;
            this.btnBack.BorderThickness = 1;
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(26)))), ((int)(((byte)(48)))));
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(231)))), ((int)(((byte)(255)))));
            this.btnBack.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(102)))), ((int)(((byte)(241)))));
            this.btnBack.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(22, 20);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(110, 38);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "← Quay lại";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // picCover
            // 
            this.picCover.BorderRadius = 14;
            this.picCover.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picCover.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(68)))));
            this.picCover.ImageRotate = 0F;
            this.picCover.Location = new System.Drawing.Point(146, 16);
            this.picCover.Name = "picCover";
            this.picCover.Size = new System.Drawing.Size(108, 108);
            this.picCover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picCover.TabIndex = 1;
            this.picCover.TabStop = false;
            // 
            // lblPlaylistName
            // 
            this.lblPlaylistName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPlaylistName.AutoEllipsis = true;
            this.lblPlaylistName.Font = new System.Drawing.Font("Segoe UI", 17F, System.Drawing.FontStyle.Bold);
            this.lblPlaylistName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.lblPlaylistName.Location = new System.Drawing.Point(268, 16);
            this.lblPlaylistName.Name = "lblPlaylistName";
            this.lblPlaylistName.Size = new System.Drawing.Size(580, 32);
            this.lblPlaylistName.TabIndex = 2;
            this.lblPlaylistName.Text = "Playlist Name";
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescription.AutoEllipsis = true;
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblDescription.Location = new System.Drawing.Point(270, 50);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(580, 22);
            this.lblDescription.TabIndex = 3;
            this.lblDescription.Text = "Mô tả playlist...";
            // 
            // pnlBadges
            // 
            this.pnlBadges.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBadges.BackColor = System.Drawing.Color.Transparent;
            this.pnlBadges.Controls.Add(this.btnPrivacyBadge);
            this.pnlBadges.Controls.Add(this.btnMovieBadge);
            this.pnlBadges.Controls.Add(this.btnAudioBadge);
            this.pnlBadges.Controls.Add(this.lblDate);
            this.pnlBadges.Location = new System.Drawing.Point(268, 80);
            this.pnlBadges.Name = "pnlBadges";
            this.pnlBadges.Size = new System.Drawing.Size(580, 36);
            this.pnlBadges.TabIndex = 4;
            // 
            // btnPrivacyBadge
            // 
            this.btnPrivacyBadge.BorderRadius = 6;
            this.btnPrivacyBadge.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(60)))));
            this.btnPrivacyBadge.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnPrivacyBadge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(210)))), ((int)(((byte)(254)))));
            this.btnPrivacyBadge.Location = new System.Drawing.Point(0, 0);
            this.btnPrivacyBadge.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnPrivacyBadge.Name = "btnPrivacyBadge";
            this.btnPrivacyBadge.Size = new System.Drawing.Size(90, 28);
            this.btnPrivacyBadge.TabIndex = 0;
            this.btnPrivacyBadge.Text = "🔒 Riêng tư";
            // 
            // btnMovieBadge
            // 
            this.btnMovieBadge.BorderRadius = 6;
            this.btnMovieBadge.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(99)))), ((int)(((byte)(102)))), ((int)(((byte)(241)))));
            this.btnMovieBadge.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnMovieBadge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(210)))), ((int)(((byte)(254)))));
            this.btnMovieBadge.Location = new System.Drawing.Point(98, 0);
            this.btnMovieBadge.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnMovieBadge.Name = "btnMovieBadge";
            this.btnMovieBadge.Size = new System.Drawing.Size(80, 28);
            this.btnMovieBadge.TabIndex = 1;
            this.btnMovieBadge.Text = "🎬 0 phim";
            // 
            // btnAudioBadge
            // 
            this.btnAudioBadge.BorderRadius = 6;
            this.btnAudioBadge.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.btnAudioBadge.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnAudioBadge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(207)))), ((int)(((byte)(232)))));
            this.btnAudioBadge.Location = new System.Drawing.Point(186, 0);
            this.btnAudioBadge.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.btnAudioBadge.Name = "btnAudioBadge";
            this.btnAudioBadge.Size = new System.Drawing.Size(80, 28);
            this.btnAudioBadge.TabIndex = 2;
            this.btnAudioBadge.Text = "🎵 0 audio";
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblDate.Location = new System.Drawing.Point(278, 6);
            this.lblDate.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(95, 15);
            this.lblDate.TabIndex = 3;
            this.lblDate.Text = "📅 Ngày tạo: ...";
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(53)))), ((int)(((byte)(90)))));
            this.btnEdit.BorderRadius = 10;
            this.btnEdit.BorderThickness = 1;
            this.btnEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEdit.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(26)))), ((int)(((byte)(48)))));
            this.btnEdit.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnEdit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(231)))), ((int)(((byte)(255)))));
            this.btnEdit.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(102)))), ((int)(((byte)(241)))));
            this.btnEdit.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnEdit.Location = new System.Drawing.Point(868, 20);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(108, 38);
            this.btnEdit.TabIndex = 5;
            this.btnEdit.Text = "✏️ Chỉnh sửa";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // pnlSeparator
            // 
            this.pnlSeparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(65)))));
            this.pnlSeparator.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSeparator.Location = new System.Drawing.Point(0, 140);
            this.pnlSeparator.Name = "pnlSeparator";
            this.pnlSeparator.Size = new System.Drawing.Size(1000, 1);
            this.pnlSeparator.TabIndex = 1;
            // 
            // flpItems
            // 
            this.flpItems.AutoScroll = true;
            this.flpItems.BackColor = System.Drawing.Color.Transparent;
            this.flpItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpItems.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpItems.Location = new System.Drawing.Point(0, 141);
            this.flpItems.Name = "flpItems";
            this.flpItems.Padding = new System.Windows.Forms.Padding(24, 14, 24, 20);
            this.flpItems.Size = new System.Drawing.Size(1000, 559);
            this.flpItems.TabIndex = 2;
            this.flpItems.WrapContents = false;
            // 
            // lblEmpty
            // 
            this.lblEmpty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEmpty.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.lblEmpty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblEmpty.Location = new System.Drawing.Point(0, 141);
            this.lblEmpty.Name = "lblEmpty";
            this.lblEmpty.Size = new System.Drawing.Size(1000, 559);
            this.lblEmpty.TabIndex = 3;
            this.lblEmpty.Text = "Playlist này chưa có mục nào.\r\nHãy mở danh sách Phim / Audio, chuột phải vào mục bất kỳ và chọn \"Thêm vào Playlist\"! ✨";
            this.lblEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEmpty.Visible = false;
            // 
            // UcPlaylistDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.Controls.Add(this.lblEmpty);
            this.Controls.Add(this.flpItems);
            this.Controls.Add(this.pnlSeparator);
            this.Controls.Add(this.pnlHeader);
            this.Name = "UcPlaylistDetail";
            this.Size = new System.Drawing.Size(1000, 700);
            this.Resize += new System.EventHandler(this.UcPlaylistDetail_Resize);
            this.pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picCover)).EndInit();
            this.pnlBadges.ResumeLayout(false);
            this.pnlBadges.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlHeader;
        private Guna.UI2.WinForms.Guna2Button btnBack;
        private Guna.UI2.WinForms.Guna2PictureBox picCover;
        private System.Windows.Forms.Label lblPlaylistName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.FlowLayoutPanel pnlBadges;
        private Guna.UI2.WinForms.Guna2Button btnPrivacyBadge;
        private Guna.UI2.WinForms.Guna2Button btnMovieBadge;
        private Guna.UI2.WinForms.Guna2Button btnAudioBadge;
        private System.Windows.Forms.Label lblDate;
        private Guna.UI2.WinForms.Guna2Button btnEdit;
        private System.Windows.Forms.Panel pnlSeparator;
        private System.Windows.Forms.FlowLayoutPanel flpItems;
        private System.Windows.Forms.Label lblEmpty;
    }
}
