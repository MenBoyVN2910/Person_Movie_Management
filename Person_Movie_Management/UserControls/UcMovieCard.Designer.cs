namespace Person_Movie_Management.UserControls
{
    partial class UcMovieCard
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new Guna.UI2.WinForms.Guna2ContextMenuStrip();
            this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddToPlaylist = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUpdateProgress = new System.Windows.Forms.ToolStripMenuItem();
            
            this.pnlBase = new Guna.UI2.WinForms.Guna2Panel();
            this.picCover = new Guna.UI2.WinForms.Guna2PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.flpTags = new System.Windows.Forms.FlowLayoutPanel();
            this.lblRating = new System.Windows.Forms.Label();
            this.lblFavorite = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.pgbWatchProgress = new Guna.UI2.WinForms.Guna2ProgressBar();
            
            this.contextMenu.SuspendLayout();
            this.pnlBase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCover)).BeginInit();
            this.picCover.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.RenderStyle.ArrowColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.contextMenu.RenderStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            this.contextMenu.RenderStyle.ColorTable = null;
            this.contextMenu.RenderStyle.RoundedEdges = true;
            this.contextMenu.RenderStyle.SelectionArrowColor = System.Drawing.Color.White;
            this.contextMenu.RenderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.contextMenu.RenderStyle.SelectionForeColor = System.Drawing.Color.White;
            this.contextMenu.RenderStyle.SeparatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(48)))), ((int)(((byte)(82)))));
            this.contextMenu.RenderStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.contextMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.contextMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.contextMenu.Size = new System.Drawing.Size(181, 76);
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEdit,
            this.menuUpdateProgress,
            this.menuAddToPlaylist,
            this.menuDelete});
            // 
            // menuEdit
            // 
            this.menuEdit.Name = "menuEdit";
            this.menuEdit.Size = new System.Drawing.Size(180, 24);
            this.menuEdit.Text = "Sửa phim";
            this.menuEdit.Click += new System.EventHandler(this.menuEdit_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.Size = new System.Drawing.Size(180, 24);
            this.menuDelete.Text = "Xóa phim";
            this.menuDelete.Click += new System.EventHandler(this.menuDelete_Click);
            // 
            // menuAddToPlaylist
            // 
            this.menuAddToPlaylist.Name = "menuAddToPlaylist";
            this.menuAddToPlaylist.Size = new System.Drawing.Size(180, 24);
            this.menuAddToPlaylist.Text = "Thêm vào Playlist";
            this.menuAddToPlaylist.Click += new System.EventHandler(this.menuAddToPlaylist_Click);
            // 
            // menuUpdateProgress
            // 
            this.menuUpdateProgress.Name = "menuUpdateProgress";
            this.menuUpdateProgress.Size = new System.Drawing.Size(180, 24);
            this.menuUpdateProgress.Text = "Cập nhật tiến độ (%)";
            this.menuUpdateProgress.Click += new System.EventHandler(this.menuUpdateProgress_Click);
            // 
            // pnlBase
            // 
            this.pnlBase.BorderRadius = 12;
            this.pnlBase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(14)))), ((int)(((byte)(29)))));
            this.pnlBase.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.pnlBase.Controls.Add(this.picCover);
            this.pnlBase.Controls.Add(this.lblTitle);
            this.pnlBase.Controls.Add(this.flpTags);
            this.pnlBase.Controls.Add(this.lblRating);
            this.pnlBase.Controls.Add(this.lblFavorite);
            this.pnlBase.Controls.Add(this.pgbWatchProgress);
            this.pnlBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBase.Location = new System.Drawing.Point(0, 0);
            this.pnlBase.Name = "pnlBase";
            this.pnlBase.Size = new System.Drawing.Size(360, 320);
            this.pnlBase.TabIndex = 0;
            this.pnlBase.Click += new System.EventHandler(this.MainControl_Click);
            // 
            // picCover
            // 
            this.picCover.BorderRadius = 10;
            this.picCover.Location = new System.Drawing.Point(6, 6);
            this.picCover.Name = "picCover";
            this.picCover.Size = new System.Drawing.Size(348, 185);
            this.picCover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCover.TabIndex = 0;
            this.picCover.TabStop = false;
            this.picCover.Controls.Add(this.lblSource);
            this.picCover.Controls.Add(this.lblInfo);
            this.picCover.Click += new System.EventHandler(this.MainControl_Click);
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(102)))), ((int)(((byte)(241)))));
            this.lblSource.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSource.ForeColor = System.Drawing.Color.White;
            this.lblSource.Location = new System.Drawing.Point(10, 10);
            this.lblSource.Name = "lblSource";
            this.lblSource.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.lblSource.Size = new System.Drawing.Size(56, 17);
            this.lblSource.TabIndex = 5;
            this.lblSource.Text = "ONLINE";
            // 
            // lblInfo
            // 
            this.lblInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblInfo.ForeColor = System.Drawing.Color.White;
            this.lblInfo.Location = new System.Drawing.Point(318, 10);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(20, 20);
            this.lblInfo.TabIndex = 6;
            this.lblInfo.Text = "i";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblInfo.Click += new System.EventHandler(this.lblInfo_Click);
            // 
            // pgbWatchProgress
            // 
            this.pgbWatchProgress.BorderRadius = 2;
            this.pgbWatchProgress.Location = new System.Drawing.Point(6, 192);
            this.pgbWatchProgress.Name = "pgbWatchProgress";
            this.pgbWatchProgress.Size = new System.Drawing.Size(348, 4);
            this.pgbWatchProgress.TabIndex = 7;
            this.pgbWatchProgress.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.pgbWatchProgress.Value = 0;
            this.pgbWatchProgress.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.pgbWatchProgress.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.pgbWatchProgress.Visible = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(10, 201);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(340, 28);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Movie Title";
            this.lblTitle.Click += new System.EventHandler(this.MainControl_Click);
            // 
            // flpTags
            // 
            this.flpTags.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.flpTags.Location = new System.Drawing.Point(10, 237);
            this.flpTags.Name = "flpTags";
            this.flpTags.Size = new System.Drawing.Size(340, 22);
            this.flpTags.TabIndex = 2;
            this.flpTags.WrapContents = false;
            // 
            // lblRating
            // 
            this.lblRating.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.lblRating.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblRating.ForeColor = System.Drawing.Color.Gold;
            this.lblRating.Location = new System.Drawing.Point(10, 273);
            this.lblRating.Name = "lblRating";
            this.lblRating.Size = new System.Drawing.Size(120, 28);
            this.lblRating.TabIndex = 3;
            this.lblRating.Text = "★★★★★";
            // 
            // lblFavorite
            // 
            this.lblFavorite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(56)))));
            this.lblFavorite.Font = new System.Drawing.Font("Segoe UI Emoji", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFavorite.ForeColor = System.Drawing.Color.White;
            this.lblFavorite.Location = new System.Drawing.Point(322, 273);
            this.lblFavorite.Name = "lblFavorite";
            this.lblFavorite.Size = new System.Drawing.Size(28, 28);
            this.lblFavorite.TabIndex = 4;
            this.lblFavorite.Text = "🤍";
            this.lblFavorite.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFavorite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblFavorite.Click += new System.EventHandler(this.lblFavorite_Click);
            // 
            // UcMovieCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.ContextMenuStrip = this.contextMenu;
            this.Controls.Add(this.pnlBase);
            this.Margin = new System.Windows.Forms.Padding(12);
            this.Name = "UcMovieCard";
            this.Size = new System.Drawing.Size(360, 320);
            this.contextMenu.ResumeLayout(false);
            this.pnlBase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picCover)).EndInit();
            this.picCover.ResumeLayout(false);
            this.picCover.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuEdit;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.ToolStripMenuItem menuAddToPlaylist;
        private System.Windows.Forms.ToolStripMenuItem menuUpdateProgress;
        
        private Guna.UI2.WinForms.Guna2Panel pnlBase;
        private Guna.UI2.WinForms.Guna2PictureBox picCover;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel flpTags;
        private System.Windows.Forms.Label lblRating;
        private System.Windows.Forms.Label lblFavorite;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.Label lblInfo;
        private Guna.UI2.WinForms.Guna2ProgressBar pgbWatchProgress;
    }
}
